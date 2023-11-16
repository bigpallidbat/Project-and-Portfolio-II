using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    
    [SerializeField] GameObject _Credits1;
    [SerializeField] GameObject _Credits2;
    [SerializeField] GameObject _Credits3;
    [SerializeField] GameObject _Credits4;
    [SerializeField] GameObject TeamCredits;

    private void Start()
    {
        StartCoroutine(Credits());
        
    }


    IEnumerator Credits()
    {
        for(int i = 1; i < 5; i++)
        {
            HideCredits(i - 1);
            showCredits(i);
            yield return new WaitForSeconds(1.5f);
        }
        backtoMain();

        yield return null;

    }

    void showCredits(int credits)
    {
        if(credits == 1)
        {
            _Credits2.SetActive(true);
        }
        else if(credits == 2)
        {
            _Credits3.SetActive(true);
        }
        else if (credits == 3)
        {
            _Credits4.SetActive(true);
        }
    }
    
    void HideCredits(int credits)
    {
        if(credits == 0)
        {
            _Credits1.SetActive(false);
            TeamCredits.SetActive(false);
        }
        else if(credits == 1)
        {
            _Credits2.SetActive(false);
        }
        else if(credits == 2)
        {
            _Credits3.SetActive(false);
        }
        else if( credits == 3)
        {
            _Credits4.SetActive(false);
        }
    }

    void backtoMain()
    {
        SceneManager.LoadScene(0);
    }
}
