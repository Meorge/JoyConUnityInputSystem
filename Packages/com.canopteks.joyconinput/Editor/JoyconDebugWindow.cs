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
                bool isLeft = joycon.SpecificControllerType == SpecificControllerTypeEnum.LeftJoyCon;
                bool isRight = joycon.SpecificControllerType == SpecificControllerTypeEnum.RightJoyCon;

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

                    // TODO: Nice dislay for the buttons

                    string calibrationText = "";
                    if (isLeft)     calibrationText = joycon.calibrationData.lStickCalibData.ToString();
                    if (isRight)    calibrationText = joycon.calibrationData.rStickCalibData.ToString();
                    EditorGUILayout.LabelField("Stick calibration data: ", calibrationText);

                    EditorGUILayout.Separator();
                    GUILayout.Label("IMU data: ", EditorStyles.boldLabel);

                    EditorGUILayout.LabelField("Angular velocity: ", joycon.angularVelocity.value.ToString());
                    EditorGUILayout.LabelField("Orientation: ", joycon.orientation.value.ToString());
                    EditorGUILayout.LabelField("Acceleration: ", joycon.acceleration.value.ToString());
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
                
            

            return fold;
        }
    }
}
