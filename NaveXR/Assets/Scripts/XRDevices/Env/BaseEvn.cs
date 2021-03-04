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

namespace Nave.XR
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
            NaveXR.GetInstance().StartCoroutine(InitEvnAsync((error)=> {
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

            TryCheckNodeState(NaveXR.headMeta, XRNode.Head);
            TryCheckNodeState(NaveXR.leftHandMeta, XRNode.LeftHand);
            TryCheckNodeState(NaveXR.rightHandMeta, XRNode.RightHand);

            TryCheckTrackNodeState(NaveXR.pelivMeta, XRNode.HardwareTracker);
            TryCheckTrackNodeState(NaveXR.leftFootMeta, XRNode.HardwareTracker);
            TryCheckTrackNodeState(NaveXR.rightFootMeta, XRNode.HardwareTracker);
        }

        private void UpdateInputDeviceStates()
        {
            for (int i = xRNodeStates.Count - 1; i >= 0; i--) {
                var nodeState = xRNodeStates[i];
                if (nodeState.uniqueID == NaveXR.headMeta.uniqueID) {
                    FillPoseMetadata(NaveXR.headMeta, ref nodeState);
                }
                else if (nodeState.uniqueID == NaveXR.pelivMeta.uniqueID) {
                    FillPoseMetadata(NaveXR.pelivMeta, ref nodeState);
                }
                else if (nodeState.uniqueID == NaveXR.leftFootMeta.uniqueID) {
                    FillPoseMetadata(NaveXR.leftFootMeta, ref nodeState);
                }
                else if (nodeState.uniqueID == NaveXR.rightFootMeta.uniqueID) {
                    FillPoseMetadata(NaveXR.rightFootMeta, ref nodeState);
                }
                else if (nodeState.uniqueID == NaveXR.leftHandMeta.uniqueID) {
                    FillMetadata(NaveXR.leftHandMeta, ref nodeState);
                }
                else if (nodeState.uniqueID == NaveXR.rightHandMeta.uniqueID) {
                    FillMetadata(NaveXR.rightHandMeta, ref nodeState);
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
            var pelvis = NaveXR.pelivMeta;
            var lfoot = NaveXR.leftFootMeta;
            var rfoot = NaveXR.rightFootMeta;

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
            NaveXR.OnHardwardConnected(metadata);
        }

        private void InputTracking_nodeRemoved(Metadata metadata)
        {
            if (metadata == null || !metadata.isValid) return;
            NaveXR.OnHardwardDisconnected(metadata);
            metadata.Disconnect();
        }
    }
}
