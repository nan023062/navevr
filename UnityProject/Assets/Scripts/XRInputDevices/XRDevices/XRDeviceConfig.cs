using System;
using UnityEngine;

namespace NaveXR.InputDevices
{
    public class DRVName
    {
        public const string OpenVR = "OpenVR";
        public const string Oculus = "Oculus";
    }

    public partial class XRDevice : MonoBehaviour
    {
        public static void GetHandInputOffset(bool left, out Vector3 position, out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;

            if (DriverName == DRVName.OpenVR)
            {
                position = new Vector3(left ? -0.003f : 0.003f, -0.006f, -0.1f);
                rotation = Quaternion.identity;
            }
            else if (DriverName == DRVName.Oculus)
            {
                float tan = Mathf.Tan(40f * Mathf.Deg2Rad);
                float z = -0.034f;
                position = new Vector3(left ? -0.0075f : 0.0075f, z * tan, z);
                rotation = Quaternion.Euler(-40f, 0f, 0f);            
            }
        }

    }
}
