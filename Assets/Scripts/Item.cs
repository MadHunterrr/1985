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
    public bool isReady { get; set; }
    #endregion

    public override string ToString()
    {
        return String.Format(
@"<b>Название:</b>  <color=#00ffffff>{0} </color>
Стоимость: {1}


Описание: {2}", Name, Cost, Description);
            ;
    }

    public void OnActive(GameObject go)
    {
        CheckInventory(go.GetComponent<Character>().Invent);
        go.GetComponent<PlayerController>().DrawAmmo();
        isReady = false;
    }

    public void CheckInventory(Inventory inv)
    {
        for (int i = 0; i < inv.Items.Count; i++)
        {
            if (inv.Items[i].Name == Name)
            {
                inv.Items[i].Count += Count;
                Destroy(gameObject);
                return;
            }
        }
        inv.Items.Add(this);
        transform.parent = inv.transform;
        transform.position = inv.transform.position;
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
