using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// 实现 OpenVR 源生库支持
    /// </summary>
    internal class InputPlugin_OpenVR : InputPlugin_Base
    {
        public override string driver { get { return DRVName.OpenVR; } }

        public override InputPlugin name { get { return InputPlugin.OpenVR; } }

        internal override void CheckDeviceRemovedOrAdded()
        {
            throw new System.NotImplementedException();
        }

        internal override void Initlize()
        {
            throw new System.NotImplementedException();
        }

        internal override void Release()
        {
            throw new System.NotImplementedException();
        }

        internal override void UpdateInputDeviceStates()
        {
            throw new System.NotImplementedException();
        }
    }
}
