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

        /// <summary>
        /// The last used/added Joy-Con (R) controller.
        /// </summary>
        public static new SwitchJoyConLHID current { get; private set; }

        /// <inheritdoc />
        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }

        /// <inheritdoc />
        protected override void OnRemoved()
        {
            base.OnRemoved();
            if (current == this)
                current = null;
        }
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

        /// <summary>
        /// The last used/added Joy-Con (R) controller.
        /// </summary>
        public static new SwitchJoyConRHID current { get; private set; }

        /// <inheritdoc />
        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }
        
        /// <inheritdoc />
        protected override void OnRemoved()
        {
            base.OnRemoved();
            if (current == this)
                current = null;
        }
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

        /// <summary>
        /// The last used/added Joy-Con (R) controller.
        /// </summary>
        public static new SwitchProControllerNewHID current { get; private set; }

        /// <inheritdoc />
        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }
        
        /// <inheritdoc />
        protected override void OnRemoved()
        {
            base.OnRemoved();
            if (current == this)
                current = null;
        }
    }
}