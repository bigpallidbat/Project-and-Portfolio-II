using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyBomb : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [Header("---- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] float explodeDist;


    bool chasePlayer;
    Color colorOrig;
    UnityEngine.Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        //colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (chasePlayer)
        {
            Debug.Log("IN RANGE");
        }
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.Instance.player.transform.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.Instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                    explode();

                return true;
            }
        }
        return false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(gameManager.Instance.player.transform.position);
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    void faceTarget()
    {
        UnityEngine.Quaternion rot = UnityEngine.Quaternion.LookRotation(playerDir);
        transform.rotation = UnityEngine.Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chasePlayer = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chasePlayer = false;
        }
    }

    void explode()
    {
        IDamage damageable = gameManager.Instance.player.GetComponent<IDamage>();

        if (damageable != null)
        {
            damageable.takeDamage(damage);
        }

        Destroy(gameObject);
    }    
}
