
using UnityEngine;

public abstract class PlayerBaseState 
{
    public abstract void EnterState(Human player);

    public abstract void Update(Human player);

    public abstract void OnCollisionEnter(Human player);

}
