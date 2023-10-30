using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beMyChild : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(gameObject.transform, true);
        }
    }

    //private void OnTrigger(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        other.gameObject.transform.SetParent(gameObject.transform, true);
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
          other.gameObject.transform.SetParent(null, true);
        }
    }
}
