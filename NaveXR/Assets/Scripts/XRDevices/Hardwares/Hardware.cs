using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System;

namespace Nave.XR
{
    public abstract class Hardware : XRDeviceObject
    {
        #region Abstract Methods

        public abstract void SetVisiable(bool visiable);
        
        #endregion
    }
}
