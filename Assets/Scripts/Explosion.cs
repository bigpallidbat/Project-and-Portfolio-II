using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] int explosionDamage;
    [SerializeField] int explosionForce;
    [SerializeField] GameObject explosionEffect;

    private void Start()
    {
        Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation);
        Destroy(gameObject, 0.07f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //this will prevent triggers from affecting eachother
        if (other.isTrigger)
            return;

        IDamage damage = other.GetComponent<IDamage>();
        if (damage != null)
        {
            damage.takeDamage(0);
        }

        IPhysics physicsenable = other.GetComponent<IPhysics>();

        if (physicsenable != null)
        {
            physicsenable.physics((other.transform.position - transform.position).normalized * explosionForce);
        }
    }
}
