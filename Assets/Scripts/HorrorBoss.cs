using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class HorrorBoss : MonoBehaviour, IDamage
{

    [Header("-----Components-----")]
    [SerializeField] Collider defCol;
    [SerializeField] Transform headPos;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Collider attackCol;
    [SerializeField] Animator anim;
    

    [Header("-----Stats-----")]
    [SerializeField] int Hp;
    [SerializeField] int Damage;
    [SerializeField] float targetFaceSpeed;
    [SerializeField] float viewAngle;
    [SerializeField] float Speed;
    [SerializeField] float attackTime;
    [SerializeField] float damageResist;

    Vector3 playerDir;
    float angleToPlayer;
    float maxDistance = 30f;
    bool knowsPlayerLocation = true;
    bool isAttacking;
    public bool huntPlayer;
    bool resist;

    // Start is called before the first frame update
    void Start()
    {
        defCol.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        //checks if it can see the player or knows where the player is. This is for if the boss has points where it does not know where the player is
        if (AiRoutine() || knowsPlayerLocal())
        {
            
        }
    }

    bool AiRoutine()
    {

        float agentVel = agent.velocity.normalized.magnitude;
        anim.SetFloat("speed", Mathf.Lerp(anim.GetFloat("speed"), agentVel, Time.deltaTime * Speed));

        //find direction to player if in range
        playerDir = gameManager.Instance.player.transform.position - headPos.position;
        //find angle to player within distance
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        
        //checks the distance and changes speed accordingly
        checkDistance();
        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit) && angleToPlayer <= viewAngle)
        {
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.Instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    //constantly faces the target while in stopping distance
                    faceTarget();
                    if (huntPlayer && !isAttacking)
                    {
                        //when in range and hunting it attacks the player
                        StartCoroutine(Attack());
                    }
                        
                }



                return true;
            }

        }

        return false;
    }

    IEnumerator Resist()
    {
        //after taking damage the boss will not take damage for a damageResist amount of time
        resist = true;
        yield return new WaitForSeconds(damageResist);
        resist = false;
    }

    IEnumerator Attack()
    {
        //start animation and set isattacking to true. then end with isattacking to false
        anim.SetTrigger("Attack");
        isAttacking = true;
        yield return new WaitForSeconds(attackTime);

        isAttacking = false;
    }

    void checkDistance()
    {
        //checks the distance and changes speed accordingly
        if (agent.isActiveAndEnabled)
        {
        if (agent.remainingDistance < maxDistance)
             agent.speed = Speed;
        else agent.speed = Speed*3;
        }

    }

    bool knowsPlayerLocal()
    {
        //if boss has knowsplayerlocation as true -which is default for this enemy- it chases the player. allows points where the boss doesn't know where the player is
        if (agent.isActiveAndEnabled)
        {
        if (knowsPlayerLocation)
        {

            agent.SetDestination(gameManager.Instance.player.transform.position);

            return true;
        }
        }

        return false;
    }

    public void setKnowledge(bool enable)
    {
        //sets if the boss knows player location or not
        knowsPlayerLocation = enable;
    }

    void faceTarget()
    { 
        //faces player
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    }

    public void takeDamage(int dam)
    {
        anim.SetTrigger("Hurt");
        //checks if resisting, if false, takes damage and sets resisting.
        if (!resist)
        {

            Hp--;
            StartCoroutine(Resist());
        }
        else return;

        if(Hp <= 0)
        {
            StartCoroutine(death());
            //calls to the function in gamemanager to turn on the level's end door
            gameManager.Instance.horrorEnd();
        }
    }

    IEnumerator death()
    {
        //allows animations before destroying the object
        agent.enabled = false;
        yield return new WaitForSeconds(0.8f);
        anim.SetBool("Dead", true);
    }
    
    IEnumerator flashDamage()
    {
        //shows damage to the player, can use sounds

        yield return new WaitForSeconds(0.5f);

    }

    public void Hunt()
    {
        //After goal is set, 
        huntPlayer = true;
        viewAngle = 100;
        agent.stoppingDistance = 7;
        defCol.enabled = true;
    }

    public void turnAttack()
    {
        attackCol.enabled = !attackCol.enabled;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamage damage = other.GetComponent<IDamage>();
            if (damage != null)
            {
                damage.takeDamage(Damage);
            }
        }
    }
}
