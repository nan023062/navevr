using Nave.VR;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Nave.VR
{
    /// <summary>
    /// XR输入模块
    /// </summary>
    public class XRPointerInputModule : StandaloneInputModule
    {
        //XR拖拽值轉換到米
        private const float Centimeters2Meters = 0.01f;

        [Header("XR拖拽参考距离(单位m)"), Range(1f, 10f)]
        public float XRPixelDragReferenceDistance = 5.0f;

        protected override void Awake()
        {
            base.Awake();
            m_InputOverride = GetComponent<XRPointerInput>();
            m_LaserPointerData = new Dictionary<int, MouseButtonEventData>();
            m_FingerPointerData = new Dictionary<int, XRPointEventData>();
        }

        public override bool IsModuleSupported()
        {
            return InputDevices.isEnabled || base.IsModuleSupported();
        }

        private bool ShouldIgnoreEventsOnNoFocus()
        {
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows:
                case OperatingSystemFamily.Linux:
                case OperatingSystemFamily.MacOSX:
#if UNITY_EDITOR
                    if (UnityEditor.EditorApplication.isRemoteConnected)
                        return false;
#endif
                    return true;
                default:
                    return false;
            }
        }

        public override void Process()
        {
            if ((!InputDevices.isFocus && !eventSystem.isFocused) && ShouldIgnoreEventsOnNoFocus()) return;

            bool usedEvent = SendUpdateEventToSelectedObject();

            if (!ProcessFingerEvents() && !ProcessLaserEvents() && Cursor.visible)
                ProcessMouseEvent();

            if (eventSystem.sendNavigationEvents)
            {
                if (!usedEvent)
                    usedEvent |= SendMoveEventToSelectedObject();

                if (!usedEvent)
                    SendSubmitEventToSelectedObject();
            }
        }

        private GameObject m_CurrentFocusedObject;

        protected new GameObject GetCurrentFocusedGameObject()
        {
            return m_CurrentFocusedObject;
        }

        public GameObject CurrentFocusObject() { return m_CurrentFocusedObject; }

        private bool ShouldStartDrag(PointerEventData pointerEvent)
        {
            if (!pointerEvent.useDragThreshold) return true;
            float sqrThreshold = eventSystem.pixelDragThreshold * eventSystem.pixelDragThreshold;
            if (pointerEvent is XRPointEventData) {
                var XRPointerEvent = pointerEvent as XRPointEventData;
                //厘米转米 && 距离修正
                float correctionFactor = Centimeters2Meters * XRPointerEvent.hitDistance / XRPixelDragReferenceDistance;
                sqrThreshold *= correctionFactor * correctionFactor;
                return (XRPointerEvent.pressHitPosition - XRPointerEvent.hitPoint).sqrMagnitude >= sqrThreshold;
            }
            else {
                return (pointerEvent.pressPosition - pointerEvent.position).sqrMagnitude >= sqrThreshold;
            }
        }

        private void ProcessXRPointerMove(PointerEventData pointerEventData)
        {
            var targetGO = pointerEventData.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointerEventData, targetGO);
        }

        protected override void ProcessDrag(PointerEventData pointerEvent)
        {
            if (!pointerEvent.IsPointerMoving() || pointerEvent.pointerDrag == null)
                return;

            if (!pointerEvent.dragging && ShouldStartDrag(pointerEvent))
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
                pointerEvent.dragging = true;
            }

            if (pointerEvent.dragging)
            {
                if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                    pointerEvent.eligibleForClick = false;
                    pointerEvent.pointerPress = null;
                    pointerEvent.rawPointerPress = null;
                }
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
            }
        }

        private void RecordPressedPosition(XRPointEventData eventData)
        {
            eventData.pressHitPosition = eventData.hitPoint;
        }

        #region Finger

        private Dictionary<int, XRPointEventData> m_FingerPointerData = null;

        private bool ProcessFingerEvents()
        {
            bool result = false;

            var fingers = XREventSystem.GetFingers();
            int length = fingers.Count;
            for (int i = 0; !result && i < length; i++)
            {
                var finger = fingers[i];
                if (!finger.isActiveAndEnabled) continue;

                bool released;
                bool pressed;
                var pointer = GetFingerPointerEventData(finger, out pressed, out released);

                //处理按压逻辑。这里会记录按下的位置pressPosition
                ProcessTouchPress(pointer, pressed, released);
                //由于Fingger 的position为固定值，所以需要从新记录为worldPosition
                if (pressed) RecordPressedPosition(pointer);

                if (!released)
                {
                    ProcessXRPointerMove(pointer);
                    ProcessDrag(pointer);
                }
                else
                    RemovePointerData(pointer);

                result = finger.available || result;
            }
            return result;
        }

        private XRPointEventData GetFingerPointerEventData(FingerPointer finger, out bool pressed, out bool released)
        {
            XRPointEventData pointerData;
            var created = GetFingerPointerData(finger.fingerId, out pointerData, true);
            pointerData.Reset();

            pointerData.origin = finger.origin;
            pointerData.direction = finger.direction;
            pointerData.hitDistance = finger.raycastDistance;
            pointerData.button = PointerEventData.InputButton.Left;
            pointerData.raycastCamera = finger.raycastCamera;
            pointerData.position = new Vector2(finger.raycastCamera.pixelWidth * 0.5f, finger.raycastCamera.pixelHeight * 0.5f);

            pointerData.useFingerRaycast = true;
            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            pointerData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();

            //更新手指状态
            finger.UpdateStateFormRaycast(ref raycast);
            pressed = created || (finger.phase == TouchPhase.Began);
            released = (finger.phase == TouchPhase.Canceled) || (finger.phase == TouchPhase.Ended);
            pointerData.force = finger.force;

            if (finger.phase == TouchPhase.Canceled)
            {
                pointerData.pointerCurrentRaycast = new RaycastResult();
            }
            else
            {
                if (created) pointerData.hitPoint = raycast.worldPosition;
                if (pressed) pointerData.delta = Vector2.zero;
                else pointerData.delta = finger.delta;
                pointerData.hitNormal = finger.hitNormal;
                pointerData.hitPoint = finger.hitPoint;
            }

            return pointerData;
        }

        private bool GetFingerPointerData(int id, out XRPointEventData data, bool create)
        {
            if (!m_FingerPointerData.TryGetValue(id, out data) && create){
                data = new XRPointEventData(eventSystem){
                    pointerId = id,
                };
                m_FingerPointerData.Add(id, data);
                return true;
            }
            return false;
        }

        #endregion

        #region Laser

        private Dictionary<int, MouseButtonEventData> m_LaserPointerData = null;

        private bool ProcessLaserEvents()
        {
            bool result = false;

            var lasers = XREventSystem.GetLasers();
            int length = lasers.Count;
            for (int i = 0; i < length; i++)
            {
                var laser = lasers[i];
                if (!laser.isActiveAndEnabled) continue;

                var mouseButtonData = GetLaserPointerEventData(laser);

                //处理按压逻辑。这里会记录按下的位置pressPosition
                ProcessMousePress(mouseButtonData);
                //由于Laser的position为固定值，所以需要从新记录为worldPosition
                if (mouseButtonData.PressedThisFrame())
                    RecordPressedPosition(mouseButtonData.buttonData as XRPointEventData);

                ProcessXRPointerMove(mouseButtonData.buttonData);
                ProcessDrag(mouseButtonData.buttonData);

                m_CurrentFocusedObject = mouseButtonData.buttonData.pointerCurrentRaycast.gameObject;

                if (!Mathf.Approximately(mouseButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
                {
                    var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(mouseButtonData.buttonData.pointerCurrentRaycast.gameObject);
                    ExecuteEvents.ExecuteHierarchy(scrollHandler, mouseButtonData.buttonData, ExecuteEvents.scrollHandler);
                }

                result = true;
            }
            return result;
        }

        private MouseButtonEventData GetLaserPointerEventData(LaserPointer laser)
        {
            MouseButtonEventData mouseButtonData;
            var created = GetLaserPointerData(laser.fingerId, out mouseButtonData, true);

            XRPointEventData pointerData = mouseButtonData.buttonData as XRPointEventData;
            pointerData.Reset();

            pointerData.origin = laser.origin;
            pointerData.direction = laser.direction;
            pointerData.hitDistance = laser.raycastDistance;
            pointerData.button = PointerEventData.InputButton.Left;
            pointerData.raycastCamera = laser.raycastCamera;
            pointerData.position = new Vector2(laser.raycastCamera.pixelWidth * 0.5f, laser.raycastCamera.pixelHeight * 0.5f);

            pointerData.useFingerRaycast = false;
            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            pointerData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();

            //更新射线状态
            laser.UpdateStateFormRaycast(ref raycast);
    
            mouseButtonData.buttonState = XRPointEventData.StateForButton(laser.isPressed, laser.isReleased);
            if (laser.isHit)
            {
                if (created) pointerData.hitPoint = laser.hitPoint;
                pointerData.scrollDelta = laser.scrollDelta;
                pointerData.delta = laser.delta;
                pointerData.hitNormal = laser.hitNormal;
                pointerData.hitPoint = laser.hitPoint;
            }
            return mouseButtonData;
        }

        private bool GetLaserPointerData(int id, out MouseButtonEventData data, bool create)
        {
            if (!m_LaserPointerData.TryGetValue(id, out data) && create)
            {
                data = new MouseButtonEventData();
                data.buttonState = PointerEventData.FramePressState.NotChanged;
                data.buttonData = new XRPointEventData(eventSystem)
                {
                    pointerId = id,
                };
                m_LaserPointerData.Add(id, data);
                return true;
            }
            return false;
        }

        #endregion
    }
}
