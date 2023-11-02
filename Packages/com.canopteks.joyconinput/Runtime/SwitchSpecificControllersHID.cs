using UnityEditor;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Switch.LowLevel;

namespace UnityEngine.InputSystem.Switch
{
    [InputControlLayout(stateType = typeof(SwitchControllerVirtualInputState), displayName = "Joy-Con (L)")]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class SwitchJoyConLHID : SwitchControllerHID
    {
        static SwitchJoyConLHID()
        {
            var matcher = new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x57E)
                .WithCapability("productId", 0x2006);
            
            InputSystem.RegisterLayout<SwitchJoyConLHID>(matches: matcher);
            
            Debug.Log("Joy-Con (L) layout registered");
        }
        
        [RuntimeInitializeOnLoadMethod]
        static void Init() { }
    }
    
    
    
    [InputControlLayout(stateType = typeof(SwitchControllerVirtualInputState), displayName = "Joy-Con (R)")]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class SwitchJoyConRHID : SwitchControllerHID
    {
        static SwitchJoyConRHID()
        {
            var matcher = new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x57E)
                .WithCapability("productId", 0x2007);
            
            InputSystem.RegisterLayout<SwitchJoyConRHID>(matches: matcher);
            
            Debug.Log("Joy-Con (R) layout registered");
        }
        
        [RuntimeInitializeOnLoadMethod]
        static void Init() { }
    }
    
    [InputControlLayout(stateType = typeof(SwitchControllerVirtualInputState), displayName = "Pro Controller")]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class SwitchProControllerNewHID : SwitchControllerHID
    {
        static SwitchProControllerNewHID()
        {
            var matcher = new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x57E)
                .WithCapability("productId", 0x2009);
            
            InputSystem.RegisterLayout<SwitchProControllerNewHID>(matches: matcher);
            
            Debug.Log("Pro Controller layout registered");
        }
        
        [RuntimeInitializeOnLoadMethod]
        static void Init() { }
    }
}