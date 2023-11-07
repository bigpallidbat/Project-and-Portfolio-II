using System.Collections;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.AI;
//using static UnityEngine.RuleTile.TilingRuleOutput;
using static UnityEngine.UI.Image;

public class specialEnemyAi : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] AudioSource soundSFX;
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
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] GameObject Gun;
    [SerializeField] Transform LeftHand;
    [SerializeField] Transform RightHand;
    [SerializeField] ParticleSystem deathSystem;
    [SerializeField] Collider damageCOL;
    [SerializeField] SkinnedMeshRenderer mainBody;
    [SerializeField] GameObject VoxelDamage;
    [SerializeField] GameObject DeathOBJ;
    float playerDist;

    [Header("----- Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] float meleeRange;
    [Range(30, 180)][SerializeField] int viewAngle;

    [Header("----- Attack Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletSpeed;
    [SerializeField] int bulletLifeSpan;
    [Range(0, 3)][SerializeField] float shotoffSet;
    [SerializeField] Collider hitBoxCOL;
    [Range(30, 180)][SerializeField] int shootAngle;

    float angleToPlayer;
    //Vector3 PlayerDir;
    Vector3 playerDir;
    Vector3 StartingPos;
    const int numDamAnims = 3;
    int lastAnim = -1;
    bool isAttacking;
    bool playerInRange;
    bool isMoving;
    bool inPain;
    int diceroll;

    // Start is called before the first frame update
    void Start()
    {
        bullet.GetComponent<Bullet>().DestroyTime = bulletLifeSpan;
        soundSFX = GetComponent<AudioSource>();
        StartingPos = transform.position;
        setModel();

    }

    // Update is called once per frame
    void Update()
    {

        //if (playerInRange && AiRoutine())
        //{

        //}
        //moving();

        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("speed", agent.velocity.normalized.magnitude);
            if (playerInRange && CanSeePlayer() && !inPain) { }
            else if (inPain) agent.SetDestination(transform.position);
        }

    }
    bool CanSeePlayer()
    {
        playerDir = gameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        playerDist = Vector3.Distance(gameManager.Instance.player.transform.position, transform.position);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(gameManager.Instance.player.transform.position);
                if (agent.remainingDistance < agent.stoppingDistance)
                    faceTarget();

                if (angleToPlayer <= shootAngle && !isAttacking)
                {
                    if (playerDist <= meleeRange) StartCoroutine(meleeAttack());
                    else StartCoroutine(shoot());
                }
                return true;
            }
        }
        return false;
    }

    void setModel()
    {
        Gun = Instantiate(Gun);
        Gun.transform.parent = RightHand;
        Gun.transform.localPosition = Vector3.zero;
        Gun.transform.localRotation = Quaternion.Euler(90, 0, 0);
        shootPos = Gun.transform.GetChild(0);
    }

    //public void takeDamage(int damage)
    //{
    //    HP -= damage;
    //    StartCoroutine(flashDamage());
    //    agent.SetDestination(gameManager.Instance.player.transform.position);
    //    if (HP <= 0)
    //    {
    //        anim.SetTrigger("Death");
    //        gameManager.Instance.updateGameGoal();
    //        StopAllCoroutines();
    //        Instantiate(deathSystem, transform.position, transform.rotation);
    //        StartCoroutine(death());
    //    }
    //}
    public void takeDamage(int amount)
    {
        HP -= amount;
        hitBoxCOL.enabled = false;
        soundSFX.PlayOneShot(VpainSound, audVpainVol);
        soundSFX.PlayOneShot(painSound, audPainVol);

        if (HP <= 0)
        {
            anim.SetBool("dead", true);
            StopAllCoroutines();
            faceTarget();
            soundSFX.PlayOneShot(VdeathSound, audVdeathVol);
            soundSFX.PlayOneShot(deathSound, audDeathVol);
            mainBody.enabled = false;
            VoxelDamage.gameObject.SetActive(false);
            DeathOBJ.gameObject.SetActive(true);
            agent.enabled = false;
            damageCOL.enabled = false;
            Quaternion Rot = Quaternion.LookRotation(playerDir);
            transform.rotation = Rot;
        }
        else
            StartCoroutine(FlashDamage());

    }
    public void pop()
    {
        StartCoroutine(Pop());
    }
    IEnumerator Pop()
    {

        yield return new WaitForSeconds(1);
    }
    IEnumerator FlashDamage()
    {
        inPain = true;
        mainBody.enabled = false;
        VoxelDamage.gameObject.SetActive(true);
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(0.085714f);
        VoxelDamage.gameObject.SetActive(false);
        mainBody.enabled = true;
        diceroll = Random.Range(0, 12);
        if (diceroll < HP)
        {
            int id = Random.Range(0, numDamAnims);
            if (numDamAnims > 1)
                while (id == lastAnim)
                    id = Random.Range(0, numDamAnims);
            lastAnim = id;
            anim.SetInteger("DamageID", id);
            anim.SetTrigger("Damage");


        }

        else inPain = false;
    }

    //IEnumerator death()
    //{
    //    yield return new WaitForSeconds(5);
    //    Destroy(gameObject);
    //}

    //IEnumerator flashDamage()
    //{
    //    int id = Random.Range(0, numDamAnims);
    //    if (numDamAnims > 1)
    //        while (id == lastAnim)
    //            id = Random.Range(0, numDamAnims);
    //    lastAnim = id;
    //    anim.SetInteger("DamageID", id);
    //    anim.SetTrigger("Damage");

    //    yield return new WaitForSeconds(0.1f);

    //}

    IEnumerator shoot()
    {
        isAttacking = true;
        anim.SetTrigger("shoot");
        //bScript.damage = shootDamage;
        //bScript.speed = bulletSpeed;
        //shootPos.transform.rotation = Quaternion.LookRotation(playerDir);
        // Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(fireRate);
        
    }
    public void shotGun()
    {
        bullet.GetComponent<Bullet>().setBulletStats(shootDamage, bulletSpeed, bulletLifeSpan, shotoffSet);
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    IEnumerator meleeAttack()
    {
        yield return null;
    }

    void straightPelletPellets()
    {
        bullet.GetComponent<Bullet>().setBulletStats(shootDamage, bulletSpeed, bulletLifeSpan, 0);
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void otherPellets()
    {
        bullet.GetComponent<Bullet>().setBulletStats(shootDamage, bulletSpeed, bulletLifeSpan, shotoffSet);
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    public void endAttck()
    {
        isAttacking = false;
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
        }
    }

    //void moving()
    //{
    //    if (agent.velocity != Vector3.zero)
    //    {
    //        isMoving = true;
    //        anim.SetBool("Aiming", !isMoving);
    //        anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
    //    }

    //}

    //bool AiRoutine()
    //{
    //    //find direction to player if in range
    //    playerDir = gameManager.Instance.player.transform.position - headPos.position;
    //    //find angle to player within distance
    //    angleToPlayer = Vector3.Angle(playerDir, transform.forward);

    //    RaycastHit hit;

    //    if (Physics.Raycast(headPos.position, playerDir, out hit) && angleToPlayer <= viewAngle)
    //    {
    //        if (hit.collider.CompareTag("Player"))
    //        {
    //            agent.SetDestination(gameManager.Instance.player.transform.position);

    //            if (agent.remainingDistance < agent.stoppingDistance)
    //            {
    //                faceTarget();
    //                anim.SetFloat("Speed", 0);
    //                anim.SetBool("Aiming", true);
    //                if (angleToPlayer <= shootAngle && !isAttacking)
    //                    StartCoroutine(shoot());
    //            }



    //            return true;
    //        }

    //    }

    //    return false;
    //}
}

