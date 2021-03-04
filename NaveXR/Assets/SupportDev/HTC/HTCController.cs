using UnityEngine;
using System;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;

namespace Nave.XR
{
    /// <summary>
    /// 虚拟控制器对象
    /// </summary>
    public class HTCController : MonoBehaviour
    {
        public bool isLeft = true;

        [Header("射线源点")]
        public Transform origin;

        private LaserPointer m_Laser;

        public LaserPointer laser { get { return m_Laser; } }

        private bool m_laserVisiable = true;

        public bool laserVisiable { get { return m_laserVisiable; } }

        public void SetLaserVisiable(bool visiable)
        {
            if (laserVisiable != visiable)
            {
                m_laserVisiable = visiable;
                m_Laser?.SetVisiable(m_laserVisiable && isActiveAndEnabled);
            }
        }

        private void OnInitLaserPointer(GameObject go)
        {
            var laser = go.transform.Find("laserpoint").gameObject;
            m_Laser = laser.AddComponent<LaserPointer>();
            m_Laser.inputType = isLeft ? LaserPointer.InputType.LeftHand : LaserPointer.InputType.RightHand;
            m_Laser.SetVisiable(m_laserVisiable);
        }
    }
}
