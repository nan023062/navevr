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

        internal override void UpdateState(InputNode xRNodeUsage)
        {
            HandInputNode handUsage = xRNodeUsage as HandInputNode;
            mTouched = handUsage.primary2DAxisTouch;

            mVec2Value = Vector2.zero;
            mKeyForce = 0f;
            if (mTouched)
            {
                mVec2Value = handUsage.primary2DAxis;
                mKeyForce = 0.1f;
            }

            bool lastPressed = mPressed;
            mPressed = handUsage.primary2DAxisPressed;
            if (mPressed) mKeyForce = 1f;

            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;
        }
    }
}
