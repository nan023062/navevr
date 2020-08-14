using UnityEngine;
using UnityEngine.XR;
namespace NaveXR.InputDevices
{
    /// <summary>
    /// 2020.08.12修改：轴和触摸板 提供与轴的夹角参数
    /// </summary>
    public class HandTouchButton : HandInputBase
    {
        public const float degree45 = 45f;
        float touchPressSqr = 0.83f * 0.83f;
        private float m_Degree = 0f;
        public float degree { get { return m_Degree; } }

        public HandTouchButton(XRKeyCode keyCode) : base(keyCode)
        {

        }

        internal override void UpdateState(InputNode xRNodeUsage)
        {
            HandInputNode handUsage = xRNodeUsage as HandInputNode;
            mTouched = handUsage.primary2DAxisTouch;
            mKeyForce = 0f;
            bool lastPressed = mPressed;
            mPressed = false;

            if (mTouched){             
                Vector2 axis = handUsage.primary2DAxis;

                mTouched = false;
                m_Degree = 0f;
                bool bInCenter = inCenter(axis);
                if (keyCode == XRKeyCode.TouchMiddle) {
                    mTouched = bInCenter;
                }
                else if (!bInCenter && keyCode == XRKeyCode.TouchNorth) {
                    m_Degree = angleToNorth(axis);
                    mTouched = m_Degree < degree45;
                }
                else if (!bInCenter && keyCode == XRKeyCode.TouchSouth) {
                    m_Degree = angleToSouth(axis);
                    mTouched = m_Degree < degree45;
                }
                else if (!bInCenter && keyCode == XRKeyCode.TouchWest) {
                    m_Degree = angleToWest(axis);
                    mTouched = m_Degree < degree45;
                }
                else if (!bInCenter && keyCode == XRKeyCode.TouchEast) {
                    m_Degree = angleToEast(axis);
                    mTouched = m_Degree < degree45;
                }
        
                if (mTouched){
                    if (keyCode == XRKeyCode.TouchMiddle || XRDevice.isTouchPad){
                        mPressed = handUsage.primary2DAxisPressed;
                    }
                    else{
                        mPressed = axis.sqrMagnitude >= touchPressSqr;
                    }
                }
            }
            mBoolDown = !lastPressed && mPressed;
            mBoolUp = lastPressed && !mPressed;
        }
    }
}
