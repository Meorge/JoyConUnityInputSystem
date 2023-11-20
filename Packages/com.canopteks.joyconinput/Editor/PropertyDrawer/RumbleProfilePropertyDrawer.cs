using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Switch;

namespace JoyConInput.Editor
{
    /// <summary>
    /// Custom property editor for <see cref="SwitchControllerRumbleProfile"/> with ranged sliders and a test button.
    /// </summary>
    [CustomPropertyDrawer(typeof(SwitchControllerRumbleProfile))]
    public class RumbleProfilePropertyDrawer : PropertyDrawer
    {
        // Ranges for high and low band frequencies, and amplitudes
        private const float highBandFrequencyMin = 82f;
        private const float highBandFrequencyMax = 1253f;
        private const float lowBandFrequencyMin = 41f;
        private const float lowBandFrequencyMax = 626f;
        private const float amplitudeMin = 0f;
        private const float amplitudeMax = 1f;
         

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Rect and Serialized properties for the left hand values
            Rect leftHandLabelRect = new Rect(position.x - 5, position.y, position.width, 30);

            SerializedProperty leftHighBandFrequencyProperty = property.FindPropertyRelative("highBandFrequencyLeft");
            Rect leftHighBandFrequencyRect = new Rect(position.x, leftHandLabelRect.yMax, position.width, EditorGUI.GetPropertyHeight(leftHighBandFrequencyProperty));
            
            SerializedProperty leftHighBandAmplitudeProperty = property.FindPropertyRelative("highBandAmplitudeLeft");
            Rect leftHighBandAmplitudeRect = new Rect(position.x, leftHighBandFrequencyRect.yMax, position.width, EditorGUI.GetPropertyHeight(leftHighBandAmplitudeProperty));

            SerializedProperty leftLowBandFrequencyProperty = property.FindPropertyRelative("lowBandFrequencyLeft");
            Rect leftLowBandFrequencyRect = new Rect(position.x, leftHighBandAmplitudeRect.yMax, position.width, EditorGUI.GetPropertyHeight(leftLowBandFrequencyProperty));

            SerializedProperty leftLowBandAmplitudeProperty = property.FindPropertyRelative("lowBandAmplitudeLeft");
            Rect leftLowBandAmplitudeRect = new Rect(position.x, leftLowBandFrequencyRect.yMax, position.width, EditorGUI.GetPropertyHeight(leftLowBandAmplitudeProperty));

            // Rect and Serialized properties for the right hand values
            Rect rightHandLabelRect = new Rect(position.x - 5, leftLowBandAmplitudeRect.yMax + 10, position.width, 30);

            SerializedProperty rightHighBandFrequencyProperty = property.FindPropertyRelative("highBandFrequencyRight");
            Rect rightHighBandFrequencyRect = new Rect(position.x, rightHandLabelRect.yMax, position.width, EditorGUI.GetPropertyHeight(rightHighBandFrequencyProperty));

            SerializedProperty rightHighBandAmplitudeProperty = property.FindPropertyRelative("highBandAmplitudeRight");
            Rect rightHighBandAmplitudeRect = new Rect(position.x, rightHighBandFrequencyRect.yMax, position.width, EditorGUI.GetPropertyHeight(rightHighBandAmplitudeProperty));

            SerializedProperty rightLowBandFrequencyProperty = property.FindPropertyRelative("lowBandFrequencyRight");
            Rect rightLowBandFrequencyRect = new Rect(position.x, rightHighBandAmplitudeRect.yMax, position.width, EditorGUI.GetPropertyHeight(rightLowBandFrequencyProperty));

            SerializedProperty rightLowBandAmplitudeProperty = property.FindPropertyRelative("lowBandAmplitudeRight");
            Rect rightLowBandAmplitudeRect = new Rect(position.x, rightLowBandFrequencyRect.yMax, position.width, EditorGUI.GetPropertyHeight(rightLowBandAmplitudeProperty));

            // Test button to play the data
            Rect rumbleProfileTestRect = new Rect(position.x - 5, rightLowBandAmplitudeRect.yMax + 10, position.width, 30);

            // GUI elements 
            EditorGUI.LabelField(leftHandLabelRect, "Left hand: ", EditorStyles.boldLabel);
            EditorGUI.Slider(leftHighBandFrequencyRect, leftHighBandFrequencyProperty, highBandFrequencyMin, highBandFrequencyMax, "High-band frequency");
            EditorGUI.Slider(leftHighBandAmplitudeRect, leftHighBandAmplitudeProperty, amplitudeMin, amplitudeMax, "High-band amplitude");
            EditorGUI.Slider(leftLowBandFrequencyRect, leftLowBandFrequencyProperty, lowBandFrequencyMin, lowBandFrequencyMax, "Low-band frequency");
            EditorGUI.Slider(leftLowBandAmplitudeRect, leftLowBandAmplitudeProperty, amplitudeMin, amplitudeMax, "Low-band amplitude");
            EditorGUI.LabelField(rightHandLabelRect, "Right hand: ", EditorStyles.boldLabel);
            EditorGUI.Slider(rightHighBandFrequencyRect, rightHighBandFrequencyProperty, highBandFrequencyMin, highBandFrequencyMax, "High-band frequency");
            EditorGUI.Slider(rightHighBandAmplitudeRect, rightHighBandAmplitudeProperty, amplitudeMin, amplitudeMax, "High-band amplitude");
            EditorGUI.Slider(rightLowBandFrequencyRect, rightLowBandFrequencyProperty, lowBandFrequencyMin, lowBandFrequencyMax, "Low-band frequency");
            EditorGUI.Slider(rightLowBandAmplitudeRect, rightLowBandAmplitudeProperty, amplitudeMin, amplitudeMax, "Low-band amplitude");

            // Play button to test the rumble profile
            bool playingRumbleProfile = GUI.Button(rumbleProfileTestRect, "Play");
            if (playingRumbleProfile)
            {
                SwitchControllerRumbleProfile profile = SwitchControllerRumbleProfile.CreateEmpty();
                profile.highBandFrequencyLeft = leftHighBandFrequencyProperty.floatValue;
                profile.highBandAmplitudeLeft = leftHighBandAmplitudeProperty.floatValue;
                profile.lowBandFrequencyLeft = leftLowBandFrequencyProperty.floatValue;
                profile.lowBandAmplitudeLeft = leftLowBandAmplitudeProperty.floatValue;
                profile.highBandFrequencyRight = rightHighBandFrequencyProperty.floatValue;
                profile.highBandAmplitudeRight = rightHighBandAmplitudeProperty.floatValue;
                profile.lowBandFrequencyRight = rightLowBandFrequencyProperty.floatValue;
                profile.lowBandAmplitudeRight = rightLowBandAmplitudeProperty.floatValue;

                profile.PlayOn(SwitchProControllerNewHID.current, SwitchJoyConLHID.current, SwitchJoyConRHID.current);
            }

            EditorGUI.EndProperty();
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 
                30f +                                                                                   // Left hand label
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("highBandFrequencyLeft")) + 
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("highBandAmplitudeLeft")) +
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("lowBandFrequencyLeft")) +
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("lowBandAmplitudeLeft")) +
                10f +                                                                                   // Spacing
                30f +                                                                                   // Right hand label
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("highBandFrequencyRight")) +
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("highBandAmplitudeRight")) +
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("lowBandFrequencyRight")) +
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("lowBandAmplitudeRight")) +
                10f +                                                                                   // Spacing
                30f;                                                                                    // Test button

            return height;
        }
    }
}
