using UnityEngine;
using UnityEngine.Rendering;

namespace NaveXR.EventSystem
{

    [RequireComponent(typeof(Camera))]
    public abstract class BasePointer : MonoBehaviour
    {
        public int fingerId { get; private set; }

        private static int s_fingerId = 0;
        public Camera raycastCamera { private set; get; }
        public Transform Transform { private set; get; }
        public bool isReleased { protected set; get; }
        public bool isPressed { protected set; get; }
        public Vector3 origin { get { return Transform.position; } }
        public Vector3 direction { get { return Transform.forward; } }

        public float raycastDistance = 5f;

        public float displayDistance = 20f;

        public LayerMask cullingMask = 0x7FFFFFFF;

        public abstract bool available { get; }

        protected virtual void Awake()
        {
            fingerId = s_fingerId++;
            Transform = transform;
            raycastCamera = GetComponent<Camera>();
            raycastCamera.enabled = false;
            raycastCamera.cullingMask = 0;
            raycastCamera.farClipPlane = 100f;
            raycastCamera.nearClipPlane = 0f;
            raycastCamera.orthographic = true;
            raycastCamera.orthographicSize = 0.2f;
            raycastCamera.cullingMask = cullingMask;
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
