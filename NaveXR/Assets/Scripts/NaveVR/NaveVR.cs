using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.XR;
using System.Linq;

namespace Nave.VR
{
    public delegate void XRDeviceDelegate(bool successed);

    public delegate void XRDeviceNodeDelegate(XRNode nodeType, string deviceName);

    public class XRLib
    {
        public const string OpenVR = "OpenVR";

        public const string Oculus = "Oculus";
    }

    public partial class NaveVR : MonoBehaviour
    {
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
            if(_instance == null)
            {
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
            UnityEngine.XR.XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
            UnityEngine.XR.XRDevice.deviceLoaded -= OnUnityXRDeviceLoaded;
            UnityEngine.XR.XRDevice.deviceLoaded += OnUnityXRDeviceLoaded;
            checkTouchPad();
        }

        private void Update()
        {
            //根据运行环境 读取源数据
            s_evn?.UpdateAllControllerState();

            ProcMetaPose();

            UpdateStandardInputState();
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

        #region Matedata

        internal static HeadMetadata headMeta = new HeadMetadata();

        internal static HandMetadata leftHandMeta = new HandMetadata(NodeType.LeftHand);

        internal static HandMetadata rightHandMeta = new HandMetadata(NodeType.RightHand);

        internal static Metadata pelivMeta = new Metadata(NodeType.Head);

        internal static Metadata leftFootMeta = new Metadata(NodeType.Head);

        internal static Metadata rightFootMeta = new Metadata(NodeType.Head);

        internal static IMatedataProcessor s_proc;

        internal static Metadata GetMetaDara(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Head:
                    return headMeta;
                case NodeType.LeftHand:
                    return leftHandMeta;
                case NodeType.RightHand:
                    return rightHandMeta;
                case NodeType.Pelive:
                    return pelivMeta;
                case NodeType.LeftFoot:
                    return leftFootMeta;
                case NodeType.RightFoot:
                    return rightFootMeta;
                default:
                    return headMeta;
            }
        }

        public static void SetMetadataProcessor(AbstractMetadataProcessor proc)
        {
            s_proc = proc;
        }

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

        #endregion

        #region Device Evn

        private static BaseEvn s_evn;
        
        public static event XRDeviceNodeDelegate onDeviceConnected;

        public static event XRDeviceNodeDelegate onDeviceDisconnected;

        public static event XRDeviceDelegate onInputPluginInitlized;

        /// <summary>
        /// 设置当前运行环境
        /// </summary>
        public static void InitEvn(System.Type evnType)
        {
            if (s_evn != null) s_evn.Release();
            s_evn = System.Activator.CreateInstance(evnType) as BaseEvn;
            if (s_evn != null) {
                GetInstance().StartCoroutine(LoadLibAsync((result)=> {
                    if(result) s_evn.Initlize(OnInputPluginInitlized);
                    else OnInputPluginInitlized($"InitEvn [{evnType.FullName}]: 驱动加载失败 !");
                }));
            }
            else {
                OnInputPluginInitlized($"InitEvn [{evnType.FullName}]: 环境对象创建失败 !");
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

        internal static void OnHardwardConnected(Metadata metadata)
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
            else if (metadata.type == NodeType.Pelive)
            {

            }
            else if (metadata.type == NodeType.LeftFoot)
            {

            }
            else if (metadata.type == NodeType.RightFoot)
            {

            }

            Log($" onHardwardConnected... [type = {metadata.type}，name = {metadata.name}]");
            onDeviceConnected?.Invoke(xRNode, metadata.name);

            //查找未匹配的设备列表
            var devices = s_Hardwares.Where((d) => !d.isTracked && d.NodeType == metadata.type);
            if (devices != null && devices.Count() > 0) {
                foreach (var device in devices) device.Connected(metadata);
            }

            checkTouchPad();
        }

        internal static void OnHardwardDisconnected(Metadata metadata)
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
            else if (metadata.type == NodeType.Pelive)
            {

            }
            else if (metadata.type == NodeType.LeftFoot)
            {

            }
            else if (metadata.type == NodeType.RightFoot)
            {

            }

            onDeviceDisconnected?.Invoke(xRNode, metadata.name);
            Log($" onHardwardDisconnected... [type = {xRNode}，name = {metadata.name}]");

            //查找已匹配的设备列表
            var devices = s_Hardwares.Where((d) => d.UniqueId == metadata.uniqueID && d.NodeType == metadata.type);
            if (devices != null && devices.Count() > 0) {
                foreach (var device in devices) device.Disconnected();
            }

            checkTouchPad();
        }

#endregion

        #region Hardware Listeners

        static List<HardwareListener> s_Hardwares = new List<HardwareListener>();

        /// <summary>
        /// 注册一个虚拟设备
        /// </summary>
        internal static void RegistHardware(HardwareListener hardware)
        {
            s_Hardwares.Add(hardware);
            Metadata metadata = default(Metadata);
            if (hardware.NodeType == NodeType.Head) metadata = headMeta;
            else if (hardware.NodeType == NodeType.LeftHand) metadata = leftHandMeta;
            else if (hardware.NodeType == NodeType.RightHand) metadata = rightHandMeta;
            else if (hardware.NodeType == NodeType.Pelive) metadata = pelivMeta;
            else if (hardware.NodeType == NodeType.LeftFoot) metadata = leftFootMeta;
            else if (hardware.NodeType == NodeType.RightFoot) metadata = rightFootMeta;
            if (metadata != null && metadata.uniqueID > 0) hardware.Connected(metadata);
        }

        /// <summary>
        /// 注销一个虚拟设备
        /// </summary>
        internal static void UnregistHardware(HardwareListener hardware)
        {
            s_Hardwares.Remove(hardware);
            hardware.Disconnected();
        }

        #endregion
    }
}