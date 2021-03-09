using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nave.VR
{
    [XRHardware()]
    public class WMRController : Controller
    {
        protected override void OnUpdate()
        {

        }
        protected override void OnDeviceConnected()
        {
            throw new NotImplementedException();
        }

        protected override void OnDeviceDisConnected()
        {
            throw new NotImplementedException();
        }
    }
}
