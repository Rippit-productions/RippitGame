using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIFocusFromHover : MonoBehaviour, IPointerEnterHandler
{
    [Tooltip("Object to focus on pointer hover. Will focus itself if none given.")]
    [SerializeField]private GameObject TargetGameObject; 
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TargetGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(TargetGameObject);
        }
    }

}
