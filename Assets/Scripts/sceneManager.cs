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
        Instance = this;
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void loadScene(int sceneNum, int doorNum, GameObject obj)
    {
        SceneManager.LoadScene(sceneNum);
        gameManager.Instance.sendDoor(doorNum, obj);
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }


}
