using System;
using System.Collections.Generic;

namespace Nave.VR
{
    public abstract class AbstractMetadataProcessor : UnityEngine.MonoBehaviour, IMatedataProcessor
    {
        public abstract bool Running();

        public abstract void PreProc();

        public abstract void Proc(ref Pose head, ref Pose leftHand, ref Pose rightHand, ref Pose pelive, ref Pose leftFoot, ref Pose rightFoot);
        
        public abstract void PostProc();
    }
}
