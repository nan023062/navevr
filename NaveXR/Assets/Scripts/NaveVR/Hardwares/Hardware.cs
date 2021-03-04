using UnityEngine;

namespace Nave.VR
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
