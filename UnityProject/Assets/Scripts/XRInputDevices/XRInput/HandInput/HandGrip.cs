using UnityEngine.XR;
namespace NaveXR.InputDevices
{
    public class HandGrip : HandInputBase
    {
        public HandGrip() : base(XRKeyCode.Grip)
        {

        }

        internal override void UpdateState(InputNode xRNodeUsage)
        {
            HandInputNode handUsage = xRNodeUsage as HandInputNode;

            bool lastPressed = mPressed;
            float lastForce = mKeyForce;
            mKeyForce = handUsage.gripTouchValue;
            mTouched = isTouched(mKeyForce);
            
            mPressed = OptimizPressByKeyForce(lastForce, mKeyForce, 0.01f, 0.2f, 0.6f);
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;
        }
    }
}
