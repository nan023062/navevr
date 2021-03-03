using UnityEngine.XR;
namespace Nave.XR
{
    public class HandGrip : HandInputBase
    {
        public HandGrip() : base(KeyCode.Grip)
        {

        }

        internal override void UpdateState(Metadata xRNodeUsage)
        {
            HandMetadata handUsage = xRNodeUsage as HandMetadata;

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
