using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlexsBossAI : MonoBehaviour , IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
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
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject leftCheck;
    [SerializeField] GameObject rightCheck;
    [SerializeField] Collider damageCOL;
    Vector3 PlayerDir;

    [Header("----- Enemy States -----")]
    [SerializeField] int Hp;
    [SerializeField] int TargetFaceSpeed;
    public AudioSource soundSFX;
    [SerializeField] int viewAngle;
    [SerializeField] int shootAngle;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    public spawnerWave whereISpawned;
    public Spawner WhereISpawned;

    [Header("----- Attack States -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletSpeed;
    [Range(0, 3)][SerializeField] float shotoffSet;

    [Header("----- Spawner Stats -----")]
    [SerializeField] List<GameObject> objectList = new List<GameObject>();
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int maxObjectsToSpawn;
    [SerializeField] int timeBetweenSpawn;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] GameObject enemy1;
    [SerializeField] GameObject enemy2;

    [SerializeField] Spawner WhereIspawned;

    int curObjectsSpawned;
    bool isSpawning;
    bool startSpawning;

    public spawnerDestroyable origin;
    int DiceRoll = 20;
    bool isAttacking;
    bool inPain;
    bool playerInRange;
    float angleToPlayer;
    float stoppingDistOrig;
    public bool leftChecker;
    public bool rightChecker;
    public bool inStafingRange;
    public bool goRight;

    // Start is called before the first frame update
    void Start()
    {
        soundSFX = GetComponent<AudioSource>();
        goRight = Random.Range(0, 2) == 0;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {

            if (playerInRange && CanSeePlayer() && !inPain)
            {
                agent.SetDestination(gameManager.Instance.player.transform.position);
            }
            else if (inPain) agent.SetDestination(transform.position);
            else agent.SetDestination(gameManager.Instance.player.transform.position);
            if (startSpawning && curObjectsSpawned < maxObjectsToSpawn)
            {
                StartCoroutine(spawn());
            }
        }
    }
    bool CanSeePlayer()
    {
        PlayerDir = gameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(PlayerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, PlayerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle || hit.collider.CompareTag("Player"))
            {
                agent.stoppingDistance = stoppingDistOrig;
                if (inStafingRange) StartCoroutine(strafe());
                else agent.SetDestination(gameManager.Instance.player.transform.position);
                if (agent.remainingDistance < agent.stoppingDistance)
                    FaceTarget();

                if (angleToPlayer <= shootAngle && !isAttacking)// && playerDist <= meleeRange)
                {
                    StartCoroutine(attack());
                }
                return true;
            }
        }
        return false;
    }

    public void changeStrafe()
    {
        goRight = !goRight;
    }

    IEnumerator strafe()
    {
        yield return null;

        if (agent.isActiveAndEnabled)
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
    }

    IEnumerator spawn()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            GameObject objectClone = forSpawn();
            objectList.Add(objectClone);

            objectClone.GetComponent<EnemyAI>().WhereISpawned = this.GetComponent<Spawner>();

            yield return new WaitForSeconds(timeBetweenSpawn);
            isSpawning = false;
        }
    }
    private GameObject forSpawn()
    {
        GameObject objectClone = null;
        DiceRoll = Random.Range(0, 20);
        switch (DiceRoll)
        {
            case 0:
                objectClone = Instantiate(enemy2, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                curObjectsSpawned++;
                break;
            case 1:
                if (Hp < 90) objectClone = Instantiate(enemy2, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                else objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                curObjectsSpawned++;
                break;
            case 2:
                if (Hp < 80) objectClone = Instantiate(enemy2, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                else objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                curObjectsSpawned++;
                break;
            case 3:
                if (Hp < 70) objectClone = Instantiate(enemy2, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                else objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                curObjectsSpawned++;
                break;
            case 4:
                if (Hp < 60) objectClone = Instantiate(enemy2, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                else objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                curObjectsSpawned++;
                break;
            case 5:
                if (Hp < 50) objectClone = Instantiate(enemy2, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                else objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                curObjectsSpawned++;
                break;
            case 6:
                if (Hp < 20) objectClone = Instantiate(enemy2, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                else objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                curObjectsSpawned++;
                break;
            case 7:
                if (Hp < 10) objectClone = Instantiate(enemy2, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                else objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                curObjectsSpawned++;
                break;
            case 8:
                if (Hp < 90)
                {
                    objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                    curObjectsSpawned++;
                }
                break;
            case 9:
                if (Hp < 80)
                {
                    objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                    curObjectsSpawned++;
                }
                break;
            case 10:
                if (Hp < 70)
                {
                    objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                    curObjectsSpawned++;
                }
                break;
            case 11:
                if (Hp < 60)
                {
                    objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                    curObjectsSpawned++;
                }
                break;
            case 12:
                if (Hp < 50)
                {
                    objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                    curObjectsSpawned++;
                }
                break;
            case 13:
                if (Hp < 40)
                {
                    objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                    curObjectsSpawned++;
                }
                break;
            case 14:
                if (Hp < 30)
                {
                    objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                    curObjectsSpawned++;
                }
                break;
            case 15:
                if (Hp < 20)
                {
                    objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                    curObjectsSpawned++;
                }
                break;
            case 16:
                if (Hp < 10)
                {
                    objectClone = Instantiate(enemy1, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
                    curObjectsSpawned++;
                }
                break;
            default:
                objectClone = null;
                break;
        }
        return objectClone;
    }
    IEnumerator attack()
    {
        isAttacking = true;
        FireSTD();
        if (Hp <= 99) tomAttack();
        if (Hp < 92) tomAttack();
        if (Hp < 84) tomAttack();
        if (Hp < 76) tomAttack();
        if (Hp < 68) tomAttack();
        if (Hp < 60) tomAttack();
        if (Hp < 52) tomAttack();
        if (Hp < 44) tomAttack();
        if (Hp < 36) tomAttack();
        if (Hp < 28) tomAttack();
        if (Hp < 20) tomAttack();
        if (Hp < 12) tomAttack();
        if (Hp < 6) tomAttack();
        if (Hp < 3) tomAttack();
        yield return new WaitForSeconds(fireRate);
        isAttacking = false;
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

    public void shoot()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    public void takeDamage(int amount)
    {
        Hp -= amount;
        soundSFX.PlayOneShot(VpainSound, audVpainVol);

        if (Hp <= 0)
        {
            FaceTarget();
            soundSFX.PlayOneShot(VdeathSound, audVdeathVol);
            mainBodyV.gameObject.SetActive(false);
            VoxelDamage.gameObject.SetActive(false);
            DeathOBJ.gameObject.SetActive(true);
            agent.enabled = false;
            damageCOL.enabled = false;
            StopAllCoroutines();
            Quaternion Rot = Quaternion.LookRotation(PlayerDir);
            transform.rotation = Rot;
            Invoke("Death", 0.8f);


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
        mainBodyV.gameObject.SetActive(false);
        VoxelDamage.gameObject.SetActive(true);
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(0.085714f);
        VoxelDamage.gameObject.SetActive(false);
        mainBodyV.gameObject.SetActive(true);
        Invoke("endPain", 0.142857f);

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


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }

        //Enemies start spawning
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }

        //Enemies stop spawning
        if (other.CompareTag("Player"))
        {
            startSpawning = false;
        }
    }

}

