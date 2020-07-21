using NaveXR.InputDevices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// XR输入模块
    /// </summary>
    public class XRPointerInputModule : StandaloneInputModule
    {
        protected override void Awake()
        {
            base.Awake();
            m_InputOverride = GetComponent<XRPointerInput>();
            m_LaserPointerData = new Dictionary<int, MouseButtonEventData>();
            m_FingerPointerData = new Dictionary<int, XRPointEventData>();
        }

        public override bool IsModuleSupported()
        {
            return XRDevice.isEnabled || base.IsModuleSupported();
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
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus()) return;

            bool usedEvent = SendUpdateEventToSelectedObject();

            if (!ProcessFingerEvents() && !ProcessLaserEvents() && input.mousePresent)
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

        #region Finger

        private Dictionary<int, XRPointEventData> m_FingerPointerData = null;

        private bool ProcessFingerEvents()
        {
            bool result = false;

            var fingers = XRDevice.GetFingers();
            int length = fingers.Count;
            for (int i = 0; !result && i < length; i++)
            {
                var finger = fingers[i];
                if (!finger.isActiveAndEnabled) continue;

                bool released;
                bool pressed;
                var pointer = GetFingerPointerEventData(finger, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if (!released)
                {
                    ProcessMove(pointer);
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

                if (pressed)
                    pointerData.delta = Vector2.zero;
                else
                {
                    //在触发的面位移
                    Vector3 lastPos = pointerData.raycastCamera.WorldToScreenPoint(pointerData.hitPoint);
                    Vector3 currPos = pointerData.raycastCamera.WorldToScreenPoint(raycast.worldPosition);
                    pointerData.delta = currPos - lastPos;
                }
                pointerData.hitNormal = raycast.worldNormal;
                pointerData.hitPoint = raycast.worldPosition;
                //Debug.DrawRay(finger.origin, raycast.worldNormal);
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

            var lasers = XRDevice.GetLasers();
            int length = lasers.Count;
            for (int i = 0; i < length; i++)
            {
                var laser = lasers[i];
                if (!laser.isActiveAndEnabled) continue;

                var mouseButtonData = GetLaserPointerEventData(laser);

                m_CurrentFocusedObject = mouseButtonData.buttonData.pointerCurrentRaycast.gameObject;

                ProcessMousePress(mouseButtonData);
                ProcessMove(mouseButtonData.buttonData);
                ProcessDrag(mouseButtonData.buttonData);

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
            mouseButtonData.buttonState = XRPointEventData.StateForButton(laser.isPressed, laser.isReleased);

            pointerData.useFingerRaycast = false;
            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            pointerData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();

            if (raycast.gameObject != null)
            {
                if (created) pointerData.hitPoint = raycast.worldPosition;

                //在触发的面位移
                Vector3 lastPos = pointerData.raycastCamera.WorldToScreenPoint(pointerData.hitPoint);
                Vector3 currPos = pointerData.raycastCamera.WorldToScreenPoint(raycast.worldPosition);
                pointerData.delta = currPos - lastPos;

                pointerData.hitNormal = raycast.worldNormal;
                pointerData.hitPoint = raycast.worldPosition;
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
