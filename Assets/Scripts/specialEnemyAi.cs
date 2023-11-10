using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class specialEnemyAi : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] GameObject Gun;
    [SerializeField] Transform LeftHand;
    [SerializeField] Transform RightHand;
    [SerializeField] ParticleSystem deathSystem;

    [Header("----- Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int targetFaceSpeed;
    [Range(30, 180)][SerializeField] int viewAngle;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;
    [SerializeField] int gunDamage;
    [SerializeField] int bulletSpeed;
    [Range(30, 180)][SerializeField] int shootAngle;

    float angleToPlayer;
    Vector3 playerDir;
    const int numDamAnims = 3;
    int lastAnim = -1;
    bool isShooting;
    bool playerInRange;
    bool isMoving;

    // Start is called before the first frame update
    void Start()
    {

        setModel();

    }

    // Update is called once per frame
    void Update()
    {

        if (playerInRange && AiRoutine())
        {

        }
        moving();
        
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
        StartCoroutine(flashDamage());
        agent.SetDestination(gameManager.Instance.player.transform.position);
        if (HP <= 0)
        {
            anim.SetTrigger("Death");
            gameManager.Instance.updateGameGoal();
            StopAllCoroutines();
            Instantiate(deathSystem, transform.position, transform.rotation);
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(death());
        }
    }
    
    IEnumerator death()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    IEnumerator flashDamage()
    {
        int id = Random.Range(0, numDamAnims);
        if (numDamAnims > 1)
            while (id == lastAnim)
                id = Random.Range(0, numDamAnims);
        lastAnim = id;
        anim.SetInteger("DamageID", id);
        anim.SetTrigger("Damage");

        yield return new WaitForSeconds(0.1f);

    }

    IEnumerator shoot()
    {
        isShooting = true;
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().damage = gunDamage;
        //bScript.damage = shootDamage;
        //bScript.speed = bulletSpeed;
        shootPos.transform.rotation = Quaternion.LookRotation(playerDir);
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(fireRate);
        isShooting = false;
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
            isMoving = true;
            anim.SetBool("Aiming", !isMoving);
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
        }

    }

    bool AiRoutine()
    {
        //find direction to player if in range
        playerDir = gameManager.Instance.player.transform.position - headPos.position;
        //find angle to player within distance
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit) && angleToPlayer <= viewAngle)
        {
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.Instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                    anim.SetFloat("Speed", 0);
                    anim.SetBool("Aiming", true);
                    if (angleToPlayer <= shootAngle && !isShooting)
                        StartCoroutine(shoot());
                }
                    


                return true;
            }

        }

        return false;
    }
}

