
using UnityEngine;
namespace Nave.VR
{
    /// <summary>
    /// 绑定专用XR模式的Module和Input
    /// </summary>
    [RequireComponent(typeof(XRPointerInput))]
    [RequireComponent(typeof(XRPointerInputModule))]
    public class XREventSystem : UnityEngine.EventSystems.EventSystem
    {


    }
}
