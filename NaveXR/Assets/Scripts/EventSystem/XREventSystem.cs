
using UnityEngine;
using System.Collections.Generic;

namespace Nave.VR
{
    /// <summary>
    /// 绑定专用XR模式的Module和Input
    /// </summary>
    [RequireComponent(typeof(XRPointerInput))]
    [RequireComponent(typeof(XRPointerInputModule))]
    public class XREventSystem : UnityEngine.EventSystems.EventSystem
    {
        #region Input Points

        private static List<LaserPointer> m_laserPointers;

        private static List<FingerPointer> m_fingerPointers;

        internal static void Regist(LaserPointer laser)
        {
            GetLasers().Add(laser);
        }

        internal static void Remove(LaserPointer laser)
        {
            GetLasers()?.Remove(laser);
        }

        internal static void Regist(FingerPointer finger)
        {
            GetFingers().Add(finger);
        }

        internal static void Remove(FingerPointer finger)
        {
            GetFingers()?.Remove(finger);
        }

        internal static List<FingerPointer> GetFingers()
        {
            if (m_fingerPointers == null) m_fingerPointers = new List<FingerPointer>();
            return m_fingerPointers;
        }

        internal static List<LaserPointer> GetLasers()
        {
            if (m_laserPointers == null) m_laserPointers = new List<LaserPointer>();
            return m_laserPointers;
        }

        #endregion
    }
}
