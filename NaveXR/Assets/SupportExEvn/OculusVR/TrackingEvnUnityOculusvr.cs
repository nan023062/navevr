using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using U3DInputDevices = UnityEngine.XR.InputDevices;

namespace Nave.VR
{
    [XREnv(name="UnityOculusvr",lib = XRLib.Oculus)]
    internal class TrackingEvnUnityOculusvr : Nave.VR.TrackingEvnBase
    {
        protected override IEnumerator InitEvnAsync(Action<string> onResult)
        {
            InputDevices.GetInstance().gameObject.AddComponent<OVRManager>();
            float duration = 5f;
            while (duration > 0f) {
                if (OVRManager.OVRManagerinitialized) {
                    OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
                    onResult?.Invoke(string.Empty);
                    yield break;
                }
                duration -= Time.deltaTime;
            }
            onResult("UnityOculusEvn.OVRManager初始化失败");
        }

        internal override void Release()
        {
            if (OVRManager.instance){
                OVRManager.instance.enabled = false;
                GameObject.Destroy(OVRManager.instance);
            }
            base.Release();
        }

        protected override void FillMetadata(HandAnchor anchor, ref XRNodeState xRNode)
        {
            var device = U3DInputDevices.GetDeviceAtXRNode(xRNode.nodeType);
            float thumb = 0f, index = 0f, middle = 0f;

            //psotion && rotation
            xRNode.TryGetPosition(out anchor.position);
            xRNode.TryGetRotation(out anchor.rotation);

            OVRInput.Controller touch = OVRInput.Controller.LTouch;
            if (anchor.type == NodeType.RightHand) touch = OVRInput.Controller.RTouch;

            //grip
            anchor.gripTouchValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, touch);
            anchor.gripPressed = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, touch);
            middle = anchor.gripTouchValue;

            //trigger
            anchor.triggerTouchValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, touch);
            anchor.triggerPressed = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, touch);
            index = anchor.triggerTouchValue;

            //system
            device.TryGetFeatureValue(CommonUsages.menuButton, out anchor.systemPressed);
            anchor.systemTouchValue = 0f;
            if (anchor.systemPressed) anchor.systemTouchValue = 1f;
            thumb = Mathf.Max(thumb, anchor.systemTouchValue);
            
            //primary
            anchor.primaryPressed = OVRInput.Get(OVRInput.Button.One, touch);
            anchor.primaryTouch = OVRInput.Get(OVRInput.Touch.One, touch);
            anchor.primaryTouchValue = 0f;
            if (anchor.primaryTouch) anchor.primaryTouchValue = 0.5f;
            if (anchor.primaryPressed) anchor.primaryTouchValue = 1.0f;
            thumb = Mathf.Max(thumb, anchor.primaryTouchValue);

            //secondary
            anchor.secondaryPressed = OVRInput.Get(OVRInput.Button.Two, touch);
            anchor.secondaryTouch = OVRInput.Get(OVRInput.Touch.Two, touch);
            anchor.secondaryTouchValue = 0f;
            if (anchor.secondaryTouch) anchor.secondaryTouchValue = 0.5f;
            if (anchor.secondaryPressed) anchor.secondaryTouchValue = 1.0f;
            thumb = Mathf.Max(thumb, anchor.secondaryTouchValue);

            //primary2DAxis
            anchor.primary2DAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, touch);
            anchor.primary2DAxisPressed = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, touch);
            anchor.primary2DAxisTouch = OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, touch);
            thumb = Mathf.Max(thumb, (anchor.primary2DAxisPressed || anchor.primary2DAxisTouch) ? 1f : 0f);
            thumb = Mathf.Max(thumb, anchor.primary2DAxis.sqrMagnitude > 0.1f ? 1f : 0f);

            //secondary2DAxis
            anchor.secondary2DAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, touch);
            thumb = Mathf.Max(thumb, anchor.secondary2DAxis.sqrMagnitude > 0.1f ? 1f : 0f);
            thumb = thumb > 0.06f ? thumb : 0;

            //fingers
            anchor.fingerCurls[0] = thumb;
            anchor.fingerCurls[1] = index;
            anchor.fingerCurls[2] = middle;
            anchor.fingerCurls[3] = middle;
            anchor.fingerCurls[4] = middle;
            anchor.handPoseChanged = true;
        }
    }
}
