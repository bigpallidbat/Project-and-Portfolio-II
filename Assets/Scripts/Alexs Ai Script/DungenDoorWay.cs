using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class DungenDoorWay : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] bool isBombDoor;

    [Header("----- Mimic stats -----")]
    [SerializeField] bool isMimic;
    [SerializeField] Collider hitBox;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform stompPos;
    [SerializeField] int Health;
    [SerializeField] float AttackRate;
    //[SerializeField] float spawnRate;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bulletAim;
    [SerializeField] int shootDamage;
    [SerializeField] int bulletSpeed;
    [SerializeField] int bulletAimSpeed;
    [SerializeField] float shotoffSetY;
    [SerializeField] GameObject ShockWave;
    [SerializeField] GameObject cameraX;
    [SerializeField] GameObject[] Brothers;
    [SerializeField] GameObject[] MonsterDoors;
    [SerializeField] float Startfire;
    [SerializeField] float Stopfire;
    [SerializeField] float StartfireSpeed;
    bool isActive;
    bool isAttacking;
    bool isInincible;
    bool dying;
    bool isAwake;

    [Header("----- Mimic Parts -----")]
    [SerializeField] GameObject doorWay;
    [SerializeField] GameObject Mimic1Wake1;
    [SerializeField] GameObject Mimic2Wake2;
    [SerializeField] GameObject Mimic3Idel;
    [SerializeField] GameObject Mimic4Attack;
    [SerializeField] GameObject Mimic5Damage1;
    [SerializeField] GameObject Mimic6Damage2;
    [SerializeField] GameObject Mimic7Damage3;
    [SerializeField] GameObject Mimic8Damage4;
    [SerializeField] GameObject Mimic9Dying1;
    [SerializeField] GameObject Mimic10Dying2;
    [SerializeField] GameObject Mimic11Dying3;
    [SerializeField] GameObject Mimic12Dying4;
    [SerializeField] GameObject Mimic13Open;
    int DiceRoll;

    [Header("----- sounds -----")]
    [SerializeField] AudioClip attckSound;
    [Range(0, 1)][SerializeField] float audAttackVol;
    [SerializeField] AudioClip VpainSound;
    [Range(0, 1)][SerializeField] float audVpainVol;
    [SerializeField] AudioClip VBlowingUp;
    [Range(0, 1)][SerializeField] float BlowUpVol;
    [SerializeField] AudioClip seeSound;
    [Range(0, 1)][SerializeField] float audSeeVol;
    [SerializeField] AudioClip ShockSound;
    [Range(0, 1)][SerializeField] float ShockVol;
    [SerializeField] AudioSource soundSFX;


    // Start is called before the first frame update
    void Start()
    {
        if (!isBombDoor) isInincible = true;
    }
    public void lowerFireRate()
    {
        AttackRate -= 0.666666f;
    }
    // Update is called once per frame
    void Update()
    {
        if (isActive && !isInincible && !isAttacking)
        {
            Attack();
        }
        // else if (isActive && dying)
    }
    void Attack()
    {
        Mimic3Idel.gameObject.SetActive(false);
        Mimic4Attack.gameObject.SetActive(true);
        isAttacking = true;
        AimedShot();
        for (float j = Startfire; j < Stopfire; j += StartfireSpeed) tomAttack(j, 15);
        isAttacking = false;
        //DiceRoll = Random.Range(0, 8);
        //switch (DiceRoll)
        //{
        //    case 0:
        //        for (float j = -42.1875f; j < 45; j += 2.8125f) tomAttack(j, 15);
        //        StartCoroutine(endAttack());
        //        break;
        //    case 1:
        //        for (float j = -87.1875f; j < 90; j += 5.625f) tomAttack(j, 15);
        //        StartCoroutine(endAttack());
        //        break;
        //    case 2:
        //        for (float j = -90f; j < 0; j += 2.8125f) tomAttack(j, 5);
        //        StartCoroutine(endAttack());
        //        break;
        //    case 3:
        //        for (float j = 0f; j < 90; j += 2.8125f) tomAttack(j, 5);
        //        StartCoroutine(endAttack());
        //        break;
        //    case 4:
        //        for (float j = -45f; j < 45; j += 2.8125f) tomAttack(j, 5);
        //        StartCoroutine(endAttack());
        //        break;
        //    default:
        //        StartCoroutine(shockWave());
        //        break;
        //}
    }
    IEnumerator shockWave()
    {
        DiceRoll = Random.Range(0, 8);
        switch (DiceRoll)
        {
            case 0:
                for (int i = 0; i < 32; i++)
                {
                    float angle = i * 5.625f - 84.375f;
                    Quaternion rotation = Quaternion.Euler(0, angle, 0);
                    Instantiate(ShockWave, transform.position + new Vector3(0, -1.75f, 0), rotation);
                    yield return new WaitForSeconds(0.028571f);
                }
                for (int i = 32; i > 0; i--)
                {
                    float angle = i * 5.625f - 87.1875f;
                    Quaternion rotation = Quaternion.Euler(0, angle, 0);
                    Instantiate(ShockWave, transform.position + new Vector3(0, -1.75f, 0), rotation);
                    yield return new WaitForSeconds(0.028571f);
                }
                StartCoroutine(endAttack());
                break;
            case 1:
                for (int i = 32; i > 0; i--)
                {
                    float angle = i * 5.625f - 84.375f;
                    Quaternion rotation = Quaternion.Euler(0, angle, 0);
                    Instantiate(ShockWave, transform.position + new Vector3(0, -1.75f, 0), rotation);
                    yield return new WaitForSeconds(0.028571f);
                }
                for (int i = 0; i < 32; i++)
                {
                    float angle = i * 5.625f - 87.1875f;
                    Quaternion rotation = Quaternion.Euler(0, angle, 0);
                    Instantiate(ShockWave, transform.position + new Vector3(0, -1.75f, 0), rotation);
                    yield return new WaitForSeconds(0.028571f);
                }
                StartCoroutine(endAttack());
                break;
            case 2:
                for (int i = 32; i > 0; i--)
                {
                    float angle = i * 5.625f - 84.375f;
                    Quaternion rotation = Quaternion.Euler(0, angle, 0);
                    Instantiate(ShockWave, transform.position + new Vector3(0, -1.75f, 0), rotation);
                    float anglej = i * -5.625f + 87.1875f;
                    Quaternion rotationj = Quaternion.Euler(0, anglej, 0);
                    Instantiate(ShockWave, transform.position + new Vector3(0, -1.75f, 0), rotationj);
                    yield return new WaitForSeconds(0.028571f);
                }
                StartCoroutine(endAttack());
                break;
            default:
                StartCoroutine(endAttack());
                break;
        }
        yield return null;
    }
    IEnumerator endAttack()
    {
        Mimic4Attack.gameObject.SetActive(false);
        Mimic3Idel.gameObject.SetActive(true);
        yield return new WaitForSeconds(AttackRate);
        isAttacking = false;
    }
    void tomAttack(float offX, int speed)
    {
        bullet.GetComponent<Bullet>().speed = speed;
        bullet.GetComponent<Bullet>().damage = shootDamage;
        bullet.GetComponent<Bullet>().offsetX = offX;
        bullet.GetComponent<Bullet>().offsetY = Random.Range(-shotoffSetY + 0.25f, shotoffSetY);
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void AimedShot()
    {
        bulletAim.GetComponent<Bullet>().speed = bulletAimSpeed;
        bulletAim.GetComponent<Bullet>().damage = shootDamage;
        bulletAim.GetComponent<Bullet>().offsetX = 0;
        bulletAim.GetComponent<Bullet>().offsetY = 0;
        Instantiate(bulletAim, shootPos.position, transform.rotation);
    }
    public void takeDamage(int amount)
    {
        if (!isInincible)
        {
            soundSFX.PlayOneShot(VpainSound, audVpainVol);
            isInincible = true;
            Health -= amount;
            if (Health <= 0)
            {
                StopAllCoroutines();
                dying = true;
                StartCoroutine(Dying());
            }
            else StartCoroutine(FlashDamage());
        }
    }
    IEnumerator FlashDamage()
    {
        Mimic4Attack.gameObject.SetActive(false);
        Mimic3Idel.gameObject.SetActive(false);
        Mimic5Damage1.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.028571f);
        Mimic5Damage1.gameObject.SetActive(false);
        Mimic6Damage2.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.028571f);
        Mimic6Damage2.gameObject.SetActive(false);
        Mimic7Damage3.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.028571f);
        Mimic7Damage3.gameObject.SetActive(false);
        Mimic8Damage4.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.028571f);
        Mimic8Damage4.gameObject.SetActive(false);
        Mimic3Idel.gameObject.SetActive(true);
        isInincible = false;
    }
    IEnumerator Dying()
    {
        Mimic4Attack.gameObject.SetActive(false);
        Mimic3Idel.gameObject.SetActive(false);
        Mimic9Dying1.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic9Dying1.gameObject.SetActive(false);
        Mimic10Dying2.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic10Dying2.gameObject.SetActive(false);
        Mimic11Dying3.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic11Dying3.gameObject.SetActive(false);
        Mimic12Dying4.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic12Dying4.gameObject.SetActive(false);
        Mimic9Dying1.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic9Dying1.gameObject.SetActive(false);
        Mimic10Dying2.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic10Dying2.gameObject.SetActive(false);
        Mimic11Dying3.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic11Dying3.gameObject.SetActive(false);
        Mimic12Dying4.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic12Dying4.gameObject.SetActive(false);
        Mimic9Dying1.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic9Dying1.gameObject.SetActive(false);
        Mimic10Dying2.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic10Dying2.gameObject.SetActive(false);
        Mimic11Dying3.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic11Dying3.gameObject.SetActive(false);
        Mimic12Dying4.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        Mimic12Dying4.gameObject.SetActive(false);
        Mimic9Dying1.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.057142f);
        foreach (GameObject BRO in Brothers)
        {
            if (BRO != null) BRO.GetComponent<DungenDoorWay>().lowerFireRate();
        }
        isActive = false;
        Mimic9Dying1.gameObject.SetActive(false);
        Mimic13Open.gameObject.SetActive(true);
    }
    public void wakeUp()
    {
        if (isMimic && !isAwake)
        {
            StartCoroutine(wakingUp());
        }
    }
    IEnumerator wakingUp()
    {
        isAwake = true;
        doorWay.gameObject.SetActive(false);
        Mimic1Wake1.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.085714f);
        Mimic1Wake1.gameObject.SetActive(false);
        Mimic2Wake2.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.085714f);
        Mimic2Wake2.gameObject.SetActive(false);
        Mimic3Idel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.142857f);
        Mimic3Idel.gameObject.SetActive(false);
        Mimic4Attack.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.285714f);
        Mimic3Idel.gameObject.SetActive(true);
        Mimic4Attack.gameObject.SetActive(false);
        isInincible = false;
        yield return new WaitForSeconds(0.2f);
        isActive = true;
    }
}
