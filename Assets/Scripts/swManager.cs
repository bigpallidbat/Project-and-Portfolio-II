using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

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
    bool buff1;
    bool buff2;
    bool buff3;
    bool buff4;

    void Awake()
    {
        instance = this;
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
        GameObject temp = Instantiate(pickups[Random.Range(0, pickups.Count)], buffSpots[0].transform.position, transform.rotation);
        spawn(0);
        yield return new WaitForSeconds(timeBetweenSpawns);
    }

    IEnumerator wave2()
    {
        spawn(0);
        GameObject temp = Instantiate(pickups[Random.Range(0, pickups.Count)], buffSpots[1].transform.position, transform.rotation);
        yield return new WaitForSeconds(timeBetweenSpawns);
        spawn(0);
    }

    IEnumerator wave3()
    {
        spawn(0);
        GameObject temp = Instantiate(pickups[Random.Range(0, pickups.Count)], buffSpots[2].transform.position, transform.rotation);
        yield return new WaitForSeconds(timeBetweenSpawns);
        spawn(0);
    }

    IEnumerator wave4()
    {
        spawn(0);
        GameObject temp = Instantiate(pickups[Random.Range(0, pickups.Count)], buffSpots[3].transform.position, transform.rotation);
        yield return new WaitForSeconds(timeBetweenSpawns);
        spawn(0);
    }

    IEnumerator wave5()
    {
        spawn(0);
        GameObject temp = Instantiate(pickups[Random.Range(0, pickups.Count)], buffSpots[0].transform.position, transform.rotation);
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

    void spawnPickup(int ID, int spawner)
    {
        GameObject objectClone;

    }
}
