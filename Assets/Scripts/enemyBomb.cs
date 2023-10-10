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
    [SerializeField] int explodeRange;

    bool playerInRange;
    bool isExploding;
    Color colorOrig;
    UnityEngine.Vector3 playerDir;

    void Start()
    {
        //colorOrig = model.material.color;
    }

    void Update()
    {
        Debug.Log(model.material.color);
        model.material.color = Color.red;

        //model.material.SetColor("_Color", Color.red);
        //if (!isExploding)
        //{
        //    if (playerInRange && canSeePlayer())
        //    {
        //    }
        //}
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
        isExploding = true;
        agent.stoppingDistance = 20;

        //model.material.color = Color.red;
        //new WaitForSeconds(1);
        //model.material.color = colorOrig;
        //new WaitForSeconds(0.9F);
        //model.material.color = Color.red;
        //new WaitForSeconds(0.8F);
        //model.material.color = colorOrig;
        //new WaitForSeconds(0.7F);
        //model.material.color = Color.red;
        //new WaitForSeconds(0.6F);
        //model.material.color = colorOrig;
        //new WaitForSeconds(0.5F);
        //model.material.color = Color.red;
        //new WaitForSeconds(0.4F);
        //model.material.color = colorOrig;
        //new WaitForSeconds(0.3F);
        //model.material.color = Color.red;
        //new WaitForSeconds(0.2F);
        //model.material.color = colorOrig;
        //new WaitForSeconds(0.1F);
        //model.material.color = Color.red;
        //new WaitForSeconds(0.05F);
        //model.material.color = colorOrig;
        //new WaitForSeconds(0.05F);
        //model.material.color = Color.red;
        //applyDamage();
    }

    void applyDamage()
    {
        IDamage damageable = gameManager.Instance.player.GetComponent<IDamage>();

        if (damageable != null)
        {
            damageable.takeDamage(damage);
        }

        Destroy(gameObject);
    }
}
