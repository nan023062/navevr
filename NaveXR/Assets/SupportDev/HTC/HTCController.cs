using UnityEngine;
using System;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;

namespace Nave.VR
{
    /// <summary>
    /// 虚拟控制器对象
    /// </summary>
    public class HTCController : Controller
    {
        protected override void OnDeviceConnected()
        {
            throw new NotImplementedException();
        }

        protected override void OnDeviceDisConnected()
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdate()
        {
        }
    }
}
