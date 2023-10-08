using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject mainBody;
    [SerializeField] GameObject VoxelDamage;
    [SerializeField] GameObject DeathOBJ;
    //[SerializeField] GameObject Player;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    Vector3 PlayerDir;
    float playerDist;

    [Header("----- Enemy States -----")]
    [SerializeField] int MaxHp;
    public int Hp;
    [SerializeField] bool knowsPlayerLocation;
    [SerializeField] bool ambusher;
    [SerializeField] float attackRange;
    [SerializeField] int dodgingSpeed;
    [SerializeField] int TargetFaceSpeed;
    [SerializeField] AudioClip painSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip VpainSound;
    [SerializeField] AudioClip VdeathSound;
    [SerializeField] AudioClip seeSound;
    public AudioSource soundSFX;
    [SerializeField] float painSpeed;
    [SerializeField] int animChangeSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int shootAngle;
    [SerializeField] float animSpeed;//uncomment when needed
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;

    [Header("----- Projectile States -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletSpeed;
    [SerializeField] float shotoffSet;
    public bool isShooting = false;
    public bool inPain = false;
    bool playerInRange = false;
    float angleToPlayer;
    float stoppingDistOrig;
    bool destinationChosen;
    Vector3 StartingPos;
    bool foundPlayer = false;

    //bool dead = false;

    Color Mcolor;
    // Start is called before the first frame update
    void Start()
    {
        Hp = MaxHp;
        soundSFX = GetComponent<AudioSource>();
        StartingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (knowsPlayerLocation) agent.SetDestination(gameManager.Instance.player.transform.position);
        else if (playerInRange && CanSeePlayer() && !inPain)
        {
            if (checkTag())
                anim.SetTrigger("Attack");
            if (!ambusher)
            {
                StartCoroutine(Roam());
            }

        }
        else if (inPain) agent.SetDestination(transform.position);
        else StartCoroutine(Roam());
        //if (isShooting) anim.SetBool("attacking", true); 
        //else anim.SetBool("attacking", false);
        if (checkTag())
        {
            if (inPain) anim.SetBool("inPain", true);
            else anim.SetBool("inPain", false);
            if (agent.velocity != Vector3.zero) anim.SetBool("isMoving", true);
            else anim.SetBool("isMoving", false);
        }
    }
    bool CanSeePlayer()
    {
        PlayerDir = gameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(PlayerDir, transform.forward);
        playerDist = Vector3.Distance(gameManager.Instance.player.transform.position, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, PlayerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                if (!foundPlayer) found();
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(gameManager.Instance.player.transform.position);
                if (agent.remainingDistance < agent.stoppingDistance)
                    FaceTarget();

                if (angleToPlayer <= shootAngle && !isShooting && playerDist <= attackRange)
                {
                    StartCoroutine(Shoot());
                }
                return true;
            }
        }
        return false;
    }

    IEnumerator Roam()
    {
        if (agent.remainingDistance < 0.05f && !destinationChosen)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamPauseTime);
            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += StartingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);

            destinationChosen = false;
        }

    }
    bool checkTag()
    {
        if (gameObject.CompareTag("lilChick"))
        { return true; }
        else { return false; }
    }
    IEnumerator Shoot()
    {
        isShooting = true;
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().damage = shootDamage;
        bullet.GetComponent<Bullet>().offsetX = Random.Range(shotoffSet * -1, shotoffSet);
        bullet.GetComponent<Bullet>().offsetY = Random.Range(shotoffSet * -1, shotoffSet);
        yield return new WaitForSeconds(fireRate);
        shootPos.transform.rotation = Quaternion.LookRotation(PlayerDir);
        Instantiate(bullet, shootPos.position, shootPos.transform.rotation);
        isShooting = false;
    }
    void found()
    {
        soundSFX.PlayOneShot(seeSound);
        foundPlayer = true;
    }

    public void takeDamage(int amount)
    {
        Hp -= amount;
        soundSFX.PlayOneShot(VpainSound);
        if (checkTag()) soundSFX.PlayOneShot(painSound);

        if (Hp <= 0)
        {
            //dead = true;
            GetComponent<CapsuleCollider>().enabled = false;
            //GetComponent<NavMeshAgent>().enabled = false;

            soundSFX.PlayOneShot(VdeathSound);
            if (checkTag()) soundSFX.PlayOneShot(deathSound);
            mainBody.gameObject.SetActive(false);
            VoxelDamage.gameObject.SetActive(false);
            DeathOBJ.gameObject.SetActive(true);
            Quaternion Rot = Quaternion.LookRotation(PlayerDir);
            transform.rotation = Rot;
            Invoke("Death", 0.8f);

            //gameManager.Instance.updateGameGoal(-1);
        }
        else
            StartCoroutine(FlashDamage());
    }
    IEnumerator FlashDamage()
    {
        inPain = true;
        //anim.SetBool("inPain", true);
        mainBody.gameObject.SetActive(false);
        VoxelDamage.gameObject.SetActive(true);
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(0.085714f);
        VoxelDamage.gameObject.SetActive(false);
        mainBody.gameObject.SetActive(true);
        if (checkTag())
            anim.SetTrigger("damaged");
        //anim.SetTrigger("damaged");
        Invoke("endPain", painSpeed);
        //model.material.color = Color.red;
        //model.material.color = Mcolor;
    }
    void endPain()
    {
        inPain = false;
        agent.SetDestination(gameManager.Instance.player.transform.position);
        //anim.SetBool("inPain", false);
    }

    void FaceTarget()
    {
        Quaternion Rot = Quaternion.LookRotation(PlayerDir);
        //transform.rotation = Rot; snap code
        transform.rotation = Quaternion.Lerp(transform.rotation, Rot, Time.deltaTime * TargetFaceSpeed);
    }
    void Death()
    {
        Destroy(gameObject);
    }

    /*public void physics(Vector3 dir)
    {
        agent.velocity += dir;   // uncomment when need
    }*/

    public void OnTriggerEnter(Collider other)
    {
        //soundSFX.PlayOneShot(seeSound);
        //soundSFX.PlayOneShot(seeSound);
        if (other.CompareTag("Player")) playerInRange = true;
    }
    public void OnTriggerExit(Collider other)
    {
        foundPlayer = false;
        if (other.CompareTag("Player")) playerInRange = false;
        agent.stoppingDistance = 0;
    }
}
