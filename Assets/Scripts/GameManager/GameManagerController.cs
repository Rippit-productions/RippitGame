using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerController : MonoBehaviour
{
    private static Coroutine SceneLoadingCoroutine = null;

    public async void LoadScene(string SceneName)
    {
        var operation =  SceneLoader.LoadScene(SceneName);

        LoadingScreen.WaitFor(operation);

        if (!operation.IsCompleted) await operation;

        operation.Result.allowSceneActivation = true;
    }

    public void QuitGame() => Application.Quit();
}
