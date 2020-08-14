using UnityEngine.XR;
namespace NaveXR.InputDevices
{
    public class HandSecondary : HandInputBase
    {
        public HandSecondary() : base(XRKeyCode.Secondary)
        {

        }

        //当前按钮 只有点击事件
        internal override void UpdateState(InputNode xRNodeUsage)
        {
            HandInputNode handUsage = xRNodeUsage as HandInputNode;

            bool lastPressed = mPressed;
            mPressed = handUsage.secondaryPressed;
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

            mKeyForce = handUsage.secondaryTouchValue;
            mTouched = isTouched(mKeyForce);
        }
    }
}
