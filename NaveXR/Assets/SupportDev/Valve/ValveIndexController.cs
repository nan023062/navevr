using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nave.VR
{
    [XRHardware()]
    public class ValveIndexController : Controller
    {
        protected override void OnUpdate()
        {
            m_Animator.SetFloat("Button 1", InputDevices.GetKeyForce(isLeft ? 0 : 1, InputKey.Primary));
            m_Animator.SetFloat("Button 2", InputDevices.GetKeyForce(isLeft ? 0 : 1, InputKey.Secondary));
            m_Animator.SetFloat("Joy X", InputDevices.GetTouchAxis(isLeft ? 0 : 1).x);
            m_Animator.SetFloat("Joy Y", InputDevices.GetTouchAxis(isLeft ? 0 : 1).y);
            m_Animator.SetFloat("Grip", InputDevices.GetKeyForce(isLeft ? 0 : 1, InputKey.Grip));
            m_Animator.SetFloat("Trigger", InputDevices.GetKeyForce(isLeft ? 0 : 1, InputKey.Trigger));
        }

        protected override void OnDeviceConnected()
        {

        }

        protected override void OnDeviceDisconnected()
        {

        }

    }
}
