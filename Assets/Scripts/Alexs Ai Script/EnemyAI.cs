using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] AudioClip painSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip attckSound;
    [SerializeField] AudioClip VpainSound;
    [SerializeField] AudioClip VdeathSound;
    [SerializeField] AudioClip seeSound;
    [SerializeField] AudioClip swosh;
    [SerializeField] SkinnedMeshRenderer mainBody;
    [SerializeField] GameObject mainBodyV;
    [SerializeField] GameObject VoxelDamage;
    [SerializeField] GameObject DeathOBJ;
    //[SerializeField] GameObject Player;
    [SerializeField] Animator anim;
    //[SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject EyeColor;
    [SerializeField] Material newMaterial;
    [SerializeField] GameObject leftCheck;
    [SerializeField] GameObject rightCheck;
    [SerializeField] Collider damageCOL;
    [SerializeField] Rigidbody RBody;
    Vector3 PlayerDir;
    float playerDist;

    [Header("----- Enemy States -----")]
    [SerializeField] int MaxHp;
    public int Hp;
    [SerializeField] bool friendly;
    [SerializeField] bool knowsPlayerLocation;
    [SerializeField] bool ambusher; // may get ride of
    [SerializeField] bool meleeOnly;
    [SerializeField] float meleeRange;
    [SerializeField] int strafingSpeed;
    [SerializeField] int TargetFaceSpeed;
    public AudioSource soundSFX;
    [SerializeField] int animChangeSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int shootAngle;
    //[SerializeField] float animSpeed;//uncomment when needed
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    public spawnerWave whereISpawned;
    public Spawner WhereISpawned;

    [Header("----- Projectile States -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletSpeed;
    [Range(0, 3)][SerializeField] float shotoffSet;
    [SerializeField] Collider hitBoxCOL;

    bool isAttacking = false;
    bool inPain;
    bool playerInRange = false;
    float angleToPlayer;
    float stoppingDistOrig;
    bool destinationChosen;
    //bool isStrafing = false;
    Vector3 StartingPos;
    bool foundPlayer = false;
    public bool leftChecker;
    public bool rightChecker;
    public bool inStafingRange;
    public bool goRight;
    bool bunnyFly;
    //bool dead = false;

    Color Mcolor;
    // Start is called before the first frame update
    void Start()
    {
        Hp = MaxHp;
        soundSFX = GetComponent<AudioSource>();
        StartingPos = transform.position;
        goRight = Random.Range(0, 2) == 0;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            if (anim != null) anim.SetFloat("speed", agent.velocity.normalized.magnitude);
            if (bunnyFly && mainBodyV.transform.position.y < 0.95f) mainBodyV.transform.position = new Vector3 (mainBodyV.transform.position.x, mainBodyV.transform.position.y + Time.deltaTime * 20, mainBodyV.transform.position.z);
            if (!friendly)
            {
                if (knowsPlayerLocation) agent.SetDestination(gameManager.Instance.player.transform.position);
                if (playerInRange && CanSeePlayer() && !inPain)
                {
                    if (!ambusher)
                    {
                        StartCoroutine(Roam());
                    }
                }
                else if (inPain) agent.SetDestination(transform.position);
                else if (!ambusher) StartCoroutine(Roam());
            }
            else StartCoroutine(Roam());
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
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle || hit.collider.CompareTag("Player") && foundPlayer)
            {

                if (!foundPlayer) found();
                agent.stoppingDistance = stoppingDistOrig;
                //if (playerDist < agent.stoppingDistance + 1) strafe();
                if (inStafingRange && !meleeOnly) StartCoroutine(strafe());
                else agent.SetDestination(gameManager.Instance.player.transform.position);
                if (agent.remainingDistance < agent.stoppingDistance)
                    FaceTarget();

                if (angleToPlayer <= shootAngle && !isAttacking)// && playerDist <= meleeRange)
                {
                    if (!meleeOnly) StartCoroutine(attack());
                    else if (playerDist <= meleeRange) StartCoroutine(attack());
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
    public void changeStrafe()
    {
        goRight = !goRight;
    }
    //void strafe()
    //{
    //    FaceTarget();
    //    Vector3 randomPos = Random.insideUnitSphere * 5;
    //    FaceTarget();
    //}

    IEnumerator strafe()
    {
        yield return null;

        //agent.stoppingDistance = 0;
        //isStrafing = true;
        if (agent.isActiveAndEnabled)
        {
            if (anim == null)
            {
                if (goRight)
                {
                    FaceTarget();
                    agent.SetDestination(rightCheck.transform.position);
                }
                else
                {
                    FaceTarget();
                    agent.SetDestination(leftCheck.transform.position);
                }
            }
            else
            {
                FaceTarget();
                agent.SetDestination(transform.position);
            }
        }
        //isStrafing = false;
        ///}
    }
    //void srafeLeft()
    //{
    //    FaceTarget();
    //    transform.position = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.forward.z + Time.deltaTime * -strafingSpeed);
    //}
    //void stafeRight()
    //{
    //    FaceTarget();
    //    transform.position = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.forward.z + Time.deltaTime * strafingSpeed);
    //}
    //bool checkTag()
    //{
    //    if (gameObject.CompareTag("lilChick"))
    //    { return true; }
    //    else { return false; }
    //}
    IEnumerator attack()
    {
        if (anim != null)
        {
            isAttacking = true;
            anim.SetTrigger("attack");
            soundSFX.PlayOneShot(attckSound);
        }
        else
        {
            isAttacking = true;
            bullet.GetComponent<Bullet>().speed = bulletSpeed;
            bullet.GetComponent<Bullet>().damage = shootDamage;
            bullet.GetComponent<Bullet>().offsetX = Random.Range(-shotoffSet, shotoffSet);
            bullet.GetComponent<Bullet>().offsetY = Random.Range(-shotoffSet, shotoffSet);
            Instantiate(bullet, shootPos.position, transform.rotation);
            //shootPos.transform.rotation = Quaternion.LookRotation(PlayerDir);
            yield return new WaitForSeconds(fireRate);
            isAttacking = false;
        }
    }
    public void playSwosh()
    {
        soundSFX.PlayOneShot(swosh);
    }
    public void stopedAttack()
    {
        isAttacking = false;
    }
    public void shoot()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    public void hitBoxOn()
    {
        hitBoxCOL.enabled = true;
    }

    public void hitBoxOff()
    {
        hitBoxCOL.enabled = false;
    }

    public void redEyes()
    {
        EyeColor.GetComponent<SkinnedMeshRenderer>().material = newMaterial;
        RBody.useGravity = false;
        knowsPlayerLocation = true;
        agent.speed *= 3;
        agent.acceleration *= 3;
        agent.angularSpeed *= 3;
        bunnyFly = true;
    }
    void found()
    {
        soundSFX.PlayOneShot(seeSound);
        foundPlayer = true;
    }

    public void takeDamage(int amount)
    {
        Hp -= amount;
        if (hitBoxCOL != null) hitBoxCOL.enabled = false;
        soundSFX.PlayOneShot(VpainSound);
        if (painSound != null) soundSFX.PlayOneShot(painSound);

        if (Hp <= 0)
        {
            FaceTarget();
            //GetComponent<CapsuleCollider>().enabled = false;
            //GetComponent<NavMeshAgent>().enabled = false;

            soundSFX.PlayOneShot(VdeathSound);
            if (deathSound != null) soundSFX.PlayOneShot(deathSound);
            if (mainBody != null) mainBody.enabled = false;
            else mainBodyV.gameObject.SetActive(false);
            VoxelDamage.gameObject.SetActive(false);
            DeathOBJ.gameObject.SetActive(true);
            agent.enabled = false;
            damageCOL.enabled = false;
            StopAllCoroutines();
            Quaternion Rot = Quaternion.LookRotation(PlayerDir);
            transform.rotation = Rot;
            //StartCoroutine(Death());
            Invoke("Death", 0.8f);


            if (WhereISpawned != null)
            {
                whereISpawned.updateEnemyNumber();
                WhereISpawned.heyIDied();
                //gameManager.Instance.updateGameGoal(-1);
            }
        }
        else
            StartCoroutine(FlashDamage());

    }
    IEnumerator FlashDamage()
    {
        inPain = true;
        //if (checkTag()) anim.SetBool("inPain", true);
        if (mainBody != null) mainBody.enabled = false;
        else mainBodyV.gameObject.SetActive(false);
        VoxelDamage.gameObject.SetActive(true);
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(0.085714f);
        VoxelDamage.gameObject.SetActive(false);
        if (mainBody != null) mainBody.enabled = true;
        else mainBodyV.gameObject.SetActive(true);
        if (anim != null) anim.SetTrigger("pain");
        else Invoke("endPain", 0.142857f);

    }
    public void endPain()
    {
        inPain = false;
        agent.SetDestination(gameManager.Instance.player.transform.position);
        //if (checkTag()) anim.SetBool("inPain", false);
    }

    void FaceTarget()
    {
        Quaternion Rot = Quaternion.LookRotation(PlayerDir);
        //transform.rotation = Rot; snap code
        transform.rotation = Quaternion.Lerp(transform.rotation, Rot, Time.deltaTime * TargetFaceSpeed);
    }
    public void Death()
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
        if (other.CompareTag("Player")) playerInRange = true;
    }
    public void OnTriggerExit(Collider other)
    {
        //foundPlayer = false;
        if (other.CompareTag("Player")) playerInRange = false;
        agent.stoppingDistance = 0;
    }

}

