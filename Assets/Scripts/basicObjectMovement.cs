using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class basicObjectMovement : MonoBehaviour
{
    [SerializeField] AudioSource GameSounds;

    [SerializeField] float speed;
    [SerializeField] float destroyTime;
    [SerializeField] int damage;
    [SerializeField] float pingPongDis;
    [SerializeField] float timeBeforeSink;
    [SerializeField] AudioClip audObjMove;
    [Range(0, 1)][SerializeField] float audObjMoveVol;
    [SerializeField] bool damageIT;
    [SerializeField] bool basicMove;
    [SerializeField] bool pingPong;
    [SerializeField] bool pingPongRotated;
    [SerializeField] bool rockSink;
    [SerializeField] bool destroyOnContact;

    bool player = false;

    private Vector3 pointA;
    private Vector3 pointB;

    //public simpleObjectSpawner orgin;
    private void Start()
    {
        if (pingPongRotated == false)
        {
            pointA = transform.position;
            pointB = new Vector3(transform.position.x + pingPongDis, transform.position.y, transform.position.z);
        }
        else
        {
            pointA = transform.position;
            pointB = new Vector3(transform.position.x, transform.position.y, transform.position.z + pingPongDis);
        }
    }

    void Update()
    {
        if (pingPong == true)
        {
            PingPong();
        }
        else if (basicMove == true)
        {
            BasicMove();
            if (GameSounds != null)
            {
                GameSounds.PlayOneShot(audObjMove, audObjMoveVol);
            }
        }
        else if (rockSink == true && player == true)
        {
            StartCoroutine(RockSink());
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

        if (other.CompareTag("Player"))
        {
            player = true;
        }

        IDamage damagable = other.GetComponent<IDamage>();
        if (damagable != null && damageIT == true)
        {
            damagable.takeDamage(damage);
            if(destroyOnContact == true)
            {
                Destroy(gameObject);
            }
        }
    }

    private void PingPong()
    {
        float time = Mathf.PingPong(Time.time * speed, 1);
        transform.position = Vector3.Lerp(pointA, pointB, time);
    }

    private void BasicMove()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        StartCoroutine(Destroy());
    }

    IEnumerator RockSink()
    {
        yield return new WaitForSeconds(timeBeforeSink);
        transform.position += -transform.right * speed * Time.deltaTime;
        GameSounds.PlayOneShot(audObjMove , audObjMoveVol);
        StartCoroutine(Destroy());
    }
}
