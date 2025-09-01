using System;
using System.IO;
using System.Xml;
using Config;
using Gears;
using Managers;
using UnityEngine;
using Utilities;

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
            AdnLogger.Debug("ModInit called - Starting Enhanced Damage Numbers initialization");

            // Initialize logging first so we can log everything else
            InitializeLogging();

            // Initialize configuration system with error boundary
            GearsManager.InitializeConfiguration();

            // Initialize managers with error boundary
            InitializeManagers();

            // Validate initialization
            ValidateInitialization();

            IsInitialized = true;
            AdnLogger.Log("Enhanced Damage Numbers mod initialized successfully!");
        }
        catch (Exception ex)
        {
            AdnLogger.Error($"Critical error during mod initialization: {ex.Message}");
            AdnLogger.Error($"Stack trace: {ex.StackTrace}");

            // Attempt graceful degradation
            try
            {
                PerformGracefulDegradation();
            }
            catch (Exception degradationEx)
            {
                AdnLogger.Error($"Failed to perform graceful degradation: {degradationEx.Message}");
            }

            throw; // Re-throw to let the game know initialization failed
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

    private CoroutineManager InitializeCoroutineManager()
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
            var testFont = FontUtils.GetConfiguredFont();
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

    private static void Cleanup()
    {
        try
        {
            if (!IsInitialized)
            {
                AdnLogger.Debug("ModInit cleanup called but mod was not initialized");
                return;
            }

            AdnLogger.Log("Starting Enhanced Damage Numbers mod cleanup");

            // Use the centralized cleanup system
            CleanUpHelper.PerformFullCleanup();

            IsInitialized = false;
            AdnLogger.Log("Enhanced Damage Numbers mod cleanup completed");
        }
        catch (Exception ex)
        {
            // Use Unity Debug in case our logging system is already cleaned up
            Debug.LogError($"[Angel-DamageNumbers] Error during cleanup: {ex.Message}");
        }
    }

    public static void Reinitialize(Mod modInstance)
    {
        if (IsInitialized) Cleanup();

        var modInit = new ModInit();
        modInit.InitMod(modInstance);
    }
}