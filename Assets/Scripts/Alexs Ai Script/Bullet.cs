using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Bullet stats -----")]
    [SerializeField] bool isHoming;
    [SerializeField] bool playerBullet;
    [SerializeField] bool nonAim;
    public int damage;
    public float speed;
    public float DestroyTime;
    public float offsetX;
    public float offsetY;
    public Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        if (!isHoming)
        {
            if (playerBullet) rb.velocity = dir * speed;
            else if (nonAim) rb.velocity = (new Vector3(transform.forward.x + offsetX, transform.forward.y + offsetY, transform.forward.z)).normalized * speed;
            else rb.velocity = (new Vector3(gameManager.Instance.player.transform.position.x + offsetX, gameManager.Instance.player.transform.position.y + offsetY, gameManager.Instance.player.transform.position.z + offsetY) - transform.position).normalized * speed;
            //Vector3.Angle();
        }
        Destroy(gameObject, DestroyTime);
    }
    void Update()
    {
        if (isHoming) rb.velocity = (gameManager.Instance.player.transform.position - transform.position).normalized * Time.deltaTime * speed;
    }
    //public void setTargetDir(Vector3 dir)
    //{
    //    targetDir = dir;
    //}

    public void setBulletStats(int newDamage = 1, float newSpeed = 20, int newDestyTime = 10, float offset = 0)
    {
        damage = newDamage;
        speed = newSpeed;
        DestroyTime = newDestyTime;
        offsetX = Random.Range(-offset, offset);
        offsetY = Random.Range(-offset, offset);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

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
