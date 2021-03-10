using System;
using System.Collections.Generic;

namespace Nave.VR
{
    /// <summary>
    /// 支持VR运环境
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false)]
    public class XREnvAttribute : Attribute
    {
        /// <summary>
        /// 自定义名称
        /// </summary>
        public string name;

        /// <summary>
        /// 使用的vr 库
        /// </summary>
        public string lib;
    }
}
