using UnityEngine;
using UnityEngine.EventSystems;

namespace NaveXR.EventSystem
{
    public class XRPointEventData : PointerEventData
    {
        public XRPointEventData(UnityEngine.EventSystems.EventSystem eventSystem) :base(eventSystem)
        {

        }

        public Vector3 origin;

        public Vector3 direction;

        public float hitDistance;

        public Vector3 hitPoint;

        public Vector3 hitNormal;

        public Camera raycastCamera;

        public static PointerEventData.FramePressState StateForButton(bool pressed, bool released)
        {
            if (pressed && released)
                return PointerEventData.FramePressState.PressedAndReleased;
            if (pressed)
                return PointerEventData.FramePressState.Pressed;
            if (released)
                return PointerEventData.FramePressState.Released;
            return PointerEventData.FramePressState.NotChanged;
        }

    }
}
