using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System;

namespace NaveXR.InputDevices
{
    public class DeviceCapture : MonoBehaviour
    {
        [Header("匹配的设备")]
        public Controller controller;
        public bool updateTransform;

        private void Awake()
        {
            XRDevice.RegistDevice(this);
        }

        private void OnDestroy()
        {
            XRDevice.UnregistDevice(this);
        }

        public void SetControlVisiable(bool visiable)
        {
            controller?.SetVisiable(visiable);
        }

        #region Draw Gizmos

        [Header("Draw Gizmos")]
        [SerializeField] Color color = Color.red;
        private Mesh mesh = null;

        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.vertices = new Vector3[] {
                    new Vector3(-1,-1,-1),
                    new Vector3(1,-1,-1),
                    new Vector3(-1,1,-1),
                    new Vector3(1,1,-1),
                    new Vector3(-1,-1,1),
                    new Vector3(1,-1,1),
                    new Vector3(-1,1,1),
                    new Vector3(1,1,1),
                };
                mesh.triangles = new int[]
                {
                    0,2,3, 0,3,1,
                    1,3,7, 1,7,5,
                    4,6,2, 4,2,0,
                    4,0,1, 4,1,5,
                    2,6,7, 2,7,3,
                    6,4,5, 6,5,7
                };
                mesh.RecalculateNormals();
            }

            Color oldColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation, new Vector3(0.02f, 0.02f, 0.1f));
            Gizmos.color = oldColor;
        }

        #endregion
        
        #region InputDevice & XRNodeState

        [Header("InputDevice & XRNodeState")]
        [NotModify, SerializeField] private ulong uniqueId;
        [SerializeField] private XRNode nodeType = XRNode.Head;
        public ulong UniqueId { get { return uniqueId; } }
        public XRNode NodeType { get { return nodeType; } }
        public InputDevice inputDevice { get; private set; }
        public XRNodeState xRNodeState { get; private set; }
        public bool isTracked { get { return uniqueId > 0; } }

        internal void Connected(ref XRNodeState nodeState,ref InputDevice inputDevice)
        {
            uniqueId = nodeState.uniqueID;
            this.xRNodeState = nodeState;
            this.inputDevice = inputDevice;
            UpdateInputDeviceAndXRNode(ref nodeState, ref inputDevice);
            Debug.LogFormat("Connected Controller: nodeType={0},uniqueId={1},device={2}!",
                nodeType, uniqueId, inputDevice.name);

            if (XRDevice.Driver == XRDVName.Oculus) InitOculusVRHandOffset();
            else InitOpenVRHandOffset();

            controller?.OnConnected(nodeState, inputDevice);
        }

        internal void Disconnected()
        {
            Debug.LogFormat("Disconnect Controller: nodeType={0},uniqueId={1},device={2}!", 
                nodeType, uniqueId, inputDevice.name);
            uniqueId = 0;
            controller?.OnDisconnected();
        }

        //使用ref關鍵字 防止數據拷貝
        internal void UpdateInputDeviceAndXRNode(ref XRNodeState nodeState, ref InputDevice inputDevice)
        {
            Quaternion rotation = Quaternion.identity;
            nodeState.TryGetRotation(out rotation);
            transform.localRotation = rotation;

            Vector3 position = Vector3.zero;
            nodeState.TryGetPosition(out position);
            transform.localPosition = position;

            if(updateTransform && controller != null)
            {
                controller.transform.position = transform.position;
                controller.transform.rotation = transform.rotation;
            }
        }

        #endregion

        #region Target Transform 

        [Serializable]
        public class TransformData
        {
            [NotModify] public Vector3 position;
            [NotModify] public Quaternion rotation;
        }

        [Header("Input Offset")]
        [SerializeField] TransformData m_InputOffset = new TransformData();

        public Vector3 position
        {
            get {
                Vector3 _position = transform.position;
                Quaternion _rotation = transform.rotation;
                if (m_InputOffset.position != Vector3.zero)
                {
                    _position = _position + _rotation * m_InputOffset.position;
                }
                return _position;
            }
        }

        public Quaternion rotation
        {
            get
            {
                Quaternion _rotation = transform.rotation;
                if (m_InputOffset.rotation != Quaternion.identity)
                {
                    var forward = _rotation * (m_InputOffset.rotation * Vector3.forward);
                    var upwards = _rotation * (m_InputOffset.rotation * Vector3.up);
                    return Quaternion.LookRotation(forward, upwards);
                }
                return _rotation;
            }
        }

        /// <summary>
        /// 设置Input数据的校准参数
        /// </summary>
        /// <param name="position">位置偏移</param>
        /// <param name="rotation">旋转偏移</param>
        public void SetInputOffset(Vector3 position, Quaternion rotation)
        {
            m_InputOffset.position = position;
            m_InputOffset.rotation = rotation;
        }

        /// <summary>
        /// 设置Input数据的校准参数
        /// </summary>
        /// <param name="anchor">持物点参照物</param>
        /// <param name="target">设备点参照物</param>
        public void SetInputOffset(Transform anchor, Transform target)
        {
            m_InputOffset.position = anchor.InverseTransformDirection(target.position - anchor.position);
            Quaternion inverse = Quaternion.Inverse(anchor.rotation);
            Vector3 forward = inverse * target.forward;
            Vector3 upwards = inverse * target.up;
            m_InputOffset.rotation = Quaternion.LookRotation(forward, upwards);
        }

        private void InitOpenVRHandOffset()
        {
            if(nodeType == XRNode.LeftHand)
            {
                SetInputOffset(new Vector3(-0.003f, -0.006f, -0.1f), Quaternion.identity);
            }
            else if (nodeType == XRNode.RightHand)
            {
                SetInputOffset(new Vector3(0.003f, -0.006f, -0.1f), Quaternion.identity);
            }
        }

        private void InitOculusVRHandOffset()
        {
            throw new Exception("还未实现Oculus手柄Offset参数！！！"); 
        }

        #endregion

    }
}
