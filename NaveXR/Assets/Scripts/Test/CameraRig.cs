using System;
using UnityEngine;
using Nave.XR;

namespace Assets.Samples
{
    public class CameraRig : MonoBehaviour
    {
        [Header("人形数据校准处理器")]
        public HumanoidCalibrateProc proc = new HumanoidCalibrateProc();

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Nave.XR.XRDevice.InitEvn(typeof(UnityOculusEvn),null);
            Nave.XR.XRDevice.SetMetaProc(proc);
        }
    }
}
