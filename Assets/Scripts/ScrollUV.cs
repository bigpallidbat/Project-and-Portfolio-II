using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUV : MonoBehaviour
{
    [SerializeField] GameObject myPlayer;
    [SerializeField] float scrollSpeed = 0.5f;
    [SerializeField] int damage;
    [SerializeField] float timeBetweenDamage;
    [SerializeField] bool damageOverTime;
    Renderer rend;


    private void Start()
    {
        rend = GetComponent<Renderer>();
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
        yield return new WaitForSeconds(timeBetweenDamage);
        IDamage damagable = myPlayer.GetComponent<IDamage>();
        if (damagable != null)
        {
            damagable.takeDamage(damage);
        }
        StartCoroutine(damageIncrement());
    }
}
