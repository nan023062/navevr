using System;
using UnityEngine;
using Nave.XR;

namespace Assets.Samples
{
    public class CameraRig : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Nave.XR.XRDevice.InitEvn(typeof(UnityOculusEvn),null);
        }
    }
}
