using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;

namespace NaveXR.Device
{
    /// <summary>
    /// 驱动名称
    /// </summary>
    public static class XRDVName
    {
        public readonly static string OpenVR = "OpenVR";
        public readonly static string Oculus = "Oculus";
    }


    public partial class XRDevice : MonoBehaviour
    {
        private static Coroutine co;

        public static bool isEnabled
        {
            get
            {
                CheckSingleton();
                return XRSettings.enabled && !string.IsNullOrEmpty(UnityEngine.XR.XRDevice.model) && !string.IsNullOrEmpty(XRSettings.loadedDeviceName);
            }
        }

        public static string Driver
        {
            get { return XRSettings.loadedDeviceName; }
        }

        public static void TryLoadDrivers(float trySec = 10f)
        {
            CheckSingleton();
            if (co != null) _instance.StopCoroutine(co);
            co = _instance.StartCoroutine(AsyncLoadingDriver(trySec));
        }

        private static IEnumerator AsyncLoadingDriver(float duration)
        {
            float start = Time.realtimeSinceStartup;
            float end = start + ((duration < 0) ? 30F : duration);
            while (Time.realtimeSinceStartup < end && string.IsNullOrEmpty(XRSettings.loadedDeviceName))
            {
                Debug.LogFormat("尝试加载设备驱动...    use time:{0}s", Time.realtimeSinceStartup- start);
                XRSettings.enabled = true;
                XRSettings.LoadDeviceByName(XRSettings.supportedDevices);
                yield return new WaitForSeconds(1f);
            }
            co = null;
        }

        #region Api

        public static XRDeviceUsage headset { private set; get; }

        public static XRDeviceUsage leftHand { private set; get; }

        public static XRDeviceUsage rightHand { private set; get; }

        public static string deviceName { get { return UnityEngine.XR.XRDevice.model; } }

        #endregion

        #region Main

        private static XRDevice _instance;

        private static void CheckSingleton()
        {
            if(_instance == null)
            {
                var go = new GameObject("[XRDevice]", typeof(XRDevice));
                GameObject.DontDestroyOnLoad(go);
                go.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
            }
        }

        private void Awake()
        {
            Debug.Assert(_instance == null, GetType().FullName + ": 单例类被多次实例化！");
            _instance = this;
            InitlizeInputs();
        }

        private void OnEnable()
        {
            UnityEngine.XR.XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);

            UnityEngine.XR.XRDevice.deviceLoaded -= XRDevice_deviceLoaded;
            UnityEngine.XR.XRDevice.deviceLoaded += XRDevice_deviceLoaded;

            //UnityEngine.XR.InputTracking.trackingLost -= InputTracking_trackingLost;
            //UnityEngine.XR.InputTracking.trackingLost += InputTracking_trackingLost;
            //UnityEngine.XR.InputTracking.trackingAcquired -= InputTracking_trackingAcquired;
            //UnityEngine.XR.InputTracking.trackingAcquired += InputTracking_trackingAcquired;
            //UnityEngine.XR.InputTracking.nodeAdded -= InputTracking_nodeAdded;
            //UnityEngine.XR.InputTracking.nodeAdded += InputTracking_nodeAdded;
            //UnityEngine.XR.InputTracking.nodeRemoved -= InputTracking_nodeRemoved;
            //UnityEngine.XR.InputTracking.nodeRemoved += InputTracking_nodeRemoved;
        }

        private void Update()
        {
            CheckInputTrackingRemovedOrAdded();
            UpdateInputDeviceAndNodeStatess();
            UpdateInputStates();
        }

        private void OnDisable()
        {
            xRDeviceUsages?.Clear();
            UnityEngine.XR.XRDevice.deviceLoaded -= XRDevice_deviceLoaded;

            //UnityEngine.XR.InputTracking.trackingLost -= InputTracking_trackingLost;
            //UnityEngine.XR.InputTracking.trackingAcquired -= InputTracking_trackingAcquired;
            //UnityEngine.XR.InputTracking.nodeAdded -= InputTracking_nodeAdded;
            //UnityEngine.XR.InputTracking.nodeRemoved -= InputTracking_nodeRemoved;
        }

        #endregion
    }
}