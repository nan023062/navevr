using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.XR;
using System.Linq;

namespace Nave.VR
{
    public delegate void TrackingAnchorDelegate(XRNode nodeType, string deviceName);

    public class XRLib
    {
        public const string OpenVR = "OpenVR";

        public const string Oculus = "Oculus";
    }

    public partial class NaveVR : MonoBehaviour
    {
        public static bool isEnabled
        {
            get {
                CheckSingleton();
                return XRSettings.enabled && !string.IsNullOrEmpty(UnityEngine.XR.XRDevice.model) 
                    && !string.IsNullOrEmpty(XRSettings.loadedDeviceName);
            }
        }

        public static string DriverName
        {
            get { return XRSettings.loadedDeviceName; }
        }

        public static string deviceName { get { return UnityEngine.XR.XRDevice.model; } }

        public static bool isTouchPad { private set; get; } = false;

        private static void checkTouchPad()
        {
            isTouchPad = leftHandAnchor.isPad || rightHandAnchor.isPad;
        }

        public static bool isFocus {
            get
            {
                return UnityEngine.XR.XRDevice.userPresence == UserPresenceState.Present;
            }
        }

        public static void GetHandInputOffset(bool left, out Vector3 position, out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;

            if (DriverName == XRLib.OpenVR)
            {
                position = new Vector3(left ? -0.003f : 0.003f, -0.006f, -0.1f);
                rotation = Quaternion.identity;
            }
            else if (DriverName == XRLib.Oculus)
            {
                float tan = Mathf.Tan(40f * Mathf.Deg2Rad);
                float z = -0.034f;
                position = new Vector3(left ? -0.0075f : 0.0075f, z * tan, z);
                rotation = Quaternion.Euler(-40f, 0f, 0f);
            }
        }

        #region Self Log

        public static void Log(string log)
        {
            UnityEngine.Debug.Log($"<b>{typeof(NaveVR).FullName}</b> : {log}");
        }

        public static void LogError(string log)
        {
            UnityEngine.Debug.LogError($"<b>{typeof(NaveVR).FullName}</b> : {log}");
        }

        #endregion

        #region Main

        private static NaveVR _instance;

        internal static NaveVR GetInstance() {
            CheckSingleton();
            return _instance;
        }

        private static void CheckSingleton()
        {
            if(_instance == null) {
                var go = new GameObject("[NaveVR]", typeof(NaveVR));
                GameObject.DontDestroyOnLoad(go);
                go.hideFlags = HideFlags.NotEditable;
                _instance = go.GetComponent<NaveVR>();
            }
        }

        private void Awake()
        {
            Debug.Assert(_instance == null, GetType().FullName + ": 单例类被多次实例化！");
            _instance = this;
            gameObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
            DontDestroyOnLoad(gameObject);
            InitInputs();
        }

        private void OnEnable()
        {
            UnityEngine.XR.XRDevice.deviceLoaded -= OnUnityXRDeviceLoaded;

            UnityEngine.XR.XRDevice.deviceLoaded += OnUnityXRDeviceLoaded;

            checkTouchPad();
        }

        private void Update()
        {
            //根据运行环境 读取源数据
            s_trackingEvn?.UpdateTrackingSpaceData();

            trackingSpace?.ProcessTrackingSpace();

            UpdateStandardInputState();
        }

        private void OnDisable()
        {
            UnityEngine.XR.XRDevice.deviceLoaded -= OnUnityXRDeviceLoaded;
        }

        private void OnDestroy()
        {
            ClearInputs();
            s_trackingEvn?.Release();
            s_trackingEvn = null;
            _instance = null;
            Log("disposed !");
        }

        private void OnUnityXRDeviceLoaded(string device)
        {
            if (!XRSettings.enabled) XRSettings.enabled = true;
            var headDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(XRNode.Head);
            Log($" OnUnityXRDeviceLoaded() device = {device}, name = {headDevice.name} , isActive = {XRSettings.isDeviceActive}!!");
            checkTouchPad();
        }

        #endregion

        #region Tracking Anchors

        internal static TrackingSpace trackingSpace { private set; get; }
        internal static HeadAnchor headAnchor => trackingSpace?.headAnchor;
        internal static HandAnchor leftHandAnchor => trackingSpace?.leftHandAnchor;
        internal static HandAnchor rightHandAnchor => trackingSpace?.rightHandAnchor;
        internal static TrackingAnchor pelivsAnchor => trackingSpace?.pelivsAnchor;
        internal static TrackingAnchor leftFootAnchor => trackingSpace?.leftFootAnchor;
        internal static TrackingAnchor rightFootAnchor => trackingSpace?.rightFootAnchor;
        internal static TrackingAnchor GetAnchor(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Head:
                    return headAnchor;
                case NodeType.LeftHand:
                    return leftHandAnchor;
                case NodeType.RightHand:
                    return rightHandAnchor;
                case NodeType.Pelive:
                    return pelivsAnchor;
                case NodeType.LeftFoot:
                    return leftFootAnchor;
                case NodeType.RightFoot:
                    return rightFootAnchor;
                default:
                    return headAnchor;
            }
        }

        #endregion

        #region Tracking Evn

        private static TrackingEvnBase s_trackingEvn;

        public static event TrackingAnchorDelegate onTrackingAnchorConnected;

        public static event TrackingAnchorDelegate onTrackingAnchorDisconnected;

        public static event System.Action<bool> onTrackingEvnInitlized;

        public static void InitEvn(System.Type evnType, TrackingSpace trackSpace)
        {
            if (s_trackingEvn != null) s_trackingEvn.Release();
            s_trackingEvn = System.Activator.CreateInstance(evnType) as TrackingEvnBase;
            trackingSpace = trackSpace;
            if (s_trackingEvn != null) {
                GetInstance().StartCoroutine(LoadLibAsync((result)=> {
                    if(result) s_trackingEvn.Initlize(OnTrackingEvnInitlized);
                    else OnTrackingEvnInitlized($"InitEvn [{evnType.FullName}]: 驱动加载失败 !");
                }));
            }
            else {
                OnTrackingEvnInitlized($"InitEvn [{evnType.FullName}]: 环境对象创建失败 !");
            }
        }

        private static IEnumerator LoadLibAsync(System.Action<bool> onFinish)
        {
            yield return new WaitForEndOfFrame();

            XRSettings.enabled = true;
            XRSettings.LoadDeviceByName(s_trackingEvn.lib);

            yield return new WaitForEndOfFrame();
            XRSettings.enabled = true;

            yield return null;
            onFinish?.Invoke(XRSettings.enabled);
        }

        private static void OnTrackingEvnInitlized(string error)
        {
            if (string.IsNullOrEmpty(error)) {
                Log($" Successfully Init [{s_trackingEvn.name}] !");
                UnityEngine.XR.XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
                onTrackingEvnInitlized?.Invoke(true);
            }
            else {
                LogError($" Failed Init Evn  error: {error} !");
                onTrackingEvnInitlized?.Invoke(false);
            }
        }

        internal static void OnTrackerConnected(TrackingAnchor anchor)
        {
            XRNode xRNode = XRNode.HardwareTracker;
            if (anchor.type == NodeType.Head)
            {
                xRNode = XRNode.Head;
            }
            else if (anchor.type == NodeType.LeftHand)
            {
                xRNode = XRNode.LeftHand;
            }
            else if (anchor.type == NodeType.RightHand)
            {
                xRNode = XRNode.RightHand;
            }
            else if (anchor.type == NodeType.Pelive)
            {

            }
            else if (anchor.type == NodeType.LeftFoot)
            {

            }
            else if (anchor.type == NodeType.RightFoot)
            {

            }
            checkTouchPad();

            Log($" onTrackerConnected... [type = {anchor.type}，name = {anchor.name}]");
            onTrackingAnchorConnected?.Invoke(xRNode, anchor.name);
        }

        internal static void OnTrackerDisconnected(TrackingAnchor anchor)
        {
            XRNode xRNode = XRNode.HardwareTracker;
            if (anchor.type == NodeType.Head)
            {
                xRNode = XRNode.Head;
            }
            else if (anchor.type == NodeType.LeftHand)
            {
                xRNode = XRNode.LeftHand;
            }
            else if (anchor.type == NodeType.RightHand)
            {
                xRNode = XRNode.RightHand;
            }
            else if (anchor.type == NodeType.Pelive)
            {

            }
            else if (anchor.type == NodeType.LeftFoot)
            {

            }
            else if (anchor.type == NodeType.RightFoot)
            {

            }
            checkTouchPad();

            onTrackingAnchorDisconnected?.Invoke(xRNode, anchor.name);
            Log($" onTrackerDisconnected... [type = {xRNode}，name = {anchor.name}]");
        }

#endregion
    }
}