using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStrafeRange : MonoBehaviour
{
    [SerializeField] GameObject mainBody;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            mainBody.GetComponent<AlexsBossAI>().inStafingRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            mainBody.GetComponent<AlexsBossAI>().inStafingRange = false;
        mainBody.GetComponent<AlexsBossAI>().changeStrafe();
    }
}