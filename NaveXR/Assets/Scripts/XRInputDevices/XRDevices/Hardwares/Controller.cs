using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// 虚拟手柄的模型
    /// 1 按键交互表现
    /// 2 按键交互提示
    /// </summary>
    public class Controller : MonoBehaviour
    {
        public int hand = 0;//0左1右

        public HandInputKey_Animation[] handInputKey_Animations;

        private Transform m_model;

        private bool m_Visiable = false;

        public bool Visiable { get { return m_Visiable && m_model != null; } }

        public void SetVisable(bool visiable)
        {
            m_Visiable = visiable;
            if(m_model != null) m_model.gameObject.SetActive(m_Visiable);
        }

        private void Start()
        {
            m_model = transform.Find("model");

            SetVisable(m_Visiable);
        }

        private void Update()
        {
            if (!m_Visiable) return;

            CheckAndUpdateKeyAnimations();
        }

        #region Keys Animations

        //按键动画表现
        private void CheckAndUpdateKeyAnimations()
        {
            if (handInputKey_Animations == null) return;

            int length = handInputKey_Animations.Length;

            for (int i = 0; i < length; i++)
            {
                var animation = handInputKey_Animations[i];

                if (animation == null) continue;

                XRKeyCode xRKeyCode = animation.keyCode;

                if (xRKeyCode == XRKeyCode.TouchAxis)
                    animation.SetValue(XRDevice.GetTouchAxis(hand));
                else
                    animation.SetValue(XRDevice.GetKeyForce(hand, xRKeyCode));
            }
        }

        public HandInputKey_Animation GetKey(XRKeyCode xRKeyCode)
        {
            if (handInputKey_Animations == null) return null;

            int length = handInputKey_Animations.Length;
            for (int i = 0; i < length; i++)
            {
                var animation = handInputKey_Animations[i];
                if (animation == null) continue;
                if (xRKeyCode == animation.keyCode) return animation;
            }
            return null;
        }

        #endregion
    }
}
