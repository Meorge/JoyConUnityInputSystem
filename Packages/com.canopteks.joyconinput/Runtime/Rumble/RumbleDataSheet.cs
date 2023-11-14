using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputSystem.Switch
{
    [CreateAssetMenu(fileName = "RumbleData", menuName = "JoyconInput/RumbleDataSheet", order = 1)]
    public class RumbleDataSheet : ScriptableObject
    {
        [SerializeField]
        private SwitchControllerRumbleProfile rumbleProfile;
        public void Awake()
        {
            rumbleProfile = SwitchControllerRumbleProfile.CreateNeutral();
        }

        public void Play()
        {
            if (SwitchControllerHID.current == null)
            {
                Debug.LogWarning("No Joycon controller detected to vibrate.");
                return;
            }
            
            SwitchControllerHID.current.Rumble(rumbleProfile);
        }
    } 
}
