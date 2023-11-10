using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapController : MonoBehaviour
{
    [SerializeField] Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            anim.SetTrigger("Trap");
        }
    }
}
