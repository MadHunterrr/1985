using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Этот скрипт нужно вешать на все элементы, с которыми при касании персонаж будет получать урон
/// </summary>
public class DamageRegion : MonoBehaviour
{
    public Character parent;
    [SerializeField]
    private bool isReady;
    [SerializeField]
    private float damage = 5;
    public bool IsReady
    {
        get
        {
            return isReady;
        }

        set
        {
            isReady = value;
        }
    }

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
            IDamageable[] dmg = GetComponentsInChildren<IDamageable>();

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
        if (IsReady)
        {
            if (collision.gameObject.GetComponent<IDamageable>() != null)
            {
                IDamageable ad = collision.gameObject.GetComponent<IDamageable>();
                if (!ad.IsDead && NotAlies(ad))
                {
                    ad.TakeDamage(Damage);
                }
            }
        }
    }
}
