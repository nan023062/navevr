using UnityEngine;
using UnityEngine.XR;

namespace NaveXR.Device
{
    public abstract class XRInputBase
    {
        float cos45 = Mathf.Cos(Mathf.Deg2Rad * 45f);
        float sin45 = Mathf.Sin(Mathf.Deg2Rad * 45f);

        public XRKeyCode keyCode { get; private set; }

        public XRInputBase(XRKeyCode keyCode)
        {
            this.keyCode = keyCode;
        }

        protected Vector2 GetAreaDir(Vector2 axis)
        {
            return new Vector2(axis.x * cos45 - axis.y * sin45, axis.x * sin45 + axis.y * cos45);
        }

        protected bool isTouched(float force)
        {
            return force > 0f && force < 0.2f;
        }

        protected bool inCenter(Vector2 axis)
        {
            return Vector2.Dot(axis, axis) <= 0.25f;
        }

        protected bool inWest(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = GetAreaDir(axis);
            return (axis.x < 0 && axis.y < 0);
        }

        protected bool inEast(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = GetAreaDir(axis);
            return (axis.x > 0 && axis.y > 0);
        }

        protected bool inNorth(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = GetAreaDir(axis);
            return (axis.x < 0 && axis.y > 0);
        }

        protected bool inSouth(Vector2 axis)
        {
            if (inCenter(axis)) return false;
            axis = GetAreaDir(axis);
            return (axis.x > 0 && axis.y < 0);
        }

        public abstract void UpdateState(InputDevice device);
    }
}
