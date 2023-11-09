using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Switch;

namespace JoyConInput.Editor
{
    public class JoyconDebuggerWindow : EditorWindow
    {
        private bool leftJoyconFold = true;
        // private bool leftGenericFold = true;
        // private bool leftColorFold = true;
        // private bool leftButtonsFold = true;
        // private bool leftIMUFold = true;
        private bool rightJoyconFold = true;

        [MenuItem("Window/Joycon Debugger")]
        public static void ShowWindow()
        {
            JoyconDebuggerWindow wnd = EditorWindow.GetWindow<JoyconDebuggerWindow>();
            wnd.titleContent = new GUIContent("Joycon Debugger");
        }
        
        private void OnGUI()
        {
            leftJoyconFold = SwitchControllerDebugGUI(leftJoyconFold, SwitchJoyConLHID.current, "Left");
            rightJoyconFold = SwitchControllerDebugGUI(rightJoyconFold, SwitchJoyConRHID.current, "Right");
        }

        private bool SwitchControllerDebugGUI(bool fold, SwitchControllerHID joycon, string sidePrefix)
        {

            if (joycon == null)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(sidePrefix + " Joycon Controller", EditorStyles.boldLabel);
                GUILayout.Label("(disconnected)");
                GUILayout.EndHorizontal();
            } 
            else 
            {
                fold = EditorGUILayout.BeginFoldoutHeaderGroup(fold, sidePrefix + " Joycon Controller");

                if (fold)
                {
                    GUILayout.Label("Generic data: ", EditorStyles.boldLabel);

                    EditorGUILayout.LabelField("Firmware version: ", joycon.FirmwareVersion);
                    EditorGUILayout.LabelField("S/N: ", "XXXX");
                    EditorGUILayout.LabelField("MAC: ", joycon.MACAddress);

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Battery level: ", joycon.BatteryLevel.ToString());
                    EditorGUILayout.LabelField("Charging: ", joycon.BatteryIsCharging.ToString());
                    EditorGUILayout.LabelField("Vibrations enabled: ", "WIP");

                    EditorGUILayout.Separator();
                    GUILayout.Label("Device colors: ", EditorStyles.boldLabel);

                    // EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ColorField("Body color: ", joycon.BodyColor);
                    EditorGUILayout.ColorField("Buttons color: ", joycon.ButtonColor);
                    // EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Separator();
                    GUILayout.Label("Buttons data: ", EditorStyles.boldLabel);

                    // TODO: Lateralize it baby
                    EditorGUILayout.LabelField("Stick calibration data: ", joycon.calibrationData.lStickCalibData.ToString());

                    EditorGUILayout.Separator();
                    GUILayout.Label("IMU data: ", EditorStyles.boldLabel);
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
                
            

            return fold;
        }
    }
}
