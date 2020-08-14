/********************************************************
 * FileName:    InputNode.cs
 * Description: 输入硬件接口层
 *              1 适配不同的输入设备
 *              2 不同设备会有提供的输入数据
 * History:    
 * ******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaveXR.InputDevices
{
    /// <summary>
    /// 设备节点类型
    /// </summary>
    public enum NodeType
    {
        Head = 1,
        LeftHand = 2,
        RightHand = 3,
        PeliveTrack = 4,
        LeftFoot = 5,
        RightFoot = 6,
    }

    /// <summary>
    /// XR设备输入节点
    /// </summary>
    internal class InputNode
    {
        public readonly NodeType type = 0;

        private ulong m_UniqueId = 0;

        private string m_Name = string.Empty;

        public ulong uniqueID { get { return m_UniqueId; } }

        public string name { get { return m_Name; } }

        public bool isValid { get { return m_UniqueId > 0; } }

        internal InputNode(NodeType type)
        {
            this.type = type;
        }

        internal virtual void OnConnected(ulong uniquedId, string name)
        {
            m_UniqueId = uniquedId;
            m_Name = name;
        }

        internal virtual void OnDisconnect()
        {
            m_UniqueId = 0;
            m_Name = string.Empty;
        }
        public Vector3 position;
        public Quaternion rotation;
    }

    /// <summary>
    /// 头盔数据
    /// </summary>
    internal class HeadInputNode : InputNode
    {
        internal HeadInputNode() :base(NodeType.Head)
        {
            //眼睛数据 eyeData
        }
    }

    /// <summary>
    /// 手柄数据
    /// </summary>
    internal class HandInputNode : InputNode
    {
        public static readonly int fingerBoneNum = 31;

        internal HandInputNode(NodeType type) : base(type)
        {
            //手势数据 valve index
        }

        internal override void OnConnected(ulong uniquedId, string name)
        {
            base.OnConnected(uniquedId, name);
            isPad = name.Contains("Vive") || name.ToLower().Contains("wmr");
        }

        internal override void OnDisconnect()
        {
            base.OnDisconnect();
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
        public float primaryTouchValue;

        //secondary
        public bool secondaryPressed;
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
}
