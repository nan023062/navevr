using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nave.VR
{
    [XRHardware()]
    public class OculusCV1Controller : Controller
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
