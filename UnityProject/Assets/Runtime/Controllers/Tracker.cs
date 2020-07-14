using UnityEngine;
using System;
using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    public class Tracker : Controller
    {
        protected override void OnAfterCreate()
        {
            nodeType = XRNode.HardwareTracker;
        }

        protected override void OnBeforeDestroy()
        {

        }

        protected override void OnConnected(XRNodeState nodeState, UnityEngine.XR.InputDevice inputDevice)
        {

        }

        protected override void OnDisconnected()
        {

        }

        protected override void OnUpdate()
        {

        }

        protected override void OnUpdateInputDevice(ref UnityEngine.XR.InputDevice inputDevice)
        {

        }

        protected override void OnUpdateTransform(ref XRNodeState nodeState)
        {

        }
    }
}
