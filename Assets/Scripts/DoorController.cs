using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    [SerializeField] int nextSceneIndex;
     public static int doorNumber;
     public int DN;
     public Transform doorSpawn;

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
            doorNumber = DN;
            sceneManager.Instance.loadScene(nextSceneIndex);
        }
    }


}
