/********************************************************
 * FileName:    TrackingEvnBase.cs
 * Description: 设备输入插件类-抽象接口
 *              1 适配不同的输入库类型
 *              2 获取XR输入需要的数据
 * History:    
 * ******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.XR;
using U3DInputDevices = UnityEngine.XR.InputDevices;
using System.Linq;

namespace Nave.VR
{
    internal abstract class TrackingEvnBase
    {
        public string name {
            get {
                var des = GetType().GetCustomAttribute<XREnvAttribute>();
                if (des == null) throw new System.Exception($"Type={GetType().FullName}没有找到特性XREnvAttribute！");
                return des.name;
            }
        }

        public string lib { 
            get {
                var des = GetType().GetCustomAttribute<XREnvAttribute>();
                if (des == null) throw new System.Exception($"Type={GetType().FullName}没有找到特性XREnvAttribute！");
                return des.lib;
            } }

        public bool valid { get; private set; } = false;

        protected List<XRNodeState> xRNodeStates = new List<XRNodeState>();

        protected List<InputDevice> inputDevices = new List<InputDevice>();

        internal void Initlize(System.Action<string> onResult)
        {
            xRNodeStates.Clear();

            //TODO: 实现驱动加载和初始化
            InputDevices.GetInstance().StartCoroutine(InitEvnAsync((error)=> {
                valid = string.IsNullOrEmpty(error);
                onResult?.Invoke(error);
            }));
        }

        protected abstract IEnumerator InitEvnAsync(System.Action<string> onResult);

        internal virtual void Release()
        {
            xRNodeStates.Clear();
        }

        internal void UpdateTrackingSpaceData()
        {
            if (!valid) return;

            CheckDeviceRemovedOrAdded();

            UpdateInputDeviceStates();
        }

        private void CheckDeviceRemovedOrAdded()
        {
            xRNodeStates.Clear();

            InputTracking.GetNodeStates(xRNodeStates);

            TryCheckNodeState(InputDevices.headAnchor, XRNode.Head);

            TryCheckNodeState(InputDevices.leftHandAnchor, XRNode.LeftHand);

            TryCheckNodeState(InputDevices.rightHandAnchor, XRNode.RightHand);

            TryCheckTrackNodeState(InputDevices.pelivsAnchor, XRNode.HardwareTracker);

            TryCheckTrackNodeState(InputDevices.leftFootAnchor, XRNode.HardwareTracker);

            TryCheckTrackNodeState(InputDevices.rightFootAnchor, XRNode.HardwareTracker);
        }

        private void UpdateInputDeviceStates()
        {
            for (int i = xRNodeStates.Count - 1; i >= 0; i--) {
                var nodeState = xRNodeStates[i];
                if (nodeState.uniqueID == InputDevices.headAnchor.uniqueID) 
                    FillPoseMetadata(InputDevices.headAnchor, ref nodeState);

                else if (nodeState.uniqueID == InputDevices.pelivsAnchor.uniqueID) 
                    FillPoseMetadata(InputDevices.pelivsAnchor, ref nodeState);

                else if (nodeState.uniqueID == InputDevices.leftFootAnchor.uniqueID) 
                    FillPoseMetadata(InputDevices.leftFootAnchor, ref nodeState);

                else if (nodeState.uniqueID == InputDevices.rightFootAnchor.uniqueID) 
                    FillPoseMetadata(InputDevices.rightFootAnchor, ref nodeState);

                else if (nodeState.uniqueID == InputDevices.leftHandAnchor.uniqueID) 
                    FillMetadata(InputDevices.leftHandAnchor, ref nodeState);

                else if (nodeState.uniqueID == InputDevices.rightHandAnchor.uniqueID) 
                    FillMetadata(InputDevices.rightHandAnchor, ref nodeState);
            }
        }

        protected abstract void FillMetadata(HandAnchor anchor, ref XRNodeState xRNode);

        protected void FillPoseMetadata(TrackingAnchor anchor, ref XRNodeState xRNode)
        {
            xRNode.TryGetPosition(out anchor.position);

            xRNode.TryGetRotation(out anchor.rotation);
        }

        private void TryCheckNodeState(TrackingAnchor anchor, XRNode xRNode)
        {
            XRNodeState xRNodeState = default(XRNodeState);

            for (int i = xRNodeStates.Count - 1; i >= 0; i--) {
                var nodeState = xRNodeStates[i];
                if (nodeState.nodeType == xRNode) {
                    if (nodeState.uniqueID == anchor.uniqueID) return;
                    if (xRNodeState.uniqueID == 0) xRNodeState = nodeState;
                    if (!anchor.connected) {
                        InputTracking_nodeAdded(anchor, ref nodeState);
                        return;
                    }
                }
            }

            if (anchor.connected) InputTracking_nodeRemoved(anchor);
            if (xRNodeState.uniqueID > 0) InputTracking_nodeAdded(anchor, ref xRNodeState);
        }

        private void TryCheckTrackNodeState(TrackingAnchor anchor, XRNode xRNode)
        {
            XRNodeState xRNodeState = default(XRNodeState);
            var pelvis = InputDevices.pelivsAnchor;
            var lfoot = InputDevices.leftFootAnchor;
            var rfoot = InputDevices.rightFootAnchor;

            for (int i = xRNodeStates.Count - 1; i >= 0; i--) {
                var nodeState = xRNodeStates[i];
                ulong uniqueID = nodeState.uniqueID;
                if (nodeState.nodeType == xRNode) {
                    if (nodeState.uniqueID == anchor.uniqueID) return;

                    if ((pelvis.connected && uniqueID == pelvis.uniqueID) ||
                        (lfoot.connected && uniqueID == lfoot.uniqueID) ||
                        (rfoot.connected && uniqueID == rfoot.uniqueID)) {
                        continue;
                    }

                    if (xRNodeState.uniqueID == 0) xRNodeState = nodeState;

                    if (!anchor.connected) {
                        InputTracking_nodeAdded(anchor, ref nodeState);
                        return;
                    }
                }
            }

            if (anchor.connected) InputTracking_nodeRemoved(anchor);
            if (xRNodeState.uniqueID > 0) InputTracking_nodeAdded(anchor, ref xRNodeState);
        }

        private void InputTracking_nodeAdded(TrackingAnchor anchor, ref XRNodeState xRNodeState)
        {
            if (anchor == null || anchor.connected) return;
            InputDevice inputDevice = U3DInputDevices.GetDeviceAtXRNode(xRNodeState.nodeType);
            anchor.Connected(xRNodeState.uniqueID, inputDevice.name);
            InputDevices.OnTrackerConnected(anchor);
        }

        private void InputTracking_nodeRemoved(TrackingAnchor anchor)
        {
            if (anchor == null || !anchor.connected) return;
            InputDevices.OnTrackerDisconnected(anchor);
            anchor.Disconnect();
        }
    }
}
