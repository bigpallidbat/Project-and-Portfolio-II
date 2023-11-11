using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public double musicDuration;
    public double goalTime;
    [SerializeField] AudioClip[] _audioClips;
    [SerializeField] AudioSource music;
    int curLoop = 0;
    int dontPlay1;
    int dontPlay2;
    bool isPicking;

    //void OnPlayMusic()
    //{

    //    goalTime = AudioSettings.dspTime + 0.5;

    //    //audioSource.clip = currentClip;
    //    //audioSource.PlayScheduled(goalTime);

    //    musicDuration = (double)curClip.samples / curClip.frequency;
    //    goalTime = goalTime + musicDuration;

    //}

    private void Awake()
    {
      

    }

    void Start()
    {
        music.PlayOneShot(_audioClips[0]);
        musicDuration = (double)_audioClips[0].samples / _audioClips[0].frequency;
        if (_audioClips.Length >= 5)
        {
            dontPlay1 = _audioClips.Length - 2;
            dontPlay2 = _audioClips.Length - 1;
        }
        else if (_audioClips.Length == 4) dontPlay1 = _audioClips.Length - 1;
        goalTime = goalTime + musicDuration + AudioSettings.dspTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (AudioSettings.dspTime > goalTime - 1 && isPicking)
        {
            //isPicking = true;
            PlayNextLoop();
            //PlayScheduledClip();
        }
        else if (AudioSettings.dspTime > goalTime - 1 && !isPicking)
        {
            isPicking = true;
            music.PlayOneShot(_audioClips[1]);
            musicDuration = (double)_audioClips[1].samples / _audioClips[1].frequency;
            goalTime = goalTime + musicDuration;
        }
    }

    void PlayNextLoop()
    {
        if (isPicking)
        {
            switch (_audioClips.Length)
            {
                case 2:
                    music.PlayOneShot(_audioClips[1]);
                    break;
                case 3:
                    music.PlayOneShot(_audioClips[2]);
                    curLoop = 2;
                    break;
                case 4:
                    curLoop = Random.Range(2, _audioClips.Length);
                    music.PlayOneShot(_audioClips[curLoop]);
                    break;
                case 5:
                    do
                    {
                        curLoop = Random.Range(2, _audioClips.Length);
                    } while (curLoop == dontPlay1);
                    dontPlay1 = curLoop;
                    music.PlayOneShot(_audioClips[curLoop]);
                    break;
                default:
                    do
                    {
                        curLoop = Random.Range(2, _audioClips.Length);
                    } while (curLoop == dontPlay1 || curLoop == dontPlay2);
                    dontPlay2 = dontPlay1;
                    dontPlay1 = curLoop;
                    music.PlayOneShot(_audioClips[curLoop]);
                    break;
            }
        }
        musicDuration = (double)_audioClips[curLoop].samples / _audioClips[curLoop].frequency;
        goalTime = goalTime + musicDuration;
    }

    public void StopMusic()
    {
        goalTime += 100000;
        music.Stop();
    }

    //void resetGoalTime()
    //{

    //    goalTime = goalTime + musicDuration;
    //}
    //void PlayScheduledClip()
    //{
    //    _audioSource[audioToggle].clip = curClip;
    //    _audioClips[audioToggle].PlayScheduled(goalTime);
    //    music.

    //    musicDuration = (double)curClip.samples / curClip.frequency;
    //    goalTime = goalTime + musicDuration;

    //    audioToggle = 1 - audioToggle;
    //}

    //public void setCurrentClip(AudioClip clip)
    //{
    //    curClip = clip;
    //}
}
