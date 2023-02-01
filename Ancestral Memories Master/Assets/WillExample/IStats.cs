using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStats 
{
    void Heal(float healFactor);
    void HealHunger(float hungerFactor);
    void SetHealth(int value);
    void TakeDamage(float damageTaken);
    void DepleteFaith(float faithModifer);
    void GainFaith(float faithModifier);
    void Evolve(float evolution);
    void ContractDisease();
    void GainPsych(float psychFactor);
    void DepletePsych(float psychFactor);
    void Hunger(float hungerFactor);
}
