using UnityEngine;
using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    public class HandTouchAxis: HandInputBase
    {
        protected UnityEngine.Vector2 mVec2Value;
        public UnityEngine.Vector2 Value2D { get { return mVec2Value; } }

        public HandTouchAxis() : base(XRKeyCode.TouchAxis)
        {

        }

        public override void UpdateState(UnityEngine.XR.InputDevice device)
        {
            device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out mTouched);

            mVec2Value = Vector2.zero;
            if (mTouched)
            {
                device.TryGetFeatureValue(CommonUsages.primary2DAxis, out mVec2Value);
                device.TryGetFeatureValue(CommonUsages.thumbTouch, out mFloatValue);
            }

            bool lastPressed = mPressed;
            device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out mPressed);
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;
        }
    }
}
