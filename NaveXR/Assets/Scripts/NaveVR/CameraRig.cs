using UnityEngine;
using System.Collections.Generic;

namespace Nave.VR
{

    public enum Evn
    {
        UnityXR = 0,
        Oculusvr = 1,
#if SUPPORT_STEAM_VR
        Steamvr = 2,
#endif
    }

    public class CameraRig : MonoBehaviour
    {
        [Header("数据处理器"), SerializeField]
        private AbstractMetadataProcessor proc;

        [Header("VR运行环境"), SerializeField]
        private Evn evn = Evn.Oculusvr;

        private void Awake()
        {
            switch (evn)
            {
                case Evn.Oculusvr:
                    NaveVR.InitEvn(typeof(UnityOculusvrEvn));
                    break;
#if SUPPORT_STEAM_VR
                case Evn.Steamvr:
                    NaveVR.InitEvn(typeof(UnitySteamvrEvn));
                    break;
#endif
                default:
                    NaveVR.InitEvn(typeof(UnityOpenvrEvn));
                    break;
            }
            NaveVR.SetMetadataProcessor(proc);
        }
    }
}
