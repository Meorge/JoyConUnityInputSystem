using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputSystem.Switch
{
    // TODO: Create a nice little CustomPropertyDrawer with clamped sliders and a Play button
    /// <summary>
    /// ScriptableObject to create reusable vibration profiles through the Editor.
    /// </summary>
    [CreateAssetMenu(fileName = "RumbleData", menuName = "JoyconInput/RumbleDataSheet", order = 1)]
    public class RumbleDataSheet : ScriptableObject
    {
        [SerializeField]
        public SwitchControllerRumbleProfile profile;

        /// <summary>
        /// Play the rumble defined by the structure on the specified controller if it exists.
        /// </summary>
        /// <param name="controller"></param>
        public void PlayOn(params SwitchControllerHID[] controllers)
        {
            foreach (SwitchControllerHID controller in controllers)
            {
                if (controller == null)
                    continue;

                controller.Rumble(profile);
            }
        }
    } 
}
