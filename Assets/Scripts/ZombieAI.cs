using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyZombie))]
public class ZombieAI : MonoBehaviour
{
    EnemyZombie zombie;
    [SerializeField]
    public LayerMask mask;
    public Transform VisStart;

    public float curDistance;
    public float dayVisibleDistance = 30;
    public float nightVisibleDistance;
    public float dayListenDistance;
    public float nightListenDistance;
    public float attackDistance;

    public float rayCount = 10;
    public float angle = 12;

    public float FindEnemyDelay = 0.5f;
    Coroutine findEnemyCoroutine;
    IEnumerator UpdateRay()
    {
        while (true)
        {
            DrawAllCast();
            yield return new WaitForSeconds(FindEnemyDelay);
        }
    }

    void DrawRaycast(Vector3 dir)
    {
        for (int i = 0; i < 2; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(VisStart.position - (transform.up * 0.9f*i), dir, out hit, dayVisibleDistance,mask.value))
            {
                if (hit.collider.GetComponent<PlayerController>() != null)
                {
                    Debug.DrawLine(VisStart.position - (transform.up * 0.9f*i), hit.point, Color.red);
                    SetTarget(hit.collider.GetComponent<Character>());
                }
                else
                {
                    Debug.DrawLine(VisStart.position - (transform.up * 0.9f*i), hit.point, Color.blue);
                }
            }
            else
            {
                Debug.DrawRay(VisStart.position - (transform.up * 0.9f*i), dir * dayVisibleDistance, Color.green);
            }
        }
    }

    void DrawAllCast()
    {
        float temp = 0;
        for (int i = 0; i < rayCount; i++)
        {
            float right = Mathf.Sin(temp); //сдвиг угла вправо(влево)
            float forward = Mathf.Cos(temp); //сдвиг вперед

            temp += angle * Mathf.Deg2Rad / rayCount; // увеличиваем  сдвиг направления

            Vector3 dir = VisStart.TransformDirection(new Vector3(right, 0, forward));
            DrawRaycast(dir);

            if (right != 0)//если это не начальный луч, который посылается в центр, то отправить луч влево от центра(без этого параметра, лучи будут отправляться только вправо от центра)
            {
                dir = VisStart.TransformDirection(new Vector3(-right, 0, forward));
                DrawRaycast(dir);
            }
        }

    }

    public void SetTarget(Character target)
    {
        if (findEnemyCoroutine != null)
            StopCoroutine(findEnemyCoroutine);
        zombie.target = target;
        zombie.Nav.speed = zombie.Char.Speed;
    }

    public bool HasTarget()
    {
        return (zombie.target != null) ? true : false;
    }
    public void ClearTarget()
    {
        zombie.target = null;
        findEnemyCoroutine = StartCoroutine(UpdateRay());
    }

    // Use this for initialization
    void Start()
    {
        zombie = GetComponent<EnemyZombie>();
        findEnemyCoroutine = StartCoroutine(UpdateRay());
    }

  

}
