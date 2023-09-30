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
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    Vector3 PlayerDir;

    [Header("----- Enemy States -----")]
    [SerializeField] int MaxHp;
    public int Hp;
    [SerializeField] int Speed;
    [SerializeField] int TargetFaceSpeed;

    [Header("----- Projectile States -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float ShootRate;
    [SerializeField] float ShootDamage;
    public bool isShooting = false;
    bool playerInRange = false;

    Color Mcolor;
    // Start is called before the first frame update
    void Start()
    {
        Hp = MaxHp;
        Mcolor = model.material.color;
        gameManager.Instance.updateGameGoal(1);
        //Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, Time.deltaTime * Speed);

        if (playerInRange)
        {
            //PlayerDir = GameManager.Instance
            PlayerDir = gameManager.Instance.player.transform.position - transform.position;
            if (agent.remainingDistance < agent.stoppingDistance) FaceTarget();
            agent.SetDestination(gameManager.Instance.player.transform.position);
            if (!isShooting) StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(ShootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        Hp -= amount;
        StartCoroutine(FlashDamage());
        if (Hp <= 0)
        {
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

    public void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) playerInRange = true; }
    public void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) playerInRange = false; }
}
