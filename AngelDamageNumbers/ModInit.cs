using System;
using AngelDamageNumbers.Config;
using AngelDamageNumbers.Gears;
using AngelDamageNumbers.Managers;
using AngelDamageNumbers.Utilities;
using UnityEngine;

namespace AngelDamageNumbers;

public class ModInit : IModApi
{
    public static bool IsInitialized { get; private set; }

    public void InitMod(Mod modInstance)
    {
        if (IsInitialized)
        {
            AdnLogger.Warning("ModInit called multiple times - skipping duplicate initialization");
            return;
        }

        try
        {
            AdnLogger.Debug("=== ModInit: Starting Enhanced Damage Numbers initialization ===");

            // 1) Bring up logging
            InitializeLogging();

            // 2) Initialize configuration service
            AdnLogger.Debug("Initializing ConfigurationService...");
            ConfigurationService.RefreshConfigurationService();
            AdnLogger.Debug($"ConfigurationService created: {ConfigurationService.Current.GetType().Name}");

            AdnLogger.Debug("Applying defaults to SettingsState...");
            SettingsHelpers.ApplyDefaults();
            AdnLogger.Debug("Defaults applied successfully");

            var cfg = ConfigurationService.Current;

            AdnLogger.Debug("Loading configuration...");
            cfg.LoadConfiguration();
            AdnLogger.Debug("Configuration loaded");

            AdnLogger.Debug("Validating configuration...");
            cfg.ValidateSettings();
            AdnLogger.Debug($"Configuration validated. Summary: {cfg.GetSettingsSummary()}");

            // 3) Apply debug flag
            AdnLogger.Debug("Applying debug logging flag from config...");
            AdnLogger.SetEnabled(SettingsState.EnableDebugLogging);
            AdnLogger.Debug($"Debug logging is now: {(SettingsState.EnableDebugLogging ? "ENABLED" : "DISABLED")}");

            // 4) If Gears exists, wire the UI
            AdnLogger.Debug("Initializing GearsManager configuration UI (if available)...");
            GearsManager.InitializeConfiguration();
            AdnLogger.Debug("GearsManager initialization complete");

            // 5) Init managers
            InitializeManagers();

            // 6) Self-checks
            ValidateInitialization();

            IsInitialized = true;
            AdnLogger.Log("=== Enhanced Damage Numbers mod initialized successfully! ===");
        }
        catch (Exception ex)
        {
            AdnLogger.Error("=== Critical error during mod initialization ===");
            AdnLogger.Error($"Exception: {ex.GetType().Name}: {ex.Message}");
            AdnLogger.Error($"Stack trace: {ex.StackTrace}");

            try
            {
                PerformGracefulDegradation();
            }
            catch (Exception degradationEx)
            {
                AdnLogger.Error($"Failed to perform graceful degradation: {degradationEx.Message}");
            }

            throw; // Let the game know initialization failed
        }
    }



    private void InitializeLogging()
    {
        try
        {
            // Set initial logging state - this will be overridden by config
            AdnLogger.SetEnabled(true);
            AdnLogger.Debug("Logging system initialized");
        }
        catch (Exception ex)
        {
            // Can't log this error since logging failed, so use Unity's debug
            Debug.LogError($"[Angel-DamageNumbers] Failed to initialize logging: {ex.Message}");
            throw;
        }
    }

    private void InitializeManagers()
    {
        try
        {
            AdnLogger.Debug("Starting manager initialization");

            // Initialize CoroutineManager with error boundary
            var coroutineManager = InitializeCoroutineManager();
            if (coroutineManager == null) throw new InvalidOperationException("CoroutineManager initialization failed");

            // Initialize other managers as needed
            // Note: CrosshairManager is initialized lazily when first needed

            AdnLogger.Debug("Manager initialization completed");
        }
        catch (Exception ex)
        {
            AdnLogger.Error($"Failed to initialize managers: {ex.Message}");
            throw;
        }
    }

    private CoroutineManager? InitializeCoroutineManager()
    {
        try
        {
            var coroutineRunner = CoroutineManager.Instance;
            if (coroutineRunner != null)
            {
                AdnLogger.Debug("CoroutineManager instance created successfully");
                return coroutineRunner;
            }

            AdnLogger.Error("Failed to create CoroutineManager instance");
            return null;
        }
        catch (Exception ex)
        {
            AdnLogger.Error($"Exception during CoroutineManager initialization: {ex.Message}");
            return null;
        }
    }

    private void ValidateInitialization()
    {
        try
        {
            AdnLogger.Debug("Validating initialization");

            // Check critical systems
            if (ConfigurationService.Current == null) throw new InvalidOperationException("Configuration service not available");

            if (CoroutineManager.Instance == null) throw new InvalidOperationException("CoroutineManager not available");

            // Check if logging is working
            var debugEnabled = ConfigurationService.Current.EnableDebugLogging;
            AdnLogger.Debug($"Debug logging validation - Enabled: {debugEnabled}");

            // Test font system
            var testFont = FontUtils.GetConfiguredTMPFont();
            if (testFont == null)
                AdnLogger.Warning("Font system returned null - text display may not work properly");
            else
                AdnLogger.Debug($"Font system validated - Using font: {testFont.name}");

            AdnLogger.Debug("Initialization validation completed successfully");
        }
        catch (Exception ex)
        {
            AdnLogger.Error($"Initialization validation failed: {ex.Message}");
            throw;
        }
    }

    private void PerformGracefulDegradation()
    {
        AdnLogger.Log("Attempting graceful degradation due to initialization failure");

        try
        {
            // Disable features that might be causing issues
            var config = ConfigurationService.Current;
            if (config != null)
            {
                config.EnableDebugLogging = false;
                config.EnableCrosshairMarkers = false;
                AdnLogger.Log("Disabled potentially problematic features");
            }
        }
        catch (Exception ex)
        {
            AdnLogger.Error($"Graceful degradation failed: {ex.Message}");
        }
    }
}