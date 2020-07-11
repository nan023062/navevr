using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NaveXR.EventSystem
{
    public class XRPointerInput : BaseInput
    {

        public override bool mousePresent
        {
            get
            {
                if (!Cursor.visible) return false;
                return base.mousePresent;
            }
        }


        public override Vector2 mousePosition
        {
            get {
                Vector2 mousePosition = base.mousePosition;
                //mousePosition.x = Screen.width * 0.5f;
                //mousePosition.y = Screen.height * 0.5f;

                return mousePosition;
            }
        }
    }
}
