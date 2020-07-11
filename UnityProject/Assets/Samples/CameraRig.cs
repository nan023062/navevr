using System;
using UnityEngine;
using NaveXR.Device;

namespace Assets.Samples
{
    public class CameraRig : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if(NaveXR.Device.XRDevice.isEnabled)
            {
                //当前VR模式生效
            }
            else
            {
                //当前为非VR模式
            }
        }

        [ContextMenu("TryLoadDevice")]
        public void TryLoadDevice()
        {
            NaveXR.Device.XRDevice.TryLoadDrivers(10f);
        }

        private void OnApplicationQuit()
        {

        }
    }
}
