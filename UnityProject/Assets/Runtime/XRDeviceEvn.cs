using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using System.Linq;

namespace NaveXR.InputDevices
{
    public partial class XRDevice : MonoBehaviour
    {
        static Dictionary<ulong,XRDeviceUsage> xRDeviceUsages = new Dictionary<ulong, XRDeviceUsage>();

        static List<DeviceCapture> captures = new List<DeviceCapture>();

        public static DeviceCapture GeDeviceCapture(XRNode nodeType)
        {
            if (captures != null && captures.Count > 0){
                for (int i = 0; i < captures.Count; i++){
                    if(captures[i].isTracked && captures[i].NodeType == nodeType){
                        return captures[i];
                    }
                }
            }
            return null;
        }

        static List<XRNodeState> xRNodeStates = new List<XRNodeState>();
        static List<XRNodeState> newXRNodeStates = new List<XRNodeState>();

        public static event XRDeviceDelegate onDeviceConnected;

        public static event XRDeviceDelegate onDeviceDisconnected;

        private bool SameNodeState(ref XRNodeState nodeState1, ref XRNodeState nodeState2)
        {
            return (nodeState1.uniqueID == nodeState2.uniqueID
                && nodeState1.nodeType == nodeState2.nodeType);
        }

        private void CheckInputTrackingRemovedOrAdded()
        {
            newXRNodeStates.Clear();
            InputTracking.GetNodeStates(newXRNodeStates);

            //更新设备 isTracked 状态
            int lengthOfOld = xRNodeStates.Count;
            int lengthOfNew = newXRNodeStates.Count;

            //查找移除removed设备
            for (int i = 0; i < lengthOfOld; i++)
            {
                var nodeState = xRNodeStates[i];
                bool removed = true;

                for (int j = 0; j < lengthOfNew; j++)
                {
                    var newNodeState = newXRNodeStates[j];
                    if (SameNodeState(ref nodeState, ref newNodeState))
                    {
                        removed = false;
                        break;
                    }
                }
                if (removed)
                {
                    InputTracking_nodeRemoved(nodeState);
                }
            }

            //查找添加added新设备
            for (int i = 0; i < lengthOfNew; i++)
            {
                var newNodeState = newXRNodeStates[i];
                bool added = true;

                for (int j = 0; j < lengthOfOld; j++)
                {
                    var nodeState = xRNodeStates[j];
                    if (SameNodeState(ref nodeState, ref newNodeState))
                    {
                        added = false;
                        break;
                    }
                }
                if (added)
                {
                    InputTracking_nodeAdded(newNodeState);
                }
            }

            xRNodeStates.Clear();
            xRNodeStates.AddRange(newXRNodeStates);
        }

        private void XRDevice_deviceLoaded(string deviceName)
        {
            if (!XRSettings.enabled) XRSettings.enabled = true;
            Debug.LogFormat(" XRDevice_onDeviceLoaded() :: deviceName = {0}, mode = {1} , isDeviceActive = {2}!!", deviceName, UnityEngine.XR.XRDevice.model, XRSettings.isDeviceActive);
            checkTouchPad();
        }

        private void InputTracking_nodeAdded(XRNodeState xRNode)
        {
            if (xRNode.nodeType == XRNode.Head || xRNode.nodeType == XRNode.LeftHand ||
                xRNode.nodeType == XRNode.RightHand || xRNode.nodeType == XRNode.HardwareTracker)
            {
                UnityEngine.XR.InputDevice inputDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(xRNode.nodeType);
                XRDeviceUsage usage = XRDeviceUsage.Get(inputDevice, xRNode);
                xRDeviceUsages.Add(xRNode.uniqueID, usage);

                if (xRNode.nodeType == XRNode.Head) headset = usage;
                else if (xRNode.nodeType == XRNode.LeftHand) leftHand = usage;
                else if (xRNode.nodeType == XRNode.RightHand) rightHand = usage;

                //Debug.LogFormat("1111 XRDevice.onDeviceConnected... [nodeType={0}，name={1}]", xRNode.nodeType, inputDevice.name);
                onDeviceConnected?.Invoke(xRNode, inputDevice);

                //查找未匹配的设备列表
                var devices = captures.Where((d) => !d.isTracked && d.NodeType == xRNode.nodeType);
                if (devices != null && devices.Count()>0){
                    foreach (var device in devices){
                        usage.isTracked = true;
                        device.Connected(ref xRNode,ref inputDevice);
                    }
                }
            }
        }

        private void InputTracking_nodeRemoved(XRNodeState xRNode)
        {
            if (xRNode.nodeType == XRNode.Head || xRNode.nodeType == XRNode.LeftHand ||
                xRNode.nodeType == XRNode.RightHand || xRNode.nodeType == XRNode.HardwareTracker)
            {
                XRDeviceUsage deviceUsage;
                if(xRDeviceUsages.TryGetValue(xRNode.uniqueID, out deviceUsage))
                {
                    InputDevice inputDevice = deviceUsage.InputDevice;
                    xRDeviceUsages.Remove(xRNode.uniqueID);
                    deviceUsage.isTracked = false;
                    XRDeviceUsage.Put(deviceUsage);

                    if (xRNode.nodeType == XRNode.Head) headset = null;
                    else if (xRNode.nodeType == XRNode.LeftHand) leftHand = null;
                    else if (xRNode.nodeType == XRNode.RightHand) rightHand = null;

                    onDeviceDisconnected?.Invoke(xRNode, inputDevice);
                    //Debug.LogFormat("2222 XRDevice.onDeviceDisconnected... [nodeType={0}，name={1}]", xRNode.nodeType, inputDevice.name);

                    //查找已匹配的设备列表
                    var devices = captures.Where((d) => d.isTracked && d.NodeType == xRNode.nodeType);
                    if (devices != null && devices.Count() > 0){
                        foreach (var device in devices){
                            device.Disconnected();
                        }
                    }
                }
            }
        }

        private void InputTracking_trackingAcquired(XRNodeState xRNode)
        {
            Debug.LogFormat("XRDevice.InputTracking_trackingAcquired... [nodeType={0},tracked={1}]", xRNode.nodeType,xRNode.tracked);
        }

        private void InputTracking_trackingLost(XRNodeState xRNode)
        {
            Debug.LogFormat("XRDevice.InputTracking_trackingLost... [nodeType={0},tracked={1}]", xRNode.nodeType, xRNode.tracked);
        }

        internal static void RegistDevice(DeviceCapture controller)
        {
            captures.Add(controller);

            //查找已匹配的设备列表
            var usages = xRDeviceUsages.Values.Where((d) => !d.isTracked && d.nodeState.nodeType == controller.NodeType);
            if (usages != null && usages.Count() > 0){
                foreach (var usage in usages){
                    usage.isTracked = true;
                    controller.Connected(ref usage.nodeState, ref usage.InputDevice);
                    return;
                }
            }
        }

        internal static void UnregistDevice(DeviceCapture controller)
        {
            XRDeviceUsage deviceUsage;
            if (xRDeviceUsages.TryGetValue(controller.UniqueId, out deviceUsage)){
                deviceUsage.isTracked = false;
            }
            captures.Remove(controller);
            controller.Disconnected();
        }

        private static void UpdateInputDeviceAndNodeStatess()
        {
            int length = captures.Count;
            int lengthOfNode = xRNodeStates.Count;
            for (int i = 0; i < length; i++){
                var device = captures[i];
                if (!device.isActiveAndEnabled) continue;

                for (int j = 0; j < lengthOfNode; j++)
                {
                    var nodeState = xRNodeStates[j];
                    if (nodeState.uniqueID == device.UniqueId)
                    {
                        XRDeviceUsage deviceUsage;
                        xRDeviceUsages.TryGetValue(nodeState.uniqueID, out deviceUsage);
                        device.UpdateInputDeviceAndXRNode(ref nodeState, ref deviceUsage.InputDevice);
                        break;
                    }
                }
            }
        }
    }
}