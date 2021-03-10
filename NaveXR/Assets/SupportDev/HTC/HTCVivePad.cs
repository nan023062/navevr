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
    public class HTCVivePad : Controller
    {
        protected override void OnDeviceConnected()
        {
            throw new NotImplementedException();
        }

        protected override void OnDeviceDisconnected()
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdate()
        {
        }
    }
}
