using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyZombie))]
public class ZombieAI : MonoBehaviour
{
    EnemyZombie zombie;
    public Character Target;
    public float Distance;
    public float attackDistance;
    // Use this for initialization
    void Start()
    {
        zombie = GetComponent<EnemyZombie>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
