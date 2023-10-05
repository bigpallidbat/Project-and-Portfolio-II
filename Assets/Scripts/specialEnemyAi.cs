using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    Color oColor;
    bool isShooting;
    bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        oColor = model.material.color;
        setModel();

    }

    // Update is called once per frame
    void Update()
    {

        if (playerInRange && AiRoutine())
        {

        }
    }

    void setModel()
    {
        Gun = Instantiate(Gun);
        Gun.transform.parent = RightHand;
        //Gun.transform.localScale = Vector3.one;
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
            gameManager.Instance.updateGameGoal();
            Destroy(gameObject);
        }
    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = oColor;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Gun.GetComponent<MeshRenderer>().enabled = true;
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().damage = gunDamage;
        //bScript.damage = shootDamage;
        //bScript.speed = bulletSpeed;
        shootPos.transform.rotation = Quaternion.LookRotation(playerDir);
        Instantiate(bullet, shootPos.position, transform.rotation);
        //Gun.GetComponent<MeshRenderer>().enabled = false;
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

    bool AiRoutine()
    {
        //find direction to player if in range
        playerDir = gameManager.Instance.player.transform.position - headPos.position;
        //find angle to player within distance
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit) && angleToPlayer <= viewAngle)
        {
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.Instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                    faceTarget();

                if (angleToPlayer <= shootAngle && !isShooting)
                    StartCoroutine(shoot());
                return true;
            }

        }

        return false;
    }
}

