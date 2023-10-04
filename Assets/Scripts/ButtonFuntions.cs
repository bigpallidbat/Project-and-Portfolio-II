using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFuntions : MonoBehaviour
{
    public void resume()
    {
        gameManager.Instance.stateUnpause();
    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.Instance.stateUnpause();
    }
    public void quit()
    {
        Application.Quit();
    }

    public void respawnPlayer()
    {
        gameManager.Instance.stateUnpause();
        gameManager.Instance.playerScript.spawnPlayer();
    }

    public void mainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
