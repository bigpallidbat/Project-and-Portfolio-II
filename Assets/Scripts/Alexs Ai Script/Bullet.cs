using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Bullet stats -----")]
     public int damage;
     public int speed;
    public float offsetX;
    public float offsetY;
    [SerializeField] float DestroyTime;

    // Start is called before the first frame update
    void Start()
    {

        rb.velocity = (new Vector3(gameManager.Instance.player.transform.position.x + offsetX, gameManager.Instance.player.transform.position.y, gameManager.Instance.player.transform.position.z + offsetY) - transform.position).normalized * speed;
        //Vector3.Angle();
        Destroy(gameObject, DestroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage damagable = other.GetComponent<IDamage>();

        if (damagable != null)
        {
            damagable.takeDamage(damage);
        }
        Destroy(gameObject);
    }
}
