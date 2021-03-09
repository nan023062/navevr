using UnityEngine;

namespace Nave.VR
{
    [RequireComponent(typeof(Animator))]
    public abstract class Controller : Hardware
    {
        [Header("左控制器"), SerializeField] GameObject m_LeftCtrl;

        [Header("右控制器"), SerializeField] GameObject m_RightCtrl;

        private Animator m_Animator;
        public bool isLeft => NodeType == NodeType.LeftHand;

        public GameObject Ctrl => isLeft ? m_LeftCtrl : m_RightCtrl;

        public virtual void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        #region Laser

        private LaserPointer m_Laser;

        private bool m_LaserShow = false;
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

            m_Laser = Ctrl.GetComponentInChildren<LaserPointer>();

            m_Laser.inputType = isLeft ? LaserPointer.InputType.LeftHand : LaserPointer.InputType.RightHand;

            m_Laser.SetVisiable(m_LaserShow);

        }

        #endregion
    }
}
