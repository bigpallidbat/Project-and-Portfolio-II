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
    [SerializeField] float attackRange;
    [SerializeField] int dodgingSpeed;
    [SerializeField] int TargetFaceSpeed;
    [SerializeField] AudioClip painSound;
    [SerializeField] AudioClip deathSound;
    public AudioSource soundSFX;
    [SerializeField] float painSpeed;
    [SerializeField] int animChangeSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int shootAngle;
    [SerializeField] float animSpeed;//uncomment when needed

    [Header("----- Projectile States -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletSpeed;
    public bool isShooting = false;
    public bool inPain = false;
    bool playerInRange = false;
    float angleToPlayer;
    float stoppingDistOrig;
    //bool dead = false;

    Color Mcolor;
    // Start is called before the first frame update
    void Start()
    {
        Hp = MaxHp;
        soundSFX = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && CanSeePlayer() && !inPain)
        {
            if(checkTag())
                anim.SetTrigger("Attack");

        }
        else if (inPain) agent.SetDestination(transform.position);
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
        if (Physics.Raycast(headPos.position, PlayerDir, out hit) && angleToPlayer <= viewAngle)
        {
            if (hit.collider.CompareTag("Player"))
            {
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
    bool checkTag()
    {
        if(gameObject.CompareTag("lilChick"))
        { return true; }
        else { return false; }
    }
    IEnumerator Shoot()
    {
        isShooting = true;
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().damage = shootDamage;
        yield return new WaitForSeconds(fireRate);
        shootPos.transform.rotation = Quaternion.LookRotation(PlayerDir);
        Instantiate(bullet, shootPos.position, shootPos.transform.rotation);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        Hp -= amount;
        soundSFX.PlayOneShot(painSound);
        agent.SetDestination(gameManager.Instance.player.transform.position);
        if (Hp <= 0)
        {
            //dead = true;
            soundSFX.PlayOneShot(deathSound);
        mainBody.gameObject.SetActive(false);
        VoxelDamage.gameObject.SetActive(false);
        DeathOBJ.gameObject.SetActive(true);
            Quaternion Rot = Quaternion.LookRotation(PlayerDir);
            transform.rotation = Rot;
            Invoke("Death", 0.75f);

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
        if(checkTag())
            anim.SetTrigger("damaged");
        //anim.SetTrigger("damaged");
        Invoke("endPain", painSpeed);
        //model.material.color = Color.red;
        //model.material.color = Mcolor;
    }
    void endPain()
    {
        inPain = false;
        //anim.SetBool("inPain", false);
    }

    void FaceTarget()
    {
        Quaternion Rot = Quaternion.LookRotation(PlayerDir);
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

    public void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) playerInRange = true; }
    public void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) playerInRange = false; }
}
