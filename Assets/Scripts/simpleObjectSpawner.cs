using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleObjectSpawner : MonoBehaviour
{
    [SerializeField] Transform[] spawnPos;
    [SerializeField] GameObject objectToSpawn;

    [SerializeField] float startSpawnTime;
    [SerializeField] float startNewSpawnTime;
    

    void Start()
    {
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating("Spawn", startSpawnTime, startNewSpawnTime);
    }

    void Update()
    {
        
    }

    void Spawn()
    {
        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        Instantiate(objectToSpawn, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
    }
}
