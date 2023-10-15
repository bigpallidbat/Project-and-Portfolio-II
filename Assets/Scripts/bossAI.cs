using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bossAI : MonoBehaviour
{
 
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] Collider hitBox;

    [Header("----- Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int targetFaceSpeed;
    [Range(30, 180)][SerializeField] int viewAngle;
    [SerializeField] int roamDistance;
    [SerializeField] int roamWaitTime;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;
    [Range(30, 180)][SerializeField] int shootAngle;

    float angleToPlayer;
    Vector3 playerDir;
    Color oColor;
    bool isShooting;
    bool playerInRange;
    bool destinationChosen;
    Vector3 startPos;
    float stoppingDistO;
    public Spawner origin;

    // Start is called before the first frame update
    void Start()
    {
        oColor = model.material.color;
        //gameManager.instance.updateGameGoal(1);
        startPos = transform.position;
        stoppingDistO = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
            if (playerInRange && !AiRoutine())
            {
                StartCoroutine(roam());
            }
            else if (!playerInRange)
            {
                StartCoroutine(roam());
            }
        }
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            gameManager.Instance.updateGameGoal(-1);
            //Destroy(gameObject);
            anim.SetBool("Death", true);
            agent.enabled = false;
            hitBox.enabled = false;
            StopAllCoroutines();

        }

        else
        {
            StartCoroutine(flashDamage());
            agent.SetDestination(gameManager.Instance.player.transform.position);
        }
    }

    IEnumerator flashDamage()
    {
        anim.SetTrigger("Damage");
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = oColor;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        anim.SetTrigger("Shoot");


        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }

    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    bool AiRoutine()
    {
        //find direction to player if in range
        playerDir = gameManager.Instance.player.transform.position - headPos.position;
        //find angle to player within distance
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDir, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit) && angleToPlayer <= viewAngle)
        {
            if (hit.collider.CompareTag("Player"))
            {
                agent.stoppingDistance = stoppingDistO;
                agent.SetDestination(gameManager.Instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                    faceTarget();

                if (angleToPlayer <= shootAngle && !isShooting)
                    StartCoroutine(shoot());
                return true;
            }

        }
        return false;
    }

    IEnumerator roam()
    {
        if (agent.remainingDistance < 0.05f && !destinationChosen)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamWaitTime);

            Vector3 randPos = Random.insideUnitSphere * roamDistance;
            randPos += startPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randPos, out hit, roamDistance, 1);
            agent.SetDestination(hit.position);



            destinationChosen = false;
        }
    }



}
