using System;
using UnityEngine;
using Nave.VR;

namespace Assets.Samples
{
    /// <summary>
    /// 测试 人形数据校准处理算法
    /// </summary>
    [Serializable]
    public class HumanoidCalibrateProc : MonoBehaviour, IMatedataProcessor
    {
        public bool run = false;

        public void SetAvatarSettings()
        {

        }

        public void PostProc()
        {

        }

        public void PreProc()
        {

        }

        public void Proc(ref Nave.VR.Pose head, ref Nave.VR.Pose leftHand, ref Nave.VR.Pose rightHand, ref Nave.VR.Pose pelive, ref Nave.VR.Pose leftFoot, ref Nave.VR.Pose rightFoot)
        {







        }

        public bool Running()
        {
            return run;
        }
    }
}
