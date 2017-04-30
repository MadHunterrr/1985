using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableParts : MonoBehaviour, IDamageable
{
    public enum BodyType { Torso, RightHand, LeftHand, LeftLeg, RightLeg, Head }
    public BodyType BodyPart;
    public float TorsoMultiply = 1f;
    public float HandMultiply = 0.8f;
    public float LegsMultiply = 0.8f;
    public float HeadMultiply = 1.8f;
    Character Char;

    public bool IsDead
    {
        get
        {
            return Char.CurHealth <= 0;
        }
    }

    public void Destruction()
    {
        try
        {
            Char.GetComponent<EnemyZombie>().Kill();
        }
        catch (NullReferenceException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void TakeDamage()
    {
        TakeDamage(10, Weapon.DamageType.balistic);
    }

    public void TakeDamage(float damage, Weapon.DamageType damageType)
    {
        if (!IsDead)
        {
            switch (BodyPart)
            {
                case BodyType.Head:
                    Char.CurHealth -= (damage * HeadMultiply) - ((damage * HeadMultiply) * ((float)Char.HeadArmor / 100));
                    break;
                case BodyType.Torso:
                    Char.CurHealth -= (damage * TorsoMultiply) - ((damage * TorsoMultiply) * ((float)Char.TorsoArmor / 100));
                    break;
                case BodyType.LeftHand:
                    Char.CurHealth -= (damage * HandMultiply) - ((damage * HandMultiply) * ((float)Char.LeftHandArmor / 100));
                    break;
                case BodyType.RightHand:
                    Char.CurHealth -= (damage * HandMultiply) - ((damage * HandMultiply) * ((float)Char.RightHandArmor / 100));
                    break;
                case BodyType.LeftLeg:
                    Char.CurHealth -= (damage * LegsMultiply) - ((damage * LegsMultiply) * ((float)Char.LeftLegArmor / 100));
                    break;
                case BodyType.RightLeg:
                    Char.CurHealth -= (damage * LegsMultiply) - ((damage * LegsMultiply) * ((float)Char.RightLegArmor / 100));
                    break;
            }
        }
        if (IsDead)
        {
            Destruction();
        }
    }

    // Use this for initialization
    void Start()
    {
        Char = GetComponentInParent<Character>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
