using UnityEngine.XR;
namespace Nave.VR
{
    public class HandPrimary : HandInputBase
    {
        public HandPrimary() : base(InputKey.Primary)
        {

        }

        internal override void UpdateState(TrackingAnchor xRNodeUsage)
        {
            HandAnchor handUsage = xRNodeUsage as HandAnchor;

            bool lastPressed = mPressed;
            mPressed = handUsage.primaryPressed;
            mTouched = handUsage.primaryTouch;
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

            mKeyForce = handUsage.primaryTouchValue;
        }
    }
}
