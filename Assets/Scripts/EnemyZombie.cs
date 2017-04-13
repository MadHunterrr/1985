using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Character))]
[RequireComponent(typeof(Animator))]
public class EnemyZombie : MonoBehaviour {
    Character Char;
    Animator Anim;
    NavMeshAgent Nav;

    [SerializeField]
    private Rigidbody[] Ragdoll;
    // Use this for initialization
    void Start () {
        Char = GetComponent<Character>();
        Anim = GetComponent<Animator>();
        Nav = GetComponent<NavMeshAgent>();
        Ragdoll = GetComponentsInChildren<Rigidbody>();
	}
	
	public void Kill()
    {
        Anim.enabled = false;
        foreach(var rd in Ragdoll)
        {
            rd.isKinematic = false;
        }

    }
}
