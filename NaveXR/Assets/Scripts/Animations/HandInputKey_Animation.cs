using UnityEngine;

namespace Nave.XR
{
    public class HandInputKey_Animation : MonoBehaviour
    {
        public const float deadZoon = 0.001f;

        public KeyCode keyCode;
        [SerializeField] protected bool m_Bool;
        [SerializeField] protected float m_Float1d;
        [SerializeField] protected Vector2 m_Float2d;

        public void SetValue(bool value)
        {
            m_Bool = value;
        }

        public void SetValue(float value)
        {
            m_Float1d = value;
        }

        public void SetValue(Vector2 value)
        {
            m_Float2d = value;
        }


        public Material highLightMat;

        private Material defaultMat;

        public void SetHighLight(bool highlight) {
            var renderer = GetComponent<Renderer>();
            if (defaultMat == null) defaultMat = renderer.sharedMaterial;
            var mat = highlight ? highLightMat : defaultMat;
            renderer.sharedMaterial = mat;
        }
    }
}
