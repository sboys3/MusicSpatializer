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
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public bool enabled { get; set; } = true;
        public bool enable360 { get; set; } = true;
        public bool enableResonance { get; set; } = true;
        public bool enableBassBoost { get; set; } = false;
        public bool enableSpatialize { get; set; } = true;
        public float musicVolumeMultiplier { get; set; } = 1.0f;
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

        public static PluginConfig config;

        internal static void Init(Config cfgProvider)
        {
            PluginConfig.Instance = cfgProvider.Generated<PluginConfig>();
            config = PluginConfig.Instance;
        }

        internal static void Save()
        {
            config.Changed();
        }
    }
}
