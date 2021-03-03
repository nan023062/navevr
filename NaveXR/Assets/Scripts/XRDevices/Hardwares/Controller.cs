using UnityEngine;
using System;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;

namespace Nave.XR
{
    /// <summary>
    /// 虚拟控制器对象
    /// </summary>
    public class Controller : Hardware
    {
        private bool isLeft = true;

        #region Connected && Asset Loading Async

        private bool m_connected = false;

        private string mDeviceName = string.Empty;

        private string mNextDeviceName = string.Empty;

        private bool mIsLoadingAsset = false;

        public string deviceName { get { return mDeviceName; } }

        public override void OnConnected()
        {
            m_connected = true;

            isLeft = NodeType == NodeType.LeftHand;

            if (mNextDeviceName == device || mDeviceName == device)
            {
                OnRefreshVisiable();
                return;
            }
            if (mIsLoadingAsset)
            {
                mNextDeviceName = device;
            }
            else
            {
                LoadHandResAsync(device);
            }
        }

        public override void OnDisconnected()
        {
            m_connected = false;
            mNextDeviceName = string.Empty;
            OnRefreshVisiable();
        }
        
        private void LoadHandResAsync(string loadDeviceName)
        {
            mDeviceName = loadDeviceName;
            mIsLoadingAsset = true;
            Hardwares.LoadControllerHardwardPrebsAsync(loadDeviceName, OnHandResLoaded);
        }

        private void OnHandResLoaded(GameObject asset)
        {
            //如果有新匹配的手柄，就直接加载
            if (!string.IsNullOrEmpty(mNextDeviceName) && m_connected)
            {
                LoadHandResAsync(mNextDeviceName);
                mNextDeviceName = string.Empty;
            }
            //模型加载完成
            else
            {
                //实例化
                GameObject go = GameObject.Instantiate(asset);
                //go.SetLayerRecursively(LayerMask.NameToLayer("UI"));
                Vector3 positionOff = new Vector3(isLeft ? -0.003f : 0.003f, -0.006f, -0.1f);
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.zero;// -positionOff;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.name = mDeviceName;

                //新模型初始化
                OnInitController(go);
                OnInitLaserPointer(go);
                OnInitControlTips(go);

                OnRefreshVisiable();

                //加载完成标记
                mNextDeviceName = string.Empty;
                mIsLoadingAsset = false;
            }
        }

        private void OnRefreshVisiable()
        {
            m_Controller?.SetVisable(handVisiable && m_connected);
            if (!(m_handVisiable && m_connected)) HideControlTips();
            m_LaserPointer?.SetVisiable(m_laserVisiable && m_connected);
        }

        #endregion

        #region Controller

        private HardwareRefs m_Controller;

        private bool m_handVisiable = true;

        public bool handVisiable { get { return m_handVisiable; } }

        public HardwareRefs controller { get { return m_Controller; } }

        public override void SetVisiable(bool visiable)
        {
            if (handVisiable != visiable)
            {
                m_handVisiable = visiable;
                m_Controller?.SetVisable(m_handVisiable && m_connected);
                if(!(m_handVisiable && m_connected)) HideControlTips();
            }
        }

        private void OnInitController(GameObject go)
        {
            if (m_Controller != null)
            {
                GameObject.DestroyImmediate(m_Controller.gameObject);
                m_Controller = null;
            }
            m_Controller = go.GetComponent<HardwareRefs>();
            if (m_Controller == null)
                m_Controller = go.AddComponent<HardwareRefs>();
            m_Controller.hand = isLeft ? 0 : 1;
        }

        #endregion

        #region Laser Renderer

        private LaserPointer m_LaserPointer;

        public LaserPointer laserPointer { get { return m_LaserPointer; } }

        private bool m_laserVisiable = true;

        public bool laserVisiable { get { return m_laserVisiable; } }

        public void SetLaserVisiable(bool visiable)
        {
            if (laserVisiable != visiable)
            {
                m_laserVisiable = visiable;
                m_LaserPointer?.SetVisiable(m_laserVisiable && m_connected);
            }
        }

        private void OnInitLaserPointer(GameObject go)
        {
            var laser = go.transform.Find("laserpoint").gameObject;
            m_LaserPointer = laser.AddComponent<LaserPointer>();
            m_LaserPointer.inputType = isLeft ? LaserPointer.InputType.LeftHand : LaserPointer.InputType.RightHand;
            m_LaserPointer.SetVisiable(m_laserVisiable);
        }

        #endregion

        #region Control Tips

        private Dictionary<KeyCode, ControlTip> m_KeyControlTips;

        public class ControlTip
        {
            public GameObject gameObject;
            //public Oasis.UI.UIText text;

            public ControlTip(Transform key)
            {

            }

            public void Show(bool active, string msg)
            {
                gameObject.SetActive(active);
                //text.text = msg;
            }
        }

        private void OnInitControlTips(GameObject go)
        {
            //m_KeyControlTips = new Dictionary<XRKeyCode, ControlTip>();

            //var grapTips = new ControlTip(m_Controller.GetKey(XRKeyCode.Grip).transform);
            //m_KeyControlTips.Add(XRKeyCode.Grip, grapTips);

            //var triggerTips = new ControlTip(m_Controller.GetKey(XRKeyCode.Trigger).transform);
            //m_KeyControlTips.Add(XRKeyCode.Trigger, triggerTips);
        }

        public void ShowControlTips(string json_tips)
        {
            if (m_KeyControlTips == null) return;
            //json_tips 格式为多个keyCode=tips格式
            //Debug.LogFormat(" ####### [ {0} ]:ShowControlTips() = {1}", m_Controller.hand, json_tips);
            var tips = JsonUtility.FromJson<Dictionary<string, string>>(json_tips);
            var e = tips.GetEnumerator();
            while (e.MoveNext())
            {
                if(e.Current.Key == "trigger")
                {
                    m_KeyControlTips[KeyCode.Trigger].Show(true, e.Current.Value); 
                }
                else if (e.Current.Key == "grip")
                {
                    m_KeyControlTips[KeyCode.Grip].Show(true, e.Current.Value);
                }
            }
        }

        public void HideControlTips()
        {
            if (m_KeyControlTips == null) return;
            var e = m_KeyControlTips.GetEnumerator();
            while (e.MoveNext()){
                e.Current.Value.Show(false, string.Empty);
            }
        }

        #endregion
    }
}
