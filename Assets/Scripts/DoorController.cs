using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    [SerializeField] int nextSceneIndex;

    // Start is called before the first frame update
    void Start()
    {

    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextSceneIndex);
            //sceneManager.Instance.loadScene(nextSceneIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
