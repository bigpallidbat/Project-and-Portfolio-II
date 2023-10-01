using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    public static sceneManager Instance;
    string sceneName;

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    public void loadScene(string scene)
    {
        SceneManager.LoadScene(scene);
        SceneManager.UnloadSceneAsync(sceneName);
        sceneName = SceneManager.GetActiveScene().name;
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }


}
