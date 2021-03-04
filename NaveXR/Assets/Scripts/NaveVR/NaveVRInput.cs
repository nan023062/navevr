using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nave.VR
{
    public partial class NaveVR : MonoBehaviour
    {
        protected Dictionary<int, HandInputBase[]> m_Inputs;

        HeadInputEye headInputEye;

        private void InitInputs()
        {
            if (m_Inputs == null) {
                m_Inputs = new Dictionary<int, HandInputBase[]>()
                {
                    [(int)KeyCode.Menu] = new HandInputBase[2] { new HandMenu(), new HandMenu() },
                    [(int)KeyCode.Primary] = new HandInputBase[2] { new HandPrimary(), new HandPrimary() },
                    [(int)KeyCode.Secondary] = new HandInputBase[2] { new HandSecondary(), new HandSecondary() },
                    [(int)KeyCode.Trigger] = new HandInputBase[2] { new HandTrigger(), new HandTrigger() },
                    [(int)KeyCode.Grip] = new HandInputBase[2] { new HandGrip(), new HandGrip() },
                    [(int)KeyCode.TouchMiddle] = new HandInputBase[2] { new HandTouchButton(KeyCode.TouchMiddle), new HandTouchButton(KeyCode.TouchMiddle) },
                    [(int)KeyCode.TouchNorth] = new HandInputBase[2] { new HandTouchButton(KeyCode.TouchNorth), new HandTouchButton(KeyCode.TouchNorth) },
                    [(int)KeyCode.TouchWest] = new HandInputBase[2] { new HandTouchButton(KeyCode.TouchWest), new HandTouchButton(KeyCode.TouchWest) },
                    [(int)KeyCode.TouchEast] = new HandInputBase[2] { new HandTouchButton(KeyCode.TouchEast), new HandTouchButton(KeyCode.TouchEast) },
                    [(int)KeyCode.TouchSouth] = new HandInputBase[2] { new HandTouchButton(KeyCode.TouchSouth), new HandTouchButton(KeyCode.TouchSouth) },
                    [(int)KeyCode.TouchAxis] = new HandInputBase[2] { new HandTouchAxis(), new HandTouchAxis() },
                    [(int)KeyCode.HandPose] = new HandInputBase[2] { new HandInputPose(true), new HandInputPose(false) },
                };
            }
            if (headInputEye == null) headInputEye = new HeadInputEye();
        }

        /// <summary>
        /// 标准化的数据状态数据
        /// </summary>
        private void UpdateStandardInputState()
        {
#if UNITY_EDITOR
            TestXRInputUsages();
#endif
            foreach (var inputs in m_Inputs.Values){
                if (leftHandMeta.isValid) inputs[0].UpdateState(leftHandMeta);
                if (rightHandMeta.isValid) inputs[1].UpdateState(rightHandMeta);
            }
            if (headMeta.isValid) headInputEye.UpdateState(headMeta);
        }

        private void ClearInputs() { m_Inputs?.Clear(); }

        internal static T GetHandInputKey<T>(int hand, KeyCode keyCode) where T : HandInputBase
        {
            CheckSingleton();
            if (_instance == null) return null;
            _instance.InitInputs();
            hand = Math.Max(0, Math.Min(1, hand));
            return _instance.m_Inputs[(int)keyCode][hand] as T;
        }

        #region API

        public static float GetKeyForce(int hand, KeyCode keyCode)
        {
            var key = GetHandInputKey<HandInputBase>(hand, keyCode);
            return key.Force;
        }

        public static bool IsKeyTouched(int hand, KeyCode keyCode)
        {
            var key = GetHandInputKey<HandInputBase>(hand, keyCode);
            return key.Touched;
        }

        public static bool IsKeyPressed(int hand, KeyCode keyCode)
        {
            var key = GetHandInputKey<HandInputBase>(hand, keyCode);
            return key.Pressed;
        }

        public static bool IsKeyDown(int hand, KeyCode keyCode)
        {
            var key = GetHandInputKey<HandInputBase>(hand, keyCode);
            return key.KeyDown;
        }

        public static bool IsKeyUp(int hand, KeyCode keyCode)
        {
            var key = GetHandInputKey<HandInputBase>(hand, keyCode);
            return key.KeyUp;
        }

        public static float GetLeftKeyForce(KeyCode keyCode)
        {
            return GetKeyForce(0, keyCode);
        }

        public static bool IsLeftKeyTouched(KeyCode keyCode)
        {
            return IsKeyTouched(0, keyCode);
        }

        public static bool IsLeftKeyPressed(KeyCode keyCode)
        {
            return IsKeyPressed(0, keyCode);
        }

        public static bool IsLeftKeyDown(KeyCode keyCode)
        {
            return IsKeyDown(0, keyCode);
        }

        public static bool IsLeftKeyUp(KeyCode keyCode)
        {
            return IsKeyUp(0, keyCode);
        }

        public static float GetRightKeyForce(KeyCode keyCode)
        {
            return GetKeyForce(1, keyCode);
        }

        public static bool IsRightKeyTouched(KeyCode keyCode)
        {
            return IsKeyTouched(1, keyCode);
        }

        public static bool IsRightKeyPressed(KeyCode keyCode)
        {
            return IsKeyPressed(1, keyCode);
        }

        public static bool IsRightKeyDown(KeyCode keyCode)
        {
            return IsKeyDown(1, keyCode);
        }

        public static bool IsRightKeyUp(KeyCode keyCode)
        {
            return IsKeyUp(1, keyCode);
        }

        public static float GetTouchButtonDegree(int hand, KeyCode keyCode)
        {
            HandTouchButton touch = null;
            switch (keyCode)
            {
                case KeyCode.TouchNorth:
                    touch = GetHandInputKey<HandTouchButton>(hand, keyCode);
                    break;
                case KeyCode.TouchWest:
                    touch = GetHandInputKey<HandTouchButton>(hand, keyCode);
                    break;
                case KeyCode.TouchSouth:
                    touch = GetHandInputKey<HandTouchButton>(hand, keyCode);
                    break;
                case KeyCode.TouchEast:
                    touch = GetHandInputKey<HandTouchButton>(hand, keyCode);
                    break;
            }
            return touch == null ? 0f : touch.degree;
        }

        public static Vector2 GetTouchAxis(int hand)
        {
            var touchInput = GetHandInputKey<HandTouchAxis>(hand, KeyCode.TouchAxis);
            return touchInput.Value2D;
        }

        public static float[] GetHandPos(int hand)
        {
            var handpose = GetHandInputKey<HandInputPose>(hand, KeyCode.HandPose);
            return handpose.handPose_Value;
        }

        public static HandPose_skeleton GetHandPose(int hand)
        {
            var handpose = GetHandInputKey<HandInputPose>(hand, KeyCode.HandPose);
            return handpose.pose;
        }

        public static HandInputPose GetHandInputPose(int hand)
        {
            return GetHandInputKey<HandInputPose>(hand, KeyCode.HandPose);
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

        [SerializeField] private HandMetadata leftHand;

        [SerializeField] private HandMetadata rightHand;

        private void TestXRInputUsages()
        {
            leftHand = NaveVR.leftHandMeta;
            rightHand = NaveVR.rightHandMeta;
        }
#endif
        
        #endregion
    }
}


