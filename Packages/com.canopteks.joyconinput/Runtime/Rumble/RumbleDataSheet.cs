using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputSystem.Switch
{
    [CreateAssetMenu(fileName = "RumbleData", menuName = "JoyconInput/RumbleDataSheet", order = 1)]
    public class RumbleDataSheet : ScriptableObject
    {
        [SerializeField]
        public SwitchControllerRumbleProfile rumbleProfile;
    } 
}
