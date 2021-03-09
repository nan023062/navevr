using UnityEngine.XR;

namespace Nave.VR
{
    public class HandMenu : HandInputBase
    {
        public HandMenu() : base(KeyCode.Menu)
        {

        }

        //当前按钮还未实现。
        internal override void UpdateState(TrackingAnchor xRNodeUsage)
        {
            HandAnchor handUsage = xRNodeUsage as HandAnchor;

            bool lastPressed = mPressed;
            mPressed = handUsage.systemPressed;
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

            mKeyForce = handUsage.systemTouchValue;
            mTouched = isTouched(mKeyForce);
        }
    }
}
