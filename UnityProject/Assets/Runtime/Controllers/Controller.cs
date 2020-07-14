using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System;

namespace NaveXR.InputDevices
{
    public abstract class Controller : MonoBehaviour
    {
        private void Awake()
        {
            OnAfterCreate();
            XRDevice.RegistDeviceCapture(this);
        }

        private void Update()
        {
            OnUpdate();
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


        private void OnDestroy()
        {
            OnBeforeDestroy();
            XRDevice.UnregistDeviceCapture(this);
        }

        #region InputDevice & XRNodeState

        [Header("InputDevice & XRNodeState")]
        [NotModify, SerializeField] private ulong uniqueId;
        [NotModify, SerializeField] protected XRNode nodeType = XRNode.Head;
        public ulong UniqueId { get { return uniqueId; } }
        public XRNode NodeType { get { return nodeType; } }
        public InputDevice inputDevice { get; private set; }
        public XRNodeState xRNodeState { get; private set; }
        public bool isTracked { get { return uniqueId > 0; } }

        public void Connected(XRNodeState nodeState, InputDevice inputDevice)
        {
            uniqueId = nodeState.uniqueID;
            UpdateInputDeviceAndXRNode(ref nodeState, ref inputDevice);
            Debug.LogFormat("Connected Controller: nodeType={0},uniqueId={1},device-{2}!",
                nodeType, uniqueId, inputDevice.name);
            OnConnected(nodeState, inputDevice);
        }

        public void Disconnected()
        {
            Debug.LogFormat("Disconnect Controller: nodeType={0},uniqueId={1},device-{2}!", 
                nodeType, uniqueId, inputDevice.name);
            uniqueId = 0;
            OnDisconnected();
        }

        //使用ref關鍵字 防止數據拷貝
        public void UpdateInputDeviceAndXRNode(ref XRNodeState nodeState, ref InputDevice inputDevice)
        {
            //更新位置
            SyncUpdateTransform(ref nodeState);

            //更新输入数据
            OnUpdateInputDevice(ref inputDevice);
        }

        #endregion

        #region Target Transform 

        [Header("Target Transform ")]
        //Target坐标偏移值
        [SerializeField, NotModify] private Vector3 targetOffset = Vector3.zero;

        //Target点的本地旋轉
        [SerializeField, NotModify] private Quaternion targetSpace = Quaternion.identity;

        public void SetTargetOffset(Vector3 position, Quaternion rotation)
        {
            targetOffset = position;
            targetSpace = rotation;
        }

        public void SetTargetOffset(Transform anchor, Transform target)
        {
            targetOffset = anchor.InverseTransformDirection(target.position - anchor.position);
            Quaternion inverse = Quaternion.Inverse(anchor.rotation);
            Vector3 forward = inverse * target.forward;
            Vector3 upwards = inverse * target.up;
            targetSpace = Quaternion.LookRotation(forward, upwards);
        }

        private void SyncUpdateTransform(ref XRNodeState nodeState)
        {
            Quaternion rotation = Quaternion.identity;
            nodeState.TryGetRotation(out rotation);

            if (targetSpace != Quaternion.identity)
            {
                var forward = rotation * (targetSpace * Vector3.forward);
                var upwards = rotation * (targetSpace * Vector3.up);
                rotation = Quaternion.LookRotation(forward, upwards);
            }
            transform.localRotation = rotation;

            Vector3 position = Vector3.zero;
            nodeState.TryGetPosition(out position);
            if (targetOffset != Vector3.zero)
            {
                position = position + rotation * targetOffset;
            }
            transform.localPosition = position;

            OnUpdateTransform(ref nodeState);
        }

        #endregion

        #region Abstract Methods

        protected abstract void OnAfterCreate();

        protected abstract void OnBeforeDestroy();

        protected abstract void OnUpdate();

        protected abstract void OnUpdateTransform(ref XRNodeState nodeState);

        protected abstract void OnUpdateInputDevice(ref UnityEngine.XR.InputDevice inputDevice);

        protected abstract void OnConnected(XRNodeState nodeState, UnityEngine.XR.InputDevice inputDevice);

        protected abstract void OnDisconnected();

        #endregion
    }
}
