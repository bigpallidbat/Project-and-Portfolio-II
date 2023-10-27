using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weightedProjectile : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider Col;

    [Header("----- Projectile stats -----")]
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] int DestroyTime;
    [SerializeField] float velUp;
    [SerializeField] bool straight;

    Vector3 targetDir;
    private void Start()
    {
        StartCoroutine(timer());
    }
    IEnumerator timer()
    {
        if (straight) rb.velocity = (transform.forward + (Vector3.up * velUp)) * speed;
        else rb.velocity = (Vector3.up * velUp) + (gameManager.Instance.player.transform.position - transform.position).normalized * speed;
        yield return new WaitForSeconds(DestroyTime);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Col.enabled = false;
        yield return new WaitForSeconds(DestroyTime);
         Destroy(gameObject);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        //Debug.Log(other.gameObject.layer);

        if (other.gameObject.layer == 0)
        {
            //Destroy(gameObject);
        }

        IDamage damagable = other.GetComponent<IDamage>();

        if (damagable != null)
        {
            damagable.takeDamage(damage);
        }
        Destroy(gameObject);
    }

}
