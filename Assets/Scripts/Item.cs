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
    public int Count;
    public Texture Icon;
    public Vector2 iconPos;
    public Vector2 iconSize;
    public bool isReady { get; set; }

    public void OnActive(GameObject go)
    {
        go.GetComponent<Inventory>().Items.Add(this);        
        transform.parent = go.transform;
        transform.position = go.transform.position;
        isReady = false;
    }
    #endregion

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
