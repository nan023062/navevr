using System;
using UnityEngine;
using Nave.XR;

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

        public void Proc(ref Nave.XR.Pose head, ref Nave.XR.Pose leftHand, ref Nave.XR.Pose rightHand, ref Nave.XR.Pose pelive, ref Nave.XR.Pose leftFoot, ref Nave.XR.Pose rightFoot)
        {







        }

        public bool Running()
        {
            return run;
        }
    }
}
