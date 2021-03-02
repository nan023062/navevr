using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using System.Linq;

namespace NaveXR.InputDevices
{
    public delegate void XRDeviceDelegate(bool successed);

    public delegate void XRDeviceNodeDelegate(XRNode nodeType, string deviceName);

    /// <summary>
    /// 单利XR设备及输入管理器
    /// 1 负责管理设备实例
    /// 2 负责更新设备KeyCode的状态
    /// 3 负责XR设备兼容适配
    /// </summary>
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

        public static string DriverName
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

        public static string deviceName { get { return UnityEngine.XR.XRDevice.model; } }

        public static bool isTouchPad { private set; get; } = false;

        private static void checkTouchPad()
        {
            isTouchPad = leftHandUsage.isPad || rightHandUsage.isPad;
        }

        public static bool isFocus {
            get
            {
                return UnityEngine.XR.XRDevice.userPresence == UserPresenceState.Present;
            }
        }

        #region Main

        private static XRDevice _instance;

        internal static XRDevice GetInstance() {
            CheckSingleton();
            return _instance;
        }

        private static void CheckSingleton()
        {
            if(_instance == null)
            {
                var go = new GameObject("[XInputRDevices]", typeof(XRDevice));
                GameObject.DontDestroyOnLoad(go);
                go.hideFlags = HideFlags.NotEditable;
                _instance = go.GetComponent<XRDevice>();
            }
        }

        private void Awake()
        {
            Debug.Assert(_instance == null, GetType().FullName + ": 单例类被多次实例化！");
            _instance = this;
            gameObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
            InitlizeInputs();
        }

        private void OnEnable()
        {
            UnityEngine.XR.XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
            UnityEngine.XR.XRDevice.deviceLoaded -= OnUnityXRDeviceLoaded;
            UnityEngine.XR.XRDevice.deviceLoaded += OnUnityXRDeviceLoaded;
            checkTouchPad();
        }

        private void Update()
        {
            if(s_driver != null && s_driver.valid){
                s_driver.Update();
                s_driver.CheckDeviceRemovedOrAdded();
                s_driver.UpdateInputDeviceStates();
            }
            UpdateInputStates();
            UpdateInputDeviceAndNodeStatess();
        }

        private void OnDisable()
        {
            UnityEngine.XR.XRDevice.deviceLoaded -= OnUnityXRDeviceLoaded;
        }

        private void OnDestroy()
        {
            ClearInputs();
            s_driver?.Release();
            s_driver = null;
            _instance = null;
            Debug.Log("<b>[NaveXR.XRDevice]</b> disposed !");
        }

        private void OnUnityXRDeviceLoaded(string deviceName)
        {
            if (!XRSettings.enabled) XRSettings.enabled = true;
            Debug.LogFormat(" OnUnityXRDeviceLoaded() :: deviceName = {0}, mode = {1} , isDeviceActive = {2}!!", deviceName, UnityEngine.XR.XRDevice.model, XRSettings.isDeviceActive);
            checkTouchPad();
        }

        #endregion

        #region Required Six XRNodes

        internal static HeadInputNode headUsage = new HeadInputNode();

        internal static HandInputNode leftHandUsage = new HandInputNode(NodeType.LeftHand);

        internal static HandInputNode rightHandUsage = new HandInputNode(NodeType.RightHand);

        internal static InputNode peliveUsage = new InputNode(NodeType.Head);

        internal static InputNode leftFootUsage = new InputNode(NodeType.Head);

        internal static InputNode rightFootUsage = new InputNode(NodeType.Head);

        #endregion

        #region Device InputPlugins

        private static InputPlugin_Base s_driver;

        public static event XRDeviceNodeDelegate onDeviceConnected;

        public static event XRDeviceNodeDelegate onDeviceDisconnected;

        public static event XRDeviceDelegate onInputPluginInitlized;

        /// <summary>
        /// 设置当前输入插件类型
        /// </summary>
        public static void SetCurrentPlugin(InputPlugin inputPlugin)
        {
            if (s_driver != null) s_driver.Release();
            s_driver = null;

            switch (inputPlugin)
            {
                case InputPlugin.Unity_OpenVR:
                    s_driver = new InputPlugin_UnityOpenVR();
                    break;
                case InputPlugin.Unity_Oculus:
                    s_driver = new InputPlugin_UnityOculus();
                    break;
#if SUPPORT_STEAM_VR
                case InputPlugin.SteamVR:
                    s_driver = new InputPlugin_SteamVR();
                    break;
#endif
                case InputPlugin.OpenVR:
                    s_driver = new InputPlugin_OpenVR();
                    break;
                case InputPlugin.Oculus:
                    s_driver = new InputPlugin_Oculus();
                    break;
                default:
                    break;
            }

            if (s_driver != null)
                s_driver.Initlize();
            else
                OnInputPluginInitlized(false);
        }

        internal static void OnInputPluginInitlized(bool successed)
        {
            if (successed)
            {
                Debug.LogFormat("<b>[NaveXR.XRDevice]</b> Successfully Inited Input Plugin [{0}] !", s_driver.name);
                UnityEngine.XR.XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
            }
            else
            {
                Debug.LogErrorFormat("<b>[NaveXR.XRDevice]</b> Failed Init Input Plugin [{0}] !", s_driver.name);
            }
            onInputPluginInitlized?.Invoke(successed);
        }

        internal static void OnXRNodeConnected(InputNode xRNodeUsage)
        {
            XRNode xRNode = XRNode.HardwareTracker;
            if (xRNodeUsage.type == NodeType.Head)
            {
                xRNode = XRNode.Head;
            }
            else if (xRNodeUsage.type == NodeType.LeftHand)
            {
                xRNode = XRNode.LeftHand;
            }
            else if (xRNodeUsage.type == NodeType.RightHand)
            {
                xRNode = XRNode.RightHand;
            }
            else if (xRNodeUsage.type == NodeType.PeliveTrack)
            {

            }
            else if (xRNodeUsage.type == NodeType.LeftFoot)
            {

            }
            else if (xRNodeUsage.type == NodeType.RightFoot)
            {

            }

            Debug.LogFormat("XRDevice.onDeviceConnected... [nodeType={0}，name={1}]", xRNodeUsage.type, xRNodeUsage.name);
            onDeviceConnected?.Invoke(xRNode, xRNodeUsage.name);

            //查找未匹配的设备列表
            var devices = captures.Where((d) => !d.isTracked && d.NodeType == xRNodeUsage.type);
            if (devices != null && devices.Count() > 0)
            {
                foreach (var device in devices) device.Connected(xRNodeUsage);
            }

            checkTouchPad();
        }

        internal static void OnXRNodeDisconnected(InputNode xRNodeUsage)
        {
            XRNode xRNode = XRNode.HardwareTracker;
            if (xRNodeUsage.type == NodeType.Head)
            {
                xRNode = XRNode.Head;
            }
            else if (xRNodeUsage.type == NodeType.LeftHand)
            {
                xRNode = XRNode.LeftHand;
            }
            else if (xRNodeUsage.type == NodeType.RightHand)
            {
                xRNode = XRNode.RightHand;
            }
            else if (xRNodeUsage.type == NodeType.PeliveTrack)
            {

            }
            else if (xRNodeUsage.type == NodeType.LeftFoot)
            {

            }
            else if (xRNodeUsage.type == NodeType.RightFoot)
            {

            }

            onDeviceDisconnected?.Invoke(xRNode, xRNodeUsage.name);
            Debug.LogFormat("XRDevice.onDeviceDisconnected... [nodeType={0}，name={1}]", xRNode, xRNodeUsage.name);

            //查找已匹配的设备列表
            var devices = captures.Where((d) => d.UniqueId == xRNodeUsage.uniqueID && d.NodeType == xRNodeUsage.type);
            if (devices != null && devices.Count() > 0)
            {
                foreach (var device in devices) device.Disconnected();
            }

            checkTouchPad();
        }

#endregion

#region XRDevice Objects

        static List<XRDeviceObject> captures = new List<XRDeviceObject>();

        public static XRDeviceObject GeDeviceCapture(NodeType nodeType)
        {
            if (captures != null && captures.Count > 0)
            {
                for (int i = 0; i < captures.Count; i++)
                {
                    if (captures[i].isTracked && captures[i].NodeType == nodeType)
                    {
                        return captures[i];
                    }
                }
            }
            return null;
        }

        internal static void RegistDevice(XRDeviceObject controller)
        {
            captures.Add(controller);
            InputNode xRNodeUsage = default(InputNode);
            if (controller.NodeType == NodeType.Head) xRNodeUsage = headUsage;
            else if (controller.NodeType == NodeType.LeftHand) xRNodeUsage = leftHandUsage;
            else if (controller.NodeType == NodeType.RightHand) xRNodeUsage = rightHandUsage;
            else if (controller.NodeType == NodeType.PeliveTrack) xRNodeUsage = peliveUsage;
            else if (controller.NodeType == NodeType.LeftFoot) xRNodeUsage = leftFootUsage;
            else if (controller.NodeType == NodeType.RightFoot) xRNodeUsage = rightFootUsage;
            if (xRNodeUsage != null && xRNodeUsage.uniqueID > 0) controller.Connected(xRNodeUsage);
        }

        internal static void UnregistDevice(XRDeviceObject controller)
        {
            captures.Remove(controller);
            controller.Disconnected();
        }

        private static void UpdateInputDeviceAndNodeStatess()
        {
            for (int i = captures.Count - 1; i >= 0; i--)
            {
                var device = captures[i];
                if (!device.isActiveAndEnabled) continue;
                if (device.NodeType == NodeType.Head)
                    device.UpdateInputDeviceAndXRNode(headUsage);
                else if (device.NodeType == NodeType.LeftHand)
                    device.UpdateInputDeviceAndXRNode(leftHandUsage);
                else if (device.NodeType == NodeType.RightHand)
                    device.UpdateInputDeviceAndXRNode(rightHandUsage);
                else if (device.NodeType == NodeType.PeliveTrack)
                    device.UpdateInputDeviceAndXRNode(peliveUsage);
                else if (device.NodeType == NodeType.LeftFoot)
                    device.UpdateInputDeviceAndXRNode(leftFootUsage);
                else if (device.NodeType == NodeType.RightFoot)
                    device.UpdateInputDeviceAndXRNode(rightFootUsage);
            }
        }

#endregion
    }
}