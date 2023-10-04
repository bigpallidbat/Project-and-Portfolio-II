using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    [SerializeField] int nextSceneIndex;
    [SerializeField] public int doorNumber;
    [SerializeField] public Transform doorSpawn;

   private GameObject door;

    // Start is called before the first frame update
    void Start()
    {
        door = GetComponent<GameObject>();
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //SceneManager.LoadScene(nextSceneIndex);
            sceneManager.Instance.loadScene(nextSceneIndex, doorNumber, door);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
