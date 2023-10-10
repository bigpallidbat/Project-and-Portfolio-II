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
    [SerializeField] int explosionRange;

    bool playerInRange;
    bool isExploding;
    Color colorOrig;
    UnityEngine.Vector3 playerDir;

    void Start()
    {
        colorOrig = model.material.color;
    }

    void Update()
    {
        if (!isExploding)
        {
            if (playerInRange && canSeePlayer())
            {
            }
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
                    StartCoroutine(explode());
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

    IEnumerator explode()
    {
        isExploding = true;
        agent.SetDestination(transform.position);

        model.material.color = Color.red;
        yield return new WaitForSeconds(.8F);
        model.material.color = colorOrig;
        yield return new WaitForSeconds(.7F);
        model.material.color = Color.red;
        yield return new WaitForSeconds(.6F);
        model.material.color = colorOrig;
        yield return new WaitForSeconds(.5F);
        model.material.color = Color.red;
        yield return new WaitForSeconds(.4F);
        model.material.color = colorOrig;
        yield return new WaitForSeconds(.3F);
        model.material.color = Color.red;
        yield return new WaitForSeconds(.2F);
        model.material.color = colorOrig;
        yield return new WaitForSeconds(.2F);
        model.material.color = Color.red;
        yield return new WaitForSeconds(.1F);
        model.material.color = colorOrig;
        yield return new WaitForSeconds(.1F);
        model.material.color = Color.red;
        applyDamage();
    }

    void applyDamage()
    {
        IDamage damageable = gameManager.Instance.player.GetComponent<IDamage>();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                float dist = Vector3.Distance(transform.position, gameManager.Instance.player.transform.position);
                if (dist < explosionRange && damageable != null)
                {
                    damageable.takeDamage(damage);
                }
            }
        }
        Destroy(gameObject);
    }
}
