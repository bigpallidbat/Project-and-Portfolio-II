using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    [SerializeField] int nextSceneIndex;
     public static int doorNumber;
     public int DN;
     public GameObject doorSpawn;    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorNumber = DN;
            sceneManager.Instance.loadScene(nextSceneIndex);
        }
    }


}
