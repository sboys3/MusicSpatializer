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
            get => Configuration.configClass.enabled;
            set => Configuration.configClass.enabled = value;
        }

        [UIValue("musicspatializer-enable-360")]
        public bool Disable_360 {
            get => Configuration.configClass.enable360;
            set => Configuration.configClass.enable360 = value;
        }

        [UIValue("musicspatializer-enable-resonance")]
        public bool Disable_resonance {
            get => Configuration.configClass.enableResonance;
            set => Configuration.configClass.enableResonance = value;
        }

        [UIValue("musicspatializer-debugspheres")]
        public bool DebugSpheres {
            get => Configuration.configClass.debugSpheres;
            set => Configuration.configClass.debugSpheres = value;
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
