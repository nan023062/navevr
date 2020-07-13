using UnityEngine;
using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// XR输入基类
    /// </summary>
    public abstract class XRInputBase
    {
        //逆时针转45°,正/余弦值
        float cos45 = Mathf.Cos(Mathf.Deg2Rad * 45f);
        float sin45 = Mathf.Sin(Mathf.Deg2Rad * 45f);

        public XRKeyCode keyCode { get; private set; }

        public XRInputBase(XRKeyCode keyCode)
        {
            this.keyCode = keyCode;
        }

        /// <summary>
        /// 根据力force，判断是触摸状态
        /// </summary>
        protected bool isTouched(float force)
        {
            return force > 0.0001f && force < 0.2f;
        }

        /// <summary>
        /// 在中心区域（半径一半以内区域）
        /// </summary>
        protected bool inCenter(Vector2 axis)
        {
            return Vector2.Dot(axis, axis) <= 0.25f;
        }

        /// <summary>
        /// 西方向（旋转45°的第3象限）
        /// </summary>
        protected bool inWest(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = GetAreaDir(axis);
            return (axis.x < 0 && axis.y < 0);
        }

        /// <summary>
        /// 东方向（旋转45°的第1象限）
        /// </summary>
        protected bool inEast(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = GetAreaDir(axis);
            return (axis.x > 0 && axis.y > 0);
        }

        /// <summary>
        /// 北方向（旋转45°的第2象限）
        /// </summary>
        protected bool inNorth(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = GetAreaDir(axis);
            return (axis.x < 0 && axis.y > 0);
        }

        /// <summary>
        /// 北方向（旋转45°的第4象限）
        /// </summary>
        protected bool inSouth(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = GetAreaDir(axis);
            return (axis.x > 0 && axis.y < 0);
        }

        /// <summary>
        /// 旋转45°
        /// </summary>
        private Vector2 GetAreaDir(Vector2 axis)
        {
            return new Vector2(axis.x * cos45 - axis.y * sin45, axis.x * sin45 + axis.y * cos45);
        }

        public abstract void UpdateState(UnityEngine.XR.InputDevice device);
    }
}
