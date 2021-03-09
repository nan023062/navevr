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

    public class CameraRig : TrackingSpace
    {
        [Header("VR运行环境"), SerializeField]
        private Evn evn = Evn.Oculusvr;

        protected override void Awake()
        {
            base.Awake();

            switch (evn)
            {
                case Evn.Oculusvr:
                    NaveVR.InitEvn(typeof(TrackingEvnUnityOculusvr),this);
                    break;
#if SUPPORT_STEAM_VR
                case Evn.Steamvr:
                    NaveVR.InitEvn(typeof(UnitySteamvrEvn),this);
                    break;
#endif
                default:
                    NaveVR.InitEvn(typeof(TrackingEvnUnityOpenvr), this);
                    break;
            }
        }

        protected override void OnPostProcessTrackingAnchors()
        {

        }

        protected override void OnPreProcessTrackingAnchors()
        {
    
        }

        protected override void OnProcessTrackingAnchors()
        {
 
        }

    }
}
