using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nave.VR
{
    /// <summary>
    /// 支持的设备名称定义
    /// 不同驱动-名称不一样
    /// </summary>
    internal class Hardwares : ScriptableObject
    {
#if UNITY_EDITOR

        private static string PATH = "Assets/Resources/HardwaresPrefabsDefs.asset";

        [MenuItem("NaveVR/Create Harewares Prefabs Defs Settings")]
        static void CreateAsset()
        {
            AssetDatabase.CreateAsset(new Hardwares(),PATH);
        }
#endif

        ////头盔
        //public const string Hmd_HTCVive = "";
        //public const string Hmd_HTCCosmas = "";
        //public const string Hmd_OculusRiftS = "";
        //public const string Hmd_OculusQuest = "";
        //public const string Hmd_ValveIndex = "";

        ////手柄
        //public const string OpenVR_HtcPad = "";
        //public const string OpenVR_ValveIndex = "";
        //public const string OpenVR_OculusTouch_L = "OpenVR Controller(Oculus Rift S (Left Controller)) - Left";
        //public const string OpenVR_OculusTouch_R = "OpenVR Controller(Oculus Rift S (Right Controller)) - Right";
        //public const string Oculus_OculusTouch_L = "Oculus Touch Controller - Left";
        //public const string Oculus_OculusTouch_R = "Oculus Touch Controller - Right";

        //readonly static Dictionary<string, string> s_HandHardwares = new Dictionary<string, string>()
        //{
        //    ["Knuckles"] = "VRHandles/index",
        //    ["Vive Controller"] = "VRHandles/htc",
        //    ["Vive. Controller"] = "VRHandles/htc",
        //    ["Oculus Quest"] = "VRHandles/oculus_rifts",
        //    ["Oculus Rift"] = "VRHandles/oculus_rifts",
        //    ["CV1"] = "VRHandles/oculus_cv1",
        //    ["WindowsMR"] = "VRHandles/wmr",
        //};

        [SerializeField] Hardware[] hardwarePrefabs;

        public Hardware CreateHardware(TrackingAnchor acnhor)
        {
            string deviceName = acnhor.name;
            Hardware hardware = acnhor.hardware;
            if (hardware != null) {
                if (hardware.TryMathingName(deviceName)) return hardware;
                else GameObject.Destroy(hardware);
            }

            foreach (var prefab in hardwarePrefabs) {
                if(prefab.TryMathingName(deviceName))
                    return GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, acnhor.transform);
            }

            Debug.LogError("ShowHardware() 没有找到匹配的设备！！！就默认使用第一个配置");
            return GameObject.Instantiate(hardwarePrefabs[0], Vector3.zero, Quaternion.identity, acnhor.transform);
        }

        public void DestroyHardware(TrackingAnchor acnhor)
        {
            Hardware hardware = acnhor.hardware;
            if (hardware != null)
                GameObject.Destroy(hardware);
        }
    }
}
