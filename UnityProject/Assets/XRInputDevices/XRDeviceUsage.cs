using UnityEngine.XR;
using System.Collections.Generic;

namespace NaveXR.InputDevices
{
    internal class XRDeviceUsage
    {
        public InputDevice InputDevice;
        public XRNodeState nodeState;
        private XRDeviceUsage() { }

        private static List<XRDeviceUsage> __UsagePool = null;

        public static XRDeviceUsage Get(UnityEngine.XR.InputDevice inputDevice, XRNodeState xRNodeState)
        {
            if (__UsagePool == null) __UsagePool = new List<XRDeviceUsage>();

            XRDeviceUsage usage;
            if (__UsagePool.Count == 0)
            {
                usage = new XRDeviceUsage();
            }
            else
            {
                usage = __UsagePool[__UsagePool.Count - 1];
                __UsagePool.RemoveAt(__UsagePool.Count - 1);
            }
            usage.InputDevice = inputDevice;
            usage.nodeState = xRNodeState;
            return usage;
        }
        public static void Put(XRDeviceUsage usage)
        {
            __UsagePool.Add(usage);
        }
    }
}
