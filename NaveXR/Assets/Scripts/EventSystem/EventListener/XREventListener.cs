using UnityEngine;
using UnityEngine.EventSystems;

namespace Nave.VR
{
    public class XREventListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
    {
        public static XREventListener Get(GameObject gameObject)
        {
            var v = gameObject.GetComponent<XREventListener>();
            if (v == null) v = gameObject.AddComponent<XREventListener>();
            return v;
        }

        public delegate void OnXRPointAction(XRPointEventData pointerEventData);

        public OnXRPointAction onPointerEnter;
        public OnXRPointAction onPointerExit;
        public OnXRPointAction onDragging;

        public void OnPointerEnter(PointerEventData eventData) {
            if(eventData is XRPointEventData)
            onPointerEnter?.Invoke(eventData as XRPointEventData); }

        public void OnPointerExit(PointerEventData eventData) {
            if (eventData is XRPointEventData)
                onPointerExit?.Invoke(eventData as XRPointEventData); }

        public void OnDrag(PointerEventData eventData) {
            if (eventData is XRPointEventData)
                onDragging?.Invoke(eventData as XRPointEventData); }
    }
}