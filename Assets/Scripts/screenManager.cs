using JetBrains.Annotations;
using System;
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
    [SerializeField] AudioSource themes;
    [SerializeField] AudioClip MainTheme;
    [Range(0, 1)][SerializeField] float audMainVol;

    private bool check;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        
        
        
    }
    private void Start()
    {
        if (gameManager.currentlevel == gameManager.Levels.MainMenu)
        {
            Logorun();
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
        themes.loop = true;
        themes.PlayOneShot(MainTheme);
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
        themes.loop = false;
        themes.Stop();
    }

    public void setScreen(int screen)
    {
        screenImg.gameObject.SetActive(false);
        screenImg = screenList[screen];
        screenImg.gameObject.SetActive(true);
    }
}
