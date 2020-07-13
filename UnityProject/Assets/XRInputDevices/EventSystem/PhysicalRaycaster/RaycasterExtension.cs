using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NaveXR.InputDevices
{
    public static class RaycasterExtension
    {
        static Vector3[] s_FourCorners = new Vector3[4];


        /// <summary>
        /// 如果WorldSpace的点point在rectTransfrom最近点在rect内部
        /// 就返回true，并返回最近点closePoint（WorldSpace）
        /// </summary>
        public static bool InRectangle(this RectTransform rectTransform, Vector3 point, out float sqrDistance)
        {
            sqrDistance = -1f;
            point = rectTransform.TransformPoint(point);
            rectTransform.GetLocalCorners(s_FourCorners);

            float minX = s_FourCorners[0].x;
            float maxX = s_FourCorners[2].x;
            float minY = s_FourCorners[0].y;
            float maxY = s_FourCorners[2].y;
            if ((point.x <= maxX && point.x >= minX ) &&
                (point.y <= maxY && point.y >= minY))
            {
                float scaleZ = rectTransform.lossyScale.z;
                sqrDistance = point.z * scaleZ * point.z * scaleZ;
                return true;
            }
            return false;
        }

    }
}
