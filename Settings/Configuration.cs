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
        public bool debugSpheres = false;
    }
    public class Configuration
    {
        private static Ref<PluginConfig> config;
        private static IConfigProvider configProvider;


        public static PluginConfig configClass { get; internal set; } = new PluginConfig();

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
            

            config.Value.enabled = configClass.enabled;
            config.Value.enable360 = configClass.enable360;
            config.Value.enableResonance = configClass.enableResonance;
            config.Value.debugSpheres = configClass.debugSpheres;

            // Store configuration
            configProvider.Store(config.Value);
        }

        private static void LoadConfig()
        {
            configClass.enabled = config.Value.enabled;
            configClass.enable360 = config.Value.enable360;
            configClass.enableResonance = config.Value.enableResonance;
            configClass.debugSpheres = config.Value.debugSpheres;
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
