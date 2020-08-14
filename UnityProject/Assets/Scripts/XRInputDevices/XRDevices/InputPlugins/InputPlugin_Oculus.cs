using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// 实现 Oculus 源生库支持
    /// </summary>
    internal class InputPlugin_Oculus : InputPlugin_Base
    {
        public override string driver { get { return DRVName.Oculus; } }

        public override InputPlugin name { get { return InputPlugin.Oculus; } }

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
