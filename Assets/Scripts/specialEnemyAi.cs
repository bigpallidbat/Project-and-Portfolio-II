using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class specialEnemyAi : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] AudioSource soundSFX;
    [SerializeField] AudioClip painSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip attckSound;
    [SerializeField] AudioClip VpainSound;
    [SerializeField] AudioClip VdeathSound;
    //[Range(0, 1)][SerializeField] float audPainVol;
    //[Range(0, 1)][SerializeField] float audDeathVol;
    //[Range(0, 1)][SerializeField] float audAttackVol;
    //[Range(0, 1)][SerializeField] float audVpainVol;
    //[Range(0, 1)][SerializeField] float audVdeathVol;
    //[Range(0, 1)][SerializeField] float audSeeVol;
    //[Range(0, 1)][SerializeField] float audWooshVol;
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
    [SerializeField] Transform[] positions;
    float playerDist;

    [Header("----- Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int targetFaceSpeed;
    [Range(30, 180)][SerializeField] int viewAngle;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;
    [SerializeField] int gunDamage;
    [SerializeField] int bulletSpeed;
    [SerializeField] int bulletLifeSpan;
    [Range(0, 3)][SerializeField] float offset;
    [SerializeField] Collider hitBoxCOL;
    [Range(30, 180)][SerializeField] int shootAngle;  

    float angleToPlayer;
    Vector3 playerDir;
    const int numDamAnims = 3;
    int lastAnim = -1;
    bool isShooting;
    bool playerInRange;
    //bool isMoving;
    bool inPain;

    // Start is called before the first frame update
    void Start()
    {
        bullet.GetComponent<Bullet>().DestroyTime = bulletLifeSpan;
        setModel();

    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {     
            moving();
            if (playerInRange && AiRoutine() && !inPain)
            {

            }
            else if(inPain) agent.SetDestination(transform.position);
        }
  
        
    }

    void setModel()
    {
        Gun = Instantiate(Gun);
        Gun.transform.parent = RightHand;
        Gun.transform.localPosition = Vector3.zero;
        Gun.transform.localRotation = Quaternion.Euler(90, 0, 0);
        shootPos = Gun.transform.GetChild(0);
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        hitBoxCOL.enabled = false;
        soundSFX.PlayOneShot(VpainSound);
        soundSFX.PlayOneShot(painSound);
        damageCOL.enabled = true;
 
        if (HP <= 0)
        {
            if (gameObject.CompareTag("Goal Point"))
            {
                gameManager.Instance.updateGameGoal();
            }
            anim.SetTrigger("Death");
            soundSFX.PlayOneShot(VdeathSound);
            soundSFX.PlayOneShot(deathSound);
            StopAllCoroutines();
            faceTarget();
            agent.enabled = false;
            damageCOL.enabled = false;
            StartCoroutine(death());
        }
        else
        {
            StartCoroutine(flashDamage());
            agent.SetDestination(gameManager.Instance.player.transform.position);
        }
    }
    
    IEnumerator death()
    {
        StartCoroutine(AllPop());
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    void pop(Transform posit)
    {
        Instantiate(deathSystem, posit.position, transform.rotation);
    }

    IEnumerator AllPop()
    {
        if(positions.Length > 0)
        {
            for(int i = 0; i < positions.Length; i++)
            {
                pop(positions[i]);
            }
        }

        yield return new WaitForSeconds(1);
    }

    IEnumerator flashDamage()
    {
        inPain = true;
        mainBody.enabled = false;
        VoxelDamage.gameObject.SetActive(true);
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(0.085f);
        VoxelDamage.gameObject.SetActive(false);
        mainBody.enabled = true;
        

        int id = Random.Range(0, numDamAnims);
        if (numDamAnims > 1)
            while (id == lastAnim)
                id = Random.Range(0, numDamAnims);
        lastAnim = id;
        anim.SetInteger("DamageID", id);
        anim.SetTrigger("Damage");
        inPain = false;

        yield return new WaitForSeconds(0.1f);

    }

    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Attack");
        for (int i = 0; i < 2; i++)
        {
            shotgun();
        }
        

        //shootPos.transform.rotation = Quaternion.LookRotation(playerDir);
        //Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }

    void shotgun()
    {
        createProjectile();
        for(int i = 0; i < 6; i++)
        {
            offsetPellets();
        }
    }

    void createProjectile()
    {
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().damage = gunDamage;
        bullet.GetComponent<Bullet>().offsetX = 0;
        bullet.GetComponent<Bullet>().offsetY = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void offsetPellets()
    {
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().damage = gunDamage;
        bullet.GetComponent<Bullet>().offsetX = Random.Range(-offset, offset);
        bullet.GetComponent<Bullet>().offsetY = Random.Range(-offset, offset);
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
        }
    }

    void moving()
    {
        if(agent.velocity != Vector3.zero)
        {
            //isMoving = true;
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
        }

    }

    bool AiRoutine()
    {
        //find direction to player if in range
        playerDir = gameManager.Instance.player.transform.position - headPos.position;
        //find angle to player within distance
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        playerDist = Vector3.Distance(gameManager.Instance.player.transform.position, transform.position);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit) && angleToPlayer <= viewAngle)
        {
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.Instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                    anim.SetBool("Aiming", true);
                    if (angleToPlayer <= shootAngle && !isShooting)
                        StartCoroutine(shoot());
                }
                else anim.SetBool("Aiming", false);
                    


                return true;
            }

        }

        return false;
    }
}

