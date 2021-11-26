using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace cmpy.Audio
{
    public class EAudioPlayer : MonoBehaviour
    {
        private static readonly string[] ignoredProperties = new string[] { "clip", "playOnAwake", "minVolume", "maxVolume", "rolloffFactor" };

        public float masterVolume = 1f;
        public float masterPitch = 1f;

        [Tooltip("Number of audio sources created on Awake.")] public int initialAudioSourceAmount = 5;
        [Tooltip("Number of audio sources created when more are needed.")] public int subsequentAudioSourceAmount = 3;
        public bool useAttachedAudioSource = true;

        private List<AudioSource> sourcePool;
        private Transform audioParent;
        private GameObject templateObject;
        private object templateAudioSource;

        private void Awake()
        {
            sourcePool = new List<AudioSource>();

            audioParent = new GameObject(Application.isEditor ? "AUDIO PARENT" : "").transform;
            audioParent.SetParent(transform, false);
            audioParent.localPosition = Vector3.zero;
            audioParent.rotation = Quaternion.identity;
            audioParent.localScale = Vector3.one;
            audioParent.hideFlags = HideFlags.HideAndDontSave;

            templateObject = new GameObject(Application.isEditor ? "AUDIO TEMPLATE" : "");
            templateObject.transform.SetParent(audioParent, false);

            AudioSource templateSource = templateObject.AddComponent<AudioSource>();

            if (useAttachedAudioSource)
            {
                AudioSource foundAudioSource = GetComponent<AudioSource>();

                // Use reflection to copy over the audio source to a "template."
                if (foundAudioSource)
                {
                    Type type = typeof(AudioSource);
                    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

                    // Set the properties.
                    PropertyInfo[] properties = type.GetProperties(flags);
                    foreach (PropertyInfo property in properties)
                    {
                        // Unity will throw a warning if any of the values specified in this array get modified.
                        if (ignoredProperties.Contains(property.Name)) continue;

                        if (property.CanWrite)
                        {
                            try
                            {
                                property.SetValue(templateAudioSource, property.GetValue(foundAudioSource, null), null);
                            }
                            catch (NotImplementedException) { }
                        }
                    }

                    // Set the fields.
                    foreach (FieldInfo field in type.GetFields(flags)) field.SetValue(templateAudioSource, field.GetValue(foundAudioSource));
                }
            }
        }

        public void CreateAudioSources(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject obj = Instantiate(templateObject);

                obj.transform.SetParent(audioParent, false);
                if (Application.isEditor) obj.name = "AUDIO SOURCE";
                sourcePool.Add(obj.GetComponent<AudioSource>());
            }
        }

        public AudioSource AcquireAudioSource(bool createIfNeeded = true)
        {
            AudioSource found = sourcePool.Find(source => !source.isPlaying);

            if (!found && createIfNeeded)
            {
                CreateAudioSources(subsequentAudioSourceAmount);
                return AcquireAudioSource(createIfNeeded: false);
            }
            else if (!found && !createIfNeeded)
            {
                throw new OutOfAudioSourcesException();
            }

            return found;
        }

        public AudioSource Play(SoundEffect effect)
        {
            AudioSource source = AcquireAudioSource();

            source.clip = effect.Clip;
            source.volume = effect.Volume * masterVolume;
            source.pitch = effect.Pitch * masterPitch;

            source.Play();

            return source;
        }

        public class OutOfAudioSourcesException : Exception
        {
            public OutOfAudioSourcesException() : base("There are no AudioSources available.") { }
        }
    }
}
