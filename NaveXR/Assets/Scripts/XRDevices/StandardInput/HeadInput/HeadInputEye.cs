using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.XR;

namespace Nave.XR
{
    public class HeadInputEye : BaseInput
    {
        private Eyes mEyes;
        public Eyes eyes { get { return mEyes; } }

        public HeadInputEye():base(KeyCode.Eye)
        {
        }

        internal override void UpdateState(Metadata xRNodeUsage)
        {

        }
    }
}
