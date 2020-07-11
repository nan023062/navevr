﻿using UnityEngine;
using UnityEngine.EventSystems;
using NaveXR.InputDevices;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// 射线输入触发点
    /// </summary>
    public class LaserPointer : BaseInputPointer
    {
        public int fingerId { get; private set; }

        private static int s_fingerId = 0;

        public enum InputType
        {
            LeftHand,RightHand,Native,
        }

        public override bool available { get { return true; } }
        public bool isReleased { private set; get; }
        public bool isPressed { private set; get; }

        public InputType inputType = InputType.Native;

        protected override void Awake()
        {
            fingerId = s_fingerId++;
            base.Awake();
            raycastCamera = GetComponent<Camera>();
            if (raycastCamera == null)
                raycastCamera = gameObject.AddComponent<Camera>();
            raycastCamera.enabled = false;
            raycastCamera.cullingMask = 0;
            raycastCamera.farClipPlane = 100f;
            raycastCamera.nearClipPlane = 0f;
            raycastCamera.orthographic = true;
            raycastCamera.orthographicSize = 0.2f;
            raycastCamera.cullingMask = cullingMask;
            raycastCamera.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            XRDevice.Regist(this);
        }

        protected override void OnDisable()
        {
            XRDevice.Remove(this);
            base.OnDisable();
        }

        private void Update()
        {
            if (inputType == InputType.LeftHand)
            {
                isPressed = XRDevice.IsLeftButtonDown(XRKeyCode.Trigger);
                isReleased = XRDevice.IsLeftButtonUp(XRKeyCode.Trigger);
            }
            else if (inputType == InputType.RightHand)
            {
                isPressed = XRDevice.IsRightButtonDown(XRKeyCode.Trigger);
                isReleased = XRDevice.IsRightButtonUp(XRKeyCode.Trigger);
            }
            else if (inputType == InputType.Native)
            {
                if(Input.touchSupported)
                {
                    int count = Input.touchCount;
                    if(count > 0)
                    {
                        Touch touch = Input.GetTouch(0);
                        isPressed = touch.phase == TouchPhase.Began;
                        isReleased = touch.phase == TouchPhase.Ended;
                    }
                    else
                    {
                        isPressed = false;
                        isReleased = false;
                    }
                }
                else
                {
                    isPressed = Input.GetMouseButtonDown(0);
                    isReleased = Input.GetMouseButtonUp(0);
                }
            }
        }
    }
}
