using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class screenManager : MonoBehaviour
{
    public static screenManager Instance;

    [SerializeField] Image screenImg;
    [SerializeField] Image[] screenList;
    [SerializeField] Image background;
    [SerializeField] Animator anim;

    private bool check;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        if (gameManager.currentlevel == gameManager.Levels.MainMenu)
        {
            // Logorun();
            Debug.Log(gameManager.currentlevel);
        }
        
        
    }

    void Logorun()
    {
        background.gameObject.SetActive(true);
        screenImg = screenList[0];
        screenImg.gameObject.SetActive(true);
        StartCoroutine(Logo());
        
    }

    void mainMenu()
    {
        background.gameObject.SetActive(true);
        screenImg = screenList[1];
        screenImg.gameObject.SetActive(true);
    }

    IEnumerator Logo()
    {
        anim.Play("Logo");

        yield return new WaitForSeconds(4.25f);

        screenImg.enabled = false;
        yield return new WaitForSeconds(.5f);

        mainMenu();
    }

    public void turnOffScreens()
    {
        screenImg.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }

    public void setScreen(int screen)
    {
        screenImg.gameObject.SetActive(false);
        screenImg = screenList[screen];
        screenImg.gameObject.SetActive(true);
    }
}
