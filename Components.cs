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
using UnityEngine.Audio;


namespace MusicSpatializer
{
    public class SpeakerCreator : MonoBehaviour
    {

        public AudioClip dclip;
        public GameObject speakerLeft;
        public GameObject speakerRight;
        public GameObject speakerResonance;
        public GameObject speakerBass;
        public float frontDistance = 5;
        public float sideDistance = 3f;
        public float volumeMultiplier = 1;
        public AudioSplitter splitter;
        public AudioMixerGroup mixerGroup;
        public bool resonance = true;
        public bool bassBoost = false;
        public bool doRotation = true;
        public bool debugSpheres = false;
        public Plugin pluginReference;
        private GameObject rotationMarker;
        private int rotationMarkerTries = 10;



        // Start is called before the first frame update
        void Start()
        {
            Create();
            pluginReference.InjectAfterStart();
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
                            PositionSpeakers();
                        }
                        rotationMarkerTries--;
                    }
                }
            }
        }

        GameObject NewSpeaker(int channel)
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

            AudioReader chfilt = speaker.AddComponent<AudioReader>();
            chfilt.channel = channel;
            chfilt.splitter = splitter;
            if (channel == -1 || channel == 21)
            {
                chfilt.allChannels = true;
            }

            AudioSource source = speaker.AddComponent<AudioSource>();
            if (mixerGroup != null)
            {
                source.outputAudioMixerGroup = mixerGroup;
            }
            source.spatialize = true;
            source.spatializePostEffects = true;
            source.dopplerLevel = 0;
            source.clip = dclip;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.spatialBlend = 1;
            source.volume = 0.275f * volumeMultiplier;
            source.priority = 0;
            source.Play();
            if (channel == -1)
            {
                source.spatialize = false;
                source.volume = 0.2f * volumeMultiplier;
                source.reverbZoneMix = 1.0f;
                source.spatialBlend = 0;

                //AudioLowPassFilter lowpass = speaker.AddComponent<AudioLowPassFilter>();
                //lowpass.cutoffFrequency = 350;

                Silencer silence = speaker.AddComponent<Silencer>();


                AudioReverbZone reverb = speaker.AddComponent<AudioReverbZone>();
                reverb.reverbPreset = AudioReverbPreset.Hangar;
            }


            if (channel == 21)
            {
                AudioLowPassFilter lowpass = speaker.AddComponent<AudioLowPassFilter>();
                lowpass.cutoffFrequency = 300;
                //source.spatialize = false;
                source.volume = 0.5f * volumeMultiplier;
            }

            //channel_filter chfilt = speaker.AddComponent<channel_filter>();
            //chfilt.channel = channel;


            return speaker;
        }

        void PositionSpeakers()
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
            if (speakerBass)
            {
                speakerBass.transform.localPosition = new Vector3(0, -3f, 10f);
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
            speakerLeft = NewSpeaker(0);
            speakerRight = NewSpeaker(1);
            if (resonance)
            {
                speakerResonance = NewSpeaker(-1);
            }
            if (bassBoost)
            {
                speakerBass = NewSpeaker(21);
            }
            PositionSpeakers();
        }
    }

    public class AudioSplitter : MonoBehaviour
    {

        public float[][] channelData;
        public bool[] beenUsed;
        public bool ready = false;
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
            ready = true;
        }
    }


    public class AudioReader : MonoBehaviour
    {
        public int channel = 0;
        public bool allChannels = false;
        public AudioSplitter splitter;

        // Start is called before the first frame update
        void Start()
        {


            //Console.WriteLine("sample {0}", slitData[0]);
        }

        // Update is called once per frame
        void Update()
        {
            /*
            Console.WriteLine("======");
            //Component[] comps = transform.GetComponents(typeof(Component));
            Component[] comps = splitter.GetComponents(typeof(Component));
            foreach (Component c in comps)
            {
                Type ctype = c.GetType();
                Console.WriteLine("Component name: {0}", ctype.ToString());
            }//*/
            //AudioSource source=transform.GetComponent<AudioSource>();
            //AudioSource mainSource=splitter.GetComponent<AudioSource>();
            //Console.WriteLine("AudioSource vol:{0} active:{1} mute:{2}", source.volume, source.isPlaying, source.mute);
            //Console.WriteLine("mainSource vol:{0} active:{1} mute:{2} priority:{3}", mainSource.volume, mainSource.isPlaying, mainSource.mute, mainSource.priority); ;
            //source.Pause();
            //source.Play();
            //source.outputAudioMixerGroup = mainSource.outputAudioMixerGroup;
        }

        

        void OnAudioFilterRead(float[] data, int channels)
        {

            if(splitter.ready == false)
            {
                return;
            }

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

    public class Silencer : MonoBehaviour
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
