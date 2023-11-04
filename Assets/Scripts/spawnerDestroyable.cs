using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnerDestroyable : MonoBehaviour, IDamage
{
    [SerializeField] List<GameObject> objectList = new List<GameObject>();
    [SerializeField] GameObject pUP;
    [SerializeField] GameObject pUpSpawn;
    [SerializeField] ParticleSystem onHit;
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int maxObjectsToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int HP;
    [SerializeField] GameObject portal;

    int curObjectsSpawned;
    bool isSpawning;
    bool startSpawning;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (startSpawning && curObjectsSpawned < maxObjectsToSpawn)
        {
            StartCoroutine(spawn());
        }
    }

    IEnumerator spawn()
    {
        if (!isSpawning)
        {
            isSpawning = true;

            curObjectsSpawned++;
            GameObject objectClone = Instantiate(objectToSpawn, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
            objectList.Add(objectClone);
            objectClone.GetComponent<EnemyAI>().origin = this;
            yield return new WaitForSeconds(timeBetweenSpawns);
            isSpawning = false;
        }
    }

    public void updateObjectNum()
    {
        curObjectsSpawned--;
        //maxObjectsToSpawn--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = false;

            for (int i = 0; i < objectList.Count; i++)
            {
                Destroy(objectList[i]);
            }
            objectList.Clear();

            curObjectsSpawned = 0;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        Instantiate(onHit, transform.position, Quaternion.identity);

        if (HP <= 0)
        {
            StartCoroutine(waitToDestroy());
        }
    }

    IEnumerator waitToDestroy()
    {
        gameManager.Instance.minorUpdateGoal(-1);
        Instantiate(pUP, pUpSpawn.transform.position, Quaternion.identity);
        portal.SetActive(true);
        yield return new WaitForSeconds(.3f);
        Destroy(gameObject);
    }
}
