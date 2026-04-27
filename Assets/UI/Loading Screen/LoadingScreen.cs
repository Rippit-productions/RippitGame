using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance => FindFirstObjectByType<LoadingScreen>();
    private Animator _animator;

    private static string ANIM_VAR = "Loading";

    private static List<Task> _Tasks = new List<Task>();
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        LoadingScreen.WaitFor(FMODBankLoader.Instance.LoadBanks());
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _Tasks.Count; i++)
        {
            if (_Tasks[i].IsCompleted)
            {
                _Tasks.RemoveAt(i);
            }
        }
        _animator.SetBool(ANIM_VAR, IsLoading()); 
    }

    public static void WaitFor(Task task)
    {
        _Tasks.Add(task);
    }

    public bool IsLoading()
    {
        return _Tasks.Count > 0;
    }
}
