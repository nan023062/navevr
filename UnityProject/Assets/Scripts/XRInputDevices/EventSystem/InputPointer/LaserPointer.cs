using UnityEngine;
using UnityEngine.EventSystems;
using NaveXR.InputDevices;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// 射线输入触发点
    /// </summary>
    public class LaserPointer : BaseInputPointer
    {
        public int fingerId { get; private set; }

        private static int s_fingerId = 0;

        public enum InputType
        {
            Mouse       = 0,
            LeftHand    = 1,
            RightHand   = 2,
        }

        public override bool available { get { return true; } }

        public bool isReleased { private set; get; }

        public bool isPressed { private set; get; }

        public InputType inputType = InputType.Mouse;

        protected override void Awake()
        {
            fingerId = s_fingerId++;
            base.Awake();

            // 为适应Unity.EventSystem 事件逻辑，需要构建射线检测相机实例
            raycastCamera = GetComponent<Camera>();
            if (raycastCamera == null)
                raycastCamera = gameObject.AddComponent<Camera>();
            raycastCamera.enabled = false;
            raycastCamera.farClipPlane = 100f;
            raycastCamera.nearClipPlane = 0f;
            raycastCamera.orthographic = true;
            raycastCamera.orthographicSize = 0.2f;
            raycastCamera.cullingMask = cullingMask;
            raycastCamera.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
        }

        protected override void Start()
        {
            PrepareLaserRenderer();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            XRDevice.Regist(this);
        }

        protected override void OnDisable()
        {
            XRDevice.Remove(this);
            base.OnDisable();
        }

        protected override void UpdateHitState(ref RaycastResult raycast)
        {
            isHit = raycast.gameObject != null;
            if (isHit)
            {
                Vector3 lastHitPoint = hitPoint;

                //左手按鍵狀態
                if (inputType == InputType.LeftHand)
                {
                    Vector2 aixs = XRDevice.GetTouchAxis(0);
                    if (aixs.sqrMagnitude > scrollThrelod)
                        scrollDelta = aixs * handScrollSensitivity;
                }
                //右手按鍵狀態
                else if (inputType == InputType.RightHand)
                {
                    Vector2 aixs = XRDevice.GetTouchAxis(1);
                    if (aixs.sqrMagnitude > scrollThrelod)
                        scrollDelta = aixs * handScrollSensitivity;
                }
                //Unity原生模式狀態
                else if (inputType == InputType.Mouse)
                {
#if UNITY_STANDALONE || UNITY_EDITOR
                    scrollDelta = Input.mouseScrollDelta;

#elif UNITY_ANDROID || UNITY_IOS
                    if (Input.touchCount > 0)
                    {
                        Touch touch = Input.GetTouch(0);
                        delta = touch.deltaPosition;
                    }                
#endif
                }

                hitPoint = raycast.worldPosition;
                hitNormal = raycast.worldNormal;
                hitTarget = raycast.gameObject;
                Vector3 moveDelta = hitPoint - lastHitPoint;
                delta = moveDelta - Vector3.Project(moveDelta, hitNormal);
            }
            UpdateLaserRenderer();
        }

        protected override void UpdatePressedAndReleased(ref RaycastResult raycast)
        {
            //左手按鍵狀態
            if (inputType == InputType.LeftHand)
            {
                isPressed = XRDevice.IsLeftKeyDown(XRKeyCode.Trigger);
                isReleased = XRDevice.IsLeftKeyUp(XRKeyCode.Trigger);
            }
            //右手按鍵狀態
            else if (inputType == InputType.RightHand)
            {
                isPressed = XRDevice.IsRightKeyDown(XRKeyCode.Trigger);
                isReleased = XRDevice.IsRightKeyUp(XRKeyCode.Trigger);
            }
            //Unity原生模式狀態
            else if (inputType == InputType.Mouse)
            {
#if UNITY_STANDALONE || UNITY_EDITOR
                isPressed = Input.GetMouseButtonDown(0);
                isReleased = Input.GetMouseButtonUp(0);

#elif UNITY_ANDROID || UNITY_IOS
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    isPressed = (touch.phase == TouchPhase.Began);
                    isReleased = (touch.phase == TouchPhase.Ended);
                }                     
#endif
            }
        }

#region Laser  Renderer

        [SerializeField] private float m_LineSize = 0.04f;
        [SerializeField] private float m_HitSize = 0.04f;
        private GameObject m_Line = null;
        private GameObject m_Hit = null;
        private Material m_LineMaterial = null;
        private bool m_showLine = false;

        public bool isVisiable { get { return m_showLine; } }

        public void SetVisiable(bool visiable) {

            if (m_showLine == visiable) return;
            m_showLine = visiable;
            m_Line?.SetActive(m_showLine);
            m_Hit?.SetActive(m_showLine && isHit);
        }

        private void PrepareLaserRenderer()
        {
            if (inputType == InputType.Mouse) return;

            //当前项目资源加载
            //var lineScaleAsset = OasisAsset.Task("builtinres/laserpointer/LineScale").Get<GameObject>();
            //if (lineScaleAsset)
            //{
            //    m_Line = GoTools.NewChild(gameObject, lineScaleAsset).transform.gameObject;
            //    m_Line.SetLayerRecursively(LayerMask.NameToLayer("UI"));
            //    LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();
            //    m_LineMaterial = lineRenderer?.material;
            //    m_Line.SetActive(m_showLine);
            //}

            //var lineHitAsset = OasisAsset.Task("builtinres/laserpointer/LineHit").Get<GameObject>();
            //if (lineHitAsset)
            //{
            //    m_Hit = GoTools.NewChild(gameObject, lineHitAsset).transform.gameObject;
            //    m_Hit.SetLayerRecursively(LayerMask.NameToLayer("UI"));
            //    m_Hit.SetActive(m_showLine && isHit);
            //}
        }

        private void UpdateLaserRenderer()
        {
            if (inputType == InputType.Mouse) return;
            if (!m_showLine) return;
            if (m_Line == null || m_Hit == null) return;

            float distance = isHit ? Vector3.Distance(hitPoint, transform.position) : displayDistance;

            Vector3 lineScale = new Vector3(m_LineSize, m_LineSize, distance);
            m_Line.transform.localScale = lineScale;
            float lerp = 1f - Mathf.Clamp01((distance - 1f) / 5f);
            m_LineMaterial?.SetFloat("_Emission", Mathf.Lerp(2f, 5f, lerp));

            if(m_Hit.activeSelf != isHit) m_Hit.SetActive(isHit);
            lerp = Mathf.Clamp01((distance - 0.5f) / 4f);
            m_Hit.transform.localScale = Vector3.one * m_HitSize * Mathf.Lerp(0.2f, 1f, lerp);
            m_Hit.transform.localPosition = new Vector3(0, 0, distance);
        }

#endregion
    }
}
