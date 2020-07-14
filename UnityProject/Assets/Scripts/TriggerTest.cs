using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TriggerTest : MonoBehaviour, IEndDragHandler,IBeginDragHandler,IDragHandler,IDropHandler,IPointerClickHandler,IPointerDownHandler,IPointerExitHandler,IPointerUpHandler
{
    public TextMeshProUGUI text11;

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
        text11.text = string.Format(" OnDrop:{0}", eventData.pointerPressRaycast.worldPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        text11.text = string.Format(" OnEndDrag:{0}", eventData.pointerPressRaycast.worldPosition);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        text11.text = string.Format(" OnBeginDrag:{0}", eventData.pointerPressRaycast.worldPosition);
    }


    public void OnDeselect(BaseEventData eventData)
    {

    }

    public void OnMove(AxisEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        text11.text = string.Format(" OnPointerClick:{0}", eventData.pointerPressRaycast.worldPosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        text11.text = string.Format(" OnPointerDown:{0}", eventData.pointerPressRaycast.worldPosition);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text11.text = string.Format(" OnPointerEnter:{0}", eventData.pointerPressRaycast.worldPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text11.text = string.Format(" OnPointerExit:{0}", eventData.pointerPressRaycast.worldPosition);
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        text11.text = string.Format(" OnPointerUp:{0}", eventData.pointerPressRaycast.worldPosition);
    }
}
