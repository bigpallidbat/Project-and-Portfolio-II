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
        gameManager.currentlevel = gameManager.Levels.MainMenu;
        gameManager.Instance.stateUnpauseWithCursor();

    }
    public void startGame()
    {
        sceneManager.Instance.nextScene(1);
        screenManager.Instance.turnOffScreens();
    }


    public void levelButton()
    {
        screenManager.Instance.setScreen(2);
    }

    public void levelSelect(int level)
    {
        sceneManager.Instance.nextScene(level);
        
        if(level < 2)
            screenManager.Instance.turnOffScreens();

        if (Time.timeScale == 0) 
        {
            gameManager.Instance.stateUnpause();
        }
    }

    public void BackFromLevelSelect()
    {
        screenManager.Instance.setScreen(1);
    }
}
