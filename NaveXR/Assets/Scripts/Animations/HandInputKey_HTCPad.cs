using UnityEngine;

namespace Nave.XR
{
    public class HandInputKey_HTCPad : HandInputKey_Animation
    {
        public Transform point;
        public Vector3 m_currentPos = new Vector3(deadZoon, 0f, 0f);
        private float sqrDeadZoon = deadZoon * deadZoon;

        public void Update()
        {
            Vector3 pos = new Vector3(m_Float2d.x, 0f, m_Float2d.y);
            if (m_Float2d.sqrMagnitude > 0) pos.y = 0.1f;
            if (Vector3.SqrMagnitude(pos- m_currentPos) >= sqrDeadZoon)
            {
                m_currentPos = pos;
                point.localPosition = m_currentPos;
            }
        }
    }
}
