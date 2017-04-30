using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour , IDamageable
{

    public bool IsDead
    {
        get
        {
            return CurHealth <= 0;
        }
    }

    [HideInInspector]
    public Inventory Invent;
    public enum Movement { Run, Walk, Crouch, Sprint}
    [Header("Передвижение")]
    [Tooltip("Walk = ходьба, Crouch = скрытность, Run = бег(базовый), Sprint = спринт")]
    public Movement CurrentMovement;
    [Tooltip("Базовая скорость перемещения. Используеться при режиме CurrentMovement = Run")]
    public float Speed = 5;
    [Range(0.2f,0.4f)]
    [Tooltip("Множитель скорости при скрытом перемещении. Скорость при скрытом перемещении равняется базовой скорости(Speed) * [этот множитель]")]
    public float CrouchMultiple = 0.3f;
    [Range(0.4f, 0.7f)]
    [Tooltip("Множитель скорости при ходьбе. Скорость при ходьбе равняется базовой скорости(Speed) * [этот множитель]")]
    public float WalkMultiple = 0.45f;
    [Range(1.2f, 2f)]
    [Tooltip("Множитель скорости при спринте. Скорость при спринте равняется базовой скорости(Speed) * [этот множитель]")]
    public float SprintMultiple = 1.3f;

    [Header("Общее здоровье")]
    public float Health = 100;
    public float CurHealth = 100;
    public float Stamina = 100;
    public float CurStamina = 100;
    public List<EffectList> currentEffects;
    [Serializable]
    public class EffectList
    {

        public Effect effects;
        public Coroutine effectRoutine;

    }
    //----Здоровье частей тела---- от 0 до 100-----//
    [Header("Здоровья разных частей тела")]
    public float HeadHealth = 100;
    public float TorsoHealth = 100;
    public float RightHandHealth = 100;
    public float LeftHeandHealth = 100;
    public float RightLegHealth = 100;
    public float LeftLegHealth = 100;

    [Header("Сопротевление")]
    [Range(0, 85)]
    public float FireProtection;
    [Range(0, 85)]
    public float ColdProtection;
    [Range(0, 85)]
    public float PoisonProtection;
    [Range(0, 100)]
    public float VirusProtection;

    //----Защита частей тела---- от 0 до 85-----//
    [Header("Броня для разных частей тела")]
    [Range(0, 85)]
    public int HeadArmor = 0;
    [Range(0, 85)]
    public int TorsoArmor = 0;
    [Range(0, 85)]
    public int RightHandArmor = 0;
    [Range(0, 85)]
    public int LeftHandArmor = 0;
    [Range(0, 85)]
    public int RightLegArmor = 0;
    [Range(0, 85)]
    public int LeftLegArmor = 0;

    [Header("Параметры анимации")]
    public bool isAIM = false;

    [Header("AI")]
    public float meleeDamage = 5;
    public float dayVisibleDistance = 30;
    public float nightVisibleDistance = 15;
    public float dayListenDistance;
    public float nightListenDistance;
    public float attackDistance = 2;

    [Header("Components")]
    public Animator Anim;
    public PlayerController pl;

    private void Awake()
    {
        Anim = GetComponent<Animator>();
        pl = GetComponent<PlayerController>();
    }
    //Static function
    public static Vector3 Direction(Vector3 target, Vector3 hero)
    {
        Vector3 temp = target - hero;
        float ftemp = temp.magnitude;
        return temp / ftemp;
    }

    public void AddEffect(Effect effect)
    {
        for (int i =0;i<currentEffects.Count;i++)
        {
            if (currentEffects[i].effects == effect)
            {
                StopCoroutine(currentEffects[i].effectRoutine);
                currentEffects.RemoveAt(i);
                break;
            }
        }
        currentEffects.Add(new EffectList {effects = effect });
        currentEffects[currentEffects.Count-1].effectRoutine = StartCoroutine(AddEffectRoutine(effect));
    }
    public IEnumerator AddEffectRoutine(Effect effect)
    {
        float t = Time.time;
        while(Time.time<t+effect.Duration)
        {
            yield return new WaitForSeconds(1);
            foreach(var temp in effect.TargetType)
            {
                if(temp == Effect.TypeOfTarget.health)
                {
                    TakeDamage(effect.Damage, effect.DamageType);
                }
                else if(temp == Effect.TypeOfTarget.stamina)
                {
                    StaminaDamage(effect.Damage);
                }

            }
        }
    }


    #region IDamageable
    public void TakeDamage(float damage, Weapon.DamageType damageType)
    {
        if (!IsDead)
        {   
            CurHealth -= damage - (damage * ((float)TorsoArmor / 100));
            if (pl != null) pl.DrawHealth();
        }

        if (IsDead)
        {
            Destruction();
        }
    }

    public void TakeDamage()
    {
        CurHealth -= 1;
        if (pl != null) pl.DrawHealth();
    }

    public void Destruction()
    {
        CurHealth = 0;
        if (pl != null) pl.DrawHealth();
    }
#endregion

    void StaminaDamage(float damage)
    {
        CurStamina -= damage;
    }
}
