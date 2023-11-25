using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public enum Scenes
    {
        Start,
        Game
    }
    public static Scenes sceneToLoad = Scenes.Start;

    void Start()
    {
        StartCoroutine(StartLoad());
    }

    private IEnumerator StartLoad()
    {
        yield return new WaitForSeconds(0.5f);

        yield return Resources.UnloadUnusedAssets();
        GC.Collect();

        yield return null;

        StartCoroutine(LoadAsyncScene());
    }

    private IEnumerator LoadAsyncScene()
    {
        // Set scene.
        string sceneName;

        switch (sceneToLoad)
        {
            case Scenes.Start:
                sceneName = "StartScene";
                break;

            case Scenes.Game:
                if (Application.genuineCheckAvailable == true)
                {
                    sceneName = Application.genuine ? "GameScene" : "StartScene";
                }
                else
                {
                    sceneName = "GameScene";
                }
                break;

            default:
                sceneName = "StartScene";
                break;
        }

        // Load async.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Wait until the scene is fully loaded.
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public static void CallLoadScreen(Scenes goToScene)
    {
        sceneToLoad = goToScene;
        SceneManager.LoadScene("Loading");
    }
}
