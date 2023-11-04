using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Transform newLocation;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = new Vector3(newLocation.position.x, newLocation.position.y, newLocation.position.z);
            other.GetComponent<CharacterController>().enabled = true;
        }
    }
}
