using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftCheck : MonoBehaviour
{
    [SerializeField] GameObject mainBody;
    private void OnTriggerEnter(Collider other)
    {
        ;
        if (other.CompareTag("Wall"))
            mainBody.GetComponent<EnemyAI>().leftChecker = true;
    }

    private void OnTriggerExit(Collider other)
    {
       
        if (other.CompareTag("Wall"))
            mainBody.GetComponent<EnemyAI>().leftChecker = false;
    }
}
