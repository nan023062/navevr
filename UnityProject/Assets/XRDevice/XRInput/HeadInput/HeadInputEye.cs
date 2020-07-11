using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.XR;

namespace NaveXR.Device
{
    public class HeadInputEye : XRInputBase
    {
        private Eyes mEyes;
        public Eyes eyes { get { return mEyes; } }
        public HeadInputEye():base(XRKeyCode.Eye)
        {
        }
        public override void UpdateState(InputDevice device)
        {
            device.TryGetFeatureValue(CommonUsages.eyesData, out mEyes);
        }
    }
}
