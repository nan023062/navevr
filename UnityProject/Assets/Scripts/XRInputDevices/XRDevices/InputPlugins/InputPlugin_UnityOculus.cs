﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using U3DInputDevices = UnityEngine.XR.InputDevices;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// 实现 使用UnityXR库 Oculus支持
    /// </summary>
    internal class InputPlugin_UnityOculus : InputPlugin_BaseUnity
    {
        public override string driver { get { return DRVName.Oculus; } }

        public override InputPlugin name { get { return InputPlugin.Unity_Oculus; } }

        internal override void Initlize()
        {
            base.Initlize();
            XRDevice.GetInstance().StartCoroutine(LoadDriverAsync());
        }

        internal override void Release()
        {
            if (OVRManager.instance){
                OVRManager.instance.enabled = false;
                GameObject.Destroy(OVRManager.instance);
            }
            base.Release();
        }

        private IEnumerator LoadDriverAsync()
        {
            yield return new WaitForEndOfFrame();
            XRSettings.LoadDeviceByName(driver);
            yield return new WaitForEndOfFrame();
            XRSettings.enabled = true;
            XRDevice.GetInstance().gameObject.AddComponent<OVRManager>();

            float duration = 5f;
            while (duration > 0f) {
                if (OVRManager.OVRManagerinitialized) {
                    OnInitlized(true);
                    yield break;
                }
                duration -= Time.deltaTime;
            }
            OnInitlized(false);
        }

        protected override void UpdateHandNodeState(HandInputNode hand, ref XRNodeState xRNode)
        {
            var device = U3DInputDevices.GetDeviceAtXRNode(xRNode.nodeType);
            float thumb = 0f, index = 0f, middle = 0f;

            //psotion && rotation
            xRNode.TryGetPosition(out hand.position);
            xRNode.TryGetRotation(out hand.rotation);

            OVRInput.Controller touch = OVRInput.Controller.LTouch;
            if (hand.type == NodeType.RightHand) touch = OVRInput.Controller.RTouch;

            //grip
            hand.gripTouchValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, touch);
            hand.gripPressed = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, touch);
            middle = hand.gripTouchValue;

            //trigger
            hand.triggerTouchValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, touch);
            hand.triggerPressed = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, touch);
            index = hand.triggerTouchValue;

            //system
            device.TryGetFeatureValue(CommonUsages.menuButton, out hand.systemPressed);
            hand.systemTouchValue = 0f;
            if (hand.systemPressed) hand.systemTouchValue = 1f;
            thumb = Mathf.Max(thumb, hand.systemTouchValue);
            
            //primary
            hand.primaryPressed = OVRInput.Get(OVRInput.Button.One, touch);
            hand.primaryTouch = OVRInput.Get(OVRInput.Touch.One, touch);
            hand.primaryTouchValue = 0f;
            if (hand.primaryTouch) hand.primaryTouchValue = 0.5f;
            if (hand.primaryPressed) hand.primaryTouchValue = 1.0f;
            thumb = Mathf.Max(thumb, hand.primaryTouchValue);

            //secondary
            hand.secondaryPressed = OVRInput.Get(OVRInput.Button.Two, touch);
            hand.secondaryTouch = OVRInput.Get(OVRInput.Touch.Two, touch);
            hand.secondaryTouchValue = 0f;
            if (hand.secondaryTouch) hand.secondaryTouchValue = 0.5f;
            if (hand.secondaryPressed) hand.secondaryTouchValue = 1.0f;
            thumb = Mathf.Max(thumb, hand.secondaryTouchValue);

            //primary2DAxis
            hand.primary2DAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, touch);
            hand.primary2DAxisPressed = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, touch);
            hand.primary2DAxisTouch = OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, touch);
            thumb = Mathf.Max(thumb, (hand.primary2DAxisPressed || hand.primary2DAxisTouch) ? 1f : 0f);
            thumb = Mathf.Max(thumb, hand.primary2DAxis.sqrMagnitude > 0.1f ? 1f : 0f);

            //secondary2DAxis
            hand.secondary2DAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, touch);
            thumb = Mathf.Max(thumb, hand.secondary2DAxis.sqrMagnitude > 0.1f ? 1f : 0f);
            thumb = thumb > 0.06f ? thumb : 0;

            //fingers
            hand.fingerCurls[0] = thumb;
            hand.fingerCurls[1] = index;
            hand.fingerCurls[2] = middle;
            hand.fingerCurls[3] = middle;
            hand.fingerCurls[4] = middle;
            hand.handPoseChanged = true;
        }
    }
}