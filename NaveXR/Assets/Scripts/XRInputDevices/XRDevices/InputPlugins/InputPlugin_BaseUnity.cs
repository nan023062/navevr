using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using U3DInputDevices = UnityEngine.XR.InputDevices;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// 基于UnityXR库
    /// </summary>
    internal abstract class InputPlugin_BaseUnity : InputPlugin_Base
    {
        protected List<XRNodeState> xRNodeStates = new List<XRNodeState>();

        internal override void Initlize()
        {
            xRNodeStates.Clear();
        }

        internal override void Release()
        {
            xRNodeStates.Clear();
        }

        internal override void CheckDeviceRemovedOrAdded()
        {
            xRNodeStates.Clear();
            InputTracking.GetNodeStates(xRNodeStates);

            TryCheckNodeState(XRDevice.headUsage, XRNode.Head);
            TryCheckNodeState(XRDevice.leftHandUsage, XRNode.LeftHand);
            TryCheckNodeState(XRDevice.rightHandUsage, XRNode.RightHand);

            TryCheckTrackNodeState(XRDevice.peliveUsage, XRNode.HardwareTracker);
            TryCheckTrackNodeState(XRDevice.leftFootUsage, XRNode.HardwareTracker);
            TryCheckTrackNodeState(XRDevice.rightFootUsage, XRNode.HardwareTracker);
        }

        internal sealed override void UpdateInputDeviceStates()
        {
            for (int i = xRNodeStates.Count - 1; i >= 0; i--)
            {
                var nodeState = xRNodeStates[i];
                if (nodeState.uniqueID == XRDevice.headUsage.uniqueID)
                {
                    UpdateTrackNodeState(XRDevice.headUsage, ref nodeState);
                }
                else if (nodeState.uniqueID == XRDevice.peliveUsage.uniqueID)
                {
                    UpdateTrackNodeState(XRDevice.peliveUsage, ref nodeState);
                }
                else if (nodeState.uniqueID == XRDevice.leftFootUsage.uniqueID)
                {
                    UpdateTrackNodeState(XRDevice.leftFootUsage, ref nodeState);
                }
                else if (nodeState.uniqueID == XRDevice.rightFootUsage.uniqueID)
                {
                    UpdateTrackNodeState(XRDevice.rightFootUsage, ref nodeState);
                }
                else if (nodeState.uniqueID == XRDevice.leftHandUsage.uniqueID)
                {
                    UpdateHandNodeState(XRDevice.leftHandUsage, ref nodeState);
                }
                else if (nodeState.uniqueID == XRDevice.rightHandUsage.uniqueID)
                {
                    UpdateHandNodeState(XRDevice.rightHandUsage, ref nodeState);
                }
            }
        }

        protected abstract void UpdateHandNodeState(HandInputNode hand, ref XRNodeState xRNode);

        protected void UpdateTrackNodeState(InputNode hardware, ref XRNodeState xRNode)
        {
            xRNode.TryGetPosition(out hardware.position);
            xRNode.TryGetRotation(out hardware.rotation);
        }

        private void TryCheckNodeState(InputNode xRNodeUsage, XRNode xRNode)
        {
            XRNodeState xRNodeState = default(XRNodeState);

            for (int i = xRNodeStates.Count - 1; i >= 0; i--)
            {
                var nodeState = xRNodeStates[i];
                if (nodeState.nodeType == xRNode)
                {
                    if (nodeState.uniqueID == xRNodeUsage.uniqueID) return;
                    if (xRNodeState.uniqueID == 0) xRNodeState = nodeState;
                    if (!xRNodeUsage.isValid)
                    {
                        InputTracking_nodeAdded(xRNodeUsage, ref nodeState);
                        return;
                    }
                }
            }

            if (xRNodeUsage.isValid) InputTracking_nodeRemoved(xRNodeUsage);
            if (xRNodeState.uniqueID > 0) InputTracking_nodeAdded(xRNodeUsage, ref xRNodeState);
        }

        private void TryCheckTrackNodeState(InputNode xRNodeUsage, XRNode xRNode)
        {
            XRNodeState xRNodeState = default(XRNodeState);
            var pelvis = XRDevice.peliveUsage;
            var lfoot = XRDevice.leftFootUsage;
            var rfoot = XRDevice.rightFootUsage;

            for (int i = xRNodeStates.Count - 1; i >= 0; i--)
            {
                var nodeState = xRNodeStates[i];
                ulong uniqueID = nodeState.uniqueID;
                if (nodeState.nodeType == xRNode)
                {
                    if (nodeState.uniqueID == xRNodeUsage.uniqueID) return;

                    if ((pelvis.isValid && uniqueID == pelvis.uniqueID) ||
                        (lfoot.isValid && uniqueID == lfoot.uniqueID) ||
                        (rfoot.isValid && uniqueID == rfoot.uniqueID))
                    {
                        continue;
                    }

                    if (xRNodeState.uniqueID == 0) xRNodeState = nodeState;

                    if (!xRNodeUsage.isValid)
                    {
                        InputTracking_nodeAdded(xRNodeUsage, ref nodeState);
                        return;
                    }
                }
            }

            if (xRNodeUsage.isValid) InputTracking_nodeRemoved(xRNodeUsage);
            if (xRNodeState.uniqueID > 0) InputTracking_nodeAdded(xRNodeUsage, ref xRNodeState);
        }

        private void InputTracking_nodeAdded(InputNode xRNodeUsage, ref XRNodeState xRNode)
        {
            if (xRNodeUsage == null || xRNodeUsage.isValid) return;
            InputDevice inputDevice = U3DInputDevices.GetDeviceAtXRNode(xRNode.nodeType);
            xRNodeUsage.OnConnected(xRNode.uniqueID, inputDevice.name);
            OnDeviceConnnected(xRNodeUsage);
        }

        private void InputTracking_nodeRemoved(InputNode xRNodeUsage)
        {
            if (xRNodeUsage == null || !xRNodeUsage.isValid) return;
            OnDeviceDisconnnected(xRNodeUsage);
            xRNodeUsage.OnDisconnect();
        }
    }
}
