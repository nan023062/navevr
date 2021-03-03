using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Nave.XR
{
    public enum SupportXRHardware
    {
        HTCVive,
        OculusRift,
        OculusCV1,
        Knuckles,
        WindowsMR,
    }
    
    [Serializable]
    public class SupportXRControlerInfo : ScriptableObject
    {
        public SupportXRHardware xRControler;
        public string[] hardwardReadableNames;
        public string assetBundleName;
    }

    /// <summary>
    /// 支持的设备名称定义
    /// 不同驱动-名称不一样
    /// </summary>
    internal static class Hardwares
    {
        //头盔
        public const string Hmd_HTCVive = "";
        public const string Hmd_HTCCosmas = "";
        public const string Hmd_OculusRiftS = "";
        public const string Hmd_OculusQuest = "";
        public const string Hmd_ValveIndex = "";

        //手柄
        public const string OpenVR_HtcPad = "";
        public const string OpenVR_ValveIndex = "";
        public const string OpenVR_OculusTouch_L = "OpenVR Controller(Oculus Rift S (Left Controller)) - Left";
        public const string OpenVR_OculusTouch_R = "OpenVR Controller(Oculus Rift S (Right Controller)) - Right";
        public const string Oculus_OculusTouch_L = "Oculus Touch Controller - Left";
        public const string Oculus_OculusTouch_R = "Oculus Touch Controller - Right";

        readonly static Dictionary<string, string> s_HandHardwares = new Dictionary<string, string>()
        {
            ["Knuckles"] = "VRHandles/index",
            ["Vive Controller"] = "VRHandles/htc",
            ["Vive. Controller"] = "VRHandles/htc",
            ["Oculus Quest"] = "VRHandles/oculus_rifts",
            ["Oculus Rift"] = "VRHandles/oculus_rifts",
            ["CV1"] = "VRHandles/oculus_cv1",
            ["WindowsMR"] = "VRHandles/wmr",
        };

        //追踪器
        public const string OpenVR_HtcVive_Tracker = "VIVE Tracker";

        static Dictionary<SupportXRHardware, string> s_SupportXRControlerRes = new Dictionary<SupportXRHardware, string>
        {
            [SupportXRHardware.Knuckles] = "Index",
            [SupportXRHardware.HTCVive] = "HTC",
            [SupportXRHardware.OculusRift] = "Oculus_rifts",
            [SupportXRHardware.OculusCV1] = "Oculus_cv1",
            [SupportXRHardware.WindowsMR] = "WMR",
        };

        static List<SupportXRControlerInfo> S_SupportXRControlerInfos;
        static string S_DefaultHandRes = "OculusTouchForQuestAndRiftS";

        /// <summary>
        /// 注册需要支持的手柄设备：名称-资源映射
        /// </summary>
        public static void RegistHandHardwarePrebs(SupportXRControlerInfo[] configs)
        {
            S_SupportXRControlerInfos = new List<SupportXRControlerInfo>();
            S_SupportXRControlerInfos.AddRange(configs);
        }

        public static string GetControllerHardwarePrebs(string name)
        {
            if(S_SupportXRControlerInfos != null) {
                foreach (var info in S_SupportXRControlerInfos) {
                    var readableNames = info.hardwardReadableNames;
                    foreach (var readableName in readableNames)
                    {
                        if (name.Contains(readableName)) return info.assetBundleName;
                    }
                }
            }
            Debug.LogErrorFormat("警告！没有找到设备{0}的配置资源，默认为 OculusRift !", name);
            return S_DefaultHandRes;
        }

        public static string GetControllerHardwarePrebs(SupportXRHardware hardware)
        {
            if(S_SupportXRControlerInfos != null){
                foreach (var info in S_SupportXRControlerInfos){
                    if (info.xRControler == hardware)
                        return info.assetBundleName;
                }
            }

            Debug.LogErrorFormat("警告！没有找到设备{0}的配置资源，默认为 OculusRift !", hardware);
            return S_DefaultHandRes;
        }

        public static void LoadControllerHardwardPrebsAsync(string name, Action<GameObject> onLoaded)
        {
            bool left = name.Contains("left") || name.Contains("Left");
            string prebs = GetControllerHardwarePrebs(name);
            string prebs_path = left ? prebs + "_Left" : prebs + "_Right";
            onLoaded?.Invoke(Resources.Load<GameObject>(prebs_path));
        }
    }
}
