using UnityEngine;
using UnityEngine.Events;

public class CollisionNotifier : MonoBehaviour
{
    public UnityEvent<Collision> OnCollisionEnterEvent;

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnterEvent?.Invoke(collision);
    }
}
