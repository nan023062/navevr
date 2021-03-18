using UnityEngine;
using UnityEngine.XR;

namespace Nave.VR
{
    public abstract class BaseInput
    {
        //逆时针转45°,正/余弦值
        float cos45 = Mathf.Cos(Mathf.Deg2Rad * 45f);
        float sin45 = Mathf.Sin(Mathf.Deg2Rad * 45f);
        float cos30 = Mathf.Cos(Mathf.Deg2Rad * 30f);

        public InputKey keyCode { get; private set; }

        public BaseInput(InputKey keyCode)
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
        /// 在中心区域
        /// </summary>
        protected bool inCenter(Vector2 axis)
        {
            return Vector2.Dot(axis, axis) <= 0.04f;
        }

        /// <summary>
        /// 西方向（旋转45°的第3象限）
        /// </summary>
        protected bool in45West(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = Rotate45AreaDir(axis);
            return (axis.x < 0 && axis.y < 0);
        }

        /// <summary>
        /// 东方向（旋转45°的第1象限）
        /// </summary>
        protected bool in45East(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = Rotate45AreaDir(axis);
            return (axis.x > 0 && axis.y > 0);
        }

        /// <summary>
        /// 北方向（旋转45°的第2象限）
        /// </summary>
        protected bool in45North(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = Rotate45AreaDir(axis);
            return (axis.x < 0 && axis.y > 0);
        }

        /// <summary>
        /// 北方向（旋转45°的第4象限）
        /// </summary>
        protected bool in45South(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = Rotate45AreaDir(axis);
            return (axis.x > 0 && axis.y < 0);
        }

        /// <summary>
        /// 旋转45°
        /// </summary>
        private Vector2 Rotate45AreaDir(Vector2 axis)
        {
            return new Vector2(axis.x * cos45 - axis.y * sin45, axis.x * sin45 + axis.y * cos45);
        }

        /// <summary>
        /// 两个轴夹角区域
        /// </summary>
        public bool CheckDirArea(Vector2 normal, Vector2 axis, float minDegree)
        {
            float angle = GetDirectionAngle(normal, axis.normalized);
            return angle < minDegree;
        }

        /// <summary>
        /// 两个方向夹角-弧度
        /// </summary>
        public float GetDirectionAngle(Vector2 from, Vector2 to)
        {
            return Mathf.Acos(Vector2.Dot(from.normalized, to.normalized)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 在西轴夹角degree范围
        /// </summary>
        protected bool inWest(Vector2 axis, float degree)
        {
            return CheckDirArea(Vector2.left, axis, degree);
        }

        /// <summary>
        /// 在东轴夹角degree范围
        /// </summary>
        protected bool inEast(Vector2 axis, float degree)
        {
            return CheckDirArea(Vector2.right, axis, degree);
        }

        /// <summary>
        /// 在北轴夹角degree范围
        /// </summary>
        protected bool inNorth(Vector2 axis, float degree)
        {
            return CheckDirArea(Vector2.up, axis, degree);
        }

        /// <summary>
        /// 在南轴夹角degree范围
        /// </summary>
        protected bool inSouth(Vector2 axis, float degree)
        {
            return CheckDirArea(Vector2.down, axis, degree);
        }

        public float angleToWest(Vector2 axis)
        {
            return GetDirectionAngle(Vector2.left, axis);
        }

        public float angleToEast(Vector2 axis)
        {
            return GetDirectionAngle(Vector2.right, axis);
        }

        public float angleToNorth(Vector2 axis)
        {
            return GetDirectionAngle(Vector2.up, axis);
        }

        public float angleToSouth(Vector2 axis)
        {
            return GetDirectionAngle(Vector2.down, axis);
        }

        internal abstract void UpdateState(TrackingAnchor xRNodeUsage);
    }
}
