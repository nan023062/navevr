using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Nave.XR
{
    public class XRPointerInput : UnityEngine.EventSystems.BaseInput
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
