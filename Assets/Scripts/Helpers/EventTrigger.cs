using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A flexable component for triggering events with a 2D Trigger Volume
/// </summary>
public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnEnter;

    [SerializeField]
    private UnityEvent OnExit;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Skater>())
        {
            OnEnter.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Skater>())
        {
            OnExit.Invoke();
        }
    }
}
