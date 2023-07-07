using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStats 
{
    void Heal(float healFactor);
    void SetHealth(int value);
    void TakeDamage(float damageTaken);
    void FaithModify(float faithModifer);
    void ContractDisease();
    void PsychModifier(float psychFactor);
    void Hunger(float hungerFactor);
    void HealHunger(float hungerFactor);
    void DiseaseDamage();
}
