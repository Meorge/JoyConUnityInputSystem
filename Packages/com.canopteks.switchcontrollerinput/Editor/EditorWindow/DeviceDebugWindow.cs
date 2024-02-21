using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Switch;

namespace SwitchControllerInput.Editor
{
    /// <summary>
    /// Editor window displaying the informations of the controllers that can't be seen in the Input Debugger
    /// </summary>
    public class DeviceDebugWindow : EditorWindow
    {
        private bool leftJoyconFold = true;
        private bool rightJoyconFold = true;
        private bool proControllerFold = true;
        private Texture2D deviceColorBackground;
        private GUIStyle deviceColorStyle = new GUIStyle();
        private bool readIMU = false;

        private void OnEnable()
        {
            // Workaround for the device color display
            // The GUI.backgroundColor works as a tint for the style background texture, so we have to set it to white 
            deviceColorBackground = Texture2D.whiteTexture;
            deviceColorBackground.wrapMode = TextureWrapMode.Repeat;
            deviceColorStyle.normal.background = deviceColorBackground;
        }
        
        [MenuItem("Window/Switch Controller Debugger/Switch Controller Data")]
        public static void ShowWindow()
        {
            DeviceDebugWindow wnd = EditorWindow.GetWindow<DeviceDebugWindow>();
            wnd.titleContent = new GUIContent("Joycon Device Data");
        }
        
        private void OnGUI()
        {
            leftJoyconFold = ControllerDebugGUI(leftJoyconFold, SwitchJoyConLHID.current, "Left Joycon");
            rightJoyconFold = ControllerDebugGUI(rightJoyconFold, SwitchJoyConRHID.current, "Right Joycon");
            proControllerFold = ControllerDebugGUI(proControllerFold, SwitchProControllerNewHID.current, "Pro");
        }

        private bool ControllerDebugGUI(bool fold, SwitchControllerHID controller, string prefix)
        {
            if (controller == null)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(prefix + " Joycon Controller", EditorStyles.boldLabel);
                GUILayout.Label("(disconnected)");
                GUILayout.EndHorizontal();
            } 
            else 
            {
                bool isLeft = controller.SpecificControllerType == SpecificControllerTypeEnum.LeftJoyCon;
                bool isRight = controller.SpecificControllerType == SpecificControllerTypeEnum.RightJoyCon;

                fold = EditorGUILayout.BeginFoldoutHeaderGroup(fold, prefix + " Controller");

                if (fold)
                {
                    GUILayout.Label("Generic data: ", EditorStyles.boldLabel);
                    // TODO: Make a tooltip with the real property name for each property display
                    GUILayout.Label(new GUIContent($"Firmware version: {controller.FirmwareVersion}", "FirmwareVersion"));
                    EditorGUILayout.LabelField("Serial number: ", controller.SerialNumber);
                    EditorGUILayout.LabelField("MAC: ", controller.MACAddress);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Battery level: ", controller.BatteryLevel.ToString());
                    EditorGUILayout.LabelField("Charging: ", controller.BatteryIsCharging.ToString());
                    EditorGUILayout.Separator();

                    // Display the device colors as a little square (button) in a big square (body)
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Device colors: ", EditorStyles.boldLabel);
                    Color oldColor = GUI.backgroundColor;
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    GUI.backgroundColor = controller.BodyColor;
                    GUILayout.Box("", deviceColorStyle, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(90), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(30));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Box("", deviceColorStyle, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(30), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(30));
                    GUI.backgroundColor = controller.ButtonColor;
                    GUILayout.Box("", deviceColorStyle, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(30), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(30));
                    GUI.backgroundColor = controller.BodyColor;
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
                    if (isLeft)     calibrationText = controller.calibrationData.lStickCalibData.ToString();
                    if (isRight)    calibrationText = controller.calibrationData.rStickCalibData.ToString();
                    EditorGUILayout.LabelField("Stick calibration data: ", calibrationText);

                    EditorGUILayout.Separator();
                    GUILayout.Label("IMU data: ", EditorStyles.boldLabel);

                    EditorGUILayout.LabelField("Angular velocity: ", controller.angularVelocity.value.ToString());
                    EditorGUILayout.LabelField("Orientation: ", controller.orientation.value.ToString());
                    EditorGUILayout.LabelField("Acceleration: ", controller.acceleration.value.ToString());

                    readIMU = EditorGUILayout.Toggle("Fast-frequency display: ", readIMU);

                    if (GUILayout.Button("Enable IMU"))
                    {
                        controller.SetIMUEnabled(true);
                    }

                    //* Little button for debug/dev purposes
                    // EditorGUILayout.Separator();
                    // GUILayout.Label("Debug: ", EditorStyles.boldLabel);
                    // if (GUILayout.Button("Tst"))
                    // {
                    //     //
                    // }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            return fold;
        }

        /// <summary>
        /// Execute roughly 10 times/s.
        /// </summary>
        private void OnInspectorUpdate()
        {
            if (readIMU)
                Repaint();
        }
    }
}
