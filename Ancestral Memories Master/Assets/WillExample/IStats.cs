﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStats 
{
    void Heal(float healFactor);
    void SetHealth(int value);
    void TakeDamage(float damageTaken);
    void DepleteFaith(float faithModifer);
    void GainFaith(float faithModifier);
    void Evolve(float evolution);
    void ContractDisease();
    void PsychModifier(float psychFactor);
    void Hunger(float hungerFactor);
    void HealHunger(float hungerFactor);
}
