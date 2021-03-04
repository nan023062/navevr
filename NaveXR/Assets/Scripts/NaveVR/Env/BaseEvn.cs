/********************************************************
 * FileName:    Driver_Base.cs
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

namespace Nave.VR
{
    internal abstract class BaseEvn
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

        internal void Initlize(System.Action<string> onResult)
        {
            xRNodeStates.Clear();

            //TODO: 实现驱动加载和初始化
            NaveVR.GetInstance().StartCoroutine(InitEvnAsync((error)=> {
                valid = string.IsNullOrEmpty(error);
                onResult?.Invoke(error);
            }));
        }

        protected abstract IEnumerator InitEvnAsync(System.Action<string> onResult);

        internal virtual void Release()
        {
            xRNodeStates.Clear();
        }

        internal void UpdateAllControllerState()
        {
            if (!valid) return;

            //检测连接情况
            CheckDeviceRemovedOrAdded();

            //Update
            UpdateInputDeviceStates();
        }

        private void CheckDeviceRemovedOrAdded()
        {
            xRNodeStates.Clear();
            InputTracking.GetNodeStates(xRNodeStates);

            TryCheckNodeState(NaveVR.headMeta, XRNode.Head);
            TryCheckNodeState(NaveVR.leftHandMeta, XRNode.LeftHand);
            TryCheckNodeState(NaveVR.rightHandMeta, XRNode.RightHand);

            TryCheckTrackNodeState(NaveVR.pelivMeta, XRNode.HardwareTracker);
            TryCheckTrackNodeState(NaveVR.leftFootMeta, XRNode.HardwareTracker);
            TryCheckTrackNodeState(NaveVR.rightFootMeta, XRNode.HardwareTracker);
        }

        private void UpdateInputDeviceStates()
        {
            for (int i = xRNodeStates.Count - 1; i >= 0; i--) {
                var nodeState = xRNodeStates[i];
                if (nodeState.uniqueID == NaveVR.headMeta.uniqueID) {
                    FillPoseMetadata(NaveVR.headMeta, ref nodeState);
                }
                else if (nodeState.uniqueID == NaveVR.pelivMeta.uniqueID) {
                    FillPoseMetadata(NaveVR.pelivMeta, ref nodeState);
                }
                else if (nodeState.uniqueID == NaveVR.leftFootMeta.uniqueID) {
                    FillPoseMetadata(NaveVR.leftFootMeta, ref nodeState);
                }
                else if (nodeState.uniqueID == NaveVR.rightFootMeta.uniqueID) {
                    FillPoseMetadata(NaveVR.rightFootMeta, ref nodeState);
                }
                else if (nodeState.uniqueID == NaveVR.leftHandMeta.uniqueID) {
                    FillMetadata(NaveVR.leftHandMeta, ref nodeState);
                }
                else if (nodeState.uniqueID == NaveVR.rightHandMeta.uniqueID) {
                    FillMetadata(NaveVR.rightHandMeta, ref nodeState);
                }
            }
        }

        protected abstract void FillMetadata(HandMetadata hand, ref XRNodeState xRNode);

        protected void FillPoseMetadata(Metadata metadata, ref XRNodeState xRNode)
        {
            xRNode.TryGetPosition(out metadata.position);
            xRNode.TryGetRotation(out metadata.rotation);
        }

        private void TryCheckNodeState(Metadata metadata, XRNode xRNode)
        {
            XRNodeState xRNodeState = default(XRNodeState);

            for (int i = xRNodeStates.Count - 1; i >= 0; i--) {
                var nodeState = xRNodeStates[i];
                if (nodeState.nodeType == xRNode) {
                    if (nodeState.uniqueID == metadata.uniqueID) return;
                    if (xRNodeState.uniqueID == 0) xRNodeState = nodeState;
                    if (!metadata.isValid) {
                        InputTracking_nodeAdded(metadata, ref nodeState);
                        return;
                    }
                }
            }

            if (metadata.isValid) InputTracking_nodeRemoved(metadata);
            if (xRNodeState.uniqueID > 0) InputTracking_nodeAdded(metadata, ref xRNodeState);
        }

        private void TryCheckTrackNodeState(Metadata metadata, XRNode xRNode)
        {
            XRNodeState xRNodeState = default(XRNodeState);
            var pelvis = NaveVR.pelivMeta;
            var lfoot = NaveVR.leftFootMeta;
            var rfoot = NaveVR.rightFootMeta;

            for (int i = xRNodeStates.Count - 1; i >= 0; i--) {
                var nodeState = xRNodeStates[i];
                ulong uniqueID = nodeState.uniqueID;
                if (nodeState.nodeType == xRNode) {
                    if (nodeState.uniqueID == metadata.uniqueID) return;

                    if ((pelvis.isValid && uniqueID == pelvis.uniqueID) ||
                        (lfoot.isValid && uniqueID == lfoot.uniqueID) ||
                        (rfoot.isValid && uniqueID == rfoot.uniqueID)) {
                        continue;
                    }

                    if (xRNodeState.uniqueID == 0) xRNodeState = nodeState;

                    if (!metadata.isValid) {
                        InputTracking_nodeAdded(metadata, ref nodeState);
                        return;
                    }
                }
            }

            if (metadata.isValid) InputTracking_nodeRemoved(metadata);
            if (xRNodeState.uniqueID > 0) InputTracking_nodeAdded(metadata, ref xRNodeState);
        }

        private void InputTracking_nodeAdded(Metadata metadata, ref XRNodeState xRNodeState)
        {
            if (metadata == null || metadata.isValid) return;
            InputDevice inputDevice = U3DInputDevices.GetDeviceAtXRNode(xRNodeState.nodeType);
            metadata.Connected(xRNodeState.uniqueID, inputDevice.name);
            NaveVR.OnHardwardConnected(metadata);
        }

        private void InputTracking_nodeRemoved(Metadata metadata)
        {
            if (metadata == null || !metadata.isValid) return;
            NaveVR.OnHardwardDisconnected(metadata);
            metadata.Disconnect();
        }
    }
}
