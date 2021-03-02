using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    public class HandMenu : HandInputBase
    {
        public HandMenu() : base(XRKeyCode.Menu)
        {

        }

        //当前按钮还未实现。
        internal override void UpdateState(InputNode xRNodeUsage)
        {
            HandInputNode handUsage = xRNodeUsage as HandInputNode;

            bool lastPressed = mPressed;
            mPressed = handUsage.systemPressed;
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

            mKeyForce = handUsage.systemTouchValue;
            mTouched = isTouched(mKeyForce);
        }
    }
}
