using UnityEngine;

namespace Nave.XR
{
    public abstract class Hardware : MonoBehaviour
    {
        public void Update()
        {
            OnUpdate();
        }

        protected abstract void OnUpdate();
    }
}
