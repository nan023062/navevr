using System;
using UnityEngine;
using NaveXR.InputDevices;

namespace Assets.Samples
{
    public class CameraRig : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            NaveXR.InputDevices.XRDevice.SetCurrentPlugin(InputPlugin.Unity_Oculus);
        }

        private void Update()
        {
            if(NaveXR.InputDevices.XRDevice.isEnabled)
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
            NaveXR.InputDevices.XRDevice.TryLoadDrivers(10f);
        }

        private void OnApplicationQuit()
        {

        }
    }
}
