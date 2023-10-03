using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    public static sceneManager Instance;
    int sceneIndex;

    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void loadScene(int sceneNum, int doorNum)
    {
        SceneManager.LoadScene(sceneNum);
       // SceneManager.UnloadSceneAsync(sceneIndex);
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }


}
