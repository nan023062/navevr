using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nave.VR
{
    public class VRDropdown : TMPro.TMP_Dropdown
    {
        protected override GameObject CreateBlocker(Canvas rootCanvas)
        {
            GameObject go = base.CreateBlocker(rootCanvas);
            XRGraphicRaycaster.AddToUGUI(go);
            return go;
        }

        protected override GameObject CreateDropdownList(GameObject template)
        {
            XRGraphicRaycaster.AddToUGUI(template);
            return (GameObject)Instantiate(template);
        }
    }
}

