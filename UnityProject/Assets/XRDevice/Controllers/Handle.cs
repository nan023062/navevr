using UnityEngine;
using System;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;

namespace NaveXR.Device
{
    public class Handle : Controller
    {
        [Header(" Witch Handle ")]
        [SerializeField] private bool Left = true;

        protected override void OnAfterCreate()
        {
            nodeType = Left ? XRNode.LeftHand : XRNode.RightHand;

            //OpenVR
            SetTargetOffset(new Vector3(Left?- 0.003f:0.003f, -0.006f, -0.1f), Quaternion.identity);

            //Oculus
            //SetTargetOffset(Vector3.zero, Quaternion.Euler(-40f, 0, 0));
        }

        protected override void OnBeforeDestroy()
        {
        }

        protected override void OnConnected(XRNodeState nodeState, InputDevice inputDevice)
        {
            //检测手柄模型是够匹配
            TryMathchDeviceModel(nodeState, inputDevice);
        }

        protected override void OnDisconnected()
        {
        }

        protected override void OnUpdate()
        {
#if UNITY_EDITOR
            AdjustTransform();
#endif
        }

        protected override void OnUpdateInputDevice(ref InputDevice inputDevice)
        {
        }

        protected override void OnUpdateTransform(ref XRNodeState nodeState)
        {
        }

        #region Renderer

        [Header("Model && Renderers ")]
        private GameObject model;
        private HandShankModel handShankModel;
        private Renderer[] renderers = null;
        private Dictionary<string, string> deviceToRes = new Dictionary<string, string>()
        {
            {"Default","handShankModel/Prefab/Oculus_rifts_Controller"},
            {"Oculus Rift S","handShankModel/Prefab/Oculus_rifts_Controller"},
            {"oculus_cv1","handShankModel/Prefab/Oculus_cv1_Controller"},
            {"vive","handShankModel/Prefab/HTC_Controller"},
        };

        private void TryMathchDeviceModel(XRNodeState nodeState, InputDevice inputDevice)
        {
            string deviceName = XRDevice.deviceName;
            string handModelRes = deviceToRes["Default"];
            if (!deviceToRes.TryGetValue(deviceName, out handModelRes))
            {
                Debug.LogErrorFormat("没匹配成功指定设备名称！{0}", XRDevice.deviceName);
                return;
            }
            if (model == null || model.name != handModelRes)
            {
                if (model != null) GameObject.DestroyImmediate(model);
                string model_res = handModelRes + (Left ? "_Left" : "_Right");
                var go = Resources.Load<GameObject>(model_res);
                if(go == null)
                {
                    Debug.LogErrorFormat("没找到指定设备的模型资源！{0}", model_res);
                    return;
                }
                model = GameObject.Instantiate(go);
                model.transform.SetParent(transform);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
                model.name = XRDevice.deviceName;
                handShankModel = model.GetComponent<HandShankModel>();

                //模型初始化
                renderers = model.GetComponentsInChildren<Renderer>(true);
                animator = model.GetComponentInChildren<Animator>();



            }
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

        #region Editor Controller Adjust Transform

#if UNITY_EDITOR

        [Header("Editor Controller Adjust Transform ")]
        public GameObject referenceHandle;

        private void AdjustTransform()
        {
            if (referenceHandle == null || model == null) return;
            if(XRDevice.IsButtonDown(Left?0:1,XRKeyCode.Trigger))
            {
                SetTargetOffset(model.transform, referenceHandle.transform);
            }
            if (XRDevice.IsButtonDown(Left ? 0 : 1, XRKeyCode.Secondary))
            {
                SetTargetOffset(Vector3.zero, Quaternion.identity);
            }
        }

#endif

        #endregion
    }
}
