using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int maxObjectToSpawn;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] List<GameObject> objectList = new List<GameObject>();

    bool isSpawning;
    bool startSpawning;
    int numberOfObjectSpawned;

    //  remove gamemanager gamegoal in enemyAI

    // Start is called before the first frame update
    void Start()
    {
        //gameManager.Instance.updateGameGoal(maxObjectToSpawn);
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
            if (objectSpawned.GetComponent<EnemyAI>() != null)
            {
                objectSpawned.GetComponent<EnemyAI>().WhereISpawned = this;
            }
            else if(objectSpawned.GetComponent<enemyBomb>() != null)
            {
                objectSpawned.GetComponent<enemyBomb>().origin = this;
            }
            else if(objectSpawned.GetComponent<itemPickup>() != null)
            {
                objectSpawned.GetComponent<itemPickup>().orgin = this;
            }

            objectList.Add(objectSpawned);
            numberOfObjectSpawned++;

            yield return new WaitForSeconds(timeBetweenSpawns);
            isSpawning = false;
        }
    }

    public void heyIDied()
    {
        numberOfObjectSpawned--;
        //maxObjectToSpawn--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            startSpawning = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = false;
        }
        
        for(int i = 0; i < objectList.Count; i++)
        {
            Destroy(objectList[i]);
        }
        objectList.Clear();
        numberOfObjectSpawned = 0;
    }
}
