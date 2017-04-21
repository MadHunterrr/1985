using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemIcon : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler,IPointerExitHandler
{

    public Item item;
    public GameObject tooltip;

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


    Coroutine cor;
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
        cor = StartCoroutine(ShowTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
        if (cor != null)
            StopCoroutine(cor);
    }
    IEnumerator ShowTooltip()
    {
       
        while (true)
        {
            tooltip.GetComponentInChildren<Text>().text = item.ToString();
            tooltip.GetComponent<RectTransform>().position = Input.mousePosition + new Vector3(5, -5);
            yield return new WaitForEndOfFrame();
        }
    }
}
