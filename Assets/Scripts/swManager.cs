using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class swManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static swManager instance;

    [SerializeField] List<GameObject> enemies = new List<GameObject>();
    [SerializeField] GameObject spawner1;
    [SerializeField] GameObject spawner2;
    [SerializeField] GameObject spawner3;
    [SerializeField] GameObject spawner4;

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
        GameObject objectClone = Instantiate(enemies[0], spawner1.transform.position, transform.rotation);
        entList.Add(objectClone);
        objectClone.GetComponent<NavMeshAgent>().SetDestination(gameManager.Instance.player.transform.position);
        yield return new WaitForSeconds(timeBetweenSpawns);
    }

}
