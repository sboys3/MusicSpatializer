﻿using BeatSaberMarkupLanguage.Settings;
using IPA;
using IPA.Config;
using IPA.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using MusicSpatializer.Settings;
using MusicSpatializer.Settings.UI;
using UnityEngine.Audio;


namespace MusicSpatializer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin {


        public static IPA.Logging.Logger log;
        public const string Name = "Music Spatializer";

        public struct PitchHookArgs
        {
            public AudioSource mainSource;
            public GameObject songControl;
        }
        public delegate void PitchHookDelegate(PitchHookArgs args);
        public static event PitchHookDelegate PitchHook;

        public delegate void VoidDelegate();
        public static event VoidDelegate SettingUiLoad;

        public static event VoidDelegate LevelFailed;

        [Init]
        public void Init(IPA.Logging.Logger logger, Config conf)
        {
            log = logger;
            Configuration.Init(conf);
            SceneManager.sceneLoaded += OnSceneLoaded;
            //Log("Spatializer Init");
        }
        

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) {
            /*
            Log("actve scene : {0}", scene.name);
            Scene[] scenes=UnityEngine.SceneManagement.SceneManager.GetAllScenes();
            foreach (Scene s in scenes) {
                Log("\tscene name : {0}", s.name);
                LogGameobjects(s);
            }//*/
            if (scene.name == "MenuViewControllers")
            {
                BSMLSettings.instance.AddSettingsMenu("Music Spatializer", "MusicSpatializer.Settings.UI.Views.mainsettings.bsml", MainSettings.instance);
                if(SettingUiLoad != null)
                {
                    SettingUiLoad.Invoke();
                }
            }

            if (scene.name == "GameplayCore") // only run in standard level scene
            {
                Inject(scene);
            }
            // PCInit HealthWarning MenuViewControllers MenuCore GameCore
            if (scene.name == "HealthWarning") 
            {
            }

            
        }
        

        [OnStart]
        public void OnStart()
        {
            
        }

        [OnExit]
        public void OnExit()
        {
            
        }



        public static void Log(string format, params object[] args) {
            Console.WriteLine($"[{Name}] " + format, args);
        }
        

        //this Function is only for debugging and should be unused in releases
        void LogGameobjects(Scene scene)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();
            GameObject rootObject = rootObjects.First<GameObject>();//.FindObjectsOfType<GameObject>();
            //*
            Transform[] allObjects = rootObject.GetComponentsInChildren<Transform>();
            //.FindGameObjectsWithTag("Untagged");
            foreach (Transform go in allObjects)
            {
                
                Log("obj name: {0}", go.name);
                //*
                Component[] comps = go.GetComponents(typeof(Component));
                foreach (Component c in comps) {
                    Type ctype = c.GetType();
                    Log("Component name: {0}", ctype.ToString());
                }//*/
            }

            /*
            Log("===================================================");
            Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>();
            foreach (Transform go in allTransforms)
                    Log("trans name: {0}", go.name);*/
        }
        
        public void Inject(Scene scene)
        {
            if (Configuration.config.enabled == false)
            {
                return;
            }
            //Log("===================================================");
            //GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            GameObject[] rootObjects = scene.GetRootGameObjects();
            GameObject rootObject = rootObjects.First<GameObject>();//.FindObjectsOfType<GameObject>();
            //*
            Transform[] allObjects = rootObject.GetComponentsInChildren<Transform>();
            //.FindGameObjectsWithTag("Untagged");
            foreach (Transform go in allObjects) {
                // if (go.activeInHierarchy)
                if (go.name== "SongController")
                {
                    GameObject songControl = go.gameObject;
                    GameObject center = new GameObject("Music Spatializer Base");
                    center.transform.parent = songControl.transform;

                    //Log("obj name: {0}", songControl.name);

                    MainSettingsModelSO gameSettings = Resources.FindObjectsOfTypeAll<MainSettingsModelSO>()[0];
                    AudioManagerSO audioManager = Resources.FindObjectsOfTypeAll<AudioManagerSO>()[0];
                    SongController songController = songControl.GetComponent<SongController>();
                    AudioSource mainSource = songControl.GetComponent<AudioSource>();
                    
                    float volumeMultiplier = 1;
                    volumeMultiplier = gameSettings.volume.value;
                    //Log("volume: {0}", volume);


                    if (PitchHook != null) {
                        //Log("PitchHook");
                        PitchHookArgs args = new PitchHookArgs();
                        args.songControl = songControl;
                        args.mainSource = mainSource;
                        PitchHook.Invoke(args);
                    }
                    AudioSplitter splitter=songControl.AddComponent<AudioSplitter>();
                    SpeakerCreator speakers= center.AddComponent<SpeakerCreator>();
                    speakers.splitter = splitter;
                    speakers.pluginReference = this;
                    if (Configuration.config.enableBassBoost == true)
                    {
                        volumeMultiplier = volumeMultiplier * 0.75f;
                        speakers.bassBoost = true;
                    }
                    if (Configuration.config.enableResonance==false)
                    {
                        speakers.resonance = false;
                    }
                    if (Configuration.config.enable360 == false)
                    {
                        speakers.doRotation = false;
                    }
                    if (Configuration.config.debugSpheres)
                    {
                        speakers.debugSpheres = true;
                    }
                    //speakers.mixerGroup = mainSource.outputAudioMixerGroup;
                    //audioManager.mainVolume = volumeMultiplier * audioManager.mainVolume;
                    mainSource.volume = volumeMultiplier;
                    //customAudioMixer.SetFloat()
                    Log("Spatializer attached to audio source");
                }
                
                
                //Log("obj name: {0}", go.name);
                /*
                Component[] comps = go.GetComponents(typeof(Component));
                foreach (Component c in comps) {
                    Type ctype = c.GetType();
                    Log("Component name: {0}", ctype.ToString());
                }*/
            }//*/

               
        }

        public void InjectAfterStart()
        {
            StandardLevelGameplayManager gameplayManager = GameObject.FindObjectsOfType<StandardLevelGameplayManager>().FirstOrDefault();
            MissionLevelGameplayManager gameplayManagerMission = GameObject.FindObjectsOfType<MissionLevelGameplayManager>().FirstOrDefault();
            if (gameplayManagerMission != null)
            {
                gameplayManagerMission.levelFailedEvent += LevelFailed.Invoke;
            }
            if (gameplayManager != null)
            {
                gameplayManager.levelFailedEvent += LevelFailed.Invoke;
            }
        }
    }
}
