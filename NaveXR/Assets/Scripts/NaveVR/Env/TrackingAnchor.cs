/********************************************************
 * FileName:    MetaData.cs
 * Description: 输入硬件接口层
 *              获取原始数据
 * History:    
 * ******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nave.VR
{
    /// <summary>
    /// 设备节点类型
    /// </summary>
    public enum NodeType
    {
        Head = 1,
        LeftHand = 2,
        RightHand = 3,
        Pelive = 4,
        LeftFoot = 5,
        RightFoot = 6,
    }

    public struct Pose
    {
        public Vector3 position;

        public Quaternion rotation;
    }

    /// <summary>
    /// 原始数据
    /// </summary>
    [System.Serializable]
    public class TrackingAnchor
    {
        public readonly NodeType type = 0;

        [SerializeField] ulong m_UniqueId = 0;

        [SerializeField] string m_Name = string.Empty;

        public Vector3 position = Vector3.zero;

        public Quaternion rotation = Quaternion.identity;

        internal Transform transform { set; get; }

        public Hardware hardware { protected set; get; }

        public ulong uniqueID { get { return m_UniqueId; } }

        public string name { get { return m_Name; } }

        public bool connected { get { return m_UniqueId > 0; } }

        public TrackingAnchor(NodeType type)
        {
            this.type = type;
        }

        internal virtual void Connected(ulong uniquedId, string name)
        {
            m_UniqueId = uniquedId;
            m_Name = name;
        }

        internal virtual void Disconnect()
        {
            m_UniqueId = 0;
            m_Name = string.Empty;
        }

        public Pose GetPose() 
        { 
            return new Pose() { position= position, rotation = rotation }; 
        }

        public void SetPose(Pose pose) 
        { 
            position = pose.position; rotation = pose.rotation; 
        }

        internal void ApplyPoseToTransform()
        {
            if(transform != null) {
                transform.localPosition = position;
                transform.localRotation = rotation;
            }
        }
    }

    /// <summary>
    /// 头盔数据
    /// </summary>
    [System.Serializable]
    public class HeadAnchor : TrackingAnchor
    {
        internal HeadAnchor() :base(NodeType.Head)
        {
            //眼睛数据 eyeData
        }
    }

    /// <summary>
    /// 手柄数据
    /// </summary>
    [System.Serializable]
    public class HandAnchor : TrackingAnchor
    {
        public static readonly int fingerBoneNum = 31;

        public HandAnchor(NodeType type) : base(type)
        {
            //手势数据 valve index
        }

        internal override void Connected(ulong uniquedId, string name)
        {
            base.Connected(uniquedId, name);
            isPad = name.Contains("Vive") || name.ToLower().Contains("wmr");
            hardware = NaveVR.trackingSpace?.hardwarePrefabsDefs.CreateHardware(this);
        }

        internal override void Disconnect()
        {
            base.Disconnect();
            NaveVR.trackingSpace?.hardwarePrefabsDefs.DestroyHardware(this);
            hardware = null;
            isPad = false;
        }

        //ispad or touch
        public bool isPad = false;

        //grip
        public bool gripPressed;
        public float gripTouchValue;

        //trigger
        public bool triggerPressed;
        public float triggerTouchValue;

        //system
        public bool systemPressed;
        public float systemTouchValue;

        //primary
        public bool primaryPressed;
        public bool primaryTouch;
        public float primaryTouchValue;

        //secondary
        public bool secondaryPressed;
        public bool secondaryTouch;
        public float secondaryTouchValue;

        //primary2DAxis
        public bool primary2DAxisTouch;
        public bool primary2DAxisPressed;
        public Vector2 primary2DAxis;

        //secondary2DAxis
        public bool secondary2DAxisPressed;
        public Vector2 secondary2DAxis;

        //hand pose
        public bool handPoseChanged = false;
        public float[] fingerCurls = new float[] { 0, 0, 0, 0, 0 };
    }


    [System.Serializable]
    public class LeftHandAnchor : HandAnchor
    {
        public LeftHandAnchor() : base(NodeType.LeftHand)
        {
        }
    }

    [System.Serializable]
    public class RightHandAnchor : HandAnchor
    {
        public RightHandAnchor() : base(NodeType.RightHand)
        {
        }
    }
}
