using BeatSaberMarkupLanguage.Settings;
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
using Harmony;
using MusicSpatializer.Settings;
using MusicSpatializer.Settings.UI;

namespace MusicSpatializer
{
    public class Plugin : IBeatSaberPlugin {


        public Logger log;
        public const string Name = "MusicSpatializer";
        public const  string Version = "1.0.0";
        
        //internal static HarmonyInstance harmonyInstance = HarmonyInstance.Create("com.sboys3.BeatSaber.MusicSpatializer");

        public void Init(Logger logger, [Config.Prefer("json")] IConfigProvider cfgProvider, PluginLoader.PluginMetadata metadata)
        {
            log = logger;
            Configuration.Init(cfgProvider);
        }

        public void OnActiveSceneChanged(Scene fromeScene, Scene toScene) {
            //Log("scene name : {0}", toScene.name);
            if (toScene.name == "MenuViewControllers" && fromeScene.name == "EmptyTransition")
            {
                BSMLSettings.instance.AddSettingsMenu("Music Spatializer", "MusicSpatializer.Settings.UI.Views.mainsettings.bsml", MainSettings.instance);
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) {
            /*
            Log("actve scene : {0}", scene.name);
            Scene[] scenes=UnityEngine.SceneManagement.SceneManager.GetAllScenes();
            foreach (Scene s in scenes) {
                Log("\tscene name : {0}", s.name);
                logGameobjects(s);
            }//*/
            
            if (scene.name == "GameplayCore") // only run in standard level scene
            {
                inject(scene);
                /*
                Scene[] scenes = UnityEngine.SceneManagement.SceneManager.GetAllScenes();
                foreach (Scene s in scenes)
                {
                    Log("actve scene : {0}", s.name);
                    inject(s);
                }*/
            }
            // PCInit HealthWarning MenuViewControllers MenuCore GameCore
            if (scene.name == "HealthWarning") 
            {
                //harmonyInstance.PatchAll();
            }

            
        }
        public void OnSceneUnloaded(Scene scene) {

        }
        //public void Init(object thisWillBeNull, Logger logger) {
        //log = logger;

        //}
        /* 
        
        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {
        }*/

        public void OnApplicationStart()
        {
            Load();
        }
        //public void OnEnable() => Load();
        //public void OnDisable() => Unload();
        public void OnApplicationQuit() => Unload();

        private void Load()
        {
            Configuration.Load();
        }

        private void Unload()
        {
            Configuration.Save();
        }


        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public static void Log(string format, params object[] args) {
            Console.WriteLine($"[{Name}] " + format, args);
        }


        //this Function is only for debugging and should be unused in releases
        void logGameobjects(Scene scene)
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
        }

        public void inject(Scene scene)
        {
            if (Configuration.configClass.enabled == false)
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
                    GameObject center = new GameObject();
                    center.transform.parent = songControl.transform;
                    
                    //Log("obj name: {0}", songControll.name);
                    audioSplitter splitter=songControl.AddComponent<audioSplitter>();
                    speakerCreator speakers= center.AddComponent<speakerCreator>();
                    speakers.splitter = splitter;
                    if (Configuration.configClass.enableResonance==false)
                    {
                        speakers.resonance = false;
                    }
                    if (Configuration.configClass.enable360 == false)
                    {
                        speakers.doRotation = false;
                    }
                    if (Configuration.configClass.debugSpheres)
                    {
                        speakers.debugSpheres = true;
                    }
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

               /*
            AudioTimeSyncController audioController = GameObject.FindObjectsOfType<AudioTimeSyncController>()[0];
            GameObject go2 = audioController.gameObject;

            Log("obj name: {0}", go2.name);*/
            /*
            GameObject songControll = rootObject.transform.Find("SongController").gameObject;
            
            Log("1111111111111111111111");
            Log("obj name: {0}", songControll.name);
            songControll.AddComponent<audioSplitter>();
            Log("obj name: {0}", songControll.name);
            */
            /*
            Log("===================================================");
            Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>();
            foreach (Transform go in allTransforms)
                    Log("trans name: {0}", go.name);*/
            //UnityEngine.SceneManagement.Scene.
        }
    }


    public class speakerCreator : MonoBehaviour
    {

        public AudioClip dclip;
        public GameObject speakerLeft;
        public GameObject speakerRight;
        public GameObject speakerResonance;
        public float frontDistance = 5;
        public float sideDistance = 3f;
        public audioSplitter splitter;
        public bool resonance = true;
        public bool doRotation = true;
        public bool debugSpheres = false;
        private GameObject rotationMarker;
        private int rotationMarkerTries=10;

        // Start is called before the first frame update
        void Start()
        {
            Create();
        }

        // Update is called once per frame
        void Update()
        {
            //sideDistance += 0.01f;
            //Create();
            if (doRotation)
            {
                //rotate audio based on the chevron in 360 maps
                if (rotationMarker != null)
                {
                    //Console.WriteLine("found Chevron");
                    
                    transform.rotation = rotationMarker.transform.rotation;
                }
                else
                {
                    //Console.WriteLine("not found Chevron");
                    if (rotationMarkerTries > 0)
                    {
                        rotationMarker = GameObject.Find("SpawnRotationChevron");
                        if (rotationMarker != null)
                        {
                            sideDistance = 1.5f;
                            positionSpeakers();
                        }
                        rotationMarkerTries--;
                    }
                }
            }
        }

        GameObject newSpeaker(int channel)
        {
            GameObject speaker;
            if (debugSpheres)
            {
                speaker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
            else
            {
                speaker = new GameObject();
            }
            speaker.transform.parent = transform;

            audioReader chfilt = speaker.AddComponent<audioReader>();
            chfilt.channel = channel;
            chfilt.splitter = splitter;
            if (channel == -1)
            {
                chfilt.allChannels = true;
            }

            AudioSource source = speaker.AddComponent<AudioSource>();
            source.spatialize = true;
            source.spatializePostEffects = true;
            source.dopplerLevel = 0;
            source.clip = dclip;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.spatialBlend = 1;
            source.volume = 0.275f;
            source.priority = 0;
            source.Play();
            if (channel == -1)
            {
                source.spatialize = false;
                source.volume = 0.2f;
                source.reverbZoneMix = 1.0f;
                source.spatialBlend = 0;

                //AudioLowPassFilter lowpass = speaker.AddComponent<AudioLowPassFilter>();
                //lowpass.cutoffFrequency = 350;

                silencer silence = speaker.AddComponent<silencer>();


                AudioReverbZone reverb = speaker.AddComponent<AudioReverbZone>();
                reverb.reverbPreset = AudioReverbPreset.Hangar;
            }
            //channel_filter chfilt = speaker.AddComponent<channel_filter>();
            //chfilt.channel = channel;


            return speaker;
        }

        void positionSpeakers()
        {
            if (speakerLeft)
            {
                speakerLeft.transform.localPosition = new Vector3(-sideDistance, 1.5f, frontDistance);
            }
            if (speakerRight)
            {
                speakerRight.transform.localPosition = new Vector3(sideDistance, 1.5f, frontDistance);
            }
            if (speakerResonance)
            {
                speakerResonance.transform.localPosition = new Vector3(0, 3, 0);
            }
        }

        void Create()
        {
            if (speakerLeft)
            {
                Destroy(speakerLeft);
            }
            if (speakerRight)
            {
                Destroy(speakerRight);
            }
            speakerLeft = newSpeaker(0);
            speakerRight = newSpeaker(1);
            if (resonance)
            {
                speakerResonance = newSpeaker(-1);
            }
            positionSpeakers();
        }
    }

    public class audioSplitter : MonoBehaviour
    {

        public float[][] channelData;
        public bool[] beenUsed;
        // Start is called before the first frame update
        void Start()
        {
            AudioSource source = gameObject.GetComponent<AudioSource>();
            source.spatialBlend = 0;
            source.reverbZoneMix = 0;
            source.dopplerLevel = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            int dataLen = data.Length / channels;
            channelData = new float[channels][];
            beenUsed = new bool[channels];
            int c = 0;
            while (c < channels)
            {
                channelData[c] = new float[dataLen];
                beenUsed[c] = false;
                c++;
            }

            int n = 0;
            while (n < dataLen)
            {

                int i = 0;
                while (i < channels)
                {
                    channelData[i][n] = data[n * channels + i];
                    data[n * channels + i] = 0;
                    i++;
                }
                n++;
            }
        }
    }


    public class audioReader : MonoBehaviour
    {
        public int channel = 0;
        public bool allChannels = false;
        public audioSplitter splitter;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            int dataLen = data.Length / channels;
            int n = 0;

            if (allChannels)
            {
                while (n < dataLen)
                {

                    int i = 0;
                    while (i < channels)
                    {
                        data[n * channels + i] = splitter.channelData[i][n];
                        i++;
                    }
                    n++;
                }
            }
            else
            {
                float[] slitData = splitter.channelData[channel];
                while (n < dataLen)
                {

                    int i = 0;
                    while (i < channels)
                    {
                        data[n * channels + i] = slitData[n];
                        i++;
                    }
                    n++;
                }
            }
        }
    }

    public class silencer : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            int dataLen = data.Length / channels;

            int n = 0;
            while (n < dataLen)
            {

                int i = 0;
                while (i < channels)
                {
                    data[n * channels + i] = 0;
                    i++;
                }
                n++;
            }
        }
    }
}
