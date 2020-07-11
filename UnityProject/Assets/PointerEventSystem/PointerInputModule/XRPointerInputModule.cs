using NaveXR.Device;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NaveXR.EventSystem
{
    public class XRPointerInputModule : StandaloneInputModule
    {
        Dictionary<int, MouseButtonEventData> m_XRPointerData = null;

        protected override void Awake()
        {
            base.Awake();
            m_InputOverride = GetComponent<XRPointerInput>();
            m_XRPointerData = new Dictionary<int, MouseButtonEventData>();
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
        private bool GetXRPointerData(int id, out MouseButtonEventData data, bool create)
        {
            if (!m_XRPointerData.TryGetValue(id, out data) && create)
            {
                data = new MouseButtonEventData();
                data.buttonState = PointerEventData.FramePressState.NotChanged;
                data.buttonData = new XRPointEventData(eventSystem){
                    pointerId = id,
                };
                m_XRPointerData.Add(id, data);
                return true;
            }
            return false;
        }


        #region Finger

        private bool ProcessFingerEvents()
        {
            bool result = false;

            var fingers = XRDevice.GetFingers();
            int length = fingers.Count;
            for (int i = 0; i < length; i++)
            {
                var finger = fingers[i];
                if (!finger.isActiveAndEnabled) continue;
                var mouseButtonData = GetFingerPointerEventData(finger);
                //var m_CurrentFocusedGameObject = mouseButtonData.buttonData.pointerCurrentRaycast.gameObject;

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

        private MouseButtonEventData GetFingerPointerEventData(FingerPointer xrPointer)
        {
            MouseButtonEventData mouseButtonData;
            var created = GetXRPointerData(xrPointer.fingerId, out mouseButtonData, true);

            XRPointEventData pointerData = mouseButtonData.buttonData as XRPointEventData;
            pointerData.Reset();

            pointerData.origin = xrPointer.origin;
            pointerData.direction = xrPointer.direction;
            pointerData.hitDistance = xrPointer.raycastDistance;
            pointerData.button = PointerEventData.InputButton.Left;
            pointerData.raycastCamera = xrPointer.raycastCamera;
            pointerData.position = new Vector2(xrPointer.raycastCamera.pixelWidth * 0.5f, xrPointer.raycastCamera.pixelHeight * 0.5f);

            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            pointerData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();

            xrPointer.UpdateStateFormRaycast(ref raycast);
            mouseButtonData.buttonState = XRPointEventData.StateForButton(xrPointer.isPressed, xrPointer.isReleased);
            if (!xrPointer.available) pointerData.position = -Vector2.one;

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



        #endregion

        #region Laser

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

                //var m_CurrentFocusedGameObject = mouseButtonData.buttonData.pointerCurrentRaycast.gameObject;

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

        private MouseButtonEventData GetLaserPointerEventData(BasePointer xrPointer)
        {
            MouseButtonEventData mouseButtonData;
            var created = GetXRPointerData(xrPointer.fingerId, out mouseButtonData, true);

            XRPointEventData pointerData = mouseButtonData.buttonData as XRPointEventData;
            pointerData.Reset();

            pointerData.origin = xrPointer.origin;
            pointerData.direction = xrPointer.direction;
            pointerData.hitDistance = xrPointer.raycastDistance;
            pointerData.button = PointerEventData.InputButton.Left;
            pointerData.raycastCamera = xrPointer.raycastCamera;
            pointerData.position = new Vector2(xrPointer.raycastCamera.pixelWidth * 0.5f, xrPointer.raycastCamera.pixelHeight * 0.5f);
            mouseButtonData.buttonState = XRPointEventData.StateForButton(xrPointer.isPressed, xrPointer.isReleased);

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



        #endregion
    }
}
