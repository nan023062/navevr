using UnityEngine;
using System.Linq;

namespace Nave.VR
{
    public abstract class Hardware : MonoBehaviour
    {
        #region List of matching names

        [Header("List of matching names"), SerializeField]
        string[] m_matchingNames = new string[0];

        public bool TryMatchName(string deviceName)
        {
            if (m_matchingNames == null || m_matchingNames.Length <= 0) return false;
            foreach (var s in m_matchingNames)
                if (deviceName.Contains(s)) return true;
            return false;
        }

        #endregion


        #region Fields And Properties

        [Header("Device Details")]
        [NotModify, SerializeField] NodeType m_nodeType = NodeType.Head;

        [NotModify, SerializeField] private ulong m_UniqueId;

        [NotModify, SerializeField] private string m_DeviceName;

        public ulong UniqueId { get { return m_UniqueId; } }
        public NodeType NodeType => m_nodeType;
        public bool isTracked { get { return isActiveAndEnabled; } }
        public string device { get { return m_DeviceName; } }

        #endregion

        public virtual void SetNodeType(NodeType nodeType)
        {
            m_nodeType = nodeType;
        }

        public void Update()
        {
            OnUpdate();
        }

        internal void Connected(TrackingAnchor metadata)
        {
            m_UniqueId = metadata.uniqueID;

            m_DeviceName = metadata.name;

            InputDevices.Log($"{name} Connected : nodeType={NodeType},id={m_UniqueId},device={m_DeviceName}!");
            
            OnDeviceConnected();
        }

        internal void Disconnected()
        {
            OnDeviceDisconnected();

            InputDevices.Log($"{GetType().FullName} Disconnect : nodeType={NodeType},id={m_UniqueId}!");

            m_UniqueId = 0;

            m_DeviceName = string.Empty;
        }

        protected abstract void OnUpdate();

        protected abstract void OnDeviceConnected();

        protected abstract void OnDeviceDisconnected();

        #region Target Transform 

        [System.Serializable]
        public class TransformData
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        TransformData m_InputOffset = new TransformData();

        public Vector3 position
        {
            get
            {
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
                InputDevices.GetHandInputOffset(true, out positionOff, out rotationOff);
                SetInputOffset(positionOff, rotationOff);
            }
            else if (NodeType == NodeType.RightHand)
            {
                InputDevices.GetHandInputOffset(false, out positionOff, out rotationOff);
                SetInputOffset(positionOff, rotationOff);
            }
        }

        #endregion
    }
}
