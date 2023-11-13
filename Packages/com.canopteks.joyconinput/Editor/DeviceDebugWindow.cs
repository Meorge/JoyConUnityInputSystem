using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Switch;

namespace JoyConInput.Editor
{
    public class DeviceDebugWindow : EditorWindow
    {
        private bool leftJoyconFold = true;
        private bool rightJoyconFold = true;
        private Texture2D deviceColorBackground;
        private GUIStyle deviceColorStyle = new GUIStyle();
        private bool readIMU = false;

        private void OnEnable()
        {
            deviceColorBackground = Texture2D.whiteTexture;
            deviceColorBackground.wrapMode = TextureWrapMode.Repeat;
            deviceColorStyle.normal.background = deviceColorBackground;
        }
        
        [MenuItem("Window/Joycon Debugger/Joycon Device Data")]
        public static void ShowWindow()
        {
            DeviceDebugWindow wnd = EditorWindow.GetWindow<DeviceDebugWindow>();
            wnd.titleContent = new GUIContent("Joycon Device Data");
        }
        
        private void OnGUI()
        {
            leftJoyconFold = JoyconDebugGUI(leftJoyconFold, SwitchJoyConLHID.current, "Left");
            rightJoyconFold = JoyconDebugGUI(rightJoyconFold, SwitchJoyConRHID.current, "Right");
        }

        private bool JoyconDebugGUI(bool fold, SwitchControllerHID joycon, string sidePrefix)
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
                    EditorGUILayout.LabelField("Serial number: ", joycon.SerialNumber);
                    EditorGUILayout.LabelField("MAC: ", joycon.MACAddress);

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Battery level: ", joycon.BatteryLevel.ToString());
                    EditorGUILayout.LabelField("Charging: ", joycon.BatteryIsCharging.ToString());

                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Device colors: ", EditorStyles.boldLabel);

                    // Display the device colors as a little square in a big square
                    Color oldColor = GUI.backgroundColor;
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    GUI.backgroundColor = joycon.BodyColor;
                    GUILayout.Box("", deviceColorStyle, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(90), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(30));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Box("", deviceColorStyle, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(30), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(30));
                    GUI.backgroundColor = joycon.ButtonColor;
                    GUILayout.Box("", deviceColorStyle, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(30), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(30));
                    GUI.backgroundColor = joycon.BodyColor;
                    GUILayout.Box("", deviceColorStyle, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(30), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(30));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Box("", deviceColorStyle, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(90), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(30));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    GUI.backgroundColor = oldColor;

                    EditorGUILayout.Separator();
                    GUILayout.Label("Stick data: ", EditorStyles.boldLabel);

                    string calibrationText = "";
                    if (isLeft)     calibrationText = joycon.calibrationData.lStickCalibData.ToString();
                    if (isRight)    calibrationText = joycon.calibrationData.rStickCalibData.ToString();
                    EditorGUILayout.LabelField("Stick calibration data: ", calibrationText);

                    EditorGUILayout.Separator();
                    GUILayout.Label("IMU data: ", EditorStyles.boldLabel);

                    EditorGUILayout.LabelField("Angular velocity: ", joycon.angularVelocity.value.ToString());
                    EditorGUILayout.LabelField("Orientation: ", joycon.orientation.value.ToString());
                    EditorGUILayout.LabelField("Acceleration: ", joycon.acceleration.value.ToString());

                    readIMU = EditorGUILayout.Toggle("Fast-frequency display: ", readIMU);

                    if (GUILayout.Button("Enable IMU"))
                    {
                        joycon.SetIMUEnabled(true);
                    }

                    EditorGUILayout.Separator();
                    GUILayout.Label("Debug: ", EditorStyles.boldLabel);
                    if (GUILayout.Button("Tst"))
                    {
                        //
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            return fold;
        }

        private void OnInspectorUpdate()
        {
            if (readIMU)
                Repaint();
        }
    }
}
