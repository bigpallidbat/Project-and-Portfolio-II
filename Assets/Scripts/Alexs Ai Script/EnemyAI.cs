using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] ParticleSystem effect;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] AudioClip painSound;
    [Range(0, 1)][SerializeField] float audPainVol;
    [SerializeField] AudioClip deathSound;
    [Range(0, 1)][SerializeField] float audDeathVol;
    [SerializeField] AudioClip attckSound;
    [Range(0, 1)][SerializeField] float audAttackVol;
    [SerializeField] AudioClip VpainSound;
    [Range(0, 1)][SerializeField] float audVpainVol;
    [SerializeField] AudioClip VdeathSound;
    [Range(0, 1)][SerializeField] float audVdeathVol;
    [SerializeField] AudioClip seeSound;
    [Range(0, 1)][SerializeField] float audSeeVol;
    [SerializeField] AudioClip woosh;
    [Range(0, 1)][SerializeField] float audWooshVol;
    [SerializeField] SkinnedMeshRenderer mainBody;
    [SerializeField] SkinnedMeshRenderer secondPart;
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
    public bool friendly;
    [SerializeField] bool knowsPlayerLocation;
    [SerializeField] bool ambusher; // may get ride of
    [SerializeField] bool meleeOnly;
    [SerializeField] float meleeRange;
    //[SerializeField] int strafingSpeed;
    [SerializeField] int TargetFaceSpeed;
    public AudioSource soundSFX;
    //[SerializeField] int animChangeSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int shootAngle;
    //[SerializeField] float animSpeed;//uncomment when needed
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    public spawnerWave whereISpawned;
    public Spawner WhereISpawned;

    [Header("----- Attack States -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform spawnPos;
    [SerializeField] float fireRate;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletSpeed;
    [SerializeField] float bulletLifeSpan;
    [Range(0, 3)][SerializeField] float shotoffSet;
    [SerializeField] Collider hitBoxCOL;
    [SerializeField] int explosionRange;
    public spawnerDestroyable origin;
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
    bool readyToExplod;
    bool IAmExploding;
    Material OGeye;
    bool bunnyFly;
    //bool dead = false;

    //Color Mcolor;
    // Start is called before the first frame update
    void Start()
    {
        Hp = MaxHp;
        if (bullet != null)
        {
            bullet.GetComponent<Bullet>().DestroyTime = bulletLifeSpan;
        }
        soundSFX = GetComponent<AudioSource>();
        StartingPos = transform.position;
        goRight = Random.Range(0, 2) == 0;
        stoppingDistOrig = agent.stoppingDistance;
        if (EyeColor != null) OGeye = EyeColor.GetComponent<SkinnedMeshRenderer>().material;

    }
    public void huntDownPlayer()
    {
        knowsPlayerLocation = true;

    }
    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            if (bunnyFly && agent.baseOffset < 1) agent.baseOffset = Mathf.Lerp(agent.baseOffset, 1, Time.deltaTime * 4);
            if (anim != null) anim.SetFloat("speed", agent.velocity.normalized.magnitude);
            //if (bunnyFly && mainBodyV.transform.position.y < 0.95f) mainBodyV.transform.position = new Vector3 (mainBodyV.transform.position.x, mainBodyV.transform.position.y + Time.deltaTime * 20, mainBodyV.transform.position.z);
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
        Vector3 playerPosition = gameManager.Instance.player.transform.position;
        Vector3 directionToPlayer = playerPosition - headPos.position;
        float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);
        float playerDist = Vector3.Distance(playerPosition, transform.position);
        PlayerDir = directionToPlayer;

        RaycastHit hit;

        // Offset the raycast origin slightly above the ground to avoid hitting walls or obstacles.
        Vector3 raycastOrigin = headPos.position + Vector3.up * 0.5f;

        if (Physics.Raycast(raycastOrigin, directionToPlayer, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle || hit.collider.CompareTag("Player") && foundPlayer)
            {
                if (!foundPlayer) found();
                agent.stoppingDistance = stoppingDistOrig;
                if (inStafingRange && !meleeOnly) StartCoroutine(strafe());
                else agent.SetDestination(playerPosition);
                if (agent.remainingDistance < agent.stoppingDistance)
                    FaceTarget();

                if (angleToPlayer <= shootAngle && !isAttacking)
                {
                    if (!meleeOnly) StartCoroutine(attack());
                    else if (playerDist <= meleeRange) StartCoroutine(attack());
                }
                return true;
            }
        }
        return false;
    }

    //bool CanSeePlayer()
    //{
    //    PlayerDir = gameManager.Instance.player.transform.position - headPos.position;
    //    angleToPlayer = Vector3.Angle(PlayerDir, transform.forward);
    //    playerDist = Vector3.Distance(gameManager.Instance.player.transform.position, transform.position);

    //    RaycastHit hit;

    //    if (Physics.Raycast(headPos.position, PlayerDir, out hit))
    //    {
    //        if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle || hit.collider.CompareTag("Player") && foundPlayer)
    //        {

    //            if (!foundPlayer) found();
    //            agent.stoppingDistance = stoppingDistOrig;
    //            if (inStafingRange && !meleeOnly) StartCoroutine(strafe());
    //            else agent.SetDestination(gameManager.Instance.player.transform.position);
    //            if (agent.remainingDistance < agent.stoppingDistance)
    //                FaceTarget();

    //            if (angleToPlayer <= shootAngle && !isAttacking)// && playerDist <= meleeRange)
    //            {
    //                if (!meleeOnly) StartCoroutine(attack());
    //                else if (playerDist <= meleeRange) StartCoroutine(attack());
    //            }
    //            return true;
    //        }
    //    }
    //    return false;
    //}

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
    }



    IEnumerator attack()
    {
        isAttacking = true;
        if (anim != null)
        {
            anim.SetTrigger("attack");
            soundSFX.PlayOneShot(attckSound, audAttackVol);
        }
        else
        {
            tomAttack();
            yield return new WaitForSeconds(fireRate);
            isAttacking = false;
        }

    }

    void tomAttack()
    {
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().damage = shootDamage;
        bullet.GetComponent<Bullet>().offsetX = Random.Range(-shotoffSet, shotoffSet);
        bullet.GetComponent<Bullet>().offsetY = Random.Range(-shotoffSet, shotoffSet);
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void FireSTD()
    {
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().damage = shootDamage;
        bullet.GetComponent<Bullet>().offsetX = 0;
        bullet.GetComponent<Bullet>().offsetY = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    public void playSwosh()
    {
        soundSFX.PlayOneShot(woosh, audWooshVol);
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
        Hp = 1;
        EyeColor.GetComponent<SkinnedMeshRenderer>().material = newMaterial;
        //RBody.useGravity = false;
        knowsPlayerLocation = true;
        agent.speed *= 5;
        agent.acceleration *= 8;
        agent.angularSpeed *= 8;
        //
        bunnyFly = true;
    }
    public void startUnFriend()
    {
        hitBoxCOL.enabled = false;
        anim.SetBool("BAttack", false);
        StartCoroutine(Roam());
        StartCoroutine(unFriend());
        EyeColor.GetComponent<SkinnedMeshRenderer>().material = OGeye;
        //anim.SetBool("BAttack", false);
        knowsPlayerLocation = false;
        //hitBoxCOL.enabled = false;
        agent.speed = 3.5f;
        agent.acceleration = 16;
        agent.angularSpeed = 600;
    }

    IEnumerator unFriend()
    {
        yield return new WaitForSeconds(fireRate);
        isAttacking = false;
        friendly = false;
    }

    public void brownAttack()
    {
        anim.SetBool("BAttack", true);
        EyeColor.GetComponent<SkinnedMeshRenderer>().material = newMaterial;
        knowsPlayerLocation = true;
        agent.speed *= 4;
        agent.acceleration *= 8;
        agent.angularSpeed *= 8;
        roamPauseTime = 0;
    }


    void found()
    {
        if (seeSound != null) soundSFX.PlayOneShot(seeSound, audSeeVol);
        foundPlayer = true;
    }

    public void takeDamage(int amount)
    {
        Hp -= amount;
        if (EyeColor != null) startUnFriend();
        if (anim != null) anim.SetBool("BAttack", false);
        if (hitBoxCOL != null) hitBoxCOL.enabled = false;
        soundSFX.PlayOneShot(VpainSound, audVpainVol);
        if (painSound != null) soundSFX.PlayOneShot(painSound, audPainVol);

        if (Hp <= 0)
        {
            StopAllCoroutines();
            FaceTarget();
            //GetComponent<CapsuleCollider>().enabled = false;
            //GetComponent<NavMeshAgent>().enabled = false;

            soundSFX.PlayOneShot(VdeathSound, audVdeathVol);
            if (deathSound != null) soundSFX.PlayOneShot(deathSound, audDeathVol);
            if (mainBody != null)
            {
                mainBody.enabled = false;
                if (secondPart != null) secondPart.enabled = false;
            }
            else mainBodyV.gameObject.SetActive(false);
            VoxelDamage.gameObject.SetActive(false);
            DeathOBJ.gameObject.SetActive(true);
            agent.enabled = false;
            damageCOL.enabled = false;
            Quaternion Rot = Quaternion.LookRotation(PlayerDir);
            transform.rotation = Rot;
            //StartCoroutine(Death());
            Invoke("Death", 0.8f);

            if (swManager.instance != null && gameManager.Instance.waveCur != 6 ) { 
            gameManager.Instance.minorUpdateGoal(-1);
            }

            if (WhereISpawned != null)
            {
                whereISpawned.updateEnemyNumber();
                WhereISpawned.heyIDied();
                //gameManager.Instance.updateGameGoal(-1);
            }
            if (origin != null)
            {
                origin.updateObjectNum();
            }
        }
        else
            StartCoroutine(FlashDamage());

    }
    IEnumerator FlashDamage()
    {
        inPain = true;
        //if (checkTag()) anim.SetBool("inPain", true);
        if (mainBody != null)
        {
            mainBody.enabled = false;
            if (secondPart != null) secondPart.enabled = false;
        }
        else mainBodyV.gameObject.SetActive(false);
        VoxelDamage.gameObject.SetActive(true);
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(0.085714f);
        VoxelDamage.gameObject.SetActive(false);
        if (secondPart != null) secondPart.enabled = true;
        if (mainBody != null) mainBody.enabled = true;
        else mainBodyV.gameObject.SetActive(true);
        if (anim != null) anim.SetTrigger("pain");
        else Invoke("endPain", 0.142857f);

    }
    public void readyExplod()
    {
        if (!readyToExplod && !IAmExploding)
        {
            readyToExplod = true;
            agent.speed *= 2;
            agent.acceleration *= 4;
            agent.angularSpeed *= 4;
            roamPauseTime = 0;
            StartCoroutine(fuseOn());
        }
    }
    IEnumerator fuseOn()
    {
        yield return new WaitForSeconds(5);
        ImExplodeing();
    }
    public void ImExplodeing()
    {
        if (!IAmExploding)
        {
            IAmExploding = true;
            readyToExplod = true;
            agent.SetDestination(gameManager.Instance.player.transform.position);
            agent.enabled = false;
        }
    }
    //IEnumerator Explode()
    //{

    //    yield return new WaitForSeconds();
    //}


    public void endPain()
    {
        inPain = false;
        if (agent.isActiveAndEnabled) agent.SetDestination(gameManager.Instance.player.transform.position);
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

