using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader 
{
    public static bool Loading => _LoadingOperation != null;
    private static AsyncOperation _LoadingOperation;
    public static IEnumerator LoadScene(string scenename)
    {
        if (_LoadingOperation != null) yield break;
        _LoadingOperation = SceneManager.LoadSceneAsync(scenename);
        _LoadingOperation.allowSceneActivation = true;
        while (_LoadingOperation.progress < 1.0f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        _LoadingOperation = null;
        
    }

}
