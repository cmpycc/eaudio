using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

namespace cmpy.Audio
{
    public enum ClipSelectionType
    {
        /// <summary>
        /// Clip will be randomly selected.
        /// </summary>
        Random,
        /// <summary>
        /// Clip will be selected randomly but will never be duplicated.
        /// </summary>
        Unique,
        /// <summary>
        /// Clip will be selected sequentially.
        /// </summary>
        Sequential,
    }

    [CreateAssetMenu(menuName = "E-Audio/Sound Effect")]
    public class SoundEffect : ScriptableObject
    {
        public AudioClip[] clips = new AudioClip[0];
        public ClipSelectionType selectionType = ClipSelectionType.Unique; private int lastClipIndex = -1; // Only used for the Sequential and Unique selection types.

        public bool randomizeVolume = false;
        public float volumeBase = 1f;
        public float volumeVariation = 0.2f;

        public bool randomizePitch = false;
        public float pitchBase = 1f;
        public float pitchVariation = 0.1f;

        public float Volume => randomizeVolume ? volumeBase + Random.Range(-volumeVariation, volumeVariation) : volumeBase;
        public float Pitch => randomizePitch ? pitchBase + Random.Range(-pitchVariation, pitchVariation) : pitchBase;

        public AudioClip Clip
        {
            get
            {
                if (clips.Length <= 1) return clips[0];

                switch (selectionType)
                {
                    case ClipSelectionType.Random:
                        return clips[Random.Range(0, clips.Length)];
                    case ClipSelectionType.Unique:
                        int clipIndex;

                        if (lastClipIndex == -1)
                        {
                            clipIndex = Random.Range(0, clips.Length);

                            lastClipIndex = clipIndex;
                            return clips[clipIndex];
                        }

                        clipIndex = Random.Range(0, clips.Length - 1);
                        if (clipIndex >= lastClipIndex) clipIndex++;

                        lastClipIndex = clipIndex;
                        return clips[clipIndex];
                    case ClipSelectionType.Sequential:
                        lastClipIndex++;
                        if (lastClipIndex == -1 || lastClipIndex >= clips.Length)
                        {
                            lastClipIndex = 0;
                        }

                        return clips[lastClipIndex];
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
