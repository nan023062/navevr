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

        protected sealed override void OnUpdate()
        {
            UpdateAnimations();
        }

        protected override void OnDeviceConnected()
        {

        }

        protected override void OnDeviceDisconnected()
        {

        }

        protected virtual void UpdateAnimations()
        {
            if (m_Animator) {
                m_Animator.SetFloat("Button 1", InputDevices.GetKeyForce(isLeft ? 0 : 1, InputKey.Primary));
                m_Animator.SetFloat("Button 2", InputDevices.GetKeyForce(isLeft ? 0 : 1, InputKey.Secondary));
                m_Animator.SetFloat("Joy X", InputDevices.GetTouchAxis(isLeft ? 0 : 1).x);
                m_Animator.SetFloat("Joy Y", InputDevices.GetTouchAxis(isLeft ? 0 : 1).y);
                m_Animator.SetFloat("Grip", InputDevices.GetKeyForce(isLeft ? 0 : 1, InputKey.Grip));
                m_Animator.SetFloat("Trigger", InputDevices.GetKeyForce(isLeft ? 0 : 1, InputKey.Trigger));
            }
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
