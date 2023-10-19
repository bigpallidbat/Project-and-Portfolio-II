using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] GameObject explosion;
    public GameObject player;

    void Start()
    {
        //rb.velocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        StartCoroutine(explode());
    }
    void Update()
    {
        if (Input.GetButtonDown("g"))
        {
            ThrowGrenade();
        }
    }
    IEnumerator explode()
    {
        yield return new WaitForSeconds(destroyTime);
        Instantiate(explosion, transform.position, explosion.transform.rotation);
        Destroy(gameObject);
    }
    void ThrowGrenade()
    {
        // Calculate the player's position and direction
        Vector3 playerPosition = player.transform.position;
        Vector3 throwDirection = (playerPosition - transform.position).normalized;

        // Create a new instance of the grenade
        GameObject grenadeInstance = Instantiate(gameObject, transform.position, transform.rotation);

        // Access the Rigidbody component of the grenade and apply velocity
        Rigidbody grenadeRigidbody = grenadeInstance.GetComponent<Rigidbody>();
        grenadeRigidbody.velocity = throwDirection * speed;

        // Start the countdown for explosion
        grenadeInstance.GetComponent<grenade>().StartCoroutine(grenadeInstance.GetComponent<grenade>().explode());
    }
}
