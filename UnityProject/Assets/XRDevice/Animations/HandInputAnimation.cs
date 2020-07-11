using UnityEngine;
using UnityEngine.XR;

namespace NaveXR.Device
{
    public class HandInputAnimation : MonoBehaviour
    {
        public bool leftHand = true;
        public XRKeyCode keyCode;
        private HandInputBase Input;

        [System.Serializable]
        public class AnimaFrame
        {
            public Vector3 position = Vector3.zero;
            public Quaternion rotation = Quaternion.identity;
            public Vector3 scale = Vector3.one;
        }

        [Header("Animation Frames"), SerializeField] private AnimaFrame end;
        private AnimaFrame start;

        private void Start()
        {
            Input = XRDevice.GetHandInputButton<HandInputBase>(leftHand ? 0 : 1, keyCode);
            start = new AnimaFrame();
            start.position = transform.localPosition;
            start.rotation = transform.localRotation;
            start.scale = transform.localScale;
        }

        private void Update()
        {
            //if(Input != null && start != null && end != null) {
            //    float value = Input.Value;
            //    transform.localPosition = Vector3.Lerp(start.position, end.position, value);
            //    transform.localRotation = Quaternion.Lerp(start.rotation, end.rotation, value);
            //    transform.localScale = Vector3.Lerp(start.scale, end.scale, value);
            //}
        }

        [ContextMenu("采样动画结束帧")]
        private void SamplerEndFrame()
        {
            end = new AnimaFrame();
            end.position = transform.localPosition;
            end.rotation = transform.localRotation;
            end.scale = transform.localScale;
        }
    }
}
