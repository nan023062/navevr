using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nave.VR
{
    public class VRDropdown : TMPro.TMP_Dropdown
    {
        /// <summary>
        /// VR是3D空间的UI， Blocker必须也是世界空间
        /// </summary>
        protected override GameObject CreateBlocker(Canvas rootCanvas)
        {
            var canvas = gameObject.GetComponentInParent<Canvas>();
            GameObject go = base.CreateBlocker(canvas);
            XRGraphicRaycaster.AddToUGUI(go);
            return go;
        }

        /// <summary>
        /// 创建下拉菜单时，替换GraphicRaycaster组件
        /// </summary>
        protected override GameObject CreateDropdownList(GameObject template)
        {
            XRGraphicRaycaster.AddToUGUI(template);
            return (GameObject)Instantiate(template);
        }
    }
}

