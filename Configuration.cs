using IPA.Config;
using IPA.Utilities;
using SaberTailor.Settings.Classes;
using SaberTailor.Settings.Utilities;
using System;
using UnityEngine;

namespace SaberTailor.Settings
{
    public class Configuration
    {
        private static Ref<PluginConfig> config;
        private static IConfigProvider configProvider;

        public static int ConfigVersion { get; private set; }                // Config version, to handle changes in config where existing configs shouldn't just get default config applied

        public static GripConfig Grip { get; internal set; } = new GripConfig();
        public static ScaleConfig Scale { get; internal set; } = new ScaleConfig();
        public static TrailConfig Trail { get; internal set; } = new TrailConfig();

        // Config vars for representing player settings before it gets mangled into something that works with the game,
        // but changes representation of these settings in the process - also avoiding floating points
        public static GripRawConfig GripCfg { get; internal set; } = new GripRawConfig();
        public static ScaleRawConfig ScaleCfg { get; internal set; } = new ScaleRawConfig();

        internal static void Init(IConfigProvider cfgProvider)
        {
            configProvider = cfgProvider;
            config = cfgProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig)
                {
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                }
                config = v;
            });
        }

        /// <summary>
        /// Save Configuration
        /// </summary>
        internal static void Save()
        {
            #region Internal settings
            config.Value.ConfigVersion = ConfigVersion;
            #endregion

            #region Saber scale
            config.Value.IsSaberScaleModEnabled = Scale.TweakEnabled;
            config.Value.SaberScaleHitbox = Scale.ScaleHitBox;
            config.Value.SaberLength = ScaleCfg.Length;
            config.Value.SaberGirth = ScaleCfg.Girth;
            #endregion

            #region Saber trail
            config.Value.IsTrailModEnabled = Trail.TweakEnabled;
            config.Value.IsTrailEnabled = Trail.TrailEnabled;
            config.Value.TrailLength = Trail.Length;
            #endregion

            #region Saber grip
            // Even though the field says GripLeftPosition/GripRightPosition, it is actually the Cfg values that are stored!
            config.Value.GripLeftPosition = new Int3(GripCfg.PosLeft);
            config.Value.GripRightPosition = new Int3(GripCfg.PosRight);

            // Even though the field says GripLeftRotation/GripRightRotation, it is actually the Cfg values that are stored!
            config.Value.GripLeftRotation = new Int3(GripCfg.RotLeft);
            config.Value.GripRightRotation = new Int3(GripCfg.RotRight);

            config.Value.ModifyMenuHiltGrip = Grip.ModifyMenuHiltGrip;
            #endregion

            // Store configuration
            configProvider.Store(config.Value);
        }

        /// <summary>
        /// Load Configuration
        /// </summary>
        internal static void Load()
        {
            BS_Utils.Utilities.Config oldConfig = new BS_Utils.Utilities.Config("modprefs");
            if (oldConfig.HasKey(Plugin.PluginName, "GripLeftPosition") && !oldConfig.GetBool(Plugin.PluginName, "IsExportedToNewConfig", false))
            {
                // Import SaberTailor's settings from the old configuration (ModPrefs)
                try
                {
                    PluginConfig importedConfig = ConfigurationImporter.ImportSettingsFromModPrefs(oldConfig);
                    importedConfig.RegenerateConfig = false;

                    config.Value = importedConfig;

                    // Store configuration in the new format immediately
                    configProvider.Store(config.Value);
                    configProvider.Save();

                    Logger.log.Info("Configuration loaded from ModPrefs.");
                }
                catch (Exception ex)
                {
                    Logger.log.Warn("Failed to import ModPrefs configuration. Loading default BSIPA configuration instead.");
                    Logger.log.Warn(ex);
                }
            }

            LoadConfig();
            UpdateConfig();
            Logger.log.Debug("Configuration has been set");

            // Update variables used by mod logic
            UpdateModVariables();
        }

        /// <summary>
        /// Reload configuration
        /// </summary>
        internal static void Reload()
        {
            LoadConfig();
            UpdateModVariables();
        }

        /// <summary>
        /// Update Saber Length, Position and Rotation
        /// </summary>
        internal static void UpdateModVariables()
        {
            UpdateSaberLength();
            UpdateSaberPosition();
            UpdateSaberRotation();
        }

        /// <summary>
        /// Update Saber Length
        /// </summary>
        internal static void UpdateSaberLength()
        {
            Scale.Length = ScaleCfg.Length / 100f;
            Scale.Girth = ScaleCfg.Girth / 100f;
        }

        /// <summary>
        /// Update Saber Position
        /// </summary>
        internal static void UpdateSaberPosition()
        {
            Grip.PosLeft = Int3.ToVector3(GripCfg.PosLeft) / 1000f;
            Grip.PosRight = Int3.ToVector3(GripCfg.PosRight) / 1000f;
        }

        /// <summary>
        /// Update Saber Rotation
        /// </summary>
        internal static void UpdateSaberRotation()
        {
            Grip.RotLeft = Quaternion.Euler(Int3.ToVector3(GripCfg.RotLeft)).eulerAngles;
            Grip.RotRight = Quaternion.Euler(Int3.ToVector3(GripCfg.RotRight)).eulerAngles;
        }

        private static void LoadConfig()
        {
            #region Internal settings
            ConfigVersion = config.Value.ConfigVersion;
            #endregion

            #region Saber scale
            Scale.TweakEnabled = config.Value.IsSaberScaleModEnabled;
            Scale.ScaleHitBox = config.Value.SaberScaleHitbox;

            if (config.Value.SaberLength < 5 || config.Value.SaberLength > 500)
            {
                ScaleCfg.Length = 100;
            }
            else
            {
                ScaleCfg.Length = config.Value.SaberLength;
            }

            if (config.Value.SaberGirth < 5 || config.Value.SaberGirth > 500)
            {
                ScaleCfg.Girth = 100;
            }
            else
            {
                ScaleCfg.Girth = config.Value.SaberGirth;
            }
            #endregion

            #region Saber trail
            Trail.TweakEnabled = config.Value.IsTrailModEnabled;
            Trail.TrailEnabled = config.Value.IsTrailEnabled;
            Trail.Length = Mathf.Clamp(config.Value.TrailLength, 5, 100);
            #endregion

            #region Saber grip
            // Even though the field says GripLeftPosition/GripRightPosition, it is actually the Cfg values that are loaded!
            Int3 gripLeftPosition = config.Value.GripLeftPosition;
            GripCfg.PosLeft = new Int3()
            {
                x = Mathf.Clamp(gripLeftPosition.x, -500, 500),
                y = Mathf.Clamp(gripLeftPosition.y, -500, 500),
                z = Mathf.Clamp(gripLeftPosition.z, -500, 500)
            };

            Int3 gripRightPosition = config.Value.GripRightPosition;
            GripCfg.PosRight = new Int3()
            {
                x = Mathf.Clamp(gripRightPosition.x, -500, 500),
                y = Mathf.Clamp(gripRightPosition.y, -500, 500),
                z = Mathf.Clamp(gripRightPosition.z, -500, 500)
            };

            // Even though the field says GripLeftRotation/GripRightRotation, it is actually the Cfg values that are loaded!
            GripCfg.RotLeft = new Int3(config.Value.GripLeftRotation);
            GripCfg.RotRight = new Int3(config.Value.GripRightRotation);

            Grip.ModifyMenuHiltGrip = config.Value.ModifyMenuHiltGrip;
            #endregion
        }

        /// <summary>
        /// Handle updates and additions to configuration.
        /// Only needed if new settings shouldn't be set to default values in (some) existing config files
        /// </summary>
        private static void UpdateConfig()
        {
            // Get latest version from default config values
            int latestVersion = new PluginConfig().ConfigVersion;

            // Do nothing if config is already up to date
            if (ConfigVersion == latestVersion)
            {
                return;
            }

            // v1/v2 -> v3: Added enable/disable options for trail and scale modifications
            // Updating v2 as well because of a beta build that is floating around with v2 already being used
            if (ConfigVersion == 1 || ConfigVersion == 2)
            {
                // Check trail modifications and disable tweak if settings are default
                if (Trail.TrailEnabled && Trail.Length == 20)
                {
                    Trail.TweakEnabled = false;
                }
                else
                {
                    Trail.TweakEnabled = true;
                }

                // Check scale modifications and disable tweak if settings are default
                if (ScaleCfg.Length == 100 && ScaleCfg.Girth == 100)
                {
                    Scale.TweakEnabled = false;
                }
                else
                {
                    // else enable tweak and hitbox scaling to preserve existing settings
                    Scale.TweakEnabled = true;
                    Scale.ScaleHitBox = true;
                }
            }

            // Updater done - set to latest version and save
            ConfigVersion = latestVersion;
            Save();
        }
    }
}
