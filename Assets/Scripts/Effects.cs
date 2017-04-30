using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public static Effect Fire = new Effect
    {
        TypeEffect = Effect.EffectType.negative,
        TargetType = new List<Effect.TypeOfTarget>() { Effect.TypeOfTarget.health} ,
        DamageType = Weapon.DamageType.fire,
        Name = "Ожог",
        Description = "Вы получаете периодический урон от огня",
        Damage = 2,
        Duration = 2
    };

    public static Effect Poison = new Effect
    {
        TypeEffect = Effect.EffectType.negative,
        TargetType = new List<Effect.TypeOfTarget>() { Effect.TypeOfTarget.health, Effect.TypeOfTarget.stamina },
        DamageType = Weapon.DamageType.poison,
        Name = "Яд",
        Description = "Вы получаете периодический урон от яда",
        Damage = 1,
        Duration = 25
    };

    public static Effect Virus = new Effect
    {
        TypeEffect = Effect.EffectType.negative,
        TargetType = new List<Effect.TypeOfTarget>() { Effect.TypeOfTarget.health, Effect.TypeOfTarget.stats },
        DamageType = Weapon.DamageType.virus,
        Name = "Вирус",
        Description = "Вы заразились вирусом, вы можете умереть если не принять антивирус",
        Damage = 1,
        Duration = -1
    };

    public static Effect Cold = new Effect
    {
        TypeEffect = Effect.EffectType.negative,
        TargetType = new List<Effect.TypeOfTarget>() { Effect.TypeOfTarget.stamina},
        DamageType = Weapon.DamageType.cold,
        Name = "Переохлаждение",
        Description = "У вас легкое переохлаждение. Согрейтесь, чтобы не было осложнений",
        Damage = 1,
        Duration = 10
    };

    public static Effect HeavyCold = new Effect
    {
        TypeEffect = Effect.EffectType.negative,
        TargetType = new List<Effect.TypeOfTarget>() { Effect.TypeOfTarget.stamina, Effect.TypeOfTarget.health },
        DamageType = Weapon.DamageType.cold,
        Name = "Переохлаждение",
        Description = "У вас сильное переохлаждение. Согрейтесь, чтобы не умереть",
        Damage = 2,
        Duration = 20
    };

}

