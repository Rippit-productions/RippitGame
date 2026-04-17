using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SelectAnim : MonoBehaviour
{
    public string EnterAnimationState;
    public string ExitAnimationState;

    private Animator _animatorComponent;

    public
    // Start is called before the first frame update
    void Start()
    {
        _animatorComponent = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var controller = _animatorComponent.runtimeAnimatorController as AnimatorController;
        
    }
}
