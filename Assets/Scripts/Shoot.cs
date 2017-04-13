using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Weapon))]
public class Shoot : MonoBehaviour {
    Weapon wep;
    public void Shot()
    {


        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out wep.hit, wep.MaxDistance))
        {
            Debug.Log("HIT");
            switch (wep.hit.collider.material.name)
            {
                case "Wood (Instance)":
                    Instantiate(wep.WoodImpact, wep.hit.point, Quaternion.LookRotation(wep.hit.normal));
                    break;

                case "Metal (Instance)":
                    Instantiate(wep.MetalImpact, wep.hit.point, Quaternion.LookRotation(wep.hit.normal));
                    break;

                case "Stone (Instance)":
                    Instantiate(wep.StoneImpact, wep.hit.point, Quaternion.LookRotation(wep.hit.normal));
                    break;

                case "Flesh (Instance)":
                    Instantiate(wep.FleshImpact, wep.hit.point, Quaternion.LookRotation(wep.hit.normal));
                    break;

                default:
                    Instantiate(wep.SandImpact, wep.hit.point, Quaternion.LookRotation(wep.hit.normal));
                    break;
            }
        }
    }
    // Use this for initialization
    void Awake () {
        wep = GetComponent<Weapon>();
	}
    private void OnEnable()
    {
        wep = GetComponent<Weapon>();
        Shot();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
