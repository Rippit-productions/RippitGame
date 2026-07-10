using RippitGameManager;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FMODBankLoader))]
public class FMODLoadingScene : MonoBehaviour
{
    [SerializeField] private SceneReference NextScene;
    [SerializeField] private FMODBankLoader _FMODBankLoader;

    private void Awake()
    {
        _FMODBankLoader = GetComponent<FMODBankLoader>();
    }
    // Use this for initialization
    void Start()
    {
        _FMODBankLoader.LoadBanks();
        StartCoroutine(_WaitForLoading());
    }

    private IEnumerator _WaitForLoading()
    {
        if (FMODBankLoader.Loading)
        {
            yield return null;
        }
        else
        {
            GameManager.Instance.LoadScene(NextScene);
        }
        yield break;
    }
}
