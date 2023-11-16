using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Switch;

namespace JoyConInput.Editor
{
    /// <summary>
    /// Editor window allowing to test different values for the rumble profiles, as well as files.
    /// </summary>
    public class RumblePlaygroundWindow : EditorWindow
    {
        private bool profilePlaygroundFold = true;
        private SwitchControllerRumbleProfile rumbleProfile = SwitchControllerRumbleProfile.CreateNeutral();
        private bool playProfileToggle = false;
        private bool playProfileToggledLastFrame = false;
        private bool dataSheetFold = false;

        [MenuItem("Window/Switch Controller Debugger/Rumble Playground")]
        public static void ShowWindow()
        {
            RumblePlaygroundWindow wnd = EditorWindow.GetWindow<RumblePlaygroundWindow>();
            wnd.titleContent = new GUIContent("Joycon Rumble Playground");
        }

        private void OnGUI()
        {
            profilePlaygroundFold = RumbleProfilePlayground(profilePlaygroundFold);
            dataSheetFold = RumbleDataSheetPlayer(dataSheetFold);
        }

        private bool RumbleProfilePlayground(bool fold)
        {
            fold = EditorGUILayout.BeginFoldoutHeaderGroup(fold, "Controllers precise rumble");

            if (fold)
            {
                if (SwitchJoyConLHID.current == null && SwitchProControllerNewHID.current == null)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label("Left controller", EditorStyles.boldLabel);
                    GUILayout.Label("(disconnected)");
                    GUILayout.EndHorizontal();
                }
                else 
                {
                    GUILayout.Label("Left controller: ", EditorStyles.boldLabel);
                    rumbleProfile.highBandFrequencyLeft = EditorGUILayout.Slider("High band frequency: ", rumbleProfile.highBandFrequencyLeft, 82, 1253);
                    rumbleProfile.highBandAmplitudeLeft = EditorGUILayout.Slider("High band amplitude: ", rumbleProfile.highBandAmplitudeLeft, 0, 1);
                    rumbleProfile.lowBandFrequencyLeft = EditorGUILayout.Slider("Low band frequency: ", rumbleProfile.lowBandFrequencyLeft, 41, 626);
                    rumbleProfile.lowBandAmplitudeLeft = EditorGUILayout.Slider("Low band amplitude: ", rumbleProfile.lowBandAmplitudeLeft, 0, 1);
                }

                EditorGUILayout.Separator();
                
                if (SwitchJoyConRHID.current == null && SwitchProControllerNewHID.current == null)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label("Right controller", EditorStyles.boldLabel);
                    GUILayout.Label("(disconnected)");
                    GUILayout.EndHorizontal();
                }
                else 
                {
                    GUILayout.Label("Right controller: ", EditorStyles.boldLabel);
                    rumbleProfile.highBandFrequencyRight = EditorGUILayout.Slider("High band frequency: ", rumbleProfile.highBandFrequencyRight, 82, 1253);
                    rumbleProfile.highBandAmplitudeRight = EditorGUILayout.Slider("High band amplitude: ", rumbleProfile.highBandAmplitudeRight, 0, 1);
                    rumbleProfile.lowBandFrequencyRight = EditorGUILayout.Slider("Low band frequency: ", rumbleProfile.lowBandFrequencyRight, 41, 626);
                    rumbleProfile.lowBandAmplitudeRight = EditorGUILayout.Slider("Low band amplitude: ", rumbleProfile.lowBandAmplitudeRight, 0, 1);
                }

                EditorGUILayout.Separator();

                playProfileToggle = GUILayout.Toggle(playProfileToggle, "Play the defined vibration");
                playProfileToggledLastFrame |= playProfileToggle;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            return fold;
        }

        /// <summary>
        /// Send the rumble data the whatever controller connected.
        /// </summary>
        /// <param name="rumble"></param>
        private void PlayRumble(SwitchControllerRumbleProfile rumble)
        {
            rumble.PlayOn(SwitchProControllerNewHID.current, SwitchJoyConLHID.current, SwitchJoyConRHID.current);
        }

        /// <summary>
        /// Create a "Play" button for each <see cref="RumbleDataSheet"/> asset currently selected. 
        /// </summary>
        /// <param name="fold">Is the panel folded or not</param>
        /// <returns></returns>
        private bool RumbleDataSheetPlayer(bool fold)
        {
            // Grab the RumbleDataSheet assets from the selection
            Object[] selection = Selection.GetFiltered(typeof(RumbleDataSheet), SelectionMode.Assets);

            // If nothing is select, don't bother displaying the fold
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
                // Each select file matches with a little play button to test the sequences of vibrations and stuff
                foreach (Object asset in selection)
                {
                    RumbleDataSheet rumbleAsset = asset as RumbleDataSheet;
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label(asset.name + ": ");

                    //! Sending a single rumble profile makes it slowly fades over roughly 1s, which I find not great
                    // TODO: Craft a dynamic sized array for button values to make the vibrations snappy
                    if (GUILayout.Button("Play"))
                    {
                        PlayRumble(rumbleAsset.profile);
                    }
                    GUILayout.EndHorizontal();
                }
            }

            return fold;
        }
        
        /// <summary>
        /// Execute roughly 10 times/s.
        /// </summary>
        private void OnInspectorUpdate()
        {
            // We have to do that in the InspectorUpdate or the irregular refresh rate messes with the vibrations
            if (playProfileToggle)
            {
                PlayRumble(rumbleProfile);
            }
            else if (playProfileToggledLastFrame)
            {
                PlayRumble(SwitchControllerRumbleProfile.CreateNeutral());
                playProfileToggledLastFrame = false;
            }
        }
    }
}
