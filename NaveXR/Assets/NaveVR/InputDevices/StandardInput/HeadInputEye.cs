using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.XR;

namespace Nave.VR
{
    public class HeadInputEye : BaseInput
    {
        private Eyes mEyes = default(Eyes);
        public Eyes eyes { get { return mEyes; } }

        public HeadInputEye():base(InputKey.Eye)
        {
        }

        internal override void UpdateState(TrackingAnchor xRNodeUsage)
        {

        }
    }
}
