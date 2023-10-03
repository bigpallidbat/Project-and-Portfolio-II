using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] string sceneName;

    // Start is called before the first frame update
    void Start()
    {

    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sceneManager.Instance.loadScene(sceneName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
