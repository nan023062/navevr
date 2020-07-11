using UnityEngine;
using UnityEngine.XR;
namespace NaveXR.Device
{
    public class HandTouchButton : HandInputBase
    {
        float touchPressSqr = 0.83f * 0.83f;

        public HandTouchButton(XRKeyCode keyCode) : base(keyCode)
        {

        }

        public override void UpdateState(InputDevice device)
        {
            device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out mTouched);

            mFloatValue = 0f;
            bool lastPressed = mPressed;
            mPressed = false;

            if (mTouched)
            {
                Vector2 axis = Vector2.zero;
                device.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis);

                mTouched = false;
                if (keyCode == XRKeyCode.TouchMiddle) mTouched = inCenter(axis);
                else if (keyCode == XRKeyCode.TouchNorth) mTouched = inNorth(axis);
                else if(keyCode == XRKeyCode.TouchSouth) mTouched = inSouth(axis);
                else if(keyCode == XRKeyCode.TouchWest) mTouched = inWest(axis);
                else if(keyCode == XRKeyCode.TouchEast) mTouched = inEast(axis);

                if (mTouched)
                {
                    device.TryGetFeatureValue(CommonUsages.thumbTouch, out mFloatValue);
                    mPressed = axis.sqrMagnitude >= touchPressSqr;
                }
            }
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;
        }
    }
}
