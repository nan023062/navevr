using UnityEngine;
using System.Collections.Generic;

namespace Nave.VR
{

    public enum Evn
    {
        None = -1,
        UnityXR = 0,
#if NAVEVR_OCULUSVR
        Oculusvr = 1,
#endif

#if NAVEVR_STEAMVR
        Steamvr = 2,
#endif
    }

    public class CameraRig : TrackingSpace
    {
        [Header("VR运行环境"), SerializeField]
        private Evn evn = Evn.UnityXR;

        protected override void Awake()
        {
            base.Awake();

            switch (evn)
            {
#if NAVEVR_OCULUSVR
                case Evn.Oculusvr:
                    InputDevices.InitEvn(typeof(TrackingEvnUnityOculusvr),this);
                    break;
#endif

#if NAVEVR_STEAMVR
                case Evn.Steamvr:
                    NaveVR.InitEvn(typeof(UnitySteamvrEvn),this);
                    break;
#endif
                case Evn.UnityXR:
                    InputDevices.InitEvn(typeof(TrackingEvnUnityOpenvr), this);
                    break;
                default:
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
