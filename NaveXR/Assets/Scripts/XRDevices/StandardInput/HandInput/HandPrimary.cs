using UnityEngine.XR;
namespace Nave.XR
{
    public class HandPrimary : HandInputBase
    {
        public HandPrimary() : base(KeyCode.Primary)
        {

        }

        internal override void UpdateState(Metadata xRNodeUsage)
        {
            HandMetadata handUsage = xRNodeUsage as HandMetadata;

            bool lastPressed = mPressed;
            mPressed = handUsage.primaryPressed;
            mTouched = handUsage.primaryTouch;
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

            mKeyForce = handUsage.primaryTouchValue;
        }
    }
}
