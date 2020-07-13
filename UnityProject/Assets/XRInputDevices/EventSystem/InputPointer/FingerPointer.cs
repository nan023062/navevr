using UnityEngine;
using UnityEngine.EventSystems;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// 手指输入触发点
    /// 需要手指交互的目标最好是2D Object（UI / Physical2D）
    /// </summary>
    public class FingerPointer : BaseInputPointer
    {
        public int fingerId { get; private set; }

        private static int s_fingerId = 0;

        /// <summary>
        /// 安全检测距离，优化可以轻微的点击穿模
        /// </summary>
        private float safeDistance = 0f;

        /// <summary>
        /// 手指事件有效距离
        /// </summary>
        private float checkDistance = 0.1f;

        /// <summary>
        /// 触发点击的距离
        /// </summary>
        private float pressedDis = 0.05f;

        private float currentDis = float.MaxValue;

        public override bool available { get { return currentDis <= checkDistance;  } }

        public TouchPhase phase { private set; get; } = TouchPhase.Canceled;

        [Header("运动参数")]
        [Range(0f,1f)] public float force;
        public Vector3 velocity;
        public Vector3 acceleration;

        protected override void Awake()
        {
            fingerId = s_fingerId++;
            base.Awake();
            displayDistance = 0f;

            // 为适应Unity.EventSystem 事件逻辑，需要构建射线检测相机实例
            raycastCamera = GetComponentInChildren<Camera>();
            if (raycastCamera == null)
            {
                var go= new GameObject("RaycastCamera", typeof(Camera));
                go.transform.SetParent(transform);
                raycastCamera = go.GetComponent<Camera>();
                //go.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
            }

            float dis = safeDistance / Mathf.Max(0.00001f, transform.lossyScale.z);
            raycastCamera.transform.localPosition = new Vector3(0, 0, -dis);
            raycastCamera.transform.localRotation = Quaternion.identity;
            raycastCamera.transform.localScale = Vector3.one;
            raycastCamera.enabled = false;
            raycastCamera.cullingMask = 0;
            raycastCamera.farClipPlane = 100f;
            raycastCamera.nearClipPlane = 0f;
            raycastCamera.orthographic = true;
            raycastCamera.orthographicSize = 0.2f;
            raycastCamera.cullingMask = cullingMask;
        }

        //注册事件
        protected override void OnEnable()
        {
            XRDevice.Regist(this);
        }

        //注销事件
        protected override void OnDisable()
        {
            XRDevice.Remove(this);
        }

        /// <summary>
        /// 处理手指当前交互状态
        /// </summary>
        public void UpdateStateFormRaycast(ref RaycastResult raycast)
        {
            currentDis = raycastDistance;
            if (raycast.gameObject != null )
                currentDis = Vector3.Distance(raycast.worldPosition, origin);

            if (currentDis >= checkDistance)
            {
                phase = TouchPhase.Canceled;
            }
            else if (currentDis < pressedDis)
            {
                if (phase == TouchPhase.Canceled|| phase == TouchPhase.Ended)
                    phase = TouchPhase.Began;
                else if (phase == TouchPhase.Began)
                    phase = TouchPhase.Moved;
            }
            else
            {
                phase = TouchPhase.Ended;
            }
        }
    }
}
