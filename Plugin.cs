using BeatSaberMarkupLanguage.Settings;
using IPA;
using IPA.Config;
using IPA.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using MusicSpatializer.Settings;
using MusicSpatializer.Settings.UI;
using UnityEngine.Audio;


namespace MusicSpatializer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {


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

        private System.Timers.Timer recurringTimer;

        [Init]
        public void Init(IPA.Logging.Logger logger, Config conf)
        {
            log = logger;
            Configuration.Init(conf);
            SceneManager.sceneLoaded += OnSceneLoaded;
            //Log("Spatializer Init");
        }


        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            //Log("actve scene : {0}", scene.name);
            /*
            
            Scene[] scenes=UnityEngine.SceneManagement.SceneManager.GetAllScenes();
            foreach (Scene s in scenes) {
                Log("\tscene name : {0}", s.name);
                LogGameobjects(s);
            }//*/
            if (scene.name == "MenuViewControllers")
            {
                BSMLSettings.instance.AddSettingsMenu("Music Spatializer", "MusicSpatializer.Settings.UI.Views.mainsettings.bsml", MainSettings.instance);
                SettingUiLoad?.Invoke();
            }

            if (scene.name == "MultiplayerGameplay")
            {
                InjectLooper(10);
            }
            else
            {
                //Inject();
                InjectLooper(5);
            }
            

            //if (scene.name == "GameplayCore" || scene.name == "StandardGameplay") // only run in standard level scene
            //{

            //LogAudioListeners();
            //}
            // PCInit HealthWarning MenuViewControllers MenuCore GameCore
            if (scene.name == "HealthWarning")
            {
            }


        }


        [OnStart]
        public void OnStart()
        {
            //SetTimer();
        }

        [OnExit]
        public void OnExit()
        {

        }



        public static void Log(string format, params object[] args)
        {
            Console.WriteLine($"[{Name}] " + format, args);
        }


        //this Function is only for debugging and should be unused in releases
        async void LogGameobjects(Scene scene)
        {
            //await Task.Delay(500);
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
                foreach (Component c in comps)
                {
                    Type ctype = c.GetType();
                    Log("Component name: {0}", ctype.ToString());
                    if (ctype.ToString() == "UnityEngine.AudioSource")
                    {
                        Log("Component UnityEngine.AudioSource UnityEngine.AudioSource UnityEngine.AudioSource UnityEngine.AudioSource");
                    }
                }//*/
            }

            /*
            Log("===================================================");
            Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>();
            foreach (Transform go in allTransforms)
                    Log("trans name: {0}", go.name);*/
        }

        private async void LogAudioListeners()
        {
            await Task.Delay(2000);
            AudioListener[] listeners = Resources.FindObjectsOfTypeAll<AudioListener>();
            foreach (AudioListener listener in listeners)
            {
                Log("listener obj name: {0}", listener.gameObject.name);
                Transform parent = listener.gameObject.transform.parent;
                int numParents = 4;
                while (parent != null)
                {
                    string spaceString = new string(' ', numParents);
                    Log("listener parent: {0}{1}", spaceString, parent.name);
                    parent = parent.transform.parent;
                    numParents += 2;
                }
            }
        }

        //run Inject multiple times over a time span
        public async void InjectLooper(double seconds)
        {
            Inject();
            int time = 0;
            int sleepTime = 200;
            while (time <= seconds * 1000) 
            { 
                await Task.Delay(sleepTime);
                Inject();
                time += sleepTime;
            } 
        }

        public void Inject()
        {
            if (Configuration.config.enabled == false)
            {
                return;
            }
            
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            //Log("===================================================");
            foreach (GameObject go in allObjects)
            {
                /*
                if (go.activeInHierarchy && go.GetComponent<AudioSource>() != null && go.GetComponent<AudioReader>() == null)
                {
                    Log("found object: {0}", go.name);
                }//*/
                if (go.gameObject.activeInHierarchy && (go.name == "SongController" || go.name == "MultiplayerLocalInactivePlayerSongSyncController" || go.name == "SongPreviewAudioSource(Clone)" || go.name == "MultiplayerLocalInactivePlayerOutroAnimator" || go.name == "MultiplayerLocalActiveOutroAnimator")) 
                {
                    GameObject songControl = go.gameObject;

                    //prevent from it being attached multiple times
                    if (songControl.GetComponent<AudioSplitter>() != null)
                    {
                        continue;
                    }

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


                    if (PitchHook != null)
                    {
                        //Log("PitchHook");
                        PitchHookArgs args = new PitchHookArgs();
                        args.songControl = songControl;
                        args.mainSource = mainSource;
                        PitchHook.Invoke(args);
                    }
                    AudioSplitter splitter = songControl.AddComponent<AudioSplitter>();
                    SpeakerCreator speakers = center.AddComponent<SpeakerCreator>();
                    speakers.splitter = splitter;
                    speakers.pluginReference = this;
                    if (Configuration.config.enableBassBoost == true)
                    {
                        volumeMultiplier = volumeMultiplier * 0.75f;
                        speakers.bassBoost = true;
                    }
                    if (Configuration.config.enableResonance == false)
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
                    speakers.volumeMultiplier = volumeMultiplier;
                    //speakers.mixerGroup = mainSource.outputAudioMixerGroup;
                    //audioManager.mainVolume = volumeMultiplier * audioManager.mainVolume;
                    //mainSource.volume = volumeMultiplier;
                    //customAudioMixer.SetFloat()
                    if (Configuration.config.debugSpheres)
                    {
                        Log("Spatializer attached to audio source");
                    }
                }


                //Log("obj name: {0}", go.name);
                /*
                Component[] comps = go.GetComponents(typeof(Component));
                foreach (Component c in comps) {
                    Type ctype = c.GetType();
                    Log("Component name: {0}", ctype.ToString());
                }*/
            }//*/


            //Log("===================");
        }

        void InvokeLevelFailed()
        {
            //Log("LevelFailed");
            LevelFailed?.Invoke();
        }
        void MultiplayerFinishEvent(LevelCompletionResults results)
        {
            if (results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Failed)
            {
                //Log("Multiplayer LevelFailed");
                InvokeLevelFailed();
            }
        }


        //run during the start of the SpeakerCreator in the scene
        public void InjectAfterStart()
        {
            StandardLevelGameplayManager gameplayManager = GameObject.FindObjectsOfType<StandardLevelGameplayManager>().FirstOrDefault();
            MissionLevelGameplayManager gameplayManagerMission = GameObject.FindObjectsOfType<MissionLevelGameplayManager>().FirstOrDefault();
            MultiplayerPlayersManager multiplayerPlayersManager = GameObject.FindObjectsOfType<MultiplayerPlayersManager>().FirstOrDefault();
            if (gameplayManagerMission != null)
            {
                gameplayManagerMission.levelFailedEvent += InvokeLevelFailed;
            }
            if (gameplayManager != null)
            {
                gameplayManager.levelFailedEvent += InvokeLevelFailed;
            }
            if (multiplayerPlayersManager != null)
            {
                multiplayerPlayersManager.playerDidFinishEvent += MultiplayerFinishEvent;
            }
            InjectAfterStartLate();
        }

        public async void InjectAfterStartLate()
        {
            await Task.Delay(500);

            //clean up garbage AudioListeners (people need to stop puting audiolisteners in their custom content)
            GameObject audioListenerGo = GameObject.Find("AudioListener");
            if (audioListenerGo != null)
            {
                AudioListener whatShouldBeTheOnlyOne = audioListenerGo.GetComponent<AudioListener>();
                if (whatShouldBeTheOnlyOne != null)
                {
                    AudioListener[] listeners = Resources.FindObjectsOfTypeAll<AudioListener>();
                    foreach (AudioListener listener in listeners)
                    {
                        if (!object.ReferenceEquals(listener, whatShouldBeTheOnlyOne))
                        {
                            if (listener.gameObject.activeInHierarchy)
                            {
                                Log("DON'T PUT AUDIOLISTENERS WHERE THEY SHOULDN'T BE: {0}", listener.gameObject.name);
                                GameObject.Destroy(listener);
                            }
                        }
                    }
                }
            }
        }


        //not currently needed there are other ways of doing it
        private void SetTimer()
        {
            recurringTimer = new System.Timers.Timer(1000);
            recurringTimer.Elapsed += OnTimedEvent;
            recurringTimer.AutoReset = true;
            recurringTimer.Enabled = true;
        }
        private void OnTimedEvent(System.Object source, System.Timers.ElapsedEventArgs e)
        {
            //Inject();
        }

    }
}
