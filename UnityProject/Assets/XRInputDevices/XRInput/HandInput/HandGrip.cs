using UnityEngine.XR;
namespace NaveXR.InputDevices
{
    public class HandGrip : HandInputBase
    {
        public HandGrip() : base(XRKeyCode.Grip)
        {

        }

        public override void UpdateState(UnityEngine.XR.InputDevice device)
        {
            bool lastPressed = mPressed;
            device.TryGetFeatureValue(CommonUsages.gripButton, out mPressed);

            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

            device.TryGetFeatureValue(CommonUsages.grip, out mFloatValue);
            mTouched = isTouched(mFloatValue);
        }
    }
}
