using UnityEngine;
using System;
using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    public class Headset : Controller
    {
        protected override void OnAfterCreate()
        {
            nodeType = UnityEngine.XR.XRNode.Head;
            //SetTargetOffset(new Vector3(0f, -0.08f, -0.09f), Quaternion.identity);
        }

        protected override void OnBeforeDestroy()
        {
        }


        protected override void OnUpdate()
        {
        }

        protected override void OnUpdateTransform(ref XRNodeState xRNodeState)
        {
        }

        protected override void OnConnected(XRNodeState nodeState, UnityEngine.XR.InputDevice inputDevice)
        {
        }

        protected override void OnDisconnected()
        {
        }

        protected override void OnUpdateInputDevice(ref UnityEngine.XR.InputDevice inputDevice)
        {
        }
    }
}
