using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [Header("----- Melee stats -----")]
    [SerializeField] int damage;
    [SerializeField] AudioSource sound;
    [SerializeField] AudioClip Hit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage damagable = other.GetComponent<IDamage>();

        if (damagable != null)
        {
            sound.PlayOneShot(Hit);
            damagable.takeDamage(damage);
        }
    }
}


