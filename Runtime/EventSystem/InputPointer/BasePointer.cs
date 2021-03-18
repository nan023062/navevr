using UnityEngine;
using UnityEngine.EventSystems;

namespace Nave.VR
{
    public abstract class BaseInputPointer : MonoBehaviour
    {
        public Camera raycastCamera { protected set; get; }
        public Transform Transform { private set; get; }
        public Vector3 origin { get { return Transform.position; } }
        public Vector3 direction { get { return Transform.forward; } }

        public float raycastDistance = 5f;

        public float displayDistance = 30f;

        public LayerMask cullingMask = 0x7FFFFFFF;

        public abstract bool available { get; }

        public bool isHit { protected set; get; }

        public Vector3 hitPoint { protected set; get; }

        public Vector3 hitNormal { protected set; get; }

        public GameObject hitTarget { protected set; get; }

        public Vector2 delta { protected set; get; }

        public Vector2 scrollDelta { protected set; get; }

        protected float scrollThrelod = 0.70f;

        protected float handScrollSensitivity = 0.015F;

        protected virtual void Awake()
        {
            Transform = transform;
        }

        protected virtual void Start()
        {

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

        internal void UpdateStateFormRaycast(ref RaycastResult raycast)
        {
            UpdatePressedAndReleased(ref raycast);

            UpdateHitState(ref raycast);
        }

        protected abstract void UpdateHitState(ref RaycastResult raycast);

        protected abstract void UpdatePressedAndReleased(ref RaycastResult raycast);

    }
}
