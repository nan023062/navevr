using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System;

namespace NaveXR.InputDevices
{
    public abstract class Controller : MonoBehaviour
    {
        #region Abstract Methods

        internal abstract void SetVisiable(bool visiable);
        
        internal abstract void OnConnected(XRNodeState nodeState, InputDevice inputDevice);

        internal abstract void OnDisconnected();

        #endregion
    }
}
