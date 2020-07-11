using UnityEngine;
using UnityEngine.EventSystems;

namespace NaveXR.EventSystem
{
    public class FingerPointer : BasePointer
    {
        private float touchDis = 0.22f;
        private float pressedDis = 0.10f;
        private float currentDis = float.MaxValue;

        public override bool available { get { return currentDis <= touchDis;  } }

        protected override void Awake()
        {
            base.Awake();
            displayDistance = 0f;
        }

        protected override void OnEnable()
        {
            NaveXR.Device.XRDevice.Regist(this);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
        }

        protected override void OnDisable()
        {
            NaveXR.Device.XRDevice.Remove(this);
        }

        public void UpdateStateFormRaycast(ref RaycastResult raycast)
        {
            currentDis = float.MaxValue;
            if (raycast.gameObject != null ) {
                currentDis = Vector3.Distance(raycast.worldPosition, origin);
            }

            isReleased = true;
            isPressed = false;

            if (currentDis < pressedDis)
            {
                isPressed = true;
                isReleased = false;
            }
        }
    }
}
