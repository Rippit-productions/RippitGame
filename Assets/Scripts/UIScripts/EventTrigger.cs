using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// You should notice this is like FinishLine.cs
// So why make this one? Good question!
// This one is more flexable as it's not tide to any specific job
// which makes it perfect for playing and experimenting with mechanics.

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
        if (other.gameObject.CompareTag("Player"))
        {
            OnEnter.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnExit.Invoke();
        }
    }
}
