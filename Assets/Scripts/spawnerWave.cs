using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnerWave : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int maxObjectToSpawn;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int timeBetweenSpawns;

    bool isSpawning;
    bool startSpawning;
    int numberOfObjectSpawned;
    int numberKilled;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager.Instance.updateGameGoal(maxObjectToSpawn); Commented Out to prevent script errors
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning && numberOfObjectSpawned < maxObjectToSpawn)
            StartCoroutine(spawn());
    }

    public IEnumerator spawn()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            int arrayPos = Random.Range(0, spawnPos.Length);
            GameObject objectSpawned = Instantiate(objectToSpawn, spawnPos[arrayPos].position, objectToSpawn.transform.rotation);

            objectSpawned.GetComponent<EnemyAI>().whereISpawned = this;

            numberOfObjectSpawned++;

            yield return new WaitForSeconds(timeBetweenSpawns);
            isSpawning = false;
        }
    }

    public void startWave()
    {
        startSpawning = true;
    }

    public void updateEnemyNumber()
    {
        numberKilled++;

        if(numberKilled >= maxObjectToSpawn)
        {
            startSpawning = false;
            StartCoroutine(waveManager.instance.startWave());
        }
    }
}
