using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Character))]
public class EnemyZombie : MonoBehaviour
{
    [Serializable]
    class MoveMap
    {
        public Vector3[] path = null;
    }
    [HideInInspector]
    public Character Char;
    [HideInInspector]
    public NavMeshAgent Nav;
    [SerializeField]
    MoveMap RoadMap;
    [SerializeField]
    private Rigidbody[] Ragdoll;
    bool isDeath = false;
    public Character target;





    // Use this for initialization
    void Start()
    {
        Char = GetComponent<Character>();
        Nav = GetComponent<NavMeshAgent>();
        Ragdoll = GetComponentsInChildren<Rigidbody>();
        //CheckMove();
    }



    public void Kill()
    {
        Nav.SetDestination(transform.position);
        GetComponent<ZombieAI>().ClearTarget();
        isDeath = true;
        Nav.enabled = false;
        Char.Anim.enabled = false;
        foreach (var rd in Ragdoll)
        {
            rd.isKinematic = false;
        }

    }

    private void Update()
    {
        if (!isDeath)
        {
            if (target != null)
            {
                if (Char.Anim.GetBool("IsReady"))
                {
                    Nav.SetDestination(target.transform.position);

                    if (Vector3.Distance(transform.position, target.transform.position) < Char.attackDistance)
                    {
                        Attack();
                    }

                }
            }
        }
    }

    public void CheckMove()
    {

        if (RoadMap.path.Length > 0)
        {

            int temp = UnityEngine.Random.Range(0, RoadMap.path.Length);
            Move(RoadMap.path[temp]);
        }
        else
        {
            Vector3 vec = new Vector3(
               UnityEngine.Random.Range(transform.position.x - 20, transform.position.x + 20),
               transform.position.y,
               UnityEngine.Random.Range(transform.position.z - 20, transform.position.z + 20));
            Move(vec);
        }
    }

    public void Move(Vector3 vec)
    {
        if (!isDeath)
        {
            Nav.speed = Char.Speed;
            Nav.SetDestination(vec);
        }
    }

    public void Attack()
    {
        //transform.rotation = Quaternion.LookRotation(target.transform.position-transform.position);
        //Nav.SetDestination(transform.position);
        Char.Anim.SetTrigger("Attack");
    }
}
