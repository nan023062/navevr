using System.Collections;
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
            base.Release();
        }

        private IEnumerator LoadDriverAsync()
        {
            yield return new WaitForEndOfFrame();
            XRSettings.LoadDeviceByName(driver);
            yield return new WaitForEndOfFrame();
            XRSettings.enabled = true;
            OnInitlized(!string.IsNullOrEmpty(XRSettings.loadedDeviceName));
        }

        protected override void UpdateHandNodeState(HandInputNode hand, ref XRNodeState xRNode)
        {
            var device = U3DInputDevices.GetDeviceAtXRNode(xRNode.nodeType);
            float thumb = 0f, index = 0f, middle = 0f;

            //psotion && rotation
            xRNode.TryGetPosition(out hand.position);
            xRNode.TryGetRotation(out hand.rotation);

            //grip
            device.TryGetFeatureValue(CommonUsages.gripButton, out hand.gripPressed);
            device.TryGetFeatureValue(CommonUsages.grip, out hand.gripTouchValue);
            hand.gripTouchValue = hand.gripTouchValue > 0.06f ? hand.gripTouchValue : 0f;
            middle = hand.gripTouchValue;

            //trigger
            device.TryGetFeatureValue(CommonUsages.triggerButton, out hand.triggerPressed);
            device.TryGetFeatureValue(CommonUsages.trigger, out hand.triggerTouchValue);
            hand.triggerTouchValue = hand.triggerTouchValue > 0.06f ? hand.triggerTouchValue : 0f;
            index = hand.triggerTouchValue;

            //system
            device.TryGetFeatureValue(CommonUsages.menuButton, out hand.systemPressed);
            hand.systemTouchValue = 0f;
            if (hand.systemPressed) hand.systemTouchValue = 1f;
            thumb = Mathf.Max(thumb, hand.systemTouchValue);

            //説明：這裏按鍵映射關係不一樣-》secondary 和 primary互換了位置2020.07.13
            //primary
            device.TryGetFeatureValue(CommonUsages.secondaryButton, out hand.primaryPressed);
            bool primaryTouch = false;
            device.TryGetFeatureValue(CommonUsages.secondaryTouch, out primaryTouch);
            hand.primaryTouchValue = (hand.primaryPressed || primaryTouch) ? 1f : 0f;
            thumb = Mathf.Max(thumb, hand.primaryTouchValue);

            //secondary
            device.TryGetFeatureValue(CommonUsages.primaryButton, out hand.secondaryPressed);
            bool secondaryTouch = false;
            device.TryGetFeatureValue(CommonUsages.primaryTouch, out secondaryTouch);
            hand.secondaryTouchValue = (hand.secondaryPressed || secondaryTouch) ? 1f : 0f;
            thumb = Mathf.Max(thumb, hand.secondaryTouchValue);

            //primary2DAxis
            device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out hand.primary2DAxisTouch);
            device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out hand.primary2DAxisPressed);
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out hand.primary2DAxis);
            thumb = Mathf.Max(thumb, (hand.primary2DAxisPressed || hand.primary2DAxisTouch) ? 1f : 0f);
            thumb = Mathf.Max(thumb, hand.primary2DAxis.sqrMagnitude > 0.1f ? 1f : 0f);

            //secondary2DAxis
            device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out hand.secondary2DAxis);
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
