using UnityEngine;
using System;
using UnityEngine.XR;

namespace Nave.VR
{
    public class Headset : Hardware
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
