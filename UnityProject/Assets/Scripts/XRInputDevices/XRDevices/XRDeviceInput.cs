using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using NaveXR;
using NaveXR.InputDevices;

namespace NaveXR.InputDevices
{
    public partial class XRDevice : MonoBehaviour
    {
        Dictionary<int, HandInputBase[]> mXRHandInputs;

        HeadInputEye headInputEye;

        private void InitlizeInputs()
        {
            if (mXRHandInputs == null) {
                mXRHandInputs = new Dictionary<int, HandInputBase[]>()
                {
                    [(int)XRKeyCode.Menu] = new HandInputBase[2] { new HandMenu(), new HandMenu() },
                    [(int)XRKeyCode.Primary] = new HandInputBase[2] { new HandPrimary(), new HandPrimary() },
                    [(int)XRKeyCode.Secondary] = new HandInputBase[2] { new HandSecondary(), new HandSecondary() },
                    [(int)XRKeyCode.Trigger] = new HandInputBase[2] { new HandTrigger(), new HandTrigger() },
                    [(int)XRKeyCode.Grip] = new HandInputBase[2] { new HandGrip(), new HandGrip() },
                    [(int)XRKeyCode.TouchMiddle] = new HandInputBase[2] { new HandTouchButton(XRKeyCode.TouchMiddle), new HandTouchButton(XRKeyCode.TouchMiddle) },
                    [(int)XRKeyCode.TouchNorth] = new HandInputBase[2] { new HandTouchButton(XRKeyCode.TouchNorth), new HandTouchButton(XRKeyCode.TouchNorth) },
                    [(int)XRKeyCode.TouchWest] = new HandInputBase[2] { new HandTouchButton(XRKeyCode.TouchWest), new HandTouchButton(XRKeyCode.TouchWest) },
                    [(int)XRKeyCode.TouchEast] = new HandInputBase[2] { new HandTouchButton(XRKeyCode.TouchEast), new HandTouchButton(XRKeyCode.TouchEast) },
                    [(int)XRKeyCode.TouchSouth] = new HandInputBase[2] { new HandTouchButton(XRKeyCode.TouchSouth), new HandTouchButton(XRKeyCode.TouchSouth) },
                    [(int)XRKeyCode.TouchAxis] = new HandInputBase[2] { new HandTouchAxis(), new HandTouchAxis() },
                    [(int)XRKeyCode.HandPose] = new HandInputBase[2] { new HandInputPose(true), new HandInputPose(false) },
                };
            }
            if (headInputEye == null) headInputEye = new HeadInputEye();
        }

        private void UpdateInputStates()
        {
#if UNITY_EDITOR
            TestXRInputUsages();
#endif
            foreach (var inputs in mXRHandInputs.Values){
                if (leftHandUsage.isValid) inputs[0].UpdateState(leftHandUsage);
                if (rightHandUsage.isValid) inputs[1].UpdateState(rightHandUsage);
            }
            if (headUsage.isValid) headInputEye.UpdateState(headUsage);
        }

        private void ClearInputs() { mXRHandInputs?.Clear(); }

        internal static T GetHandInputKey<T>(int hand, XRKeyCode keyCode) where T : HandInputBase
        {
            CheckSingleton();
            if (_instance == null) return null;
            _instance.InitlizeInputs();
            hand = Math.Max(0, Math.Min(1, hand));
            return _instance.mXRHandInputs[(int)keyCode][hand] as T;
        }

        #region API

        public static float GetKeyForce(int hand, XRKeyCode keyCode)
        {
            var key = GetHandInputKey<HandInputBase>(hand, keyCode);
            return key.Force;
        }

        public static bool IsKeyTouched(int hand, XRKeyCode keyCode)
        {
            var key = GetHandInputKey<HandInputBase>(hand, keyCode);
            return key.Touched;
        }

        public static bool IsKeyPressed(int hand, XRKeyCode keyCode)
        {
            var key = GetHandInputKey<HandInputBase>(hand, keyCode);
            return key.Pressed;
        }

        public static bool IsKeyDown(int hand, XRKeyCode keyCode)
        {
            var key = GetHandInputKey<HandInputBase>(hand, keyCode);
            return key.KeyDown;
        }

        public static bool IsKeyUp(int hand, XRKeyCode keyCode)
        {
            var key = GetHandInputKey<HandInputBase>(hand, keyCode);
            return key.KeyUp;
        }

        public static float GetLeftKeyForce(XRKeyCode keyCode)
        {
            return GetKeyForce(0, keyCode);
        }

        public static bool IsLeftKeyTouched(XRKeyCode keyCode)
        {
            return IsKeyTouched(0, keyCode);
        }

        public static bool IsLeftKeyPressed(XRKeyCode keyCode)
        {
            return IsKeyPressed(0, keyCode);
        }

        public static bool IsLeftKeyDown(XRKeyCode keyCode)
        {
            return IsKeyDown(0, keyCode);
        }

        public static bool IsLeftKeyUp(XRKeyCode keyCode)
        {
            return IsKeyUp(0, keyCode);
        }

        public static float GetRightKeyForce(XRKeyCode keyCode)
        {
            return GetKeyForce(1, keyCode);
        }

        public static bool IsRightKeyTouched(XRKeyCode keyCode)
        {
            return IsKeyTouched(1, keyCode);
        }

        public static bool IsRightKeyPressed(XRKeyCode keyCode)
        {
            return IsKeyPressed(1, keyCode);
        }

        public static bool IsRightKeyDown(XRKeyCode keyCode)
        {
            return IsKeyDown(1, keyCode);
        }

        public static bool IsRightKeyUp(XRKeyCode keyCode)
        {
            return IsKeyUp(1, keyCode);
        }

        public static float GetTouchButtonDegree(int hand, XRKeyCode keyCode)
        {
            HandTouchButton touch = null;
            switch (keyCode)
            {
                case XRKeyCode.TouchNorth:
                    touch = GetHandInputKey<HandTouchButton>(hand, keyCode);
                    break;
                case XRKeyCode.TouchWest:
                    touch = GetHandInputKey<HandTouchButton>(hand, keyCode);
                    break;
                case XRKeyCode.TouchSouth:
                    touch = GetHandInputKey<HandTouchButton>(hand, keyCode);
                    break;
                case XRKeyCode.TouchEast:
                    touch = GetHandInputKey<HandTouchButton>(hand, keyCode);
                    break;
            }
            return touch == null ? 0f : touch.degree;
        }

        public static Vector2 GetTouchAxis(int hand)
        {
            var touchInput = GetHandInputKey<HandTouchAxis>(hand, XRKeyCode.TouchAxis);
            return touchInput.Value2D;
        }

        public static float[] GetHandPos(int hand)
        {
            var handpose = GetHandInputKey<HandInputPose>(hand, XRKeyCode.HandPose);
            return handpose.handPose_Value;
        }

        public static HandPose_skeleton GetHandPose(int hand)
        {
            var handpose = GetHandInputKey<HandInputPose>(hand, XRKeyCode.HandPose);
            return handpose.pose;
        }

        public static HandInputPose GetHandInputPose(int hand)
        {
            return GetHandInputKey<HandInputPose>(hand, XRKeyCode.HandPose);
        }

        #endregion

        #region Input Points

        private static List<LaserPointer> m_laserPointers;

        private static List<FingerPointer> m_fingerPointers;

        internal static void Regist(LaserPointer laser)
        {
            GetLasers().Add(laser);
        }

        internal static void Remove(LaserPointer laser)
        {
            GetLasers()?.Remove(laser);
        }

        internal static void Regist(FingerPointer finger)
        {
            GetFingers().Add(finger);
        }

        internal static void Remove(FingerPointer finger)
        {
            GetFingers()?.Remove(finger);
        }

        internal static List<FingerPointer> GetFingers()
        {
            if (m_fingerPointers == null) m_fingerPointers = new List<FingerPointer>();
            return m_fingerPointers;
        }

        internal static List<LaserPointer> GetLasers()
        {
            if (m_laserPointers == null) m_laserPointers = new List<LaserPointer>();
            return m_laserPointers;
        }

        #endregion

        #region TestXRInputUsages

#if UNITY_EDITOR

        [SerializeField] private bool left = true;

        [Header("Bool")]
        [SerializeField] private bool isTracked;
        [SerializeField] private bool primaryButton;
        [SerializeField] private bool primaryTouch;
        [SerializeField] private bool secondaryButton;
        [SerializeField] private bool secondaryTouch;
        [SerializeField] private bool gripButton;
        [SerializeField] private bool triggerButton;
        [SerializeField] private bool menuButton;
        [SerializeField] private bool primary2DAxisClick;
        [SerializeField] private bool primary2DAxisTouch;
        [SerializeField] private bool thumbrest;

        [Header("Float")]
        [SerializeField] private float indexTouch;
        [SerializeField] private float thumbTouch;
        [SerializeField] private float batteryLevel;
        [SerializeField] private float trigger;
        [SerializeField] private float grip;
        [SerializeField] private float indexFinger;
        [SerializeField] private float middleFinger;
        [SerializeField] private float ringFinger;
        [SerializeField] private float pinkyFinger;

        [Header("Vector2")]
        [SerializeField] private Vector2 primary2DAxis;
        [SerializeField] private Vector2 dPad;
        [SerializeField] private Vector2 secondary2DAxis;

        private void TestXRInputUsages()
        {
            var trigger = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(left ? XRNode.LeftHand : XRNode.RightHand);

            //Vector2
            trigger.TryGetFeatureValue(CommonUsages.primary2DAxis, out primary2DAxis);
            trigger.TryGetFeatureValue(CommonUsages.dPad, out dPad);
            trigger.TryGetFeatureValue(CommonUsages.secondary2DAxis, out secondary2DAxis);

            //Float
            trigger.TryGetFeatureValue(CommonUsages.indexTouch, out indexTouch);
            trigger.TryGetFeatureValue(CommonUsages.thumbTouch, out thumbTouch);
            trigger.TryGetFeatureValue(CommonUsages.batteryLevel, out batteryLevel);
            trigger.TryGetFeatureValue(CommonUsages.trigger, out this.trigger);
            trigger.TryGetFeatureValue(CommonUsages.grip, out grip);
            trigger.TryGetFeatureValue(CommonUsages.indexFinger, out indexFinger);
            trigger.TryGetFeatureValue(CommonUsages.middleFinger, out middleFinger);
            trigger.TryGetFeatureValue(CommonUsages.ringFinger, out ringFinger);
            trigger.TryGetFeatureValue(CommonUsages.pinkyFinger, out pinkyFinger);

            //Bool
            trigger.TryGetFeatureValue(CommonUsages.isTracked, out isTracked);
            trigger.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButton);
            trigger.TryGetFeatureValue(CommonUsages.primaryTouch, out primaryTouch);
            trigger.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButton);
            trigger.TryGetFeatureValue(CommonUsages.secondaryTouch, out secondaryTouch);
            trigger.TryGetFeatureValue(CommonUsages.gripButton, out gripButton);
            trigger.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButton);
            trigger.TryGetFeatureValue(CommonUsages.menuButton, out menuButton);
            trigger.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out primary2DAxisClick);
            trigger.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out primary2DAxisTouch);
            trigger.TryGetFeatureValue(CommonUsages.thumbrest, out thumbrest);
        }
#endif
        
        #endregion
    }
}


