using UnityEngine.XR;

namespace Nave.VR
{
    public class HandTrigger : HandInputBase
    { 
        public HandTrigger() :base(InputKey.Trigger)
        {

        }

        internal override void UpdateState(TrackingAnchor xRNodeUsage)
        {
            bool lastPressed = mPressed;
            float lastForce = mKeyForce;

            HandAnchor handUsage = xRNodeUsage as HandAnchor;
            mKeyForce = handUsage.triggerTouchValue;
            mTouched = isTouched(mKeyForce);

            //这里不使用UnityXR的按钮状态，因为会有感官上的延迟
            //device.TryGetFeatureValue(CommonUsages.triggerButton, out mPressed);
            mPressed = OptimizPressByKeyForce(lastForce, mKeyForce, 0.01f, 0.2f, 0.6f);
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;
        }
    }
}
