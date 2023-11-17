using UnityEngine;
using UnityEngine.Events;

public class CollisionNotifier : MonoBehaviour
{
    public UnityEvent<Collision> OnCollisionEnterEvent;

    private void Awake()
    {
        OnCollisionEnterEvent = new UnityEvent<Collision>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (OnCollisionEnterEvent == null)
        {
            OnCollisionEnterEvent = new UnityEvent<Collision>();
        }
        OnCollisionEnterEvent.Invoke(collision);
    }

}
