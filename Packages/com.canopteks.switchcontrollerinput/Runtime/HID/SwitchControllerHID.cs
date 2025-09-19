using System;
using UnityEditor;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Switch.LowLevel;
using UnityEngine.InputSystem.Utilities;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using System.Text;
using System.Collections.Generic;

namespace UnityEngine.InputSystem.Switch
{
    [InputControlLayout(stateType = typeof(SwitchControllerVirtualInputState))]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public abstract class SwitchControllerHID : InputDevice, IInputStateCallbackReceiver, IEventPreProcessor
    {
        #region Accelerometer/gyroscope controls
        public Vector3Control angularVelocity { get; private set; }
        public Vector3Control orientation { get; private set; }
        public Vector3Control acceleration { get; private set; }
        public QuaternionControl deviceRotation { get; private set; }

        // Uncalibrated values for debugging
        public Vector3Control uncalibratedAngularVelocity { get; private set; }
        public Vector3Control uncalibratedAcceleration { get; private set; }
        #endregion

        #region Button controls
        public DpadControl dpad { get; private set; }

        public ButtonControl buttonWest { get; private set; }
        public ButtonControl buttonNorth { get; private set; }
        public ButtonControl buttonSouth { get; private set; }
        public ButtonControl buttonEast { get; private set; }

        public ButtonControl leftShoulder { get; private set; }
        public ButtonControl rightShoulder { get; private set; }
        public ButtonControl leftStickPress { get; private set; }
        public ButtonControl rightStickPress { get; private set; }
        public ButtonControl leftTrigger { get; private set; }
        public ButtonControl rightTrigger { get; private set; }
        public ButtonControl start { get; private set; }
        public ButtonControl select { get; private set; }
        public ButtonControl leftShoulderMini { get; private set; }
        public ButtonControl rightShoulderMini { get; private set; }

        public StickControl leftStick { get; private set; }
        public StickControl rightStick { get; private set; }
        #endregion

        #region Device colors
        public Color BodyColor { get; protected set; } = Color.black;
        public Color ButtonColor { get; protected set; } = Color.black;
        public Color LeftGripColor { get; protected set; } = Color.black;
        public Color RightGripColor { get; protected set; } = Color.black;
        #endregion

        #region Stick and IMU data
        public struct CalibrationData
        {
            public StickCalibrationData lStickCalibData;
            public StickCalibrationData rStickCalibData;
            public IMUCalibrationData imuCalibData;
        }

        public CalibrationData calibrationData = new CalibrationData()
        {
            lStickCalibData = new StickCalibrationData()
            {
                xMin = 720,
                xMax = 3408,
                yMin = 654,
                yMax = 2908
            },
            rStickCalibData = new StickCalibrationData()
            {
                xMin = 720,
                xMax = 3408,
                yMin = 654,
                yMax = 2908
            }
        };
        #endregion

        private Vector3 m_currentOrientation = new Vector3();
        private double m_lastUpdateTime;
        private IMUData m_lastIMUData;

        // Madgwick's Algorithm variables
        public float MadgwickBeta = 0.01f; // Algorithm gain
        private float q0 = 1.0f, q1 = 0.0f, q2 = 0.0f, q3 = 0.0f; // Quaternion of sensor frame relative to auxiliary frame

        // public float kComplementaryFilterGain = 0.98f; // No longer used

        #region Generic data
        public BatteryLevelEnum BatteryLevel { get; protected set; } = BatteryLevelEnum.Empty;
        public bool BatteryIsCharging { get; protected set; } = false;
        public ControllerTypeEnum ControllerType { get; protected set; } = ControllerTypeEnum.JoyCon;
        public SpecificControllerTypeEnum SpecificControllerType { get; protected set; } = SpecificControllerTypeEnum.Unknown;
        public bool IsPoweredBySwitchOrUSB { get; protected set; } = false;
        public string FirmwareVersion { get; protected set; } = "X.X";
        public string MACAddress { get; protected set; } = "XX.XX.XX.XX.XX.XX";
        public string SerialNumber { get; protected set; } = "N/A";
        #endregion

        #region Data re/loading
        // Are the different device informations loaded ?
        private bool m_IMUConfigDataLoaded = false;
        private bool m_stickConfigDataLoaded = false;
        private bool m_deviceInfoLoaded = false;
        private bool m_colorsLoaded = false;
        private bool m_serialNumberLoaded = false;
        private bool m_imuCalibrationDataLoaded = false;

        // Register the time of last request to retry to fetch them in case of timeout
        private double m_stickCalibrationTimeOfLastRequest;
        private double m_infoTimeOfLastRequest;
        private double m_colorsTimeOfLastRequest;
        private double m_serialNumberTimeOfLastRequest;

        // Timeout vars
        private const int configTimerDataDefault = 500;
        private int m_configDataTimer = configTimerDataDefault;
        private const double timeout = 1.0;
        #endregion

        #region Calibration Process
        public enum CalibrationState
        {
            None,
            InProgress,
            Done
        }
        public CalibrationState CurrentCalibrationState { get; private set; } = CalibrationState.None;
        public float CalibrationProgress { get; private set; } = 0f;
        private double m_calibrationStartTime;
        private float m_calibrationDuration;
        private Vector3 m_accumulatedAccel;
        private Vector3 m_accumulatedGyro;
        private int m_sampleCount;
        #endregion

        #region Unity InputDevice boilerplate

        /// <summary>
        /// The last used/added Joy-Con (R) controller.
        /// </summary>
        public static SwitchControllerHID current { get; private set; }

        static SwitchControllerHID() { }

        [RuntimeInitializeOnLoadMethod]
        static void Init() { }

        /// <inheritdoc />
        public unsafe void OnStateEvent(InputEventPtr eventPtr)
        {
            StateEvent.FromUnchecked(eventPtr)->stateFormat = new FourCC("SCVS");
            InputState.Change(this, eventPtr);
        }

        /// <inheritdoc />
        public bool GetStateOffsetForEvent(InputControl control, InputEventPtr eventPtr, ref uint offset)
        {
            return false;
        }

        /// <inheritdoc />
        protected override void FinishSetup()
        {
            base.FinishSetup();

            buttonWest = GetChildControl<ButtonControl>("buttonWest");
            buttonNorth = GetChildControl<ButtonControl>("buttonNorth");
            buttonSouth = GetChildControl<ButtonControl>("buttonSouth");
            buttonEast = GetChildControl<ButtonControl>("buttonEast");
            angularVelocity = GetChildControl<Vector3Control>("angularVelocity");
            orientation = GetChildControl<Vector3Control>("orientation");
            acceleration = GetChildControl<Vector3Control>("acceleration");
            deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
            uncalibratedAngularVelocity = GetChildControl<Vector3Control>("uncalibratedAngularVelocity");
            uncalibratedAcceleration = GetChildControl<Vector3Control>("uncalibratedAcceleration");

            dpad = GetChildControl<DpadControl>("dpad");

            leftShoulder = GetChildControl<ButtonControl>("leftShoulder");
            rightShoulder = GetChildControl<ButtonControl>("rightShoulder");
            leftStickPress = GetChildControl<ButtonControl>("leftStickPress");
            rightStickPress = GetChildControl<ButtonControl>("rightStickPress");
            leftTrigger = GetChildControl<ButtonControl>("leftTrigger");
            rightTrigger = GetChildControl<ButtonControl>("rightTrigger");
            start = GetChildControl<ButtonControl>("start");
            select = GetChildControl<ButtonControl>("select");
            leftShoulderMini = GetChildControl<ButtonControl>("leftShoulderMini");
            rightShoulderMini = GetChildControl<ButtonControl>("rightShoulderMini");

            leftStick = GetChildControl<StickControl>("leftStick");
            rightStick = GetChildControl<StickControl>("rightStick");
        }

        /// <inheritdoc />
        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }
        #endregion

        #region Adding, removal and late info updates on the device
        /// <inheritdoc />
        protected override void OnAdded()
        {
            base.OnAdded();

            // Set the controller to 60Hz reports mode instead of button events mode
            // TODO: Check if it's the one that fails sometimes and if so, try to put it on NextUpdate with a timeout like the others
            SetInputReportMode(InputModeEnum.Standard);

            // Let the controller breathe for a sec before asking its info, color and calibration datas
            m_colorsTimeOfLastRequest = InputRuntime.s_Instance.currentTime;
            m_infoTimeOfLastRequest = InputRuntime.s_Instance.currentTime;
            m_stickCalibrationTimeOfLastRequest = InputRuntime.s_Instance.currentTime;
            m_serialNumberTimeOfLastRequest = InputRuntime.s_Instance.currentTime;
            m_lastUpdateTime = InputRuntime.s_Instance.currentTime;
        }

        /// <inheritdoc />
        protected override void OnRemoved()
        {
            base.OnRemoved();
            if (current == this)
                current = null;
        }

        /// <inheritdoc />
        public void OnNextUpdate()
        {
            // Handle calibration process first to ensure it's not blocked by other updates.
            if (CurrentCalibrationState == CalibrationState.InProgress)
            {
                double elapsedTime = InputRuntime.s_Instance.currentTime - m_calibrationStartTime;
                CalibrationProgress = (float)(elapsedTime / m_calibrationDuration);

                // Accumulate data
                m_accumulatedAccel += new Vector3(m_lastIMUData.accelX, m_lastIMUData.accelY, m_lastIMUData.accelZ);
                m_accumulatedGyro += new Vector3(m_lastIMUData.gyro1, m_lastIMUData.gyro2, m_lastIMUData.gyro3);
                m_sampleCount++;

                if (elapsedTime >= m_calibrationDuration)
                {
                    // Finish calibration
                    Vector3 avgAccel = m_accumulatedAccel / m_sampleCount;
                    Vector3 avgGyro = m_accumulatedGyro / m_sampleCount;
                    WriteUserIMUCalibrationData(avgAccel, avgGyro);
                }
            }

            // Keep track of time for sensor fusion
            m_lastUpdateTime = InputRuntime.s_Instance.currentTime;
            if (!m_colorsLoaded)
            {
                UpdateColors();
                return;
            }
            if (!m_deviceInfoLoaded)
            {
                UpdateDeviceInfo();
                return;
            }
            if (!m_stickConfigDataLoaded)
            {
                UpdateStickCalibrationData();
                return;
            }
            if (!m_serialNumberLoaded)
            {
                UpdateSerialNumber();
                return;
            }
        }

        /// <summary>
        /// Update the devices color info if it is outdated
        /// </summary>
        private void UpdateColors()
        {
            double currentTime = InputRuntime.s_Instance.currentTime;

            // Are we waiting for a response?
            if (currentTime > m_colorsTimeOfLastRequest + timeout)
            {
                m_colorsTimeOfLastRequest = currentTime;
                ReadColors();
            }
        }

        private void UpdateDeviceInfo()
        {
            double currentTime = InputRuntime.s_Instance.currentTime;

            // Are we waiting for a response?
            if (currentTime > m_infoTimeOfLastRequest + timeout)
            {
                m_infoTimeOfLastRequest = currentTime;
                ReadControllerInfo();

                // Also grab the serial number, because it's a device info that is not in the device info section
                ReadSerialNumber();
            }
        }

        private void UpdateStickCalibrationData()
        {
            double currentTime = InputRuntime.s_Instance.currentTime;

            if (currentTime > m_stickCalibrationTimeOfLastRequest + timeout)
            {
                m_stickCalibrationTimeOfLastRequest = currentTime;
                ReadStickCalibrationData();
            }
        }

        private void UpdateSerialNumber()
        {
            double currentTime = InputRuntime.s_Instance.currentTime;

            if (currentTime > m_serialNumberTimeOfLastRequest + timeout)
            {
                m_serialNumberTimeOfLastRequest = currentTime;
                ReadSerialNumber();
            }
        }

        #endregion

        #region Public interactions with the controller (output reports)
        public void Rumble(SwitchControllerRumbleProfile rumbleProfile)
        {
            var c = SwitchControllerCommand.Create(rumbleProfile);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Rumble command failed");
        }

        public void ReadControllerInfo()
        {
            Debug.Log("Requesting device info...");
            var c = SwitchControllerCommand.Create(subcommand: new SwitchControllerRequestInfoSubcommand());
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Request device info failed");
        }

        public void DoBluetoothPairing()
        {
            // step 1
            // TODO: This step should send the bluetooth address of the host as the second argument
            var s1 = new SwitchControllerBluetoothManualPairingSubcommand();
            s1.ValueByte = 0x01;
            var c1 = SwitchControllerCommand.Create(subcommand: s1);

            var s2 = new SwitchControllerBluetoothManualPairingSubcommand();
            s2.ValueByte = 0x02;
            var c2 = SwitchControllerCommand.Create(subcommand: s2);

            var s3 = new SwitchControllerBluetoothManualPairingSubcommand();
            s3.ValueByte = 0x03;
            var c3 = SwitchControllerCommand.Create(subcommand: s3);
            if (ExecuteCommand(ref c1) < 0)
                Debug.LogError("Step 1 of bluetooth pairing failed");

            if (ExecuteCommand(ref c2) < 0)
                //! Fails here (see TODO above)
                Debug.LogError("Step 2 of bluetooth pairing failed");

            if (ExecuteCommand(ref c3) < 0)
                Debug.LogError("Step 3 of bluetooth pairing failed");
        }

        public void SetInputReportMode(InputModeEnum mode)
        {
            var c = SwitchControllerCommand.Create(subcommand: new SwitchControllerInputModeSubcommand(mode));
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError($"Set report mode to {mode} failed");
            m_IMUConfigDataLoaded = (mode == InputModeEnum.Standard);
        }

        public void SetIMUEnabled(bool active)
        {
            var s = new SwitchControllerSetImuEnabledSubcommand();
            s.Enabled = active;

            var c = SwitchControllerCommand.Create(subcommand: s);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError($"Set IMU active to {active} failed");
        }

        public void SetVibrationEnabled(bool active)
        {
            var s = new SwitchControllerSetVibrationEnabledSubcommand();
            s.Enabled = active;

            var c = SwitchControllerCommand.Create(subcommand: s);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError($"Set vibration active to {active} failed");
        }

        public void SetLEDs(
            LEDStatusEnum p1 = LEDStatusEnum.Off,
            LEDStatusEnum p2 = LEDStatusEnum.Off,
            LEDStatusEnum p3 = LEDStatusEnum.Off,
            LEDStatusEnum p4 = LEDStatusEnum.Off)
        {
            var c = SwitchControllerCommand.Create(subcommand: new SwitchControllerSetLEDSubcommand(p1, p2, p3, p4));
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Set LEDs failed");
        }

        public void ReadFactoryIMUCalibrationData()
        {
            var readSubcommand = new SwitchControllerReadSPIFlashSubcommand(atAddress: (uint)SPIFlashReadAddressEnum.FactoryIMUCalibration, withLength: 0x18);
            Debug.Log($"Requesting Factory IMU calibration info...");
            var c = SwitchControllerCommand.Create(subcommand: readSubcommand);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Read Factory IMU calibration info failed");
        }

        public void ReadUserIMUCalibrationData()
        {
            var readSubcommand = new SwitchControllerReadSPIFlashSubcommand(atAddress: (uint)SPIFlashReadAddressEnum.UserIMUCalibration, withLength: 0x1A);
            Debug.Log($"Requesting User IMU calibration info...");
            var c = SwitchControllerCommand.Create(subcommand: readSubcommand);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Read User IMU calibration info failed");
        }

        public void ReadColors()
        {
            var readSubcommand = new SwitchControllerReadSPIFlashSubcommand(atAddress: (uint)SPIFlashReadAddressEnum.ColorData, withLength: 0x0B);
            Debug.Log($"Requesting color info...");
            var c = SwitchControllerCommand.Create(subcommand: readSubcommand);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Read color info failed");
        }

        public void ReadStickCalibrationData()
        {
            var readSubcommand = new SwitchControllerReadSPIFlashSubcommand(atAddress: (uint)SPIFlashReadAddressEnum.FactoryStickCalibration, withLength: 0x12);
            Debug.Log($"Requesting factory stick calibration info...");
            var c = SwitchControllerCommand.Create(subcommand: readSubcommand);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Read factory stick calibration info failed");
        }

        public void ReadSerialNumber()
        {
            var readSubcommand = new SwitchControllerReadSPIFlashSubcommand(atAddress: (uint)SPIFlashReadAddressEnum.SerialNumber, withLength: 0x10);
            Debug.Log($"Requesting device serial number...");
            var c = SwitchControllerCommand.Create(subcommand: readSubcommand);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Read device serial number failed");
        }

        public void StartIMUCalibration(float duration = 5.0f)
        {
            if (CurrentCalibrationState == CalibrationState.InProgress)
            {
                Debug.LogWarning("Calibration is already in progress.");
                return;
            }
            Debug.Log($"Starting IMU calibration for {duration} seconds. Keep the controller stable.");
            CurrentCalibrationState = CalibrationState.InProgress;
            m_calibrationStartTime = InputRuntime.s_Instance.currentTime;
            m_calibrationDuration = duration;
            m_accumulatedAccel = Vector3.zero;
            m_accumulatedGyro = Vector3.zero;
            m_sampleCount = 0;
            CalibrationProgress = 0f;
        }

        /// <summary>
        /// Calibrates the IMU by setting the current raw sensor values as the new base/offset.
        /// The controller should be on a flat, stable surface when this is called.
        /// This will write to the user calibration data section in the controller's SPI flash.
        /// </summary>
        private unsafe void WriteUserIMUCalibrationData(Vector3 averageAccel, Vector3 averageGyro)
        {
            Debug.Log($"Calibration finished. Average Accel: {averageAccel}, Average Gyro: {averageGyro}. Writing to controller...");

            // Create a 26-byte (0x1A) payload for user calibration data.
            byte[] payload = new byte[0x1A];

            // Magic number `0xB2 0xA1` at the start.
            payload[0] = 0xB2;
            payload[1] = 0xA1;

            // Create a new calibration structure.
            // The new 'base' is the averaged raw sensor value.
            // For the accelerometer, we calculate the offset by removing the gravity component from the reading.
            // 1. Normalize the average acceleration vector to get the direction of gravity.
            Vector3 gravityDirection = new Vector3(averageAccel.x, averageAccel.y, averageAccel.z).normalized;
            // 2. 1G corresponds to a raw value of ~4096 (since 4G is ~16384).
            const float oneGAsRaw = 16384f / 4f;
            // 3. Calculate the ideal gravity vector in the current orientation.
            Vector3 idealGravityVector = gravityDirection * oneGAsRaw;
            // 4. The offset is the measured value minus the ideal gravity vector.
            Vector3 accelOffset = averageAccel - idealGravityVector;

            // For the gyroscope, we zero out all axes as it should be perfectly still.
            IMUCalibrationData newUserCalib = new IMUCalibrationData
            {
                accelBase = new Vector3UInt16 { x = (ushort)accelOffset.x, y = (ushort)accelOffset.y, z = (ushort)accelOffset.z },
                // keep factory coefficients unchanged
                accelSensitivity = calibrationData.imuCalibData.accelSensitivity,

                gyroBase = new Vector3UInt16 { x = (ushort)averageGyro.x, y = (ushort)averageGyro.y, z = (ushort)averageGyro.z },
                gyroSensitivity = calibrationData.imuCalibData.gyroSensitivity,
                // keep factory coefficients unchanged
            };

            // Manually copy the relevant fields (Base and Sensitivity) to the payload to avoid overflow
            // and to match the 24-byte structure expected by the controller's SPI flash.
            // The coefficients are not stored on the device.
            fixed (byte* p = &payload[2])
            {
                ushort* ushortPtr = (ushort*)p;
                ushortPtr[0] = newUserCalib.accelBase.x;
                ushortPtr[1] = newUserCalib.accelBase.y;
                ushortPtr[2] = newUserCalib.accelBase.z;
                ushortPtr[3] = newUserCalib.accelSensitivity.x;
                ushortPtr[4] = newUserCalib.accelSensitivity.y;
                ushortPtr[5] = newUserCalib.accelSensitivity.z;
                ushortPtr[6] = newUserCalib.gyroBase.x;
                ushortPtr[7] = newUserCalib.gyroBase.y;
                ushortPtr[8] = newUserCalib.gyroBase.z;
                ushortPtr[9] = newUserCalib.gyroSensitivity.x;
                ushortPtr[10] = newUserCalib.gyroSensitivity.y;
                ushortPtr[11] = newUserCalib.gyroSensitivity.z;
            }

            Debug.LogWarning("Writing current IMU calibration data to User SPI section. This is permanent!");

            var writeSubcommand = new SwitchControllerWriteSPIFlashSubcommand(
                atAddress: (uint)SPIFlashReadAddressEnum.UserIMUCalibration,
                data: payload
            );
            var c = SwitchControllerCommand.Create(subcommand: writeSubcommand);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Write User IMU calibration info failed");

            CurrentCalibrationState = CalibrationState.Done;
            CalibrationProgress = 1f;
        }

        /// <summary>
        /// Resets the Yaw component of the orientation to zero, using the current direction as the new forward.
        /// </summary>
        public void ResetYaw()
        {
            // Get the current orientation as a quaternion
            var currentOrientationQuat = new Quaternion(q1, q2, q3, q0);

            // Get the current yaw angle
            float currentYaw = currentOrientationQuat.eulerAngles.z;

            // Create a quaternion that represents the inverse of the current yaw rotation
            var yawCorrection = Quaternion.AngleAxis(-currentYaw, Vector3.forward);

            // Apply the correction to the current orientation
            var correctedQuat = yawCorrection * currentOrientationQuat;

            // Update the Madgwick quaternion state
            q0 = correctedQuat.w;
            q1 = correctedQuat.x;
            q2 = correctedQuat.y;
            q3 = correctedQuat.z;

            Debug.Log("Yaw has been reset.");
        }
        #endregion

        #region Input reports (answers from the controller)
        [StructLayout(LayoutKind.Explicit)]
        private struct SwitchControllerGenericInputReport
        {
            public static FourCC Format => new FourCC('H', 'I', 'D');
            [FieldOffset(0)] public byte reportId;
        }

        [StructLayout(LayoutKind.Explicit, Size = kSize)]
        private unsafe struct SwitchControllerSubcommandResponseInputReport
        {
            public static FourCC Format => new FourCC('H', 'I', 'D');
            public const byte kSize = 50;
            public const byte expectedReportId = 0x21;

            [FieldOffset(0)] public byte reportId;
            [FieldOffset(2)] public byte batteryAndConnectionInfo;

            [FieldOffset(13)] public byte ack;
            [FieldOffset(14)] public byte subcommandId;
            [FieldOffset(15)] public fixed byte replyData[35];
        }

        unsafe bool IEventPreProcessor.PreProcessEvent(InputEventPtr eventPtr)
        {
            // Use the virtual controller events as-is, and skip other delta state events
            if (eventPtr.type == DeltaStateEvent.Type)
                return DeltaStateEvent.FromUnchecked(eventPtr)->stateFormat == SwitchControllerVirtualInputState.Format;

            // Use all other non-state/non-delta-state events
            if (eventPtr.type != StateEvent.Type)
                return true;

            var stateEvent = StateEvent.FromUnchecked(eventPtr);
            var size = stateEvent->stateSizeInBytes;

            // If state format was the virtual controller, just use it
            if (stateEvent->stateFormat == SwitchControllerVirtualInputState.Format)
                return true;

            // Skip unrecognized state events?
            if (stateEvent->stateFormat != SwitchControllerGenericInputReport.Format || size < sizeof(SwitchControllerGenericInputReport))
                return false;

            var genericReport = (SwitchControllerGenericInputReport*)stateEvent->state;

            // Simple report mode!
            if (genericReport->reportId == (byte)InputModeEnum.Simple)
            {
                Debug.Log("PreProcessEvent: Simple report mode");
                SetInputReportMode(InputModeEnum.Standard);
                return false;
            }

            // Subcommand reply!
            else if (genericReport->reportId == (byte)InputModeEnum.ReadSubcommands)
            {
                Debug.Log("PreProcessEvent: Subcommand report mode");
                var data = ((SwitchControllerSubcommandResponseInputReport*)stateEvent->state);
                HandleSubcommand(*data);
              return false;
            }

            // Full report mode!
            else if (genericReport->reportId == (byte)InputModeEnum.Standard)
            {
                HandleFullReport(stateEvent);
                return true;
            }

            else if (genericReport->reportId == (byte)InputModeEnum.NFCOrIR)
            {
                Debug.Log("NFC or infra-red report");
                return false;
            }
            else
            {
                Debug.Log($"Unknown report ID: {genericReport->reportId:X2}");
            }
            return false;
        }

        private unsafe void HandleFullReport(StateEvent* stateEvent)
        {
            var currentTime = InputRuntime.s_Instance.currentTime;
            var deltaTime = (float)(currentTime - m_lastUpdateTime);
            // In editor mode (not playing), deltaTime can be close to zero, which stalls the orientation calculation.
            // We'll use a fixed delta time based on the controller's report frequency (approx. 60Hz -> 15ms) as a fallback.
            if (deltaTime < 0.001f)
            {
                deltaTime = 0.015f;
            }
            m_lastUpdateTime = currentTime;

            SwitchControllerFullInputReport* fullInputReport = ((SwitchControllerFullInputReport*)stateEvent->state);
            m_lastIMUData = fullInputReport->imuData0ms; // Store the latest raw IMU data
            var hidInputReport = fullInputReport->ToHIDInputReport(ref calibrationData, SpecificControllerType);

            // Sensor fusion using Madgwick's algorithm
            if (m_IMUConfigDataLoaded)
            {
                // Madgwick's algorithm expects gyroscope data in radians per second.
                float gx = hidInputReport.angularVelocity.x * Mathf.Deg2Rad;
                float gy = hidInputReport.angularVelocity.y * Mathf.Deg2Rad;
                float gz = hidInputReport.angularVelocity.z * Mathf.Deg2Rad;

                MadgwickAHRSUpdate(gx, gy, gz, hidInputReport.acceleration.x, hidInputReport.acceleration.y, hidInputReport.acceleration.z, deltaTime);
            }
            hidInputReport.deviceRotation = new Quaternion(q1, q2, q3, q0);
            hidInputReport.orientation = hidInputReport.deviceRotation.eulerAngles;
            *((SwitchControllerVirtualInputState*)stateEvent->state) = hidInputReport;
            stateEvent->stateFormat = SwitchControllerVirtualInputState.Format;
        }

        private unsafe void HandleSubcommand(SwitchControllerSubcommandResponseInputReport response)
        {
            // Connection info
            BatteryLevel = (BatteryLevelEnum)((response.batteryAndConnectionInfo & 0xE0) >> 4);
            BatteryIsCharging = (response.batteryAndConnectionInfo & 0x10) >> 4 != 0;

            int connectionInfo = response.batteryAndConnectionInfo & 0x0F;
            ControllerType = (ControllerTypeEnum)((connectionInfo >> 1) & 3);
            IsPoweredBySwitchOrUSB = (connectionInfo & 1) == 1;

            SubcommandIDEnum subcommandReplyId = (SubcommandIDEnum)response.subcommandId;
            var subcommandWasAcknowledged = (response.ack & 0x80) != 0;

            Debug.Log($"Subcommand response for {subcommandReplyId}: {response.ack:X2}");

            if (subcommandWasAcknowledged)
            {
                switch (subcommandReplyId)
                {
                    case SubcommandIDEnum.RequestDeviceInfo:
                        HandleDeviceInfo(response.replyData);
                        break;
                    case SubcommandIDEnum.SPIFlashRead:
                        HandleFlashRead(response.replyData);
                        break;
                    default:
                        break;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct DeviceInfo
        {
            public byte firmwareVersionMaj;
            public byte firmwareVersionMin;
            public byte joyConType;
            public byte unknown1;
            public fixed byte macAddress[6];
            public byte unknown2;
            public byte useColorsFromSPI;
        }

        private unsafe void HandleDeviceInfo(byte* dataPtr)
        {
            DeviceInfo deviceInfo = (DeviceInfo)Marshal.PtrToStructure((IntPtr)dataPtr, typeof(DeviceInfo));

            SpecificControllerType = (SpecificControllerTypeEnum)deviceInfo.joyConType;
            FirmwareVersion = String.Format($"{deviceInfo.firmwareVersionMaj:X1}.{deviceInfo.firmwareVersionMin:X2}");
            MACAddress = String.Format($"{deviceInfo.macAddress[0]:X2}:{deviceInfo.macAddress[1]:X2}:{deviceInfo.macAddress[2]:X2}:{deviceInfo.macAddress[3]:X2}:{deviceInfo.macAddress[4]:X2}:{deviceInfo.macAddress[5]:X2}");

            Debug.Log($"Firmware version is: {FirmwareVersion}, Joy-Con type is {SpecificControllerType}");
            m_deviceInfoLoaded = true;
        }

        [StructLayout(LayoutKind.Explicit, Size = 18)]
        private unsafe struct RawStickCalibrationData
        {
            [FieldOffset(0)] public fixed byte lStick[9];
            [FieldOffset(9)] public fixed byte rStick[9];
        }


        [StructLayout(LayoutKind.Explicit, Size = 3)]
        private struct Rgb24Bit
        {
            [FieldOffset(0)] public byte r;
            [FieldOffset(1)] public byte g;
            [FieldOffset(2)] public byte b;

            public Color ToUnityColor()
            {
                return new Color32(r, g, b, 255);
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 3 * 4)]
        private struct ControllerColors
        {
            [FieldOffset(0)] public Rgb24Bit bodyColor;
            [FieldOffset(3)] public Rgb24Bit buttonColor;
            [FieldOffset(6)] public Rgb24Bit leftGripColor;
            [FieldOffset(9)] public Rgb24Bit rightGripColor;
        }

        private unsafe void HandleFlashRead(byte* dataPtr)
        {
            byte[] data = new byte[35];
            Marshal.Copy(new IntPtr(dataPtr), data, 0, 35);
            uint address = BitConverter.ToUInt32(data, 0);
            byte length = data[4];

            byte* response = dataPtr + 5;

            // Shipment data, unsure
            if (address == 0x5000 && length == 0x01)
            {
                Debug.Log("Read shipment data... (not implemented)");
            }

            // Serial number in NON-extended ASCII
            else if (address == (uint)SPIFlashReadAddressEnum.SerialNumber && length == 0x10)
            {
                Debug.Log("Read serial number...");
                DecodeSerialNumberData(response);
                m_serialNumberLoaded = true;
            }

            // IMU factory calibration
            else if (address == (uint)SPIFlashReadAddressEnum.FactoryIMUCalibration && length == 0x18)
            {
                Debug.Log($"Read factory IMU calibration data...");
                DecodeIMUCalibrationData((ushort*)response);
                m_imuCalibrationDataLoaded = true;
            }

            // Factory analog stick calibration
            else if (address == (uint)SPIFlashReadAddressEnum.FactoryStickCalibration && length == 0x12)
            {
                Debug.Log("Read factory analog stick calibration data...");
                DecodeStickCalibrationData(response);
            }

            // Colors
            else if (address == (uint)SPIFlashReadAddressEnum.ColorData && length == 0x0B)
            {
                Debug.Log("Read controller color data...");
                DecodeColorData(response);
            }

            // Stick device parameters 1
            else if (address == 0x6086 && length == 0x12)
            {
                Debug.Log("Read stick device params 1 data... (not implemented)");
            }

            // Stick device parameters 2
            else if (address == 0x6098 && length == 0x12)
            {
                Debug.Log("Read stick device params 2 data... (not implemented)");
            }

            // User analog stick calibration
            else if (address == (uint)SPIFlashReadAddressEnum.UserStickCalibration && length == 0x16)
            {
                Debug.Log("Read user analog stick calibration data... (not implemented)");
            }

            // IMU user calibration
            else if (address == (uint)SPIFlashReadAddressEnum.UserIMUCalibration && length == 0x1A)
            {
                Debug.Log($"Read user IMU calibration data...");
                // The user calibration data has a 2-byte magic number at the start.
                // We need to skip it before decoding.
                // The magic number is 0xB2 0xA1.
                if (response[0] == 0xB2 && response[1] == 0xA1)
                {
                    DecodeIMUCalibrationData((ushort*)(response + 2)); // Skip the 2-byte magic number
                }
                else
                {
                    Debug.LogWarning("User IMU calibration data does not contain the expected magic number. don't decode.");
                    // DecodeIMUCalibrationData((ushort*)response);
                }
                m_imuCalibrationDataLoaded = true;
            }

            else
            {
                Debug.Log($"Unrecognized range: 0x{address:X4}-0x{address + length:X2} (length is {length:X2})");
            }

        }
        #endregion
        #region Data decoding (byte to text/)
        [StructLayout(LayoutKind.Sequential)]
        public struct Vector3Int16
        {
            public short x;
            public short y;
            public short z;

            public override string ToString()
            {
                return $"({x}, {y}, {z})";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector3UInt16
        {
            public ushort x;
            public ushort y;
            public ushort z;

            public override string ToString()
            {
                return $"({x}, {y}, {z})";
            }
        }

        public struct IMUCalibrationData
        {
            public Vector3UInt16 accelBase;
            public Vector3UInt16 accelSensitivity;

            public Vector3UInt16 gyroBase;
            public Vector3UInt16 gyroSensitivity;
            
            public Vector3 accelCoeff;
            public Vector3 gyroCoeff;

            public unsafe static IMUCalibrationData FromResponse(ushort* response)
            {
                return new IMUCalibrationData()
                {
                    accelBase = new Vector3UInt16()
                    {
                        x = response[0],
                        y = response[1],
                        z = response[2]
                    },

                    accelSensitivity = new Vector3UInt16()
                    {
                        x = response[3],
                        y = response[4],
                        z = response[5]
                    },

                    gyroBase = new Vector3UInt16()
                    {
                        x = response[6],
                        y = response[7],
                        z = response[8]
                    },

                    gyroSensitivity = new Vector3UInt16()
                    {
                        x = response[9],
                        y = response[10],
                        z = response[11]
                    }
                };
            }

            public override string ToString()
            {
                return $"accelBase = {accelBase}\naccelSen = {accelSensitivity}\ngyroBase = {gyroBase}\ngyroSen = {gyroSensitivity}";
            }
        }

        private unsafe void DecodeSerialNumberData(byte* response)
        {
            StringBuilder snStringBuilder = new StringBuilder(15);

            Debug.Log($"First byte is {(*response):X2}");
            if ((*response) >= 0X80)
            {
                Debug.LogWarning("No valid serial number retrieved");
                return;
            }

            for (int i = 0; i < 16; i++)
            {
                byte* serialDigit = response + i;
                if (*serialDigit == 0x00)
                    continue;

                snStringBuilder.Append(Encoding.ASCII.GetString(serialDigit, 1));
            }

            SerialNumber = snStringBuilder.ToString();
            m_serialNumberLoaded = true; // for prevent read serial infinity loop 
        }

        private unsafe void DecodeIMUCalibrationData(ushort* response)
        {
            var newCalibData = IMUCalibrationData.FromResponse(response);

            // Pre-calculate the coefficients once when calibration data is loaded.
            // This avoids redundant calculations in every input report.
            newCalibData.accelCoeff.x = 4.0f / ((short)newCalibData.accelSensitivity.x - (short)newCalibData.accelBase.x);
            newCalibData.accelCoeff.y = 4.0f / ((short)newCalibData.accelSensitivity.y - (short)newCalibData.accelBase.y);
            newCalibData.accelCoeff.z = 4.0f / ((short)newCalibData.accelSensitivity.z - (short)newCalibData.accelBase.z);

            newCalibData.gyroCoeff.x = 936.0f / ((short)newCalibData.gyroSensitivity.x - (short)newCalibData.gyroBase.x);
            newCalibData.gyroCoeff.y = 936.0f / ((short)newCalibData.gyroSensitivity.y - (short)newCalibData.gyroBase.y);
            newCalibData.gyroCoeff.z = 936.0f / ((short)newCalibData.gyroSensitivity.z - (short)newCalibData.gyroBase.z);

            calibrationData.imuCalibData = newCalibData;

            Debug.Log("Calibration data: " + calibrationData.imuCalibData);
        }

        #region Madgwick Algorithm
        /// <summary>
        /// Madgwick's IMU implementation. See: http://www.x-io.co.uk/open-source-imu-and-ahrs-algorithms/
        /// </summary>
        private void MadgwickAHRSUpdate(float gx, float gy, float gz, float ax, float ay, float az, float samplePeriod)
        {
            float recipNorm;
            float s0, s1, s2, s3;
            float qDot1, qDot2, qDot3, qDot4;
            float _2q0, _2q1, _2q2, _2q3, _4q0, _4q1, _4q2, _8q1, _8q2, q0q0, q1q1, q2q2, q3q3;

            // Rate of change of quaternion from gyroscope
            qDot1 = 0.5f * (-q1 * gx - q2 * gy - q3 * gz);
            qDot2 = 0.5f * (q0 * gx + q2 * gz - q3 * gy);
            qDot3 = 0.5f * (q0 * gy - q1 * gz + q3 * gx);
            qDot4 = 0.5f * (q0 * gz + q1 * gy - q2 * gx);

            // Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
            if (!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f)))
            {
                // Normalise accelerometer measurement
                recipNorm = 1.0f / Mathf.Sqrt(ax * ax + ay * ay + az * az);
                ax *= recipNorm;
                ay *= recipNorm;
                az *= recipNorm;

                // Auxiliary variables to avoid repeated arithmetic
                _2q0 = 2.0f * q0;
                _2q1 = 2.0f * q1;
                _2q2 = 2.0f * q2;
                _2q3 = 2.0f * q3;
                _4q0 = 4.0f * q0;
                _4q1 = 4.0f * q1;
                _4q2 = 4.0f * q2;
                _8q1 = 8.0f * q1;
                _8q2 = 8.0f * q2;
                q0q0 = q0 * q0;
                q1q1 = q1 * q1;
                q2q2 = q2 * q2;
                q3q3 = q3 * q3;

                // Gradient decent algorithm corrective step
                s0 = _4q0 * q2q2 + _2q2 * ax + _4q0 * q1q1 - _2q1 * ay;
                s1 = _4q1 * q3q3 - _2q3 * ax + 4.0f * q0q0 * q1 - _2q0 * ay - _4q1 + _8q1 * q1q1 + _8q1 * q2q2 + _4q1 * az;
                s2 = 4.0f * q0q0 * q2 + _2q0 * ax + _4q2 * q3q3 - _2q3 * ay - _4q2 + _8q2 * q1q1 + _8q2 * q2q2 + _4q2 * az;
                s3 = 4.0f * q1q1 * q3 - _2q1 * ax + 4.0f * q2q2 * q3 - _2q2 * ay;
                recipNorm = 1.0f / Mathf.Sqrt(s0 * s0 + s1 * s1 + s2 * s2 + s3 * s3); // normalise step magnitude
                s0 *= recipNorm;
                s1 *= recipNorm;
                s2 *= recipNorm;
                s3 *= recipNorm;

                // Apply feedback step
                qDot1 -= MadgwickBeta * s0;
                qDot2 -= MadgwickBeta * s1;
                qDot3 -= MadgwickBeta * s2;
                qDot4 -= MadgwickBeta * s3;
            }

            // Integrate rate of change of quaternion to yield quaternion
            q0 += qDot1 * samplePeriod;
            q1 += qDot2 * samplePeriod;
            q2 += qDot3 * samplePeriod;
            q3 += qDot4 * samplePeriod;

            // Normalise quaternion
            recipNorm = 1.0f / Mathf.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
            q0 *= recipNorm;
            q1 *= recipNorm;
            q2 *= recipNorm;
            q3 *= recipNorm;
        }


        #endregion

        private unsafe void DecodeStickCalibrationData(byte* response)
        {
            DecodeLeftStickData(response);
            DecodeRightStickData(response + 9);

            Debug.Log($"Calibration data loaded");
            m_stickConfigDataLoaded = true;
        }

        private unsafe void DecodeColorData(byte* response)
        {
            ControllerColors colors = (ControllerColors)Marshal.PtrToStructure((IntPtr)response, typeof(ControllerColors));
            BodyColor = colors.bodyColor.ToUnityColor();
            ButtonColor = colors.buttonColor.ToUnityColor();
            LeftGripColor = colors.leftGripColor.ToUnityColor();
            RightGripColor = colors.rightGripColor.ToUnityColor();
            m_colorsLoaded = true;

            Debug.Log($"Colors loaded: {BodyColor}, {ButtonColor}");
        }

        /// <summary>
        /// Decode the calibration part of the packet into human readable values.
        /// </summary>
        /// <param name="stickCal">Array of 9 bytes.</param>
        /// <returns>Array of 6 ushort values meaning different wether it is for left or right stick (see <see cref="DecodeLeftStickData"/> and <see cref="DecodeRightStickData"/> </returns>
        private unsafe ushort[] DecodeStickData(byte* stickCal)
        {
            // yoinked from
            // https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/spi_flash_notes.md#analog-stick-factory-and-user-calibration
            ushort[] data = new ushort[6];
            data[0] = (ushort)((stickCal[1] << 8) & 0xF00 | stickCal[0]);
            data[1] = (ushort)((stickCal[2] << 4) | (stickCal[1] >> 4));
            data[2] = (ushort)((stickCal[4] << 8) & 0xF00 | stickCal[3]);
            data[3] = (ushort)((stickCal[5] << 4) | (stickCal[4] >> 4));
            data[4] = (ushort)((stickCal[7] << 8) & 0xF00 | stickCal[6]);
            data[5] = (ushort)((stickCal[8] << 4) | (stickCal[7] >> 4));
            return data;
        }

        /// <summary>
        /// Decode the calibration part of the packet for the left stick.
        /// </summary>
        /// <param name="lStickCal">Array of 9 bytes.</param>
        private unsafe void DecodeLeftStickData(byte* lStickCal)
        {
            ushort[] decoded = DecodeStickData(lStickCal);

            ushort xAxisMaxAboveCenter = decoded[0];
            ushort yAxisMaxAboveCenter = decoded[1];
            ushort xAxisCenter = decoded[2];
            ushort yAxisCenter = decoded[3];
            ushort xAxisMinBelowCenter = decoded[4];
            ushort yAxisMinBelowCenter = decoded[5];


            ushort lStickXMin = (ushort)(xAxisCenter - xAxisMinBelowCenter);
            ushort lStickXMax = (ushort)(xAxisCenter + xAxisMaxAboveCenter);

            ushort lStickYMin = (ushort)(yAxisCenter - yAxisMinBelowCenter);
            ushort lStickYMax = (ushort)(yAxisCenter + yAxisMaxAboveCenter);

            calibrationData.lStickCalibData = new StickCalibrationData()
            {
                xMin = lStickXMin,
                xMax = lStickXMax,
                yMin = lStickYMin,
                yMax = lStickYMax,
                xCenter = xAxisCenter,
                yCenter = yAxisCenter
            };
        }

        private unsafe void DecodeRightStickData(byte* rStickCal)
        {
            ushort[] decoded = DecodeStickData(rStickCal);

            ushort xAxisCenter = decoded[0];
            ushort yAxisCenter = decoded[1];
            ushort xAxisMinBelowCenter = decoded[2];
            ushort yAxisMinBelowCenter = decoded[3];
            ushort xAxisMaxAboveCenter = decoded[4];
            ushort yAxisMaxAboveCenter = decoded[5];

            ushort rStickXMin = (ushort)(xAxisCenter - xAxisMinBelowCenter);
            ushort rStickXMax = (ushort)(xAxisCenter + xAxisMaxAboveCenter);

            ushort rStickYMin = (ushort)(yAxisCenter - yAxisMinBelowCenter);
            ushort rStickYMax = (ushort)(yAxisCenter + yAxisMaxAboveCenter);

            calibrationData.rStickCalibData = new StickCalibrationData()
            {
                xMin = rStickXMin,
                xMax = rStickXMax,
                yMin = rStickYMin,
                yMax = rStickYMax,
                xCenter = xAxisCenter,
                yCenter = yAxisCenter
            };
        }

        public struct StickCalibrationData
        {
            public ushort xMin;
            public ushort xMax;

            public ushort yMin;
            public ushort yMax;
            public ushort xCenter;
            public ushort yCenter;

            public static StickCalibrationData CreateEmpty()
            {
                return new StickCalibrationData() { xMin = 0, xMax = 0, yMin = 0, yMax = 0, xCenter = 0, yCenter = 0 };
            }

            public override string ToString()
            {
                return String.Format($"Center: ({xCenter:X2};{yCenter:X2}), X range: [{xMin:X2}-{xMax:X2}], Y range: [{yMin:X2}-{yMax:X2}]");
            }
        }
        #endregion
    }
}