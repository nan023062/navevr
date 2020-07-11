namespace NaveXR.Device
{
    /// <summary>
    /// 手柄输入
    /// </summary>
    public abstract class HandInputBase : XRInputBase
    {
        protected float mFloatValue;

        protected bool mBoolDown, mBoolUp, mPressed, mTouched;

        public HandInputBase(XRKeyCode keyCode):base(keyCode)
        {
            mFloatValue = 0f;
            mBoolDown = false;
            mBoolUp = false;
            mPressed = false;
            mTouched = false;
        }

        public float Value { get { return mFloatValue; } }

        public bool KeyDown { get { return mBoolDown; } }

        public bool KeyUp { get { return mBoolUp; } }

        public bool Pressed { get { return mPressed; } }

        public bool Touched { get { return mTouched; } }
    }
}

