using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    public class HandMenu : HandInputBase
    {
        public HandMenu() : base(XRKeyCode.Menu)
        {

        }

        //当前按钮还未实现。
        public override void UpdateState(UnityEngine.XR.InputDevice device)
        {
            bool lastPressed = mPressed;
            device.TryGetFeatureValue(CommonUsages.menuButton, out mPressed);
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

            mKeyForce = 0f;
            if (mPressed) device.TryGetFeatureValue(CommonUsages.thumbTouch, out mKeyForce);
            mTouched = isTouched(mKeyForce);
        }
    }
}
