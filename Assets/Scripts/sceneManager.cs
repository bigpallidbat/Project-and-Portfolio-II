using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    public static sceneManager Instance;
    static int sceneIndex;
    public static bool scenechange;

    private void Start()
    {
        Instance = this;
        sceneIndex = 0;
    }

    public void loadScene(int sceneNum)
    {
        gameManager.Instance.playerSpawnPoint = null;
        scenechange = true;
        gameManager.Instance.playerScript.setHP(); 
        SceneManager.LoadScene(sceneNum);
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    public void nextScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }


}
