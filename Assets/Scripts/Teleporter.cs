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
            other.GetComponent<CharacterController>().enabled = true;
            parent.SetActive(false);
        }
    }
}
