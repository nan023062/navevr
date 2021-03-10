using UnityEngine;

namespace Nave.VR
{
    public abstract class Controller : Hardware
    {
        [Header("Controller Defines")] 
        [SerializeField] GameObject m_LeftCtrl;

        [SerializeField] GameObject m_RightCtrl;

        protected Animator m_Animator;
        public bool isLeft => NodeType == NodeType.LeftHand;

        public GameObject Ctrl => isLeft ? m_LeftCtrl : m_RightCtrl;

        public virtual void Awake()
        {

        }

        #region Laser

        private LaserPointer m_Laser;

        private bool m_LaserShow = true;
        public bool LaserShow { get { return m_LaserShow && m_Laser.isActiveAndEnabled; } }
        public void SetLaserVisiable(bool visiable) {
            if (LaserShow != visiable) {
                m_LaserShow = visiable;
                m_Laser?.SetVisiable(m_LaserShow && isActiveAndEnabled);
            }
        }

        public override void SetNodeType(NodeType nodeType)
        {
            base.SetNodeType(nodeType);

            m_LeftCtrl.SetActive(isLeft);

            m_RightCtrl.SetActive(!isLeft);

            m_Animator = Ctrl.GetComponent<Animator>();

            m_Laser = Ctrl.GetComponentInChildren<LaserPointer>();

            m_Laser.inputType = isLeft ? LaserPointer.InputType.LeftHand : LaserPointer.InputType.RightHand;

            m_Laser.SetVisiable(m_LaserShow);

        }

        #endregion
    }
}
