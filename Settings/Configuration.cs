using IPA.Config;
using IPA.Utilities;
using System;
using UnityEngine;

namespace MusicSpatializer.Settings
{
    public class PluginConfig
    {
        public bool RegenerateConfig = true;
        public bool enabled = true;
        public bool enable360 = true;
        public bool enableResonance = true;
        public bool enableBassBoost = false;
        public bool debugSpheres = false;
    }
    public class Configuration
    {
        private static Ref<PluginConfig> config;
        private static IConfigProvider configProvider;


        public static PluginConfig ConfigClass { get; internal set; } = new PluginConfig();

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
            

            config.Value.enabled = ConfigClass.enabled;
            config.Value.enable360 = ConfigClass.enable360;
            config.Value.enableResonance = ConfigClass.enableResonance;
            config.Value.enableBassBoost = ConfigClass.enableBassBoost;
            config.Value.debugSpheres = ConfigClass.debugSpheres;

            // Store configuration
            configProvider.Store(config.Value);
        }

        private static void LoadConfig()
        {
            ConfigClass.enabled = config.Value.enabled;
            ConfigClass.enable360 = config.Value.enable360;
            ConfigClass.enableResonance = config.Value.enableResonance;
            ConfigClass.enableBassBoost = config.Value.enableBassBoost;
            ConfigClass.debugSpheres = config.Value.debugSpheres;
        }

        /// <summary>
        /// Load Configuration
        /// </summary>
        internal static void Load()
        {
            LoadConfig();
        }

        /// <summary>
        /// Reload configuration
        /// </summary>
        internal static void Reload()
        {
            LoadConfig();
        }

        
    }
}
