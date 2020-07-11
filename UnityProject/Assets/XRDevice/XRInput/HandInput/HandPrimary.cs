﻿using UnityEngine.XR;
namespace NaveXR.Device
{
    public class HandPrimary : HandInputBase
    {
        public HandPrimary() : base(XRKeyCode.Primary)
        {

        }

        public override void UpdateState(InputDevice device)
        {
            bool lastPressed = mPressed;
            device.TryGetFeatureValue(CommonUsages.primaryButton, out mPressed);
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;

            device.TryGetFeatureValue(CommonUsages.thumbTouch, out mFloatValue);
            device.TryGetFeatureValue(CommonUsages.primaryTouch, out mTouched);
        }
    }
}
