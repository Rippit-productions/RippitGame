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
        var loadingOperation = GameManager.LoadScene(SceneName);
        var WaitTask = Task.Run(async () =>
        {
            while (loadingOperation.progress < 0.9f) 
            {
                await Task.Yield();
            }
            await Task.Delay(500);
            Debug.Log($"Loading {loadingOperation.progress}");
        });
        LoadingScreen.WaitFor(WaitTask);
    }

    IEnumerator _LoadSceneAsync(string sceneName)
    {
        var sceneAsync = SceneManager.LoadSceneAsync(sceneName);

        while (!sceneAsync.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);
    }



    public void QuitGame() => Application.Quit();
}
