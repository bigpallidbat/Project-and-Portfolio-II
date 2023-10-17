using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSoundInTrigger : MonoBehaviour
{
    [SerializeField] AudioSource sound;
    //[SerializeField] bool something;
    [SerializeField] AudioClip enterSound;
    [SerializeField] float enterTime;
    [SerializeField] AudioClip exitSound;
    [SerializeField] float exitTime;
    bool isPlaying;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (enterSound != null) StartCoroutine(PlayEnter());
    }
    private void OnTriggerExit(Collider other)
    {
        if (exitSound != null) StartCoroutine(PlayExit());
    }

    IEnumerator PlayEnter()
    {
        if (isPlaying)
        {
            sound.PlayOneShot(enterSound);
            isPlaying = true;
            yield return new WaitForSeconds(enterTime);
            isPlaying = false;
        }
        else yield return null;
    }

    IEnumerator PlayExit()
    {
        if (isPlaying)
        {
            sound.PlayOneShot(exitSound);
            isPlaying = true;
            yield return new WaitForSeconds(exitTime);
            isPlaying = false;
        }
        else yield return null;
    }
}
