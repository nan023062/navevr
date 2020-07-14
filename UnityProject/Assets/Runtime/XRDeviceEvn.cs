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

        static List<Controller> controllers = new List<Controller>();

        static List<XRNodeState> xRNodeStates = new List<XRNodeState>();

        static List<XRNodeState> newXRNodeStates = new List<XRNodeState>();

        private bool SameNodeState(XRNodeState nodeState1, XRNodeState nodeState2)
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
                    if (SameNodeState(nodeState, newNodeState))
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
                    if (SameNodeState(nodeState, newNodeState))
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
        }

        private void InputTracking_nodeAdded(XRNodeState xRNode)
        {
            if (xRNode.nodeType == XRNode.Head || xRNode.nodeType == XRNode.LeftHand ||
                xRNode.nodeType == XRNode.RightHand || xRNode.nodeType == XRNode.HardwareTracker)
            {
                UnityEngine.XR.InputDevice inputDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(xRNode.nodeType);
                XRDeviceUsage usage = XRDeviceUsage.Get(inputDevice, xRNode);
                xRDeviceUsages.Add(xRNode.uniqueID, usage);

                if(xRNode.nodeType == XRNode.Head) headset = usage;
                else if (xRNode.nodeType == XRNode.LeftHand) leftHand = usage;
                else if (xRNode.nodeType == XRNode.RightHand) rightHand = usage;

                Debug.LogFormat("XRDevice.InputTracking_nodeAdded... [nodeType={0}]", xRNode.nodeType);

                //查找未匹配的设备列表
                var devices = controllers.Where((d) => !d.isTracked && d.NodeType == xRNode.nodeType);
                if (devices != null && devices.Count()>0){
                    foreach (var device in devices){
                        device.Connected(xRNode,inputDevice);
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
                    xRDeviceUsages.Remove(xRNode.uniqueID);
                    XRDeviceUsage.Put(deviceUsage);

                    if (xRNode.nodeType == XRNode.Head) headset = null;
                    else if (xRNode.nodeType == XRNode.LeftHand) leftHand = null;
                    else if (xRNode.nodeType == XRNode.RightHand) rightHand = null;

                    Debug.LogFormat("XRDevice.InputTracking_nodeRemoved... [nodeType={0}", xRNode.nodeType);

                    //查找已匹配的设备列表
                    var devices = controllers.Where((d) => d.isTracked && d.NodeType == xRNode.nodeType);
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

        public static void RegistDeviceCapture(Controller controller)
        {
            controllers.Add(controller);

            //查找已匹配的设备列表
            var usages = xRDeviceUsages.Values.Where((d) => d.nodeState.nodeType == controller.NodeType);
            if (usages != null && usages.Count() > 0)
            {
                foreach (var usage in usages)
                {
                    controller.Connected(usage.nodeState,controller.inputDevice);
                    return;
                }
            }
        }

        public static void UnregistDeviceCapture(Controller controller)
        {
            controllers.Remove(controller);
        }

        private static void UpdateInputDeviceAndNodeStatess()
        {
            int length = controllers.Count;
            int lengthOfNode = xRNodeStates.Count;
            for (int i = 0; i < length; i++){
                var device = controllers[i];
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