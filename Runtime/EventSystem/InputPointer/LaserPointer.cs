using UnityEngine;
using UnityEngine.EventSystems;
using Nave.VR;

namespace Nave.VR
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

        [SerializeField, Header("射线")] private GameObject m_Line;

        [SerializeField, Header("Hit")] private GameObject m_Hit;

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

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            XREventSystem.Regist(this);
        }

        protected override void OnDisable()
        {
            XREventSystem.Remove(this);
            base.OnDisable();
        }

        protected override void UpdateHitState(ref RaycastResult raycast)
        {
            isHit = raycast.gameObject != null;
            if (isHit) {
                Vector3 lastHitPoint = hitPoint;

                //左手按鍵狀態
                if (inputType == InputType.LeftHand) {
                    Vector2 aixs = InputDevices.GetTouchAxis(0);
                    if (aixs.sqrMagnitude > scrollThrelod)
                        scrollDelta = aixs * handScrollSensitivity;
                }
                //右手按鍵狀態
                else if (inputType == InputType.RightHand) {
                    Vector2 aixs = InputDevices.GetTouchAxis(1);
                    if (aixs.sqrMagnitude > scrollThrelod)
                        scrollDelta = aixs * handScrollSensitivity;
                }
                //Unity原生模式狀態
                else if (inputType == InputType.Mouse) {
#if UNITY_STANDALONE || UNITY_EDITOR
                    scrollDelta = Input.mouseScrollDelta;

#elif UNITY_ANDROID || UNITY_IOS
                    if (Input.touchCount > 0) {
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

            UpdateRenderer();
        }

        protected override void UpdatePressedAndReleased(ref RaycastResult raycast)
        {
            //左手按鍵狀態
            if (inputType == InputType.LeftHand)
            {
                isPressed = InputDevices.IsLeftKeyDown(InputKey.Trigger);
                isReleased = InputDevices.IsLeftKeyUp(InputKey.Trigger);
            }
            //右手按鍵狀態
            else if (inputType == InputType.RightHand)
            {
                isPressed = InputDevices.IsRightKeyDown(InputKey.Trigger);
                isReleased = InputDevices.IsRightKeyUp(InputKey.Trigger);
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

        public void SetVisiable(bool visiable) {

            gameObject.SetActive(visiable);
        }

        private void UpdateRenderer()
        {
            if (!isActiveAndEnabled) return;
            if (inputType == InputType.Mouse) return;

            float distance = isHit ? Vector3.Distance(hitPoint, transform.position) : displayDistance;

            Vector3 lineScale = m_Line.transform.localScale;
            lineScale.z = distance;
            m_Line.transform.localScale = lineScale;

            if (isHit != m_Hit.activeSelf) m_Hit.SetActive(isHit);
            m_Hit.transform.localPosition = new Vector3(0, 0, distance);
        }
    }
}
