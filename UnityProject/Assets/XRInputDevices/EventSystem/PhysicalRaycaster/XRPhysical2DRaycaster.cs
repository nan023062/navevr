using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// XR 2D物理射线检测
    /// </summary>
    public class XRPhysical2DRaycaster : Physics2DRaycaster
    {
        private Camera xrPointCamera;

        public override Camera eventCamera
        {
            get
            {
                if (xrPointCamera != null) return xrPointCamera;
                return base.eventCamera;
            }
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            //XR输入事件
            var xrPointData = eventData as XRPointEventData;
            if (xrPointData != null)
            {
                xrPointCamera = xrPointData.raycastCamera;
                xrPointCamera.farClipPlane = xrPointData.hitDistance;
            }
            //原生输入事件
            else
            {
                xrPointCamera = null;
            }
            base.Raycast(eventData, resultAppendList);
        }

    }
}

