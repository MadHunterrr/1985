using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect
{
    public enum EffectType { positive, negative, neutral }
    public EffectType TypeEffect;
    public enum TypeOfTarget { health, stamina, armor, stats }
    public List<TypeOfTarget> TargetType;
    public Weapon.DamageType DamageType;
    public string Name;
    public string Description;
    public float Damage;
    public float Healing;
    public float Duration;

}
