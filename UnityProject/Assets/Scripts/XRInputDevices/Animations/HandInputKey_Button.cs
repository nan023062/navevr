using UnityEngine;
using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    public class HandInputKey_Button : HandInputKey_Animation
    {
        [System.Serializable]
        public class AnimaFrame
        {
            public Vector3 position = Vector3.zero;
            public Quaternion rotation = Quaternion.identity;
            public Vector3 scale = Vector3.one;
        }

        [Header("Animation Frames"), SerializeField] private AnimaFrame end;
        private AnimaFrame start;

        [ContextMenu("采样动画结束帧")]
        private void SamplerEndFrame()
        {
            end = new AnimaFrame();
            end.position = transform.localPosition;
            end.rotation = transform.localRotation;
            end.scale = transform.localScale;
        }

        private void Start()
        {
            start = new AnimaFrame();
            start.position = transform.localPosition;
            start.rotation = transform.localRotation;
            start.scale = transform.localScale;
        }

        private float m_currentFloat = deadZoon;

        private void Update()
        {
            if(!Mathf.Approximately(m_currentFloat, m_Float1d))
            {
                m_currentFloat = Mathf.Lerp(m_currentFloat, m_Float1d, 5 * Time.deltaTime);
                if (Mathf.Abs(m_currentFloat - m_Float1d) <= deadZoon) m_currentFloat = m_Float1d;
                transform.localPosition = Vector3.Lerp(start.position, end.position, m_currentFloat);
                transform.localRotation = Quaternion.Slerp(start.rotation, end.rotation, m_currentFloat);
                transform.localScale = Vector3.Lerp(start.scale, end.scale, m_currentFloat);
            }
        }
    }
}
