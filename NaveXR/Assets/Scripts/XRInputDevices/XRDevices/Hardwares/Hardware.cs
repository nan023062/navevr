using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System;

namespace NaveXR.InputDevices
{
    public abstract class Hardware : MonoBehaviour
    {
        #region Abstract Methods

        public abstract void SetVisiable(bool visiable);
        
        internal abstract void OnConnected(InputNode xRNodeUsage);

        internal abstract void OnDisconnected();

        #endregion
    }
}
