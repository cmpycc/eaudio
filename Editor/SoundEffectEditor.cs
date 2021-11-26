using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace cmpy.Audio
{
    [CustomEditor(typeof(SoundEffect))]
    public class SoundEffectEditor : Editor
    {
        private SoundEffect effect;

        private void OnEnable()
        {
            effect = (SoundEffect)target;
        }

        public override void OnInspectorGUI()
        {
            Header("General");

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SoundEffect.clips)));
            if (effect.clips.Length > 1)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SoundEffect.selectionType)));
            }

            Header("Volume");

            effect.randomizeVolume = EditorGUILayout.Toggle("Randomize Volume?", effect.randomizeVolume);
            string volumeLabel = effect.randomizeVolume ? "Volume Base" : "Volume";

            effect.volumeBase = EditorGUILayout.FloatField(volumeLabel, effect.volumeBase);
            if (effect.randomizeVolume)
            {
                effect.volumeVariation = EditorGUILayout.FloatField("Volume Variation", effect.volumeVariation);
                GUILayout.Label($"Volume will range from {effect.volumeBase - effect.volumeVariation} to {effect.volumeBase + effect.volumeVariation}.");
            }

            Header("Pitch");

            effect.randomizePitch = EditorGUILayout.Toggle("Randomize Pitch?", effect.randomizePitch);
            string pitchLabel = effect.randomizePitch ? "Pitch Base" : "Pitch";

            effect.pitchBase = EditorGUILayout.FloatField(pitchLabel, effect.pitchBase);
            if (effect.randomizePitch)
            {
                effect.pitchVariation = EditorGUILayout.FloatField("Pitch Variation", effect.pitchVariation);
                GUILayout.Label($"Pitch will range from {effect.pitchBase - effect.pitchVariation} to {effect.pitchBase + effect.pitchVariation}.");
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void Header(string text)
        {
            EditorGUILayout.Space(3f);
            EditorGUILayout.LabelField(text, new GUIStyle(EditorStyles.boldLabel) { fontSize = 15 });
            EditorGUILayout.Space(5f);
        }
    }
}
