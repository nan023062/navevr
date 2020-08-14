using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NaveXR.InputDevices
{
    public class XRPointerInput : BaseInput
    {
        public override bool mousePresent
        {
            get
            {
                //if (!Cursor.visible) return false;
                return base.mousePresent;
            }
        }
    }
}
