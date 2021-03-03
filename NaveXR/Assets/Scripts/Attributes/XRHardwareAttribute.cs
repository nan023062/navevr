using System;
using System.Collections.Generic;

namespace Nave.XR
{
    /// <summary>
    /// 支持的VR硬件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class XRHardwareAttribute : Attribute
    {
        public string[] match;
    }
}
