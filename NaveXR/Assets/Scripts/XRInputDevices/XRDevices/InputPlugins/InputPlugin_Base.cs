/********************************************************
 * FileName:    Driver_Base.cs
 * Description: 设备输入插件类-抽象接口
 *              1 适配不同的输入库类型
 *              2 获取XR输入需要的数据
 * History:    
 * ******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// 输入支持类型
    /// </summary>
    public enum InputPlugin
    {
        None,

        /// <summary>
        /// UnityEngine.XR库 + OpenVR支持
        /// </summary>
        Unity_OpenVR,

        /// <summary>
        /// UnityEngine.XR库 + Oculus支持
        /// </summary>
        Unity_Oculus,

        /// <summary>
        /// SteamVR插件持支持
        /// </summary>
        SteamVR,

        /// <summary>
        /// OpenVR原生库支持
        /// </summary>
        OpenVR,

        /// <summary>
        /// Oculus原生库支持
        /// </summary>
        Oculus,
    }

    internal abstract class InputPlugin_Base
    {
        public abstract InputPlugin name { get; }

        public abstract string driver { get; }

        public bool valid { get; private set; } = false;

#region State Methods

        internal abstract void Initlize();

        internal virtual void Update() { }

        internal abstract void CheckDeviceRemovedOrAdded();

        internal abstract void UpdateInputDeviceStates();

        internal abstract void Release();

        protected void OnInitlized(bool successed)
        {
            XRDevice.OnInputPluginInitlized(successed);
            valid = successed;
        }

        protected void OnDeviceConnnected(InputNode usage)
        {
            XRDevice.OnXRNodeConnected(usage);
        }

        protected void OnDeviceDisconnnected(InputNode usage)
        {
            XRDevice.OnXRNodeDisconnected(usage);
        }

#endregion
    }
}
