
using UnityEngine;

public abstract class PlayerBaseState 
{
    public abstract void EnterState(CharacterClass player);

    public abstract void Update(CharacterClass player);

    public abstract void OnCollisionEnter(CharacterClass player);

}
