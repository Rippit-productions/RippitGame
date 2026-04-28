using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance => FindFirstObjectByType<LoadingScreen>();
    private Animator _animator;

    private static string ANIM_VAR = "Loading";

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool(ANIM_VAR, FMODBankLoader.Loading || SceneLoader.Loading); 
    }


}
