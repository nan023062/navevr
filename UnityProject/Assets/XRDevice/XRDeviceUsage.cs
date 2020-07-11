using UnityEngine.XR;
using System.Collections.Generic;

namespace NaveXR.Device
{
    public class XRDeviceUsage
    {
        public InputDevice InputDevice { private set; get; }
        public XRNodeState nodeState { private set; get; }
        private XRDeviceUsage() { }

        private static List<XRDeviceUsage> __UsagePool = null;
        public static XRDeviceUsage Get(InputDevice inputDevice, XRNodeState xRNodeState)
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
