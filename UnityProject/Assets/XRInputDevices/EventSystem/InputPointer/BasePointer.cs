using UnityEngine;

namespace NaveXR.InputDevices
{
    public abstract class BaseInputPointer : MonoBehaviour
    {
        public Camera raycastCamera { protected set; get; }
        public Transform Transform { private set; get; }
        public Vector3 origin { get { return Transform.position; } }
        public Vector3 direction { get { return Transform.forward; } }

        public float raycastDistance = 5f;

        public float displayDistance = 20f;

        public LayerMask cullingMask = 0x7FFFFFFF;

        public abstract bool available { get; }

        protected virtual void Awake()
        {
            Transform = transform;
        }

        protected virtual void OnDestroy()
        {

        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {

        }

        public void SetCullingMask(int layer)
        {
            cullingMask.value |= layer;
            raycastCamera.cullingMask = cullingMask;
        }


    }
}
