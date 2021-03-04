using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.XR;
using System.Linq;

namespace Nave.XR
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

        public static string deviceName { get { return UnityEngine.XR.XRDevice.model; } }

        public static bool isTouchPad { private set; get; } = false;

        private static void checkTouchPad()
        {
            isTouchPad = leftHandMeta.isPad || rightHandMeta.isPad;
        }

        public static bool isFocus {
            get
            {
                return UnityEngine.XR.XRDevice.userPresence == UserPresenceState.Present;
            }
        }

        #region Self Log

        public static void Log(string log)
        {
            UnityEngine.Debug.Log($"<b>{typeof(XRDevice).FullName}</b> : {log}");
        }

        public static void LogError(string log)
        {
            UnityEngine.Debug.LogError($"<b>{typeof(XRDevice).FullName}</b> : {log}");
        }

        #endregion

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
            if(s_evn != null && s_evn.valid){
                s_evn.Update();
                s_evn.CheckDeviceRemovedOrAdded();
                s_evn.UpdateInputDeviceStates();
            }
            ProcMetaPose();
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
            s_evn?.Release();
            s_evn = null;
            s_proc = null;
            _instance = null;
            Log("disposed !");
        }

        private void OnUnityXRDeviceLoaded(string deviceLib)
        {
            if (!XRSettings.enabled) XRSettings.enabled = true;
            var headDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(XRNode.Head);
            Log($" OnUnityXRDeviceLoaded() lib = {deviceLib}, device = {headDevice.name} , isActive = {XRSettings.isDeviceActive}!!");
            checkTouchPad();
        }

        #endregion

        #region Matedata

        internal static HeadMetadata headMeta = new HeadMetadata();

        internal static HandMetadata leftHandMeta = new HandMetadata(NodeType.LeftHand);

        internal static HandMetadata rightHandMeta = new HandMetadata(NodeType.RightHand);

        internal static Metadata pelivMeta = new Metadata(NodeType.Head);

        internal static Metadata leftFootMeta = new Metadata(NodeType.Head);

        internal static Metadata rightFootMeta = new Metadata(NodeType.Head);

        private static IMatedataProcessor s_proc;

        private static void ProcMetaPose()
        {
            if (s_proc == null || !s_proc.Running()) return;

            s_proc.PreProc();

            var head = headMeta.GetPose();

            var leftHand = leftHandMeta.GetPose();

            var rightHand = rightHandMeta.GetPose();

            var pelive = pelivMeta.GetPose();

            var leftFoot = leftFootMeta.GetPose();

            var rightFoot = rightFootMeta.GetPose();

            s_proc.Proc(ref head, ref leftHand, ref rightHand, ref pelive, ref leftFoot, ref rightFoot);

            headMeta.SetPose(head);

            leftHandMeta.SetPose(leftHand);

            rightHandMeta.SetPose(rightHand);

            pelivMeta.SetPose(pelive);

            leftFootMeta.SetPose(leftFoot);

            rightFootMeta.SetPose(rightFoot);

            s_proc.PostProc();
        }

        public static void SetMetaProc<T>(T proc) where T : IMatedataProcessor
        {
            s_proc = proc;
        }

        #endregion

        #region Device Evn

        private static BaseEvn s_evn;
        
        public static event XRDeviceNodeDelegate onDeviceConnected;

        public static event XRDeviceNodeDelegate onDeviceDisconnected;

        public static event XRDeviceDelegate onInputPluginInitlized;

        /// <summary>
        /// 设置当前运行环境
        /// </summary>
        public static void InitEvn(System.Type evnType, System.Type[] supportHardwares)
        {
            s_evn = System.Activator.CreateInstance(evnType) as BaseEvn;
            if (s_evn != null) {
                GetInstance().StartCoroutine(LoadLibAsync((result)=> {
                    if(result) s_evn.Initlize(OnInputPluginInitlized);
                    else OnInputPluginInitlized("InitEvn: 驱动加载失败 !");
                }));
            }
            else {
                OnInputPluginInitlized("InitEvn: 环境对象创建失败 !");
            }
        }


        private static IEnumerator LoadLibAsync(System.Action<bool> onFinish)
        {
            yield return new WaitForEndOfFrame();

            XRSettings.enabled = true;
            XRSettings.LoadDeviceByName(s_evn.lib);

            yield return new WaitForEndOfFrame();
            XRSettings.enabled = true;

            yield return null;
            onFinish?.Invoke(XRSettings.enabled);
        }

        private static void OnInputPluginInitlized(string error)
        {
            if (string.IsNullOrEmpty(error)) {
                Log($" Successfully Init [{s_evn.name}] !");
                UnityEngine.XR.XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
                XRInputSubsystem xRInput = new XRInputSubsystem();
                onInputPluginInitlized?.Invoke(true);
            }
            else {
                LogError($" Failed Init Evn  error: {error} !");
                onInputPluginInitlized?.Invoke(false);
            }
        }

        internal static void OnXRNodeConnected(Metadata metadata)
        {
            XRNode xRNode = XRNode.HardwareTracker;
            if (metadata.type == NodeType.Head)
            {
                xRNode = XRNode.Head;
            }
            else if (metadata.type == NodeType.LeftHand)
            {
                xRNode = XRNode.LeftHand;
            }
            else if (metadata.type == NodeType.RightHand)
            {
                xRNode = XRNode.RightHand;
            }
            else if (metadata.type == NodeType.PeliveTrack)
            {

            }
            else if (metadata.type == NodeType.LeftFoot)
            {

            }
            else if (metadata.type == NodeType.RightFoot)
            {

            }

            Log($" onDeviceConnected... [type = {metadata.type}，name = {metadata.name}]");
            onDeviceConnected?.Invoke(xRNode, metadata.name);

            //查找未匹配的设备列表
            var devices = captures.Where((d) => !d.isTracked && d.NodeType == metadata.type);
            if (devices != null && devices.Count() > 0) {
                foreach (var device in devices) device.Connected(metadata);
            }

            checkTouchPad();
        }

        internal static void OnXRNodeDisconnected(Metadata metadata)
        {
            XRNode xRNode = XRNode.HardwareTracker;
            if (metadata.type == NodeType.Head)
            {
                xRNode = XRNode.Head;
            }
            else if (metadata.type == NodeType.LeftHand)
            {
                xRNode = XRNode.LeftHand;
            }
            else if (metadata.type == NodeType.RightHand)
            {
                xRNode = XRNode.RightHand;
            }
            else if (metadata.type == NodeType.PeliveTrack)
            {

            }
            else if (metadata.type == NodeType.LeftFoot)
            {

            }
            else if (metadata.type == NodeType.RightFoot)
            {

            }

            onDeviceDisconnected?.Invoke(xRNode, metadata.name);
            Log($" onDeviceDisconnected... [type = {xRNode}，name = {metadata.name}]");

            //查找已匹配的设备列表
            var devices = captures.Where((d) => d.UniqueId == metadata.uniqueID && d.NodeType == metadata.type);
            if (devices != null && devices.Count() > 0) {
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
            Metadata xRNodeUsage = default(Metadata);
            if (controller.NodeType == NodeType.Head) xRNodeUsage = headMeta;
            else if (controller.NodeType == NodeType.LeftHand) xRNodeUsage = leftHandMeta;
            else if (controller.NodeType == NodeType.RightHand) xRNodeUsage = rightHandMeta;
            else if (controller.NodeType == NodeType.PeliveTrack) xRNodeUsage = pelivMeta;
            else if (controller.NodeType == NodeType.LeftFoot) xRNodeUsage = leftFootMeta;
            else if (controller.NodeType == NodeType.RightFoot) xRNodeUsage = rightFootMeta;
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
                    device.UpdateInputDeviceAndXRNode(headMeta);
                else if (device.NodeType == NodeType.LeftHand)
                    device.UpdateInputDeviceAndXRNode(leftHandMeta);
                else if (device.NodeType == NodeType.RightHand)
                    device.UpdateInputDeviceAndXRNode(rightHandMeta);
                else if (device.NodeType == NodeType.PeliveTrack)
                    device.UpdateInputDeviceAndXRNode(pelivMeta);
                else if (device.NodeType == NodeType.LeftFoot)
                    device.UpdateInputDeviceAndXRNode(leftFootMeta);
                else if (device.NodeType == NodeType.RightFoot)
                    device.UpdateInputDeviceAndXRNode(rightFootMeta);
            }
        }

#endregion
    }
}