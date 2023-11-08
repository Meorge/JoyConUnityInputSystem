using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.InputSystem.Switch.LowLevel
{
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    unsafe struct SwitchControllerFullInputReport
    {
        public const int kSize = 362;
        public const byte ExpectedReportId = 0x30;

        [FieldOffset(0)] public byte reportId;
        [FieldOffset(1)] public byte timer;
        [FieldOffset(2)] public byte batteryAndConnectionInfo;

        [FieldOffset(3)] public byte rightButtons;

        [FieldOffset(4)] public byte sharedButtons;

        [FieldOffset(5)] public byte leftButtons;

        [FieldOffset(6)] public fixed byte leftStick[3];
        [FieldOffset(9)] public fixed byte rightStick[3];

        [FieldOffset(12)] public byte vibratorInputReport;

        // For 0x21 (Subcommand replies)
        // [FieldOffset(13)] public byte subcommandAck;
        // [FieldOffset(14)] public byte subcommandReplyId;
        // [FieldOffset(15)] public fixed byte subcommandReplyData[35];


        // For 0x23 (NFC/IR MCU FW)
        // [FieldOffset(13)] public fixed byte nfcIRMCUFWDataInputReport[37];

        // For 0x30, 0x31, 0x32, 0x33 (normal mode)
        [FieldOffset(13)] public IMUData imuData0ms;
        [FieldOffset(25)] public IMUData imuData5ms;
        [FieldOffset(37)] public IMUData imuData10ms;

        // For 0x31 (NFC/IR?)
        // [FieldOffset(49)] public fixed byte nfcIRDataInputReport[313];


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SwitchControllerVirtualInputState ToHIDInputReport(ref SwitchControllerHID.CalibrationData calibData, SwitchControllerHID.SpecificControllerTypeEnum controllerType, Vector3 currentOrientation)
        {
            var leftStickVec = Vector2.zero;
            var rightStickVec = Vector2.zero;
            if (controllerType == SwitchControllerHID.SpecificControllerTypeEnum.LeftJoyCon ||
                controllerType == SwitchControllerHID.SpecificControllerTypeEnum.ProController)
            {
                // Left analog stick data
                var lStickCalibData = calibData.lStickCalibData;
                var l0 = leftStick[0];
                var l1 = leftStick[1];
                var l2 = leftStick[2];
                var rawLeftStickHoriz = l0 | ((l1 & 0xF) << 8);
                var rawLeftStickVert = (l1 >> 4) | (l2 << 4);

                var leftStickX = (Mathf.InverseLerp(lStickCalibData.xMin, lStickCalibData.xMax, rawLeftStickHoriz) * 2) - 1;
                var leftStickY = (Mathf.InverseLerp(lStickCalibData.yMin, lStickCalibData.yMax, rawLeftStickVert) * 2) - 1;
                leftStickVec = new Vector2(leftStickX, leftStickY);

                // Debug.Log($"Left stick data: Raw: ({rawLeftStickHoriz:X3};{rawLeftStickVert:X3}) Calibration data: Center=({lStickCalibData.xCenter:X3},{lStickCalibData.yCenter:X3}); X=[{lStickCalibData.xMin:X3} - {lStickCalibData.xMax:X3}]; Y=[{lStickCalibData.yMin:X3} - {lStickCalibData.yMax:X3}]   Final data: {leftStickVec}");
            }

            if (controllerType == SwitchControllerHID.SpecificControllerTypeEnum.RightJoyCon ||
                controllerType == SwitchControllerHID.SpecificControllerTypeEnum.ProController)
            {
                // Right analog stick data
                var rStickCalibData = calibData.rStickCalibData;
                var r0 = rightStick[0];
                var r1 = rightStick[1];
                var r2 = rightStick[2];
                var rawRightStickHoriz = r0 | ((r1 & 0xF) << 8);
                var rawRightStickVert = (r1 >> 4) | (r2 << 4);

                var rightStickX = (Mathf.InverseLerp(rStickCalibData.xMin, rStickCalibData.xMax, rawRightStickHoriz) * 2) - 1;
                var rightStickY = (Mathf.InverseLerp(rStickCalibData.yMin, rStickCalibData.yMax, rawRightStickVert) * 2) - 1;
                rightStickVec = new Vector2(rightStickX, rightStickY);

                // Debug.Log($"Right stick data: Raw: ({rawRightStickHoriz:X3};{rawRightStickVert:X3}) Calibration data: Center=({rStickCalibData.xCenter:X3},{rStickCalibData.yCenter:X3}); X=[{rStickCalibData.xMin:X3} - {rStickCalibData.xMax:X3}]; Y=[{rStickCalibData.yMin:X3} - {rStickCalibData.yMax:X3}]   Final data: {rightStickVec}");
            }

            // Debug.Log($"Creating input stuff: right stick is {rightStickVec}");
            var state = new SwitchControllerVirtualInputState
            {
                leftStick = leftStickVec,
                rightStick = rightStickVec,
                acceleration = imuData0ms.UncalibratedAcceleration,
                orientation = currentOrientation + imuData0ms.UncalibratedGyro,
                angularVelocity = imuData0ms.UncalibratedGyro
            };

            state.Set(SwitchControllerVirtualInputState.Button.Y, (rightButtons & 0x01) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.X, (rightButtons & 0x02) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.B, (rightButtons & 0x04) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.A, (rightButtons & 0x08) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.SR, ((leftButtons | rightButtons) & 0x10) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.SL, ((leftButtons | rightButtons) & 0x20) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.R, (rightButtons & 0x40) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.ZR, (rightButtons & 0x80) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Minus, (sharedButtons & 0x01) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Plus, (sharedButtons & 0x02) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.StickR, (sharedButtons & 0x04) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.StickL, (sharedButtons & 0x08) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Down, (leftButtons & 0x01) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Up, (leftButtons & 0x02) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Right, (leftButtons & 0x04) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Left, (leftButtons & 0x08) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.L, (leftButtons & 0x40) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.ZL, (leftButtons & 0x80) != 0);

            return state;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 12)]
    struct IMUData
    {
        [FieldOffset(0)] public short accelX;
        [FieldOffset(2)] public short accelY;
        [FieldOffset(4)] public short accelZ;

        [FieldOffset(6)] public short gyro1;
        [FieldOffset(8)] public short gyro2;
        [FieldOffset(10)] public short gyro3;

        public Vector3 UncalibratedAcceleration => new Vector3(accelX, accelY, accelZ) * 0.000244f;
        public Vector3 UncalibratedGyro => new Vector3(gyro1, gyro2, gyro3) * 0.070f;

        public Vector3 CalibratedAcceleration(ref SwitchControllerHID.IMUCalibrationData calib)
        {
            // var coeffs = new Vector3()
            // {
            //     x = (float)(1.0f / (float)(0x4000 - uint16_to_int16(cal_acc_origin))) * 4.0f
            // };

            var output = new Vector3();
            return output;
        }

        public Vector3 CalibratedGyro(ref SwitchControllerHID.IMUCalibrationData calib)
        {
            var offset = calib.gyroBase.ToVector3Int16();
            var coeff = calib.gyroSensitivity;

            float gyro_x = 0;
            float gyro_y = 0;
            float gyro_z = 0;
            
            // gyro X
            var gyro_cal_coeff_x = (float)(816.0f / (float)(coeff.x - offset.x));
            gyro_x = (gyro1 - offset.x) * gyro_cal_coeff_x;

            // gyro Y
            var gyro_cal_coeff_y = (float)(816.0f / (float)(coeff.y - offset.y));
            gyro_y = (gyro2 - offset.y) * gyro_cal_coeff_y;

            // gyro Z
            var gyro_cal_coeff_z = (float)(816.0f / (float)(coeff.z - offset.z));
            gyro_z = (gyro3 - offset.z) * gyro_cal_coeff_z;

            return new Vector3(gyro_x, gyro_y, gyro_z);
        }
    }
}