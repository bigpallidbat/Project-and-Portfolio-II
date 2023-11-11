using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUV : MonoBehaviour , IDamage
{
    [SerializeField] AudioSource GameSounds;

    [SerializeField] GameObject myPlayer;
    [SerializeField] ParticleSystem onHit;
    [SerializeField] AudioClip audUV;
    [Range(0, 2)][SerializeField] float audUVVol;
    [SerializeField] float scrollSpeed = 0.5f;
    [SerializeField] int damage;
    [SerializeField] float timeBetweenDamage;
    [SerializeField] int HP;
    [SerializeField] bool damageOverTime;
    [SerializeField] bool isDamagable;
    Renderer rend;


    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (GameSounds != null)
        {
            GameSounds.PlayOneShot(audUV, audUVVol);
        }
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (damageOverTime == true)
        {
            StartCoroutine(DamageStart());
        }
        else
        {
            IDamage damagable = other.GetComponent<IDamage>();
            if (damagable != null)
            {
                damagable.takeDamage(damage);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;
        StopAllCoroutines();
    }

    private IEnumerator DamageStart()
    {
        yield return new WaitForSeconds(.01f);
        StartCoroutine(damageIncrement());
    }

    private IEnumerator damageIncrement()
    {
        yield return new WaitForSeconds(.01f);
        IDamage damagable = myPlayer.GetComponent<IDamage>();
        if (damagable != null)
        {
            damagable.takeDamage(damage);
        }
        yield return new WaitForSeconds(timeBetweenDamage);
        StartCoroutine(damageIncrement());
    }

    public void takeDamage(int amount)
    {
        if (isDamagable == true)
        {
            HP -= amount;
            Instantiate(onHit, transform.position, Quaternion.identity);

            if (HP <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
