using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class SelectAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator _animatorComponent;

    [Tooltip("Include selection of Children to trigger animation.")]
    public bool IncludeChildren = false;
    public string AnimatorVariableName = "";

    private bool _PointerHover = false;
    // Start is called before the first frame update
    void Start()
    {
        _animatorComponent = GetComponent<Animator>();
    }

    void Update()
    {
        GameObject SelectedObj = EventSystem.current.currentSelectedGameObject ;
        if (SelectedObj == null) return;

        Transform SelectedTransform = SelectedObj.transform;
        bool Selected = SelectedTransform == this.transform;
        if (IncludeChildren)
        {
            Selected = SelectedTransform == this.transform || SelectedTransform.IsChildOf(this.transform);
        }
        _animatorComponent.SetBool(AnimatorVariableName, Selected || _PointerHover);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _PointerHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _PointerHover = false;
    }
}
