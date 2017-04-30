using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Этот скрипт нужно вешать на все элементы, с которыми при касании персонаж будет получать урон
/// </summary>
public class DamageRegion : MonoBehaviour
{
    public Character parent;
    public Weapon.DamageType damageType = Weapon.DamageType.balistic;
    [SerializeField]
    private float damage = 5;


    public float Damage
    {
        get
        {
            return damage;
        }

        set
        {
            damage = value;
        }
    }

    bool NotAlies(IDamageable check)
    {
        if (parent != null)
        {
            IDamageable[] dmg = parent.GetComponentsInChildren<IDamageable>();

            foreach (var i in dmg)
            {
                if (i == check) return false;
            }
            return true;
        }
        else return true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable ad = collision.gameObject.GetComponent<IDamageable>();
        if (ad != null)
        {
            if (!ad.IsDead)
            {
                if (parent != null && !parent.Anim.GetBool("IsReady"))
                {
                    if (NotAlies(ad))
                    {
                        ad.TakeDamage(parent.meleeDamage,damageType);
                    }

                }
                else if(parent == null)
                {
                    ad.TakeDamage(Damage,damageType);
                }
            }
        }
    }
}
