using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IActiveable
{
    #region Declaration
    public string Name;
    public string Description;
    public int Cost;
    public uint Count = 1;
    public Sprite Icon;
    public Vector2 iconPos;
    public Vector2 iconSize;
    public bool isReady { get; set; }
    #endregion

    public void OnActive(GameObject go)
    {
        CheckInventory(go.GetComponent<Inventory>());
        isReady = false;
    }

    void CheckInventory(Inventory inv)
    {
        if (inv.Items.Count == 0)
        {
            inv.Items.Add(this);
            transform.parent = inv.transform;
            transform.position = inv.transform.position;
        }
        else
        {
            foreach (Item item in inv.Items)
            {
                if (item.Name == Name)
                {
                    item.Count += Count;
                    Destroy(gameObject);
                }
                else
                {
                    inv.Items.Add(this);
                    transform.parent = inv.transform;
                    transform.position = inv.transform.position;
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        isReady = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
