using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MusicSpatializer.Settings.UI
{
    public class MainSettings : PersistentSingleton<MainSettings>
    {
        [UIParams]
        private BSMLParserParams parserParams;


        [UIValue("musicspatializer-enabled")]
        public bool Disabled
        {
            get => Configuration.ConfigClass.enabled;
            set => Configuration.ConfigClass.enabled = value;
        }

        [UIValue("musicspatializer-enable-360")]
        public bool Disable_360 {
            get => Configuration.ConfigClass.enable360;
            set => Configuration.ConfigClass.enable360 = value;
        }

        [UIValue("musicspatializer-enable-resonance")]
        public bool Disable_resonance {
            get => Configuration.ConfigClass.enableResonance;
            set => Configuration.ConfigClass.enableResonance = value;
        }

        [UIValue("musicspatializer-enable-bass-boost")]
        public bool enableBassBoost {
            get => Configuration.ConfigClass.enableBassBoost;
            set => Configuration.ConfigClass.enableBassBoost = value;
        }

        [UIValue("musicspatializer-debugspheres")]
        public bool DebugSpheres {
            get => Configuration.ConfigClass.debugSpheres;
            set => Configuration.ConfigClass.debugSpheres = value;
        }


        

        [UIAction("#apply")]
        public void OnApply() => StoreConfiguration();

        [UIAction("#ok")]
        public void OnOk() => StoreConfiguration();

        [UIAction("#cancel")]
        public void OnCancel() => ReloadConfiguration();

        /// <summary>
        /// Save and update configuration
        /// </summary>
        private void StoreConfiguration()
        {
            Configuration.Save();
        }

        /// <summary>
        /// Reload configuration and refresh UI
        /// </summary>
        private void ReloadConfiguration()
        {
            Configuration.Reload();
            RefreshModSettingsUI();
        }

        /// <summary>
        /// Refresh the entire UI
        /// </summary>
        private void RefreshModSettingsUI()
        {
            parserParams.EmitEvent("refresh-musicspatializer-values");
        }

    }
}
