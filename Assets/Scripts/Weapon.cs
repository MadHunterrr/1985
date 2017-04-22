
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    #region Declaration
    [Header("Базовые параметры")]
    [Tooltip("Тип боеприпасов, которые использует даное оружие")]
    public Ammo.AmmoType ammo;
    public float Damage = 5;
    [Range(1,5)]
    public float hitImpulse = 1;
    [Tooltip("Выстрелов в минуту(Shoot per minute)")]
    public float SPM = 60;
    public int MagSize = 8;
    public int CurMagSize = 8;
    public float MaxDistance = 100;
    [Header("Дисперсия")]
    [Tooltip("Базовая дисперсия оружия(влияет на отклонение при стрельбе на определенный угол)")]
    public float fire_dispersion_base = 0.014f;//угол(в градусах) базовой дисперсии оружия(оружия зажатого в тисках)
    public float cur_fire_dispersion;
    public float fire_dispersion_inc = 0.001f;
    public float max_fire_dispersion = 0.040f;
    [Tooltip("Базовая дисперсия оружия при прицеливании(влияет на отклонение при стрельбе на определенный угол)")]
    public float fire_dispersion_base_AIM = 0.01f;//угол(в градусах) базовой дисперсии оружия(при прицеливании)
    public float cur_fire_dispersion_AIM;
    public float fire_dispersion_inc_AIM = 0.0005f;//угол(в градусах) базовой дисперсии оружия(при прицеливании)
    public float max_fire_dispersion_AIM = 0.025f;

    public float delay_before_return = 0.2f;
    public float relax_speed = 0.2f;
    Coroutine relaxRoutine;
    //отдача
    [Tooltip("увеличения угла(в градусах) с каждым выстрелом")]
    public float base_cam_dispersion = 0.5f;// базовое увеличения угла(в градусах) с каждым выстрелом
    public float cam_dispersion = 0.5f;// увеличения угла(в градусах) с каждым выстрелом
    [Tooltip("сувеличениe cam_dispersion с каждым выстрелом")]
    public float cam_dispersion_inc = 0.15f; //увеличениe cam_dispersion с каждым выстрелом 

    [Tooltip("максимальный угол отдачи")]
    public float cam_max_angle = 4.0f; //максимальный угол отдачи
    [Header("Стельба")]
    public Transform BulletSpawnPoint;
    public GameObject FleshImpact;
    public GameObject MetalImpact;
    public GameObject WoodImpact;
    public GameObject SandImpact;
    public GameObject StoneImpact;
    public GameObject WaterImpact;
    public GameObject WaterContainerImpact;
    [Header("Звуки")]
    public AudioClip ShootSound;
    public AudioClip ReloadSound;
    public AudioClip EquipSound;
    public AudioClip UnEquipSound;
    private AudioSource ASource;
    //Переменные которые влияют на механику и работу скрипта(НИЕГО НЕ МЕНЯТЬ)
    [HideInInspector]
    public RaycastHit hit;
    [HideInInspector]
    public float lastShootTime;

    [HideInInspector]
    public RectTransform AIM;
    public RectTransform AIMHit;
    public delegate void ForShoot();
    private PlayerController Equiper;
    public Vector3 ss;

    //coroutine
    Coroutine aimHitRoutine;
    private void Awake()
    {
        AIM = GameObject.FindGameObjectWithTag("Game AIM").GetComponent<RectTransform>();
        AIMHit = GameObject.FindGameObjectWithTag("Game AIMHit").GetComponent<RectTransform>();
        ASource = GetComponent<AudioSource>();
        cur_fire_dispersion = fire_dispersion_base;
        cur_fire_dispersion_AIM = fire_dispersion_base_AIM;
        AIMHit.gameObject.SetActive(false);
    }

    #endregion

    #region Function
    public void Equip(ref PlayerController baseParametr)
    {
        Equiper = baseParametr;
        baseParametr.HandAnimator.SetFloat("AttackSpeed", SPM / 60);
    }

    public IEnumerator Shoot(float delay, bool isAIM, float horizontal, ForShoot fs, ForShoot otdacha )
    {

        yield return new WaitForSeconds(delay);
        if (relaxRoutine != null)
            StopCoroutine(relaxRoutine);
        fs();
        ASource.clip = ShootSound;
        ASource.Play();
        bool readyraycast;
        float realDisp = cur_fire_dispersion + Equiper.JumpModification * fire_dispersion_inc + Equiper.SpeedModification * fire_dispersion_inc;
        if (isAIM)
        {
            if (horizontal != 0)
                readyraycast = Physics.Raycast(BulletSpawnPoint.position, BulletSpawnPoint.forward + new Vector3(Random.Range(-cur_fire_dispersion_AIM, cur_fire_dispersion_AIM), Random.Range(-cur_fire_dispersion_AIM, cur_fire_dispersion_AIM)), out hit, MaxDistance);
            else
                readyraycast = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward + new Vector3(Random.Range(-cur_fire_dispersion_AIM, cur_fire_dispersion_AIM), Random.Range(-cur_fire_dispersion_AIM, cur_fire_dispersion_AIM)), out hit, MaxDistance);
            if(cur_fire_dispersion_AIM < max_fire_dispersion_AIM)
                cur_fire_dispersion_AIM += fire_dispersion_inc_AIM;
        }
        else
        {
            readyraycast = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward + new Vector3(Random.Range(-realDisp, realDisp), Random.Range(-realDisp, realDisp)), out hit, MaxDistance);
            if (cur_fire_dispersion < max_fire_dispersion)
                cur_fire_dispersion += fire_dispersion_inc;
        }
        if (readyraycast)
        {
            if (hit.collider.GetComponent<IDamageable>() != null)
            {
                if (!hit.collider.GetComponent<IDamageable>().IsDead)
                {
                    if (aimHitRoutine != null)
                        StopCoroutine(aimHitRoutine);
                    aimHitRoutine = StartCoroutine(AIMHitDynamic());
                    hit.collider.GetComponent<IDamageable>().TakeDamage(Damage);
                }
            }
            GameObject go;
            switch (hit.collider.material.name)
            {
                case "Wood (Instance)":
                    go = Instantiate(WoodImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    break;

                case "Metal (Instance)":
                    go = Instantiate(MetalImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    break;

                case "Stone (Instance)":
                    go = Instantiate(StoneImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    break;

                case "Flesh (Instance)":
                    go = Instantiate(FleshImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    break;

                case "Water Container (Instance)":
                    go = Instantiate(WaterContainerImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    break;

                default:
                    go = Instantiate(SandImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    break;
            }
            go.transform.parent = hit.transform;
        }

        try
        {
            hit.collider.GetComponent<Rigidbody>().AddForce(hit.point* hitImpulse, ForceMode.Impulse);
        }
        catch
        {
            Debug.Log("Can`t");
        }
       relaxRoutine = StartCoroutine(RelaxCamera(fs));
        //уменьшение патронов в магазине и перезарядка
        CurMagSize--;
        otdacha();
        

    }

    public IEnumerator Reload(int BulletCount, float delay, ForShoot redrawAmmoCount)
    {
        ASource.clip = ReloadSound;
        ASource.Play();
        yield return new WaitForSeconds(delay);
        CurMagSize += BulletCount;
        redrawAmmoCount();
    }

    IEnumerator RelaxCamera(ForShoot fs)
    {
        yield return new WaitForSeconds(delay_before_return);
        while (cur_fire_dispersion > fire_dispersion_base )
        {
            yield return new WaitForSeconds(relax_speed);
            cur_fire_dispersion -= fire_dispersion_inc;
            fs();
        }
        while( cur_fire_dispersion_AIM > fire_dispersion_base_AIM)
        {
            yield return new WaitForSeconds(relax_speed);
            cur_fire_dispersion_AIM -= fire_dispersion_inc_AIM;
        }
        while(cam_dispersion>base_cam_dispersion)
        {
            yield return new WaitForSeconds(relax_speed);
            cam_dispersion -= cam_dispersion_inc;
        }
        cur_fire_dispersion = fire_dispersion_base;
        cur_fire_dispersion_AIM = fire_dispersion_base_AIM;
    }

    IEnumerator AIMHitDynamic()
    {
        AIMHit.gameObject.SetActive(true);
        int i = 40;
        while (i>30)
        {
            AIMHit.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, i);
            AIMHit.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i);
            i--;
            yield return new WaitForFixedUpdate();
        }

        AIMHit.gameObject.SetActive(false);
    }
    #endregion
}
