using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader 
{
    public static async Task<AsyncOperation> LoadScene(string scenename)
    {
        var loadOP = SceneManager.LoadSceneAsync(scenename);
        loadOP.allowSceneActivation = false;
        while (loadOP.progress < 0.9f)
        {
            await Task.Yield();
        }

        await Task.Delay(4000);

        return loadOP;
    }

}
