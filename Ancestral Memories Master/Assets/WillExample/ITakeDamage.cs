using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeDamage 
{

    void HealDamage(float healthHealed);

    void TakeDamage(float damageTaken);

    void Death();


}
