using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    // Added Multiplier for arc
    [SerializeField] int upMult;
    [SerializeField] Rigidbody rb;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] GameObject explosion;
    public GameObject player;

    void Start()
    {
        //rb.velocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;

        //grenade explodes on start.- Chance
        //StartCoroutine(explode());
    }
    void Update()
    {

        // Error use Grenade not g - Chance
        // Also The player throws the grenade
        //if (Input.GetButtonDown("g"))
        //{
        //    ThrowGrenade();
        //}
    }
    IEnumerator explode()
    {
        yield return new WaitForSeconds(destroyTime);
        Instantiate(explosion, transform.position, explosion.transform.rotation);
        Destroy(gameObject);
    }
    public void ThrowGrenade()
    {
        // Calculate the player's position and direction
        Vector3 playerPosition = player.transform.position;

        // Bug: Need the grenade to move forward from the player, This causes it to move in a direction according to prefab position
        Vector3 throwDirection = (playerPosition - transform.position).normalized;


        // Create a new instance of the grenade
        GameObject grenadeInstance = Instantiate(gameObject, player.transform.position, Quaternion.identity);

        //enable Collider to keep physics -CHance
        grenadeInstance.GetComponent<SphereCollider>().enabled = true;

        // Access the Rigidbody component of the grenade and apply velocity
        Rigidbody grenadeRigidbody = grenadeInstance.GetComponent<Rigidbody>();

        //enable gravity, had to disable it for the pickups, need to make a pickup model. -Chance
        grenadeRigidbody.useGravity = true;

        //throw
        grenadeRigidbody.velocity = (Vector3.up * upMult) + (throwDirection) * speed;

        // Start the countdown for explosion
        grenadeInstance.GetComponent<grenade>().StartCoroutine(grenadeInstance.GetComponent<grenade>().explode());
    }
}
