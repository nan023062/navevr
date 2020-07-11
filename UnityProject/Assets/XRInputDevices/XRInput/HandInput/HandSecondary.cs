using UnityEngine.XR;
namespace NaveXR.InputDevices
{
    public class HandSecondary : HandInputBase
    {
        public HandSecondary() : base(XRKeyCode.Secondary)
        {

        }

        //当前按钮 只有点击事件
        public override void UpdateState(UnityEngine.XR.InputDevice device)
        {
            bool lastPressed = mPressed;
            device.TryGetFeatureValue(CommonUsages.secondaryButton, out mPressed);
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

            device.TryGetFeatureValue(CommonUsages.thumbTouch, out mFloatValue);
            device.TryGetFeatureValue(CommonUsages.secondaryTouch, out mTouched);
        }
    }
}
