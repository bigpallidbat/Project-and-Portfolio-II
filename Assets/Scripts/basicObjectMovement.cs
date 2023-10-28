using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicObjectMovement : MonoBehaviour
{
    [SerializeField] int speed;
    [SerializeField] float destroyTime;
    [SerializeField] int damage;
    [SerializeField] bool damageIT;

    //public simpleObjectSpawner orgin;

    
    void Update()
    {
            transform.position += transform.forward * speed * Time.deltaTime;
            StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage damagable = other.GetComponent<IDamage>();
        if (damagable != null && damageIT == true)
        {
            damagable.takeDamage(damage);
        }
    }
}
