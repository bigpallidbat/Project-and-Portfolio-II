using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveManager : MonoBehaviour
{
    public static waveManager instance;

    [SerializeField] spawnerWave[] spawners;
    [SerializeField] int timeBetweenWaves;

    public int waveCurrent;

    void Awake()
    {
        instance = this;
        StartCoroutine(startWave());
    }

    public IEnumerator startWave()
    {
        waveCurrent++;

        if (waveCurrent <= spawners.Length)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            spawners[waveCurrent - 1].startWave();
        }
        else
            StartCoroutine(gameManager.Instance.youWin());
    }
}
