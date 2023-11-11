using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Transform newLocation;
    [SerializeField] GameObject parent;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = new Vector3(newLocation.position.x, newLocation.position.y, newLocation.position.z);
            other.gameObject.transform.rotation = Quaternion.identity;
            other.GetComponent<CharacterController>().enabled = true;

            if (parent != null)
            {
                for (int i = 0; i < parent.transform.childCount; i++) {
                    if(parent.transform.GetChild(i).GetComponent<Teleporter>() == null)
                    {
                        if (parent.transform.GetChild(i).GetComponent<simpleObjectSpawner>() != null)
                        {
                            parent.transform.GetChild(i).GetComponent<simpleObjectSpawner>().CancelInvoke();
                        }
                        parent.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    
                   }
                parent.SetActive(false);
            }
        }
    }
}
