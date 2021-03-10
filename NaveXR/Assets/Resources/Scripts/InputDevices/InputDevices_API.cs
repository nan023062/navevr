using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nave.VR
{
    public partial class InputDevices : MonoBehaviour
    {
        private Dictionary<int, HandInputBase[]> m_Inputs = new Dictionary<int, HandInputBase[]>()
        {
            [(int)InputKey.Menu] = new HandInputBase[2] { new HandMenu(), new HandMenu() },
            [(int)InputKey.Primary] = new HandInputBase[2] { new HandPrimary(), new HandPrimary() },
            [(int)InputKey.Secondary] = new HandInputBase[2] { new HandSecondary(), new HandSecondary() },
            [(int)InputKey.Trigger] = new HandInputBase[2] { new HandTrigger(), new HandTrigger() },
            [(int)InputKey.Grip] = new HandInputBase[2] { new HandGrip(), new HandGrip() },
            [(int)InputKey.Middle] = new HandInputBase[2] { new HandTouchButton(InputKey.Middle), new HandTouchButton(InputKey.Middle) },
            [(int)InputKey.North] = new HandInputBase[2] { new HandTouchButton(InputKey.North), new HandTouchButton(InputKey.North) },
            [(int)InputKey.West] = new HandInputBase[2] { new HandTouchButton(InputKey.West), new HandTouchButton(InputKey.West) },
            [(int)InputKey.East] = new HandInputBase[2] { new HandTouchButton(InputKey.East), new HandTouchButton(InputKey.East) },
            [(int)InputKey.South] = new HandInputBase[2] { new HandTouchButton(InputKey.South), new HandTouchButton(InputKey.South) },
            [(int)InputKey.Axis] = new HandInputBase[2] { new HandTouchAxis(), new HandTouchAxis() },
            [(int)InputKey.Pose] = new HandInputBase[2] { new HandInputPose(true), new HandInputPose(false) },
        };

        private HeadInputEye m_HeadInputEye = new HeadInputEye();

        /// <summary>
        /// 标准化的数据状态数据
        /// </summary>
        private void UpdateStandardInputState()
        {
#if UNITY_EDITOR
            TestInputUsages();
#endif
            if(leftHandAnchor!= null && rightHandAnchor!=null && headAnchor != null) {
                foreach (var inputs in m_Inputs.Values){
                    if (leftHandAnchor.connected) inputs[0].UpdateState(leftHandAnchor);
                    if (rightHandAnchor.connected) inputs[1].UpdateState(rightHandAnchor);
                }
                if (headAnchor.connected) m_HeadInputEye.UpdateState(headAnchor);
            }
        }
            
        internal static T GetHandInput<T>(int hand, InputKey keyCode) where T : HandInputBase
        {
            CheckSingleton();
            if (_instance == null) return null;
            hand = Math.Max(0, Math.Min(1, hand));
            return _instance.m_Inputs[(int)keyCode][hand] as T;
        }

        #region API

        public static float GetKeyForce(int hand, InputKey keyCode)
        {
            var key = GetHandInput<HandInputBase>(hand, keyCode);
            return key.Force;
        }

        public static bool IsKeyTouched(int hand, InputKey keyCode)
        {
            var key = GetHandInput<HandInputBase>(hand, keyCode);
            return key.Touched;
        }

        public static bool IsKeyPressed(int hand, InputKey keyCode)
        {
            var key = GetHandInput<HandInputBase>(hand, keyCode);
            return key.Pressed;
        }

        public static bool IsKeyDown(int hand, InputKey keyCode)
        {
            var key = GetHandInput<HandInputBase>(hand, keyCode);
            return key.KeyDown;
        }

        public static bool IsKeyUp(int hand, InputKey keyCode)
        {
            var key = GetHandInput<HandInputBase>(hand, keyCode);
            return key.KeyUp;
        }

        public static float GetLeftKeyForce(InputKey keyCode)
        {
            return GetKeyForce(0, keyCode);
        }

        public static bool IsLeftKeyTouched(InputKey keyCode)
        {
            return IsKeyTouched(0, keyCode);
        }

        public static bool IsLeftKeyPressed(InputKey keyCode)
        {
            return IsKeyPressed(0, keyCode);
        }

        public static bool IsLeftKeyDown(InputKey keyCode)
        {
            return IsKeyDown(0, keyCode);
        }

        public static bool IsLeftKeyUp(InputKey keyCode)
        {
            return IsKeyUp(0, keyCode);
        }

        public static float GetRightKeyForce(InputKey keyCode)
        {
            return GetKeyForce(1, keyCode);
        }

        public static bool IsRightKeyTouched(InputKey keyCode)
        {
            return IsKeyTouched(1, keyCode);
        }

        public static bool IsRightKeyPressed(InputKey keyCode)
        {
            return IsKeyPressed(1, keyCode);
        }

        public static bool IsRightKeyDown(InputKey keyCode)
        {
            return IsKeyDown(1, keyCode);
        }

        public static bool IsRightKeyUp(InputKey keyCode)
        {
            return IsKeyUp(1, keyCode);
        }

        public static float GetTouchButtonDegree(int hand, InputKey keyCode)
        {
            HandTouchButton touch = null;
            switch (keyCode)
            {
                case InputKey.North:
                    touch = GetHandInput<HandTouchButton>(hand, keyCode);
                    break;
                case InputKey.West:
                    touch = GetHandInput<HandTouchButton>(hand, keyCode);
                    break;
                case InputKey.South:
                    touch = GetHandInput<HandTouchButton>(hand, keyCode);
                    break;
                case InputKey.East:
                    touch = GetHandInput<HandTouchButton>(hand, keyCode);
                    break;
            }
            return touch == null ? 0f : touch.degree;
        }

        public static Vector2 GetTouchAxis(int hand)
        {
            var touchInput = GetHandInput<HandTouchAxis>(hand, InputKey.Axis);
            return touchInput.Value2D;
        }

        public static HandInputPose GetHandPose(int hand)
        {
            return GetHandInput<HandInputPose>(hand, InputKey.Pose);
        }

        #endregion

        #region InputUsages

#if UNITY_EDITOR

        [SerializeField] private HandAnchor leftHand;

        [SerializeField] private HandAnchor rightHand;

        private void TestInputUsages()
        {
            leftHand = InputDevices.leftHandAnchor;
            rightHand = InputDevices.rightHandAnchor;
        }
#endif
        
        #endregion
    }
}


