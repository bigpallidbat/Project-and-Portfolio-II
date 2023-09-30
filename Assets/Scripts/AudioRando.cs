using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRando : MonoBehaviour
{
    public static void PlayRandomClip(AudioSource source, AudioClip[] clips, float pitchmin = 1f, float pitchmax = 1f)
    {
        if (clips.Length < 2)
        {
            source.Play();
        }// checks to make sure there is more then one.

        source.pitch = Random.Range(pitchmin, pitchmax);

        int randomClip = Random.Range(0, clips.Length);

        //this loop changes the sound. It cant leave the while loop if the clip it assigns is the same as the one playing
        while (source.clip == clips[randomClip])
        {
            randomClip = Random.Range(0, clips.Length);
        }

        source.clip = clips[randomClip];
        source.Play();
    }
}
