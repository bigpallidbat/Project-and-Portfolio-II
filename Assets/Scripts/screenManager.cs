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
    [SerializeField] RectTransform Settings;
    //Vector3 newpos = new Vector3(0,0,0);

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
           
        }
    }

    public void setSettings()
    {
        StartCoroutine(SettinsSettings());
    }
    IEnumerator SettinsSettings()
    {
        new WaitForSeconds(0.004f);
        screenImg = screenList[3];
        screenImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.0001f);
        screenImg.gameObject.SetActive(false);
        Settings.anchoredPosition = Vector2.zero;
        screenImg = null;
    }


    //run settings for milisecond to set audio prefs
    IEnumerator settingsrun()
    {
        screenImg = screenList[3];
        screenImg.gameObject.SetActive(true);
        new WaitForSeconds(0.3f);
        screenImg.gameObject.SetActive(false);
        screenImg = null;
        yield return new WaitForSeconds(0.01f);
        Logorun();
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

        screenImg.gameObject.SetActive(false);
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
        if (screenImg != null)
        {
            screenImg.gameObject.SetActive(false);
        }
        screenImg = screenList[screen];
        screenImg.gameObject.SetActive(true);
    }
}
