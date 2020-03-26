using IPA.Config;
using IPA.Utilities;
using IPA.Config.Data;
using IPA.Config.Stores;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace MusicSpatializer.Settings
{
    public class PluginConfigOld
    {
        public bool RegenerateConfig = false;
        public bool enabled = true;
        public bool enable360 = true;
        public bool enableResonance = true;
        public bool enableBassBoost = false;
        public bool debugSpheres = false;
    }
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public bool enabled { get; set; } = true;
        public bool enable360 { get; set; } = true;
        public bool enableResonance { get; set; } = true;
        public bool enableBassBoost { get; set; } = false;
        public bool debugSpheres { get; set; } = false;

        public virtual void Changed()
        {
            // this is called whenever one of the virtual properties is changed
            // can be called to signal that the content has been changed
        }

        public virtual void OnReload()
        {
            // this is called whenever the config file is reloaded from disk
            // use it to tell all of your systems that something has changed

            // this is called off of the main thread, and is not safe to interact
            //   with Unity in
        }
    }
    public class Configuration
    {
        private static Ref<PluginConfig> config;
        private static ConfigProvider configProvider;
        private static Value currentConfig;

        public static PluginConfig ConfigClass;

        internal static void Init(Config cfgProvider)
        {
            PluginConfig.Instance = cfgProvider.Generated<PluginConfig>();
            ConfigClass = PluginConfig.Instance;
            //Console.WriteLine("Hello {0}", configProvider == null);
            //configProvider = cfgProvider;
            /*config = cfgProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig)
                {
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                }
                config = v;
            });*/
        }

        /*
        private static void SaveValue<U>(string name, U value)
        {
            try
            {
                currentConfig.SetField<Value, U>(name, value);
            }
            catch (MissingFieldException e)
            {

            }
        }

        /// <summary>
        /// Save Configuration
        /// </summary>
        internal static void Save()
        {

            SaveValue<bool>("enabled", ConfigClass.enabled);
            SaveValue<bool>("enable360", ConfigClass.enable360);
            SaveValue<bool>("enableResonance", ConfigClass.enableResonance);
            SaveValue<bool>("enableBassBoost", ConfigClass.enableBassBoost);
            SaveValue<bool>("debugSpheres", ConfigClass.debugSpheres);
            SaveValue<bool>("RegenerateConfig", ConfigClass.RegenerateConfig);

            // Store configuration
            configProvider.Store(currentConfig);
        }

        private static U LoadValueOrDefault<U>(string name, U defaultValue)
        {
            try
            {
                return currentConfig.GetField<U, Value>(name);
            } catch (MissingFieldException e)
            {
                return defaultValue;
            }
        }

        private static void ResetConfig()
        {
            ConfigClass = new PluginConfig();
            Save();
            LoadConfig();
        }

        private static void LoadConfig()
        {
            
            currentConfig = configProvider.Load();
            if (currentConfig==null)
            {
                ResetConfig();
                return;
            }
            ConfigClass.RegenerateConfig = LoadValueOrDefault <bool> ("RegenerateConfig", ConfigClass.RegenerateConfig);
            if (ConfigClass.RegenerateConfig)
            {
                ResetConfig();
                return;
            }
            ConfigClass.enabled = LoadValueOrDefault <bool> ("enabled", ConfigClass.enabled);
            ConfigClass.enable360 = LoadValueOrDefault <bool> ("enable360", ConfigClass.enable360);
            ConfigClass.enableResonance = LoadValueOrDefault <bool> ("enableResonance", ConfigClass.enableResonance);
            ConfigClass.enableBassBoost = LoadValueOrDefault <bool> ("enableBassBoost", ConfigClass.enableBassBoost);
            ConfigClass.debugSpheres = LoadValueOrDefault <bool> ("debugSpheres", ConfigClass.debugSpheres);
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
        */
        
    }
}
