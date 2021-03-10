using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using U3DInputDevices = UnityEngine.XR.InputDevices;

namespace Nave.VR
{
    [XREnv(name = "UnityOpenvr", lib = XRLib.OpenVR)]
    internal class TrackingEvnUnityOpenvr : TrackingEvnBase
    {
        protected override IEnumerator InitEvnAsync(Action<string> onResult)
        {
            if (!string.IsNullOrEmpty(XRSettings.loadedDeviceName))
                onResult?.Invoke(string.Empty);
            else
                onResult?.Invoke("UnityOpenvrEvn初始化失败");
            yield return null;
        }

        internal override void Release()
        {
            base.Release();
        }

        protected override void FillMetadata(HandAnchor anchor, ref XRNodeState xRNode)
        {
            var device = U3DInputDevices.GetDeviceAtXRNode(xRNode.nodeType);
            float thumb = 0f , index = 0f , middle = 0f;

            //psotion && rotation
            xRNode.TryGetPosition(out anchor.position);
            xRNode.TryGetRotation(out anchor.rotation);

            //grip
            device.TryGetFeatureValue(CommonUsages.gripButton, out anchor.gripPressed);
            device.TryGetFeatureValue(CommonUsages.grip, out anchor.gripTouchValue);
            anchor.gripTouchValue = anchor.gripTouchValue > 0.06f ? anchor.gripTouchValue : 0f;
            middle = anchor.gripTouchValue;

            //trigger
            device.TryGetFeatureValue(CommonUsages.triggerButton, out anchor.triggerPressed);
            device.TryGetFeatureValue(CommonUsages.trigger, out anchor.triggerTouchValue);
            anchor.triggerTouchValue = anchor.triggerTouchValue > 0.06f ? anchor.triggerTouchValue : 0f;
            index = anchor.triggerTouchValue;

            //system
            device.TryGetFeatureValue(CommonUsages.menuButton, out anchor.systemPressed);
            anchor.systemTouchValue = 0f;
            if (anchor.systemPressed) anchor.systemTouchValue = 1f;
            thumb = Mathf.Max(thumb, anchor.systemTouchValue);

            //primary
            device.TryGetFeatureValue(CommonUsages.primaryButton, out anchor.primaryPressed);
            bool primaryTouch = false;
            device.TryGetFeatureValue(CommonUsages.primaryTouch, out primaryTouch);
            anchor.primaryTouchValue = (anchor.primaryPressed || primaryTouch) ? 1f : 0f;
            thumb = Mathf.Max(thumb, anchor.primaryTouchValue);

            //secondary
            device.TryGetFeatureValue(CommonUsages.secondaryButton, out anchor.secondaryPressed);
            bool secondaryTouch = false;
            device.TryGetFeatureValue(CommonUsages.secondaryTouch, out secondaryTouch);
            anchor.secondaryTouchValue = (anchor.secondaryPressed||secondaryTouch) ? 1f : 0f;
            thumb = Mathf.Max(thumb, anchor.secondaryTouchValue);

            //primary2DAxis
            device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out anchor.primary2DAxisTouch);
            device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out anchor.primary2DAxisPressed);
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out anchor.primary2DAxis);
            thumb = Mathf.Max(thumb, (anchor.primary2DAxisPressed || anchor.primary2DAxisTouch) ? 1f : 0f);
            thumb = Mathf.Max(thumb, anchor.primary2DAxis.sqrMagnitude > 0.1f ? 1f : 0f);

            //secondary2DAxis
            device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out anchor.secondary2DAxis);
            thumb = Mathf.Max(thumb, anchor.secondary2DAxis.sqrMagnitude > 0.1f ? 1f : 0f);
            thumb = thumb > 0.06f ? thumb: 0;

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
