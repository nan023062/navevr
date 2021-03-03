namespace Nave.XR
{
    /// <summary>
    /// 手柄输入
    /// </summary>
    public abstract class HandInputBase : BaseInput
    {
        protected float mKeyForce;

        protected bool mBoolDown, mBoolUp, mPressed, mTouched;

        public HandInputBase(KeyCode keyCode):base(keyCode)
        {
            mKeyForce = 0f;
            mBoolDown = false;
            mBoolUp = false;
            mPressed = false;
            mTouched = false;
        }

        /// <summary>
        /// 按键压力值
        /// </summary>
        public float Force { get { return mKeyForce; } }

        /// <summary>
        /// 当按键抬起时触发
        /// </summary>
        public bool KeyDown { get { return mBoolDown; } }

        /// <summary>
        /// 当按键按下时触发
        /// </summary>
        public bool KeyUp { get { return mBoolUp; } }

        /// <summary>
        /// 按下状态
        /// </summary>
        public bool Pressed { get { return mPressed; } }

        /// <summary>
        /// 触摸状态
        /// </summary>
        public bool Touched { get { return mTouched; } }

        /// <summary>
        /// 使用按键力 优化的按下\抬起判断
        /// </summary>
        /// <returns></returns>
        public static bool OptimizPressByKeyForce(float lastForce, float currForce, 
            float subThreshold, float addThreshold, float forceThreshold)
        {
            //当按键力在变小或小于阈值，认为是抬起了按键
            if (currForce < lastForce - subThreshold || currForce < forceThreshold)
                return false;
            //当按键力在变大或大于阈值，认为是按下了按键
            else if (currForce > lastForce + addThreshold || currForce >= forceThreshold)
                return true;
            return false;
        }
    }
}

