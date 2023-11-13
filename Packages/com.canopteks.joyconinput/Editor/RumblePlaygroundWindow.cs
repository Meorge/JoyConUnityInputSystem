using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Switch;

namespace JoyConInput.Editor
{
    public class RumblePlaygroundWindow : EditorWindow
    {
        private bool joyconsControlFold = true;

        private float highBandFrequencyL;
        private float highBandAmplitudeL;
        private float lowBandFrequencyL;
        private float lowBandAmplitudeL;
    
        private float highBandFrequencyR;
        private float highBandAmplitudeR;
        private float lowBandFrequencyR;
        private float lowBandAmplitudeR;

        private bool profilePlayerFold = false;

        [MenuItem("Window/Joycon Debugger/Rumble Playground")]
        public static void ShowWindow()
        {
            RumblePlaygroundWindow wnd = EditorWindow.GetWindow<RumblePlaygroundWindow>();
            wnd.titleContent = new GUIContent("Joycon Rumble Playground");
        }

        private void OnGUI()
        {
            joyconsControlFold = JoyConRumblePlayground(joyconsControlFold);
        }

        private bool JoyConRumblePlayground(bool fold)
        {
            fold = EditorGUILayout.BeginFoldoutHeaderGroup(fold, "Joycons precise rumble");
            SwitchControllerHID leftJoycon = SwitchJoyConLHID.current;
            SwitchControllerHID rightJoycon = SwitchJoyConRHID.current;

            if (fold)
            {
                if (leftJoycon == null)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label("Left Joycon Controller", EditorStyles.boldLabel);
                    GUILayout.Label("(disconnected)");
                    GUILayout.EndHorizontal();
                }
                else 
                {
                    GUILayout.Label("Left Joycon: ", EditorStyles.boldLabel);
                    highBandFrequencyL = EditorGUILayout.Slider(highBandFrequencyL, 82, 1253);
                    highBandAmplitudeL = EditorGUILayout.Slider(highBandAmplitudeL, 0, 1);
                    lowBandFrequencyL = EditorGUILayout.Slider(lowBandFrequencyL, 41, 626);
                    lowBandAmplitudeL = EditorGUILayout.Slider(lowBandAmplitudeL, 0, 1);
                }

                
                EditorGUILayout.Separator();
                
                if (rightJoycon == null)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label("Right Joycon Controller", EditorStyles.boldLabel);
                    GUILayout.Label("(disconnected)");
                    GUILayout.EndHorizontal();
                }
                else 
                {
                    GUILayout.Label("Right Joycon: ", EditorStyles.boldLabel);
                    highBandFrequencyR = EditorGUILayout.Slider(highBandFrequencyR, 82, 1253);
                    highBandAmplitudeR = EditorGUILayout.Slider(highBandAmplitudeR, 0, 1);
                    lowBandFrequencyR = EditorGUILayout.Slider(lowBandFrequencyR, 41, 626);
                    lowBandAmplitudeR = EditorGUILayout.Slider(lowBandAmplitudeR, 0, 1);
                }

                EditorGUILayout.Separator();

                if (GUILayout.Button("Play the defined vibration"))
                    {
                        PlayRumble();
                    }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            return fold;
        }

        private void PlayRumble()
        {
            Debug.Log("Brrrrrr");
            // TODO: todo
        }

        private bool RumbleProfilePlayer(bool fold)
        {
            // TODO: Create a ScriptableObject to store rumble profiles
            return fold;
        }
    }
}
