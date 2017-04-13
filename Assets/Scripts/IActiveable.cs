using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActiveable
{
    void OnActive(GameObject go);
    bool isReady { get; set;}
}
