using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    //[SerializeField] GameObject Player;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    Vector3 PlayerDir;

    [Header("----- Enemy States -----")]
    [SerializeField] int MaxHp;
    public int Hp;
    [SerializeField] int dodgingSpeed;
    [SerializeField] int TargetFaceSpeed;
    [SerializeField] int animChangeSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int shootAngle;
    [SerializeField] float animSpeed;//uncomment when needed

    [Header("----- Projectile States -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletSpeed;
    [SerializeField] Bullet bScript;
    public bool isShooting = false;
    bool playerInRange = false;
    float angleToPlayer;

    Color Mcolor;
    // Start is called before the first frame update
    void Start()
    {
        
        //bScript = bullet.GetComponent<Bullet>();
        //bScript.damage = shootDamage;
        //bScript.speed = bulletSpeed;
        Hp = MaxHp;
        Mcolor = model.material.color;
        gameManager.Instance.updateGameGoal(1);
        //Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //float agentVelo = agent.velocity.normalized.magnitude;
        //transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, Time.deltaTime * Speed);
        //anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentVelo, Time.deltaTime * animChangeSpeed));

        if (playerInRange && CanSeePlayer())
        {
            //PlayerDir = GameManager.Instance
            //agent.SetDestination(GameManager.Instance.player.transform.position);
            //if (!isShooting) StartCoroutine(Shoot());
        }

        if (agent.velocity != Vector3.zero) anim.SetBool("isMoving", true);
        else anim.SetBool("isMoving", false);

    }
    bool CanSeePlayer()
    {
        PlayerDir = gameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(PlayerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, PlayerDir, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.Instance.player.transform.position);
                if (agent.remainingDistance < agent.stoppingDistance)
                    FaceTarget();

                if (angleToPlayer <= shootAngle && !isShooting) StartCoroutine(Shoot());
                return true;
            }
        }
        return false;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        anim.SetTrigger("attack");
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().damage = shootDamage;
        //bScript.damage = shootDamage;
        //bScript.speed = bulletSpeed;
        shootPos.transform.rotation = Quaternion.LookRotation(PlayerDir);
        Instantiate(bullet, shootPos.position, shootPos.transform.rotation);
        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        Hp -= amount;
        anim.SetTrigger("damaged");
        StartCoroutine(FlashDamage());
        if (Hp <= 0)
        {
            anim.SetTrigger("death");
            Destroy(gameObject);
            gameManager.Instance.updateGameGoal(-1);
        }
    }

    IEnumerator FlashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.028571f);
        model.material.color = Mcolor;
    }

    void FaceTarget()
    {
        Quaternion Rot = Quaternion.LookRotation(PlayerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, Rot, Time.deltaTime * TargetFaceSpeed);
    }

    /*public void physics(Vector3 dir)
    {
        agent.velocity += dir;   // uncomment when need
    }*/

    public void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) playerInRange = true; }
    public void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) playerInRange = false; }
}
