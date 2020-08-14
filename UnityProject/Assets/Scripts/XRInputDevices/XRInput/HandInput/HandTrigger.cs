using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    public class HandTrigger : HandInputBase
    { 
        public HandTrigger() :base(XRKeyCode.Trigger)
        {

        }

        internal override void UpdateState(InputNode xRNodeUsage)
        {
            bool lastPressed = mPressed;
            float lastForce = mKeyForce;

            HandInputNode handUsage = xRNodeUsage as HandInputNode;
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
