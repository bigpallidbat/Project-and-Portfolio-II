using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AlexsBossAI : MonoBehaviour, IDamage
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
    [SerializeField] MeshRenderer VoxelRender;
    [SerializeField] Material Normal;
    [SerializeField] Material Negive;
    [SerializeField] Material Black;
    [SerializeField] Material White;
    [SerializeField] GameObject DeathOBJ;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform stompPos;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject leftCheck;
    [SerializeField] GameObject rightCheck;
    [SerializeField] Collider damageCOL;
    Vector3 PlayerDir;
    float playerDist;

    [Header("----- Enemy States -----")]
    [SerializeField] int Hp;
    private int HpMax;
    int HpCheck;
    [SerializeField] int TargetFaceSpeed;
    public AudioSource soundSFX;
    [SerializeField] int viewAngle;
    [SerializeField] int shootAngle;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;

    [Header("----- Attack States -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bulletS;
    [SerializeField] GameObject shockWave;
    [SerializeField] float reAttackRate;
    [SerializeField] float fireRate;
    [SerializeField] float att1Rate;
    [SerializeField] float att1multiRate;
    float att1CurFireRate;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletSpeed;
    [Range(0, 5)][SerializeField] float shotoffSet;

    [Header("----- Spawner Stats -----")]
    [SerializeField] int maxObjectsToSpawn;
    [SerializeField] int timeBetweenSpawn;
    [SerializeField] Transform spawnPos;
    [SerializeField] GameObject enemy1;
    [SerializeField] GameObject enemy2;

    int curObjectsSpawned;
    bool isSpawning;
    bool startSpawning;

    int diceroll = 20;
    bool isAttacking;
    bool inPain;
    bool isInvincible;
    bool playerInRange;
    float angleToPlayer;
    float stoppingDistOrig;
    public bool leftChecker;
    public bool rightChecker;
    public bool inStafingRange;
    public bool goRight;
    public bool cutSceneOver;
    bool att2part1;
    bool att2part2;
    bool att2part3;
    bool att2part4;
    int attfail;
    bool doingAttack;
    bool slamDown;
    //bool jumping;
    // bool jumpLanding;
    //bool flyingUp;
    //bool gournded;

    // Start is called before the first frame update
    void Start()
    {
        //agent.enabled = false;
        //isInvincible = true;
        att1CurFireRate = att1Rate;
        soundSFX = GetComponent<AudioSource>();
        goRight = Random.Range(0, 2) == 0;
        HpMax = Hp;
        updateHpUI();
    }


    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            if (doingAttack)
            {
                playerDist = Vector3.Distance(gameManager.Instance.player.transform.position, transform.position);
                //if 
                if (att2part1 && !att2part2 && !att2part3 && !att2part4)
                {
                    isInvincible = true;
                    if (agent.baseOffset < 1.8f)
                    {
                        agent.baseOffset = Mathf.Lerp(agent.baseOffset, 2, Time.deltaTime * 8);
                        //Debug.Log(agent.baseOffset.ConvertTo<float>());
                    }
                    else
                    {
                        att2part1 = false;
                        att2part2 = true;
                    }
                }
                //else attfail--;
                if (att2part2 && !att2part1 && !att2part3 && !att2part4)
                {
                    isInvincible = true;
                    if (agent.baseOffset > 1.2f)
                    {
                        agent.baseOffset = Mathf.Lerp(agent.baseOffset, 1, Time.deltaTime * 8);
                    }
                    else
                    {
                        att2part2 = false;
                        agent.speed *= 2;
                        agent.acceleration *= 8;
                        agent.angularSpeed *= 8;
                        att2part3 = true;
                        StartCoroutine(startSlamDown());
                    }
                }
                if (att2part3 && !att2part2 && !att2part1 && !att2part4)
                {
                    isInvincible = true;
                    agent.SetDestination(gameManager.Instance.player.transform.position);
                    if (agent.baseOffset < 22.8f)
                    {
                        agent.baseOffset = Mathf.Lerp(agent.baseOffset, 23, Time.deltaTime * 8);
                    }
                    else //if (playerDist < 12.22f)
                    {
                        att2part3 = false;
                        agent.speed /= 2;
                        agent.acceleration /= 8;
                        agent.angularSpeed /= 8;
                        att2part4 = true;
                    }
                }
                if (att2part4 && !att2part2 && !att2part3 && !att2part1)
                {
                    isInvincible = true;
                    if (slamDown && agent.baseOffset > 1.2f)
                    {
                        agent.baseOffset = Mathf.Lerp(agent.baseOffset, 1, Time.deltaTime * 32);
                        agent.SetDestination(transform.position);
                    }
                    else if (slamDown)
                    {
                        doingAttack = false;
                        slamDown = false;
                        att2part4 = false;
                        agent.baseOffset = 1;
                        for (int i = 0; i < 32; i++)
                        {
                            float angle = i * 11.25f;
                            Quaternion rotation = Quaternion.Euler(0, angle, 0);
                            Instantiate(shockWave, transform.position + new Vector3(0, -1.75f, 0), rotation);
                        }
                        StartCoroutine(endAttack());
                    }
                }
                if (!att2part4 && !att2part2 && !att2part3 && !att2part1) StartCoroutine(endAttack());
            }
            else
            {
                agent.baseOffset = 1;
                if (playerInRange && CanSeePlayer() && !inPain)
                {
                    agent.SetDestination(gameManager.Instance.player.transform.position);
                }
                else if (inPain) agent.SetDestination(transform.position);
                else agent.SetDestination(gameManager.Instance.player.transform.position);
                if (startSpawning)// && curObjectsSpawned < maxObjectsToSpawn)
                {
                    StartCoroutine(spawn());
                }
            }
        }
    }
    public void StartFight()
    {
        isInvincible = false;
        agent.enabled = true;
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
                    FaceTarget();
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
            diceroll = Random.Range(0, 20);
            switch (diceroll)
            {
                case 0:
                    Instantiate(enemy2, spawnPos.position, transform.rotation);
                    curObjectsSpawned++;
                    break;
                case 1:
                    if (Hp < 90) Instantiate(enemy2, spawnPos.position, transform.rotation);
                    else Instantiate(enemy1, spawnPos.position, transform.rotation);
                    curObjectsSpawned++;
                    break;
                case 2:
                    if (Hp < 80) Instantiate(enemy2, spawnPos.position, transform.rotation);
                    else Instantiate(enemy1, spawnPos.position, transform.rotation);
                    curObjectsSpawned++;
                    break;
                case 3:
                    if (Hp < 70) Instantiate(enemy2, spawnPos.position, transform.rotation);
                    else Instantiate(enemy1, spawnPos.position, transform.rotation);
                    curObjectsSpawned++;
                    break;
                case 4:
                    if (Hp < 60) Instantiate(enemy2, spawnPos.position, transform.rotation);
                    else Instantiate(enemy1, spawnPos.position, transform.rotation);
                    curObjectsSpawned++;
                    break;
                case 5:
                    if (Hp < 50) Instantiate(enemy2, spawnPos.position, transform.rotation);
                    else Instantiate(enemy1, spawnPos.position, transform.rotation);
                    curObjectsSpawned++;
                    break;
                case 6:
                    if (Hp < 20) Instantiate(enemy2, spawnPos.position, transform.rotation);
                    else Instantiate(enemy1, spawnPos.position, transform.rotation);
                    curObjectsSpawned++;
                    break;
                case 7:
                    if (Hp < 10) Instantiate(enemy2, spawnPos.position, transform.rotation);
                    else Instantiate(enemy1, spawnPos.position, transform.rotation);
                    curObjectsSpawned++;
                    break;
                case 8:
                    if (Hp < 90)
                    {
                        Instantiate(enemy1, spawnPos.position, transform.rotation);
                        curObjectsSpawned++;
                    }
                    break;
                case 9:
                    if (Hp < 80)
                    {
                        Instantiate(enemy1, spawnPos.position, transform.rotation);
                        curObjectsSpawned++;
                    }
                    break;
                case 10:
                    if (Hp < 70)
                    {
                        Instantiate(enemy1, spawnPos.position, transform.rotation);
                        curObjectsSpawned++;
                    }
                    break;
                case 11:
                    if (Hp < 60)
                    {
                        Instantiate(enemy1, spawnPos.position, transform.rotation);
                        curObjectsSpawned++;
                    }
                    break;
                case 12:
                    if (Hp < 50)
                    {
                        Instantiate(enemy1, spawnPos.position, transform.rotation);
                        curObjectsSpawned++;
                    }
                    break;
                case 13:
                    if (Hp < 40)
                    {
                        Instantiate(enemy1, spawnPos.position, transform.rotation);
                        curObjectsSpawned++;
                    }
                    break;
                case 14:
                    if (Hp < 30)
                    {
                        Instantiate(enemy1, spawnPos.position, transform.rotation);
                        curObjectsSpawned++;
                    }
                    break;
                case 15:
                    if (Hp < 20)
                    {
                        Instantiate(enemy1, spawnPos.position, transform.rotation);
                        curObjectsSpawned++;
                    }
                    break;
                case 16:
                    if (Hp < 10)
                    {
                        Instantiate(enemy1, spawnPos.position, transform.rotation);
                        curObjectsSpawned++;
                    }
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(timeBetweenSpawn);
            isSpawning = false;
        }
    }
    IEnumerator attack()
    {
        isAttacking = true;
        //StartCoroutine(attack2());
        diceroll = Random.Range(0, 9);
        switch (diceroll)
        {
            case 0:
                StartCoroutine(attack2());
                break;
            case 1:
                if (Hp < 70) StartCoroutine(attack2());
                else StartCoroutine(attack1());
                break;
            case 2:
                if (Hp < 40) StartCoroutine(attack2());
                else StartCoroutine(attack1());
                break;
            case 3:
                if (Hp < 10) StartCoroutine(attack2());
                else StartCoroutine(attack1());
                break;
            case 4:
                if (Hp < 70) StartCoroutine(attack1());
                break;
            case 5:
                if (Hp < 50) StartCoroutine(attack1());
                break;
            case 6:
                if (Hp < 30) StartCoroutine(attack1());
                break;
            case 7:
                if (Hp < 20) StartCoroutine(attack1());
                break;
            case 8:
                if (Hp < 10) StartCoroutine(attack1());
                break;
            default:
                break;
        }
        yield return null;
        //isAttacking = false;
    }

    IEnumerator attack1()
    {
        att1CurFireRate = (Hp * att1multiRate) + 0.25f;
        //HpCheck = HpMax;
        //HpCheck -= Hp;
        //att1CurFireRate = HpCheck * att1multiRate;
        for (int i = 3; i > 0; i--)
        {
            if (inPain) break;
            FireSTD();
            for (int j = 99; j > Hp; j -= 12) tomAttack();
            yield return new WaitForSeconds(att1CurFireRate);
        }

        isAttacking = false;
    }
    IEnumerator attack2()
    {
        ///jumping = true;
        attfail = 100;
        att2part1 = true;
        doingAttack = true;
        isInvincible = true;
        yield return null;
        // isAttacking = false;
    }
    IEnumerator startSlamDown()
    {

        yield return new WaitForSeconds(1);
        slamDown = true;
    }
    IEnumerator attack3()
    {
        yield return new WaitForSeconds(reAttackRate);
        isAttacking = false;
    }
    IEnumerator attack4()
    {
        yield return new WaitForSeconds(reAttackRate);
        isAttacking = false;
    }
    IEnumerator endAttack()
    {
        isInvincible = false;
        doingAttack = false;
        yield return new WaitForSeconds(reAttackRate);
        isAttacking = false;
    }
    void tomAttack()
    {
        bulletS.GetComponent<Bullet>().speed = bulletSpeed;
        bulletS.GetComponent<Bullet>().damage = shootDamage;
        bulletS.GetComponent<Bullet>().offsetX = Random.Range(-shotoffSet, shotoffSet);
        bulletS.GetComponent<Bullet>().offsetY = Random.Range(-shotoffSet, shotoffSet);
        Instantiate(bulletS, shootPos.position, transform.rotation);
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
        if (!isInvincible)
        {
            agent.baseOffset = 1;
            Hp -= amount;
            updateHpUI();
            soundSFX.PlayOneShot(VpainSound, audVpainVol);
            if (Hp < 70) timeBetweenSpawn = 1;
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
            }
            else
                StartCoroutine(FlashDamage());
        }
    }
    IEnumerator FlashDamage()
    {
        inPain = true;
        isInvincible = true;
        mainBodyV.gameObject.SetActive(false);
        VoxelDamage.gameObject.SetActive(true);
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(0.028571f);
        VoxelRender.material = White;
        yield return new WaitForSeconds(0.028571f);
        VoxelRender.material = Negive;
        yield return new WaitForSeconds(0.028571f);
        VoxelRender.material = Black;
        yield return new WaitForSeconds(0.028571f);
        VoxelRender.material = Normal;
        //yield return new WaitForSeconds(0.028571f);
        //VoxelRender.material = White;
        //yield return new WaitForSeconds(0.028571f);
        //VoxelRender.material = Negive;
        //yield return new WaitForSeconds(0.028571f);
        //VoxelRender.material = Black;
        //yield return new WaitForSeconds(0.028571f);
        //VoxelRender.material = Normal;
        yield return new WaitForSeconds(0.057142f);
        VoxelDamage.gameObject.SetActive(false);
        mainBodyV.gameObject.SetActive(true);
        StartCoroutine(attack());
        inPain = false;
        isInvincible = false;
        if (gameManager.Instance.player != null) agent.SetDestination(gameManager.Instance.player.transform.position);
    }

    void FaceTarget()
    {
        Quaternion Rot = Quaternion.LookRotation(PlayerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, Rot, Time.deltaTime * TargetFaceSpeed);
    }
    public void Death()
    {
        gameManager.Instance.updateGameGoal();

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

    void updateHpUI()
    {
        gameManager.Instance.BossHPBar.fillAmount = (float)Hp / HpMax;
        gameManager.Instance.BossHpFill.gameObject.SetActive(true);
    }

}

