using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Character))]
[RequireComponent(typeof(Animator))]
public class EnemyZombie : MonoBehaviour
{
    [Serializable]
    class MoveMap
    {
        public Vector3[] path = null;
    }
  
    Character Char;
    Animator Anim;
    NavMeshAgent Nav;
    [SerializeField]
    MoveMap RoadMap;
    [SerializeField]
    ZombieAI zombieAI;
    [SerializeField]
    private Rigidbody[] Ragdoll;





    // Use this for initialization
    void Start()
    {
        Char = GetComponent<Character>();
        Anim = GetComponent<Animator>();
        Nav = GetComponent<NavMeshAgent>();
        Ragdoll = GetComponentsInChildren<Rigidbody>();
        CheckMove();
    }

    private void FixedUpdate()
    {
        if (Nav.enabled && !Nav.hasPath)
        {
            CheckMove();
        }
    }


    public void Kill()
    {
        Nav.SetDestination(transform.position);
        Nav.enabled = false;
        Anim.enabled = false;
        foreach (var rd in Ragdoll)
        {
            rd.isKinematic = false;
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
        Nav.speed = Char.Speed;
        Nav.SetDestination(vec);
    }

    public void Attack()
    {
        Anim.SetTrigger("Attack");
    }
}
