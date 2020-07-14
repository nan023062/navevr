using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NaveXR.InputDevices
{
    public class XRPointerInput : BaseInput
    {
        public bool useNative = false;


        public override bool mousePresent
        {
            get
            {
                if (!useNative) return false;
                if (!Cursor.visible) return false;
                return base.mousePresent;
            }
        }
    }
}
