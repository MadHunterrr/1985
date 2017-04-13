using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    private Inventory VisInventory;
    public bool IsActive { get; set; }
    Image mainImage;
    GameObject[] ItemsUI;

    //size inventory 13xN
    // Use this for initialization
    private void Start()
    {
        mainImage = GetComponent<Image>();
    }
    void DrawItem(int i)
    {        
        RectTransform rect = ItemsUI[i].AddComponent<RectTransform>();
        
    }
    public void Activate(Inventory visInventory)
    {
        IsActive = true;
        mainImage.enabled = true;
        VisInventory = visInventory;
        ItemsUI = new GameObject[visInventory.Items.Count];
        for(int i=0;i<visInventory.Items.Count;i++)
        {
            ItemsUI[i] = new GameObject(visInventory.Items[i].Name);
            DrawItem(i);
        }

    }
    public void DeActivate()
    {
        IsActive = false;
        mainImage.enabled = false;
        for (int i = 0; i < ItemsUI.Length; i++)
        {
            Destroy(ItemsUI[i]);
        }
    }
}
