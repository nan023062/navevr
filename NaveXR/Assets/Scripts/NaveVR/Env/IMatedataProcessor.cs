using System;
using System.Collections.Generic;

namespace Nave.VR
{
    public interface IMatedataProcessor 
    {
        bool Running();

        void PreProc();

        void Proc(ref Pose head, ref Pose leftHand, ref Pose rightHand, 
                ref Pose pelive, ref Pose leftFoot, ref Pose rightFoot);

        void PostProc();
    }
}
