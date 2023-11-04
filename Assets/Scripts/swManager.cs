using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using TMPro;

public class swManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static swManager instance;

    [SerializeField] List<GameObject> enemies = new List<GameObject>();
    [SerializeField] List<GameObject> spawnerList = new List<GameObject>();

    [Header("---------- UI ----------")]
    [SerializeField] TMP_Text enemiesRemainingText;
    List<GameObject> entList = new List<GameObject>();
    private int waveCurrent = 0;
    private int enemiesRemaining;
    float timeBetweenSpawns = 0.3f;

    void Awake()
    {
        instance = this;
        startWave();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(entList.Count);
        //enemiesRemainingText.text = entList.Count.ToString();
    }

    void startWave()
    {
        waveCurrent++;
        switch (waveCurrent)
        {
            case 1:
                StartCoroutine(wave1());
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
        }
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

    void spawn(int enemyID)
    {
        GameObject objectClone;
        for (int i = 0; i < 4; i++)
        {
            objectClone = Instantiate(enemies[enemyID], spawnerList[i].transform.position, transform.rotation);
            entList.Add(objectClone);
            objectClone.GetComponent<NavMeshAgent>().SetDestination(gameManager.Instance.player.transform.position);
            enemiesRemaining++;
        }
    }

}
