using UnityEngine;
using System;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;

namespace NaveXR.InputDevices
{
    public class Handle : Controller
    {
        internal override void SetVisiable(bool visiable)
        {
           
        }

        internal override void OnConnected(XRNodeState nodeState, UnityEngine.XR.InputDevice inputDevice)
        {
            //检测手柄模型是够匹配
            MathchDeviceModel(nodeState, inputDevice);
        }

        internal override void OnDisconnected()
        {
        }

        #region Renderer

        [Header("Model && Renderers ")]
        private GameObject model;
        private HandShankModel handShankModel;
        private Renderer[] renderers = null;

        private void MathchDeviceModel(XRNodeState nodeState, UnityEngine.XR.InputDevice inputDevice)
        {
            return;
            //if (model == null || model.name != XRDevice.deviceName)
            //{
            //    GameObject go = LoadAndInitModel(nodeState.nodeType == XRNode.LeftHand);
            //    if (model != null) GameObject.DestroyImmediate(model);
            //    model = GameObject.Instantiate(go);
            //    model.transform.SetParent(transform);
            //    model.transform.localPosition = Vector3.zero;
            //    model.transform.localRotation = Quaternion.identity;
            //    model.transform.localScale = Vector3.one;
            //    model.name = XRDevice.deviceName;
            //    handShankModel = model.GetComponent<HandShankModel>();

            //    //模型初始化
            //    renderers = model.GetComponentsInChildren<Renderer>(true);
            //    animator = model.GetComponentInChildren<Animator>();
            //}
        }

        #endregion

        #region Animations

        [Header("Animations ")]
        [SerializeField]
        private Animator animator;

        public void OnTrigger(string json_args)
        {
            if (animator == null) return;
            var args = JsonUtility.FromJson<Dictionary<string, float>>(json_args);
            foreach (var kv in args) animator.SetFloat(kv.Key, kv.Value);
        }

        #endregion

        #region 模型资源加载--当前项目逻辑

        private Dictionary<string, string> deviceToRes = new Dictionary<string, string>()
        {
            {"Default","VRHandles/oculus_rifts"},
            {"Oculus Rift S","VRHandles/oculus_rifts"},
            {"oculus_cv1","VRHandles/oculus_cv1"},
            {"vive","VRHandles/htc_controller"},
        };

        //private GameObject LoadAndInitModel(bool left)
        //{
        //    string bundleName = deviceToRes["Default"];
        //    if (!deviceToRes.TryGetValue(XRDevice.deviceName, out bundleName))
        //        Debug.LogWarningFormat("没匹配成功指定设备名称！{0}", XRDevice.deviceName);

        //    string assetName = left ? "Left" : "Right";
        //    //var asset = OasisAsset.Task(bundleName, assetName).Method(ZFrame.Asset.LoadMethod.Forever).Sync<GameObject>();
        //    if (asset == null)
        //    {
        //        Debug.LogErrorFormat("没找到指定设备的模型资源！{0}", bundleName);
        //        return null;
        //    }
        //    return asset;
        //}

        #endregion
    }
}
