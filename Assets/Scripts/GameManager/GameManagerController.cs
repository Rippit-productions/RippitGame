using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerController : MonoBehaviour
{
    private static Coroutine SceneLoadingCoroutine = null;

    public void LoadScene(string SceneName)
    {
        StartCoroutine(SceneLoader.LoadScene(SceneName));
    }

    public void QuitGame() => Application.Quit();
}
