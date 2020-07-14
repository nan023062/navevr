using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System;

namespace NaveXR.InputDevices
{
    public class HandShankModel : MonoBehaviour
    {
        public HandInputAnimation trigger;
        public HandInputAnimation grip;
        public HandInputAnimation primary;
        public HandInputAnimation secondary;
        public HandInputAnimation menu;
        public HandInputAnimation touch;
        public GameObject laserpoint;
    }
}
