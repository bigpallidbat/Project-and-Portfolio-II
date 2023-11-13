using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFuntions : MonoBehaviour
{
    

    public void resume()
    {
        gameManager.Instance.stateUnpause();
        gameManager.Instance.playSFX();
    }
    public void restart()
    {
        if(gameManager.currentlevel == gameManager.Levels.SpawnerDestroy) sceneManager.scenechange = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.Instance.stateUnpause();
        gameManager.Instance.playSFX();
    }
    public void quit()
    {
        Application.Quit();
        gameManager.Instance.playSFX();
    }

    public void respawnPlayer()
    {
        gameManager.Instance.stateUnpause();
        gameManager.Instance.playerScript.spawnPlayerminor();
        gameManager.Instance.playSFX();
    }

    public void mainMenu()
    {
        SceneManager.LoadScene(0);
        gameManager.currentlevel = gameManager.Levels.MainMenu;
        gameManager.Instance.stateUnpauseWithCursor();
        gameManager.Instance.playSFX();

    }
    public void startGame()
    {
        sceneManager.Instance.nextScene(1);
        screenManager.Instance.turnOffScreens();
        gameManager.Instance.playSFX();
    }


    public void levelButton()
    {
        screenManager.Instance.setScreen(2);
        gameManager.Instance.playSFX();
    }

    public void levelSelect(int level)
    {
        sceneManager.Instance.nextScene(level);
        
        if(level < 0)
            screenManager.Instance.turnOffScreens();

        if (Time.timeScale == 0) 
        {
            gameManager.Instance.stateUnpause();
        }
    }

    public void Back()
    {
        screenManager.Instance.setScreen(1);
        gameManager.Instance.playSFX();
    }

    public void settingsButton()
    {
        screenManager.Instance.setScreen(3);
        gameManager.Instance.playSFX();
    }

    public void Controlsbutton()
    {
        screenManager.Instance.setScreen(4);
        gameManager.Instance.playSFX();
    }

    public void BackControls()
    {
        screenManager.Instance.setScreen(3);
        gameManager.Instance.playSFX();
    }
}
