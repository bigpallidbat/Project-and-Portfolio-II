using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightCheck : MonoBehaviour
{
    [SerializeField] GameObject mainBody;
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Wall"))
            mainBody.GetComponent<EnemyAI>().rightChecker = true;
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("Wall"))
            mainBody.GetComponent<EnemyAI>().rightChecker = false;
    }
}
