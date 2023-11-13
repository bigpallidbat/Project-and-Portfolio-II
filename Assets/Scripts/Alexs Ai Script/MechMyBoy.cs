using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechMyBoy : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;
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
    [SerializeField] GameObject mainBodyV;
    [SerializeField] GameObject VoxelDamage;
    [SerializeField] GameObject DeathOBJ;
    [SerializeField] Animator anim;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Collider damageCOL;
    Vector3 PlayerDir;
    float playerDist;

    [Header("----- Enemy States -----")]
    [SerializeField] int MaxHp;
    public int Hp;
    [SerializeField] bool infected;
    [SerializeField] bool friendly;
    [SerializeField] bool knowsPlayerLocation;
    [SerializeField] bool ambusher;
    [SerializeField] bool meleeOnly;
    [SerializeField] float meleeRange;
    [SerializeField] int TargetFaceSpeed;
    public AudioSource soundSFX;
    [SerializeField] int viewAngle;
    [SerializeField] int shootAngle;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] spawnerWave whereISpawned;
    public Spawner WhereISpawned;

    [Header("----- Attack States -----")]
    [SerializeField] GameObject bullet;
    [Range(1, 32)][SerializeField] int ammoMax;
    [SerializeField] float fireRate;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletSpeed;
    [SerializeField] int bulletLifeSpan;
    [Range(0, 3)][SerializeField] float shotoffSet;
    [SerializeField] Collider hitBoxCOL;
    int ammoAmount;
    public spawnerDestroyable origin;
    bool isAttacking = false;
    bool inPain;
    bool playerInRange = false;
    float angleToPlayer;
    float stoppingDistOrig;
    bool destinationChosen;
    Vector3 StartingPos;
    bool foundPlayer = false;
    bool reloading;
    bool isDead;
    void Start()
    {
        Hp = MaxHp;
        ammoAmount = ammoMax;
        soundSFX = GetComponent<AudioSource>();
        StartingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;

    }
    public void huntDownPlayer()
    {
        knowsPlayerLocation = true;
    }
    void Update()
    {
        if (!isDead)
        {
            anim.SetFloat("speed", agent.velocity.normalized.magnitude);
            if (!friendly)
            {
                if (isAttacking && meleeOnly || reloading) agent.SetDestination(transform.position);
                else
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
                agent.SetDestination(gameManager.Instance.player.transform.position);
                if (agent.remainingDistance < agent.stoppingDistance)
                    FaceTarget();

                if (angleToPlayer <= shootAngle && !isAttacking)
                {
                    if (!meleeOnly && !reloading) StartCoroutine(attack());
                    else if (playerDist <= meleeRange) StartCoroutine(attack());
                }
                return true;
            }
        }
        return false;
    }

    IEnumerator Roam()
    {
        if (agent.remainingDistance < 0.5f && !destinationChosen && !isAttacking)
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

    IEnumerator attack()
    {
        isAttacking = true;

        anim.SetTrigger("attack");
        //soundSFX.PlayOneShot(attckSound, audAttackVol);
        //else yield return null;
        if (!meleeOnly)
        {
            ammoAmount--;
            yield return new WaitForSeconds(fireRate);
            if (ammoAmount <= 0)
            {
                reloading = true;
                anim.SetTrigger("Reload");
                agent.SetDestination(transform.position);
            }
            else
            {
                isAttacking = false;
            }
        }
    }
    public void reload()
    {
        //anim.SetBool("reload", false);
        ammoAmount = ammoMax;
        isAttacking = false;
        reloading = false;
        agent.SetDestination(gameManager.Instance.player.transform.position);
    }
    void FireSTD()
    {
        FaceTarget();
        bullet.GetComponent<Bullet>().setBulletStats(shootDamage, bulletSpeed, bulletLifeSpan, shotoffSet);
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    IEnumerator CheckForReload()
    {
        yield return new WaitForSeconds(fireRate);
        if (ammoAmount <= 0)
        {
            reloading = true;
            anim.SetTrigger("Reload");
            agent.SetDestination(transform.position);
        }
        else
        {
            isAttacking = false;
        }
    }

    public void playSwosh()
    {
        soundSFX.PlayOneShot(woosh, audWooshVol);
    }
    public void deathCry()
    {
        soundSFX.PlayOneShot(deathSound, audDeathVol);
    }
    public void stopedAttack()
    {
        isAttacking = false;
        //if (agent.isActiveAndEnabled) agent.SetDestination(gameManager.Instance.player.transform.position);
    }
    public void hitBoxOn()
    {
        hitBoxCOL.enabled = true;
    }

    public void hitBoxOff()
    {
        hitBoxCOL.enabled = false;
        //if (agent.isActiveAndEnabled) agent.SetDestination(gameManager.Instance.player.transform.position);
    }

    void found()
    {
        if (seeSound != null) soundSFX.PlayOneShot(seeSound, audSeeVol);
        foundPlayer = true;
    }

    public void takeDamage(int amount)
    {
        Hp -= amount;
        if (hitBoxCOL != null) hitBoxCOL.enabled = false;
        soundSFX.PlayOneShot(painSound, audPainVol);
        if (infected) soundSFX.PlayOneShot(VpainSound, audVpainVol);
        if (Hp <= 0)
        {
            isDead = true;
            StopAllCoroutines();
            FaceTarget();
            if (infected)
            {
                soundSFX.PlayOneShot(deathSound, audDeathVol);
                soundSFX.PlayOneShot(VdeathSound, audVdeathVol);
                damageCOL.enabled = false;
                mainBodyV.gameObject.SetActive(false);
                VoxelDamage.gameObject.SetActive(false);
                DeathOBJ.gameObject.SetActive(true);
                Invoke("Death", 0.8f);
                Quaternion Rot = Quaternion.LookRotation(PlayerDir);
                transform.rotation = Rot;
            }
            else
            {
                damageCOL.enabled = false;
                anim.StopPlayback();
                anim.SetBool("die", true);
                agent.SetDestination(transform.position);
                rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }

            StartCoroutine(DisableComponentsAfterDelay(1.0f));

            if (origin != null)
            {
                origin.updateObjectNum();
            }
            if (swManager.instance != null && gameManager.Instance.waveCur != 6)
            {
                gameManager.Instance.minorUpdateGoal(-1);
            }
        }
        else
        {
            StartCoroutine(FlashDamage());
        }
    }

    IEnumerator DisableComponentsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        agent.enabled = false;
        damageCOL.enabled = false;

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

    IEnumerator FlashDamage()
    {
        inPain = true;
        agent.SetDestination(transform.position);
        if (infected)
        {
            mainBodyV.gameObject.SetActive(false);
            VoxelDamage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.085714f);
            VoxelDamage.gameObject.SetActive(false);
            mainBodyV.gameObject.SetActive(true);
        }
        anim.SetTrigger("pain");
    }

    public void endPain()
    {
        inPain = false;
        if (agent.isActiveAndEnabled) agent.SetDestination(gameManager.Instance.player.transform.position);
    }
    public void FaceTarget()
    {
        Quaternion Rot = Quaternion.LookRotation(PlayerDir);
        Quaternion newYRotation = Quaternion.Euler(0f, Rot.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, newYRotation, Time.deltaTime * TargetFaceSpeed);
    }
    public void Death()
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        //soundSFX.PlayOneShot(seeSound);
        if (other.CompareTag("Player")) playerInRange = true;
    }
    public void OnTriggerExit(Collider other)
    {
        //foundPlayer = false;
        if (other.CompareTag("Player") && !isAttacking) playerInRange = false;
        agent.stoppingDistance = 0;
    }

}

