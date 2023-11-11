using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicCheck : MonoBehaviour
{
    [SerializeField] bool isMimic;
    [SerializeField] GameObject closeMimcDoor;
    private void OnTriggerEnter(Collider other)
    {
        if (isMimic) if (other.CompareTag("Player"))
            {
                closeMimcDoor.GetComponent<DungeonDoor>().closeMimic();
                gameObject.SetActive(false);
            }
    }
}
