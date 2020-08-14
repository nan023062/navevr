using UnityEngine.XR;
namespace NaveXR.InputDevices
{
    public class HandPrimary : HandInputBase
    {
        public HandPrimary() : base(XRKeyCode.Primary)
        {

        }

        internal override void UpdateState(InputNode xRNodeUsage)
        {
            HandInputNode handUsage = xRNodeUsage as HandInputNode;

            bool lastPressed = mPressed;
            mPressed = handUsage.primaryPressed;
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

            mKeyForce = handUsage.primaryTouchValue;
            mTouched = isTouched(mKeyForce);
        }
    }
}
