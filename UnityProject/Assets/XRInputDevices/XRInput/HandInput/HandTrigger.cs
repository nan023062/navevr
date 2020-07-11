using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    public class HandTrigger : HandInputBase
    { 
        public HandTrigger() :base(XRKeyCode.Trigger)
        {

        }

        public override void UpdateState(UnityEngine.XR.InputDevice device)
        {
            device.TryGetFeatureValue(CommonUsages.trigger, out mFloatValue);
            mTouched = isTouched(mFloatValue);

            bool lastPressed = mPressed;

            //Oculus 驱动时 triggerButton不是真的点击效果，所以使用Touched来判断
            if (XRDevice.Driver == XRDVName.Oculus)
            {
                mPressed = mFloatValue > 0.88F;
            }
            else if (XRDevice.Driver == XRDVName.OpenVR)
            {
                device.TryGetFeatureValue(CommonUsages.triggerButton, out mPressed);
            }
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

        }
    }
}
