using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class basicObjectMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float destroyTime;
    [SerializeField] int damage;
    [SerializeField] float pingPongDis;
    [SerializeField] bool damageIT;
    [SerializeField] bool pingPong;

    private Vector3 pointA;
    private Vector3 pointB;

    //public simpleObjectSpawner orgin;
    private void Start()
    {
        pointA = transform.position;
        pointB = new Vector3(transform.position.x + pingPongDis, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (pingPong == true)
        {
            float time = Mathf.PingPong(Time.time * speed, 1);
            transform.position = Vector3.Lerp(pointA, pointB, time);
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            StartCoroutine(Destroy());
        }
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
