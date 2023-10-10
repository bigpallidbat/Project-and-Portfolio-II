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

    bool playerInRange;
    Color colorOrig;
    UnityEngine.Vector3 playerDir;

    void Start()
    {
    }

    void Update()
    {
        Debug.Log(agent.remainingDistance);
        if (playerInRange && canSeePlayer())
        { 
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
                if (agent.remainingDistance < agent.stoppingDistance && agent.remainingDistance > 0)
                {
                    explode();
                }

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
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
