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
        private SwitchControllerRumbleProfile rumbleProfile = SwitchControllerRumbleProfile.CreateNeutral();

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

            profilePlayerFold = RumbleProfilePlayer(profilePlayerFold);
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
                    rumbleProfile.highBandFrequencyLeft = EditorGUILayout.Slider("High band frequency: ", rumbleProfile.highBandFrequencyLeft, 82, 1253);
                    rumbleProfile.highBandAmplitudeLeft = EditorGUILayout.Slider("High band amplitude: ", rumbleProfile.highBandAmplitudeLeft, 0, 1);
                    rumbleProfile.lowBandFrequencyLeft = EditorGUILayout.Slider("Low band frequency: ", rumbleProfile.lowBandFrequencyLeft, 41, 626);
                    rumbleProfile.lowBandAmplitudeLeft = EditorGUILayout.Slider("Low band amplitude: ", rumbleProfile.lowBandAmplitudeLeft, 0, 1);
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
                    rumbleProfile.highBandFrequencyRight = EditorGUILayout.Slider("High band frequency: ", rumbleProfile.highBandFrequencyRight, 82, 1253);
                    rumbleProfile.highBandAmplitudeRight = EditorGUILayout.Slider("High band amplitude: ", rumbleProfile.highBandAmplitudeRight, 0, 1);
                    rumbleProfile.lowBandFrequencyRight = EditorGUILayout.Slider("Low band frequency: ", rumbleProfile.lowBandFrequencyRight, 41, 626);
                    rumbleProfile.lowBandAmplitudeRight = EditorGUILayout.Slider("Low band amplitude: ", rumbleProfile.lowBandAmplitudeRight, 0, 1);
                }

                EditorGUILayout.Separator();

                if (GUILayout.Button("Play the defined vibration"))
                    {
                        PlayRumble(rumbleProfile);
                    }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            return fold;
        }

        private void PlayRumble(SwitchControllerRumbleProfile rumble)
        {
            SwitchControllerHID leftJoycon = SwitchJoyConLHID.current;
            SwitchControllerHID rightJoycon = SwitchJoyConRHID.current;

            if (leftJoycon != null)
            {
                leftJoycon.Rumble(rumble);
            }

            if (rightJoycon != null)
            {
                rightJoycon.Rumble(rumble);
            }

            // Test: abort the vibration
            // var brr = SwitchControllerRumbleProfile.CreateNeutral();
            // if (leftJoycon != null)
            // {
            //     leftJoycon.Rumble(brr);
            // }

            // if (rightJoycon != null)
            // {
            //     rightJoycon.Rumble(brr);
            // }
        }

        private bool RumbleProfilePlayer(bool fold)
        {

            Object[] selection = Selection.GetFiltered(typeof(RumbleDataSheet), SelectionMode.Assets);
            if(selection.Length == 0)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Rumble data sheet player: ", EditorStyles.boldLabel);
                GUILayout.Label("(none selected)");
                GUILayout.EndHorizontal();
                return fold;
            }
            fold = EditorGUILayout.BeginFoldoutHeaderGroup(fold, "Rumble data sheet player: ");

            if (fold)
            {
                foreach (Object asset in selection)
                {
                    RumbleDataSheet rumbleAsset = asset as RumbleDataSheet;
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label(asset.name + ": ");
                    if (GUILayout.Button("Play"))
                    {
                        PlayRumble(rumbleAsset.rumbleProfile);
                    }
                    GUILayout.EndHorizontal();
                }
            }

            return fold;
        }
    }
}
