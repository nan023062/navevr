using UnityEngine;

namespace Nave.VR
{
    [RequireComponent(typeof(Animator))]
    public class Controller : Hardware
    {
        public bool isLeft = true;

        [Header("射线"),SerializeField] LaserPointer m_Laser;

        private Animator m_Animator;

        public void Awake()
        {
            m_Laser.inputType = isLeft ? LaserPointer.InputType.LeftHand : LaserPointer.InputType.RightHand;
            m_Laser.SetVisiable(m_LaserShow);
            m_Animator = GetComponent<Animator>();
        }

        protected override void OnUpdate()
        {
            
        }

        #region Laser

        private bool m_LaserShow = false;
        public bool LaserShow { get { return m_LaserShow && m_Laser.isActiveAndEnabled; } }
        public void SetLaserVisiable(bool visiable) {
            if (LaserShow != visiable) {
                m_LaserShow = visiable;
                m_Laser?.SetVisiable(m_LaserShow && isActiveAndEnabled);
            }
        }

        #endregion
    }
}
