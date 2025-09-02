using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using AngelDamageNumbers.Utilities;

namespace AngelDamageNumbers.Config
{
    /// <summary>
    ///     Smart migration system that can incrementally upgrade configs through multiple versions
    /// </summary>
    public static class ConfigMigrator
    {
        private const string OldRootTag = "FloatingDamageNumbersConfig";
        private const string CurrentRootTag = "AngelDamageNumbersConfig";

        // Version progression - add new versions to the end
        private static readonly string[] VersionChain =
        {
            "1.0", // Old FloatingDamageNumbersConfig format
            "2.0", // AngelDamageNumbersConfig with TextStyling section
            "3.0" // Current version with ModMetaData tag
        };

        private static readonly string CurrentVersion = ModMetaData.ConfigVersion;

        /// <summary>
        ///     Migration rules - each version defines what changes from the previous version
        /// </summary>
        private static readonly Dictionary<string, MigrationRule> MigrationRules = new Dictionary<string, MigrationRule>
        {
            ["1.0.0->2.0.0"] = new MigrationRule
            {
                FromVersion = "1.0.0",
                ToVersion = "2.0.0",
                Description = "Add TextStyling section and rename root tag",
                RootTagChanges = new RootTagChange { From = OldRootTag, To = CurrentRootTag },
                NewSections = new[] { "TextStyling" }
                // Don't need to specify individual settings - XML handler will fill defaults
            },

            ["2.0.0->3.0.0"] = new MigrationRule
            {
                FromVersion = "2.0.0",
                ToVersion = "3.0.0",
                Description = "Add ModMetaData version tag",
                VersionTagChanges = new VersionTagChange { From = "Version", To = "ModMetaData" }
                // Future: could add RenamedSettings, RemovedSettings, etc.
            }
        };

        /// <summary>
        ///     Analyze and migrate config if needed
        /// </summary>
        public static MigrationResult MigrateConfigIfNeeded(string configPath)
        {
            var result = new MigrationResult
            {
                WasMigrationNeeded = false,
                MigrationSuccessful = false,
                StartVersion = "Unknown",
                EndVersion = CurrentVersion,
                MigrationPath = null,
                BackupPath = null,
                ErrorMessage = null
            };

            try
            {
                if (!File.Exists(configPath))
                {
                    result.MigrationSuccessful = true; // No file = no migration needed
                    return result;
                }

                // Detect current version
                var detectedVersion = DetectConfigVersion(configPath);
                result.StartVersion = detectedVersion;

                // Calculate migration path
                var migrationPath = CalculateMigrationPath(detectedVersion, CurrentVersion);
                result.MigrationPath = migrationPath;

                if (migrationPath.Length == 0)
                {
                    // Already current version
                    result.MigrationSuccessful = true;
                    AdnLogger.Debug($"Config is already current version: {detectedVersion}");
                    return result;
                }

                result.WasMigrationNeeded = true;
                AdnLogger.Log($"Config migration needed: {detectedVersion} -> {CurrentVersion} (via {string.Join(" -> ", migrationPath)})");

                // Create backup
                result.BackupPath = CreateBackup(configPath);

                // Apply incremental migrations
                var currentPath = configPath;
                foreach (var step in migrationPath)
                {
                    ApplyMigrationStep(currentPath, step);
                    AdnLogger.Debug($"Applied migration step: {step}");
                }

                result.MigrationSuccessful = true;
                AdnLogger.Log($"Config migration completed successfully: {result.StartVersion} -> {CurrentVersion}");

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Migration failed: {ex.Message}";
                AdnLogger.Error(result.ErrorMessage);

                // Restore backup if we created one
                if (!string.IsNullOrEmpty(result.BackupPath) && File.Exists(result.BackupPath))
                    try
                    {
                        File.Copy(result.BackupPath, configPath, true);
                        AdnLogger.Log("Restored original config from backup");
                    }
                    catch (Exception restoreEx)
                    {
                        AdnLogger.Error($"Failed to restore backup: {restoreEx.Message}");
                    }

                return result;
            }
        }

        private static string DetectConfigVersion(string configPath)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(configPath);

                var rootTag = doc.DocumentElement?.Name;

                // Old format detection
                if (rootTag == OldRootTag) return "1.0.0";

                // New format - check version tags
                if (rootTag == CurrentRootTag)
                {
                    // Check for ModMetaData tag first (3.0.0+)
                    var modInfoNode = doc.SelectSingleNode($"/{CurrentRootTag}/ModMetaData");
                    if (modInfoNode != null) return modInfoNode.InnerText.Trim();

                    // Check for Version tag (2.0.0)
                    var versionNode = doc.SelectSingleNode($"/{CurrentRootTag}/Version");
                    if (versionNode != null) return versionNode.InnerText.Trim();

                    // New root tag but no version = assume early 2.0.0
                    return "2.0.0";
                }

                AdnLogger.Warning($"Unknown config format with root tag: {rootTag}");
                return "Unknown";
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Failed to detect config version: {ex.Message}");
                return "Unknown";
            }
        }

        private static string[] CalculateMigrationPath(string fromVersion, string toVersion)
        {
            if (fromVersion == toVersion)
                return new string[0];

            var fromIndex = Array.IndexOf(VersionChain, fromVersion);
            var toIndex = Array.IndexOf(VersionChain, toVersion);

            if (fromIndex == -1 || toIndex == -1 || fromIndex >= toIndex)
            {
                AdnLogger.Error($"Invalid migration path: {fromVersion} -> {toVersion}");
                return new string[0];
            }

            // Build step-by-step migration path
            var path = new List<string>();
            for (var i = fromIndex; i < toIndex; i++)
            {
                var step = $"{VersionChain[i]}->{VersionChain[i + 1]}";
                path.Add(step);
            }

            return path.ToArray();
        }

        private static void ApplyMigrationStep(string configPath, string migrationStep)
        {
            if (!MigrationRules.TryGetValue(migrationStep, out var rule)) throw new InvalidOperationException($"No migration rule defined for step: {migrationStep}");

            var doc = new XmlDocument();
            doc.Load(configPath);

            AdnLogger.Debug($"Applying migration: {rule.Description}");

            // Apply root tag changes
            if (rule.RootTagChanges != null) ApplyRootTagChange(doc, rule.RootTagChanges);

            // Apply version tag changes
            if (rule.VersionTagChanges != null) ApplyVersionTagChange(doc, rule.VersionTagChanges, rule.ToVersion);

            // Add new sections (XML handler will fill in defaults when loading)
            if (rule.NewSections != null) AddNewSections(doc, rule.NewSections);

            // Save changes
            doc.Save(configPath);
        }

        private static void ApplyRootTagChange(XmlDocument doc, RootTagChange change)
        {
            if (doc.DocumentElement?.Name == change.From)
            {
                // Create new root with new name
                var newRoot = doc.CreateElement(change.To);

                // Copy all children and attributes
                while (doc.DocumentElement is { HasChildNodes: true }) newRoot.AppendChild(doc.DocumentElement.FirstChild);

                if (doc.DocumentElement != null)
                {
                    foreach (XmlAttribute attr in doc.DocumentElement.Attributes) newRoot.SetAttribute(attr.Name, attr.Value);

                    // Replace root
                    doc.RemoveChild(doc.DocumentElement);
                }

                doc.AppendChild(newRoot);

                AdnLogger.Debug($"Changed root tag: {change.From} -> {change.To}");
            }
        }

        private static void ApplyVersionTagChange(XmlDocument doc, VersionTagChange change, string newVersion)
        {
            var root = doc.DocumentElement;
            if (root == null) return;

            // Remove old version tag if it exists
            if (!string.IsNullOrEmpty(change.From))
            {
                var oldVersionNode = root.SelectSingleNode(change.From);
                if (oldVersionNode != null) root.RemoveChild(oldVersionNode);
            }

            // Add new version tag at the beginning
            var newVersionNode = doc.CreateElement(change.To);
            newVersionNode.InnerText = newVersion;

            if (root.HasChildNodes)
                root.InsertBefore(newVersionNode, root.FirstChild);
            else
                root.AppendChild(newVersionNode);

            AdnLogger.Debug($"Updated version tag: {change.From} -> {change.To} = {newVersion}");
        }

        private static void AddNewSections(XmlDocument doc, string[] newSections)
        {
            var root = doc.DocumentElement;
            if (root == null) return;

            foreach (var sectionName in newSections)
            {
                // Check if section already exists
                var existingSection = root.SelectSingleNode(sectionName);
                if (existingSection == null)
                {
                    // Add empty section - XML handler will populate with defaults
                    var newSection = doc.CreateElement(sectionName);
                    root.AppendChild(newSection);
                    AdnLogger.Debug($"Added new section: {sectionName}");
                }
            }
        }

        private static string CreateBackup(string configPath)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var backupPath = $"{configPath}.backup-{timestamp}";
            File.Copy(configPath, backupPath);
            return backupPath;
        }

        /// <summary>
        ///     Add a new migration rule for future versions
        /// </summary>
        public static void RegisterMigrationRule(string fromVersion, string toVersion, MigrationRule rule)
        {
            var key = $"{fromVersion}->{toVersion}";
            MigrationRules[key] = rule;
            AdnLogger.Debug($"Registered migration rule: {key}");
        }

        public struct MigrationResult
        {
            public bool WasMigrationNeeded;
            public bool MigrationSuccessful;
            public string StartVersion;
            public string EndVersion;
            public string[] MigrationPath;
            public string BackupPath;
            public string ErrorMessage;
        }

        // Data structures for migration rules
        public class MigrationRule
        {
            public string Description;
            public string FromVersion;
            public string[] NewSections;
            public string[] RemovedSettings;
            public Dictionary<string, string> RenamedSettings;
            public RootTagChange RootTagChanges;
            public string ToVersion;
            public VersionTagChange VersionTagChanges;
        }

        public class RootTagChange
        {
            public string From;
            public string To;
        }

        public class VersionTagChange
        {
            public string From;
            public string To;
        }
    }
}