using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    public static sceneManager Instance;
    int sceneIndex;
    public static bool scenechange;

    private void Start()
    {
        Instance = this;
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void loadScene(int sceneNum)
    {
       
        scenechange = true;
        SceneManager.LoadScene(sceneNum);
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }


}
