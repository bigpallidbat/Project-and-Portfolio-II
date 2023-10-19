using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [Header("----- Melee stats -----")]
    [SerializeField] int damage;
    [SerializeField] AudioSource sound;
    [SerializeField] AudioClip Hit;
    [Range(0, 1)][SerializeField] float audHitVol;
    [SerializeField] bool usesBAttack;
    [SerializeField] GameObject enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage damagable = other.GetComponent<IDamage>();

        if (damagable != null)
        {
            sound.PlayOneShot(Hit, audHitVol);
            damagable.takeDamage(damage);
            if (usesBAttack && other.CompareTag("Player"))
            {
                enemy.GetComponent<EnemyAI>().friendly = true;
                enemy.GetComponent<EnemyAI>().startUnFriend();
            }
        }
    }
}


