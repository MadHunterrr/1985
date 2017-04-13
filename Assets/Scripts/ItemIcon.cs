using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemIcon : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    public Item item;
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("start");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("GO GO");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("END");
    }
    
}
