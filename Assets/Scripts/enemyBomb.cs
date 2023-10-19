using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyBomb : MonoBehaviour, IDamage
{
    [Header("---- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem effect;
    public Spawner origin;

    [Header("---- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int damage;
    [SerializeField] int explosionRange;
    [SerializeField] AudioSource aud;

    [SerializeField] float volume;

    public AudioClip explodeSound;
    public AudioClip beepSound;

    bool playerInRange;
    bool isExploding;
    Color colorOrig;
    UnityEngine.Vector3 playerDir;

    void Start()
    {
        colorOrig = model.material.color;
        //aud.PlayOneShot(explodeSound, 5);
    }

    void Update()
    {
        if (!isExploding)
        {
            anim.SetFloat("speed", agent.velocity.normalized.magnitude);
            if (playerInRange && canSeePlayer())
            {
            }
        }
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.Instance.player.transform.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.Instance.player.transform.position);
                if (agent.remainingDistance < agent.stoppingDistance && agent.remainingDistance > 0)
                {
                    StartCoroutine(explode());
                }

                return true;
            }
        }
        return false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(gameManager.Instance.player.transform.position);
        if (HP <= 0)
        {
            if (origin != null)
                origin.heyIDied();
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    IEnumerator explode()
    {
        isExploding = true;
        agent.SetDestination(transform.position);
        anim.SetFloat("Speed", 0);
        yield return new WaitForSeconds(.5F);
        anim.enabled = false;
        

        model.material.color = Color.red;
        aud.PlayOneShot(beepSound, volume);
        yield return new WaitForSeconds(.8F);
        model.material.color = colorOrig;
        aud.PlayOneShot(beepSound, volume);
        yield return new WaitForSeconds(.7F);
        model.material.color = Color.red;
        aud.PlayOneShot(beepSound, volume);
        yield return new WaitForSeconds(.6F);
        model.material.color = colorOrig;
        aud.PlayOneShot(beepSound, volume);
        yield return new WaitForSeconds(.5F);
        model.material.color = Color.red;
        aud.PlayOneShot(beepSound, volume);
        yield return new WaitForSeconds(.4F);
        model.material.color = colorOrig;
        aud.PlayOneShot(beepSound, volume);
        yield return new WaitForSeconds(.3F);
        model.material.color = Color.red;
        aud.PlayOneShot(beepSound, volume);
        yield return new WaitForSeconds(.2F);
        model.material.color = colorOrig;
        aud.PlayOneShot(beepSound, volume);
        yield return new WaitForSeconds(.2F);
        model.material.color = Color.red;
        aud.PlayOneShot(beepSound, volume);
        yield return new WaitForSeconds(.1F);
        model.material.color = colorOrig;
        aud.PlayOneShot(beepSound, volume);
        yield return new WaitForSeconds(.1F);
        model.material.color = Color.red;
        StartCoroutine(applyDamage());
    }

    IEnumerator applyDamage()
    {
        Instantiate(effect, transform.position, Quaternion.identity);
        
        aud.PlayOneShot(explodeSound, volume);
        IDamage damageable = gameManager.Instance.player.GetComponent<IDamage>();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                float dist = Vector3.Distance(transform.position, gameManager.Instance.player.transform.position);
                if (dist < explosionRange && damageable != null)
                {
                    damageable.takeDamage(damage);
                }
            }
        }

        if (origin != null)
            origin.heyIDied();
        model.enabled = false;

        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
