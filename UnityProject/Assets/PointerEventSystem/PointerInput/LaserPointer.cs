using UnityEngine;
using UnityEngine.EventSystems;
using NaveXR.Device;

namespace NaveXR.EventSystem
{
    public class LaserPointer : BasePointer
    {
        public enum InputType
        {
            LeftHand,RightHand,Mouse,
        }

        public override bool available { get { return true; } }

        public InputType inputType = InputType.Mouse;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            XRDevice.Regist(this);
        }

        protected override void OnDisable()
        {
            XRDevice.Remove(this);
            base.OnDisable();
        }

        private void Update()
        {
            if (inputType == InputType.LeftHand)
            {
                isPressed = XRDevice.IsLeftButtonPressed(XRKeyCode.Trigger);
                isReleased = !isPressed;
            }
            else if (inputType == InputType.RightHand)
            {
                isPressed = XRDevice.IsRightButtonPressed(XRKeyCode.Trigger);
                isReleased = !isPressed;
            }
            else if (inputType == InputType.Mouse)
            {
                isPressed = Input.GetMouseButton(0);
                isReleased = !isPressed;
            }
        }
    }
}
