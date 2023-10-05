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
        background.gameObject.SetActive(true); 
        SceneCheck();
    }

    void SceneCheck()
    {
        
        
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {       

            screenImg = screenList[0];
            screenImg.gameObject.SetActive(true);
            StartCoroutine(Logo());
        }
        else if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            screenImg = screenList[1];
            screenImg.gameObject.SetActive(true);
        }
    }

    IEnumerator Logo()
    {
        anim.Play("Logo");

        yield return new WaitForSeconds(4.25f);

        screenImg.enabled = false;
        yield return new WaitForSeconds(.5f);
        sceneManager.Instance.nextScene(1);
    }




}
