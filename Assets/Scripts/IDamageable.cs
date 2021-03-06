﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

interface IDamageable
{
    void TakeDamage(float damage, Weapon.DamageType damageType);
    void TakeDamage();
    void Destruction();
    bool IsDead { get; }
}

