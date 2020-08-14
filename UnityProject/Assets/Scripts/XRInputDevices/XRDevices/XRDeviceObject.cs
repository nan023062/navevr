using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System;

namespace NaveXR.InputDevices
{
    public class XRDeviceObject : MonoBehaviour
    {
        [Header("匹配的设备")]
        public Hardware hardware;

        public bool auto_pose;

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
            hardware?.SetVisiable(visiable);
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
        [NotModify, SerializeField] private ulong m_UniqueId;
        [NotModify, SerializeField] private string m_DeviceName;
        [SerializeField] private NodeType nodeType = NodeType.Head;
        public ulong UniqueId { get { return m_UniqueId; } }
        public NodeType NodeType { get { return nodeType; } }
        public bool isTracked { get { return m_UniqueId > 0; } }
        public string deviceName { get { return m_DeviceName; } }

        internal void Connected(InputNode xRNodeUsage)
        {
            m_UniqueId = xRNodeUsage.uniqueID;
            m_DeviceName = xRNodeUsage.name;
            UpdateInputDeviceAndXRNode(xRNodeUsage);
            Debug.LogFormat("Connected Controller: nodeType={0},uniqueId={1},device={2}!",
                nodeType, m_UniqueId, xRNodeUsage.name);

            InitHandOffset();
            hardware?.OnConnected(xRNodeUsage);
        }

        internal void Disconnected()
        {
            Debug.LogFormat("Disconnect Controller: nodeType={0},uniqueId={1}!", nodeType, m_UniqueId);
            m_UniqueId = 0;
            m_DeviceName = string.Empty;
            hardware?.OnDisconnected();
        }

        //使用ref關鍵字 防止數據拷貝
        internal void UpdateInputDeviceAndXRNode(InputNode xRNodeUsage)
        {
            transform.localRotation = xRNodeUsage.rotation;
            transform.localPosition = xRNodeUsage.position;

            if(auto_pose && hardware != null)
            {
                hardware.transform.position = position;
                hardware.transform.rotation = rotation;
            }
        }

        #endregion

        #region Target Transform 

        [Serializable]
        public class TransformData
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        [Header("Input Offset")]
        [SerializeField, NotModify] TransformData m_InputOffset = new TransformData();

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

        public void GetInputOffset(out Vector3 position, out Quaternion rotation)
        {
            position = m_InputOffset.position;
            rotation = m_InputOffset.rotation;
        }

        private void InitHandOffset()
        {
            Vector3 positionOff = Vector3.zero;
            Quaternion rotationOff = Quaternion.identity;

            if (nodeType == NodeType.LeftHand)
            {
                XRDevice.GetHandInputOffset(true, out positionOff, out rotationOff);
                SetInputOffset(positionOff, rotationOff);
            }
            else if (nodeType == NodeType.RightHand)
            {
                XRDevice.GetHandInputOffset(false, out positionOff, out rotationOff);
                SetInputOffset(positionOff, rotationOff);
            }
        }

        #endregion

    }
}
