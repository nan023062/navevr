using UnityEngine.XR;
namespace Nave.VR
{
    public class HandGrip : HandInputBase
    {
        public HandGrip() : base(InputKey.Grip)
        {

        }

        internal override void UpdateState(TrackingAnchor xRNodeUsage)
        {
            HandAnchor handUsage = xRNodeUsage as HandAnchor;

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
