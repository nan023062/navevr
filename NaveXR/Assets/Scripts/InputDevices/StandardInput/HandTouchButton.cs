using UnityEngine;
using UnityEngine.XR;
namespace Nave.VR
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

        public HandTouchButton(InputKey keyCode) : base(keyCode)
        {

        }

        internal override void UpdateState(TrackingAnchor xRNodeUsage)
        {
            HandAnchor handUsage = xRNodeUsage as HandAnchor;
            mTouched = handUsage.primary2DAxisTouch;
            mKeyForce = 0f;
            bool lastPressed = mPressed;
            mPressed = false;

            if (mTouched){             
                Vector2 axis = handUsage.primary2DAxis;

                mTouched = false;
                m_Degree = 0f;
                bool bInCenter = inCenter(axis);
                if (keyCode == InputKey.Middle) {
                    mTouched = bInCenter;
                }
                else if (!bInCenter && keyCode == InputKey.North) {
                    m_Degree = angleToNorth(axis);
                    mTouched = m_Degree < degree45;
                }
                else if (!bInCenter && keyCode == InputKey.South) {
                    m_Degree = angleToSouth(axis);
                    mTouched = m_Degree < degree45;
                }
                else if (!bInCenter && keyCode == InputKey.West) {
                    m_Degree = angleToWest(axis);
                    mTouched = m_Degree < degree45;
                }
                else if (!bInCenter && keyCode == InputKey.East) {
                    m_Degree = angleToEast(axis);
                    mTouched = m_Degree < degree45;
                }
        
                if (mTouched){
                    if (keyCode == InputKey.Middle || InputDevices.isTouchPad){
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
