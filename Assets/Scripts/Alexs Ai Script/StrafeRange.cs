using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeRange : MonoBehaviour
{
    [SerializeField] GameObject mainBody;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            mainBody.GetComponent<EnemyAI>().inStafingRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            mainBody.GetComponent<EnemyAI>().inStafingRange = false;
        mainBody.GetComponent<EnemyAI>().changeStrafe();
    }
}
