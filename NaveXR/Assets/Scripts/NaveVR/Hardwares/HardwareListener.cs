using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System;

namespace Nave.VR
{
    public class HardwareListener : MonoBehaviour
    {
        private void Awake()
        {
            NaveVR.RegistHardware(this);
        }

        private void Update()
        {
            UpdatePoseAndController();
        }

        private void OnDestroy()
        {
            NaveVR.UnregistHardware(this);
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

        [SerializeField] NodeType m_nodeType = NodeType.Head;

        [NotModify, SerializeField] private ulong m_UniqueId;

        [NotModify, SerializeField] private string m_DeviceName;

        [SerializeField] private Hardware m_controller;

        public ulong UniqueId { get { return m_UniqueId; } }
        public NodeType NodeType => m_nodeType;
        public bool isTracked { get { return m_UniqueId > 0; } }
        public string device { get { return m_DeviceName; } }

        internal void Connected(Metadata metadata)
        {
            m_UniqueId = metadata.uniqueID;

            m_DeviceName = metadata.name;

            UpdatePoseAndController();

            NaveVR.Log($"{GetType().FullName} Connected : nodeType={NodeType},id={m_UniqueId},device={m_DeviceName}!");

            //显示虚拟设备
            Hardwares.Hide(m_controller);
            m_controller = Hardwares.Show(this);
        }

        internal void Disconnected()
        {
            //隐藏虚拟设备
            Hardwares.Hide(m_controller);

            NaveVR.Log($"{GetType().FullName} Disconnect : nodeType={NodeType},id={m_UniqueId}!");

            m_UniqueId = 0;

            m_DeviceName = string.Empty;
        }

        internal void UpdatePoseAndController()
        {
            var metadata = NaveVR.GetMetaDara(NodeType);
            transform.localRotation = metadata.rotation;
            transform.localPosition = metadata.position;
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

        public void SetInputOffset(Vector3 position, Quaternion rotation)
        {
            m_InputOffset.position = position;
            m_InputOffset.rotation = rotation;
        }

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

            if (NodeType == NodeType.LeftHand)
            {
                NaveVR.GetHandInputOffset(true, out positionOff, out rotationOff);
                SetInputOffset(positionOff, rotationOff);
            }
            else if (NodeType == NodeType.RightHand)
            {
                NaveVR.GetHandInputOffset(false, out positionOff, out rotationOff);
                SetInputOffset(positionOff, rotationOff);
            }
        }

        #endregion
    }
}
