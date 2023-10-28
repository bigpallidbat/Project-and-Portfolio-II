using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Bullet stats -----")]
     public int damage;
     public float speed;
    public float offsetX;
    public float offsetY;
    public float DestroyTime;
    [SerializeField] bool playerBullet;
    [SerializeField] bool nonAim;
    public Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        if (playerBullet) rb.velocity = dir * speed;
        else if (nonAim) rb.velocity = transform.forward * speed;
        else rb.velocity = (new Vector3(gameManager.Instance.player.transform.position.x + offsetX, gameManager.Instance.player.transform.position.y + offsetY, gameManager.Instance.player.transform.position.z + offsetY) - transform.position).normalized * speed;
        //Vector3.Angle();
        Destroy(gameObject, DestroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        Debug.Log(other.gameObject.layer);

        if (other.gameObject.layer == 0)
        {
            Destroy(gameObject);
        }

        IDamage damagable = other.GetComponent<IDamage>();

        if (damagable != null)
        {
            damagable.takeDamage(damage);
        }
        Destroy(gameObject);
    }
}
