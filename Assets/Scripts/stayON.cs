using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stayON : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(gameObject.transform, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(null);
        }
    }
}
