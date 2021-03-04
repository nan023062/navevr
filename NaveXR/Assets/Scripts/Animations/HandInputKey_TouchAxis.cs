using UnityEngine;

namespace Nave.VR
{
    public class HandInputKey_TouchAxis : HandInputKey_Animation
    {
        public Vector2 m_currentAxis = new Vector2(deadZoon,deadZoon); 

        public void Update()
        {
            if((m_Float2d != m_currentAxis))
            {
                m_currentAxis = m_Float2d;
                var localForward = new Vector3(m_Float2d.x, m_Float2d.y, 1f);
                transform.localRotation = Quaternion.LookRotation(localForward, Vector3.up);
            }
        }
    }
}

