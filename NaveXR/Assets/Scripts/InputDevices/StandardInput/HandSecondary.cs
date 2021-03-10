using UnityEngine.XR;
namespace Nave.VR
{
    public class HandSecondary : HandInputBase
    {
        public HandSecondary() : base(InputKey.Secondary)
        {

        }

        //当前按钮 只有点击事件
        internal override void UpdateState(TrackingAnchor xRNodeUsage)
        {
            HandAnchor handUsage = xRNodeUsage as HandAnchor;

            bool lastPressed = mPressed;
            mPressed = handUsage.secondaryPressed;
            mTouched = handUsage.secondaryTouch;
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;
            
            mKeyForce = handUsage.secondaryTouchValue;
        }
    }
}
