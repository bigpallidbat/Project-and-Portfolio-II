using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class swManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static swManager instance;

    [SerializeField] List<GameObject> enemies = new List<GameObject>();
    [SerializeField] List<GameObject> spawnerList = new List<GameObject>();
    [SerializeField] List<GameObject> buffSpots = new List<GameObject>();
    [SerializeField] List<ParticleSystem> buffParticles = new List<ParticleSystem>();
    [SerializeField] List<GameObject> pickups = new List<GameObject>();

    [SerializeField] TMP_Text WaveStartText;
    [SerializeField] TMP_Text WaveDisplayText;

    [Header("---------- UI ----------")]
    List<GameObject> entList = new List<GameObject>();
    private int waveCurrent = 0;
    private int waveMax = 5;
    private int enemiesRemaining;
    float timeBetweenSpawns = 0.3f;

    bool spawning;

    bool spawnBuff = true;
    GameObject pickup1;
    GameObject pickup2;
    GameObject pickup3;
    GameObject pickup4;

    void Awake()
    {
        instance = this;
        buffParticles[0].Pause();
        buffParticles[1].Pause();
        buffParticles[2].Pause();
        buffParticles[3].Pause();
    }

    private void Start()
    {
        gameManager.Instance.setWaveMax(waveMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawning && gameManager.Instance.getEnemiesRemaining() == 0)
        {
            StartCoroutine(startWave());
        }

        if (spawnBuff)
        {
            StartCoroutine(spawnPickup());
        }

        if (buffParticles[0].isPlaying && pickup1 == null)
        {
            //buffParticles[0].Pause();
            buffParticles[0].Stop();
        }
        if (buffParticles[1].isPlaying && pickup2 == null)
        {
            //buffParticles[1].Pause();
            buffParticles[1].Stop();
        }
        if (buffParticles[2].isPlaying && pickup3 == null)
        {
            //buffParticles[2].Pause();
            buffParticles[2].Stop();
        }
        if (buffParticles[3].isPlaying && pickup4 == null)
        {
            //buffParticles[3].Pause();
            buffParticles[3].Stop();
        }
        //Debug.Log(gameManager.Instance.getEnemiesRemaining());
        //Debug.Log(entList.Count);
        //Debug.Log(SceneManager.GetActiveScene().buildIndex);
        //enemiesRemainingText.text = entList.Count.ToString();
    }

    IEnumerator startWave()
    {
        spawning = true;
        // display: ~WAVE #~
        waveCurrent++;
        WaveDisplayText.text = waveCurrent.ToString();
        WaveStartText.enabled = true;
        WaveDisplayText.enabled = true;
        gameManager.Instance.setWaveCur(waveCurrent);
        switch (waveCurrent)
        {
            case 1:
                StartCoroutine(wave1());
                break;
            case 2:
                StartCoroutine(wave2());
                break;
            case 3:
                StartCoroutine(wave3());
                break;
            case 4:
                StartCoroutine(wave4());
                break;
            case 5:
                StartCoroutine(wave5());
                break;
        }
        yield return new WaitForSeconds(5);
        spawning = false;
        WaveStartText.enabled = false;
        WaveDisplayText.enabled = false;
    }

    IEnumerator wave1()
    {
        spawn(0);
        yield return new WaitForSeconds(timeBetweenSpawns);
    }

    IEnumerator wave2()
    {
        spawn(0);
        yield return new WaitForSeconds(timeBetweenSpawns);
        spawn(0);
    }

    IEnumerator wave3()
    {
        spawn(0);
        yield return new WaitForSeconds(timeBetweenSpawns);
        spawn(0);
    }

    IEnumerator wave4()
    {
        spawn(0);
        yield return new WaitForSeconds(timeBetweenSpawns);
        spawn(0);
    }

    IEnumerator wave5()
    {
        spawn(0);
        yield return new WaitForSeconds(timeBetweenSpawns);
        spawn(0);
    }

    void spawn(int enemyID)
    {
        GameObject objectClone;
        for (int i = 0; i < 4; i++)
        {
            objectClone = Instantiate(enemies[enemyID], spawnerList[i].transform.position, transform.rotation);
            entList.Add(objectClone);
            objectClone.GetComponent<NavMeshAgent>().SetDestination(gameManager.Instance.player.transform.position);
            objectClone.GetComponent<EnemyAI>().huntDownPlayer();
            gameManager.Instance.setEnemiesRemaining(gameManager.Instance.getEnemiesRemaining() + 1);
        }
    }

    void spawn(int enemyID, int spawner)
    {
        GameObject objectClone;
        objectClone = Instantiate(enemies[enemyID], spawnerList[spawner].transform.position, transform.rotation);
        entList.Add(objectClone);
        objectClone.GetComponent<NavMeshAgent>().SetDestination(gameManager.Instance.player.transform.position);
        objectClone.GetComponent<EnemyAI>().huntDownPlayer();
        gameManager.Instance.setEnemiesRemaining(gameManager.Instance.getEnemiesRemaining() + 1);
    }

    IEnumerator spawnPickup()
    {
        spawnBuff = false;
        bool spawned = false;
        while (!spawned)
        {
            if (pickup1 != null && pickup2 != null && pickup3 != null && pickup4 != null)
                break;
            int buffSpot = Random.Range(1, 5);
            //Debug.Log(buffSpot);
            if (buffSpot == 1 && pickup1 == null)
            {
                pickup1 = Instantiate(pickups[Random.Range(0, pickups.Count)], buffSpots[0].transform.position, transform.rotation);
                buffParticles[0].Play();
                spawned = true;
            }
            else if (buffSpot == 2 && pickup2 == null)
            {
                pickup2 = Instantiate(pickups[Random.Range(0, pickups.Count)], buffSpots[1].transform.position, transform.rotation);
                buffParticles[1].Play();
                spawned = true;
            }
            else if (buffSpot == 3 && pickup3 == null)
            {
                pickup3 = Instantiate(pickups[Random.Range(0, pickups.Count)], buffSpots[2].transform.position, transform.rotation);
                buffParticles[2].Play();
                spawned = true;
            }
            else if (buffSpot == 4 && pickup4 == null)
            {
                pickup4 = Instantiate(pickups[Random.Range(0, pickups.Count)], buffSpots[3].transform.position, transform.rotation);
                buffParticles[3].Play();
                spawned = true;
            }
        }
        yield return new WaitForSeconds(120);
        spawnBuff = true;
    }
}
