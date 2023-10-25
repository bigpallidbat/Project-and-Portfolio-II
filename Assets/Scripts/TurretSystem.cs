using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretSystem : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] GameObject baseRotation;
    [SerializeField] GameObject gunBody;
    [SerializeField] GameObject gunBarrel;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem tracer;
    [SerializeField] string tagName;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] AudioClip gunShot;

    [Header("----- Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int rotationSpeed;
    [SerializeField] int Damage;
    [SerializeField] int bulletsSpeed;
    [SerializeField] int FiringRange;
    [SerializeField] float Frequency;
    [SerializeField] float offSet;

    bool canFire;
    bool isFiring;
    Transform targetLocal;
    float currentRotSpeed;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SphereCollider>().radius = FiringRange;
        bulletsSpeed = (int)(tracer.main.startSpeed.constant / offSet);
        muzzleFlash.Stop();
        tracer.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        AimAndFire();
    }

    void AimAndFire()
    {
        if (targetLocal != null)
        {
            Vector3 targetPos = targetLocal.position - shootPos.position;

            //want to start barrel rotation
            gunBarrel.transform.Rotate(0, 0, currentRotSpeed * Time.deltaTime);

            RaycastHit hit;
            Debug.DrawRay(shootPos.position, targetPos, Color.blue);
            if (Physics.Raycast(shootPos.position, targetPos, out hit))
            {
                if (canFire && hit.collider.CompareTag(tagName))
                {
                    //start rotation
                    currentRotSpeed = rotationSpeed;
                    //aim at enemy

                    Vector3 targetBase = new Vector3(targetLocal.transform.position.x, this.transform.position.y, targetLocal.transform.position.z);
                    Vector3 targetGun = new Vector3(targetLocal.transform.position.x, targetLocal.transform.position.y, targetLocal.transform.position.z);

                    baseRotation.transform.LookAt(targetBase);
                    gunBody.transform.LookAt(targetGun);


                    //start coroutine bulletStorm
                    if(!isFiring)
                        StartCoroutine(bulletStorm());
                }
                else
                {
                    //slowdown rotation with lerp
                    currentRotSpeed = Mathf.Lerp(currentRotSpeed, 0, Time.deltaTime);
                    //stop coroutine shoot
                    StopCoroutine(bulletStorm());
                    //stop particle system
                    muzzleFlash.Stop();
                    //set isFiring to false
                    isFiring = false;
                }
            }
        }
    }

    IEnumerator bulletStorm()
    {
        isFiring = true;

        createBullet();
        if (muzzleFlash != null && !muzzleFlash.isPlaying)
        {
            muzzleFlash.Play();
        }

        yield return new WaitForSeconds(Frequency);

        isFiring = false;
    }

    private void createBullet()
    {
        bullet.GetComponent<Bullet>().speed = bulletsSpeed;
        bullet.GetComponent<Bullet>().damage = Damage;
        bullet.GetComponent<Bullet>().offsetX = 0;
        bullet.GetComponent<Bullet>().offsetY = 0;
        bullet.GetComponent<AudioSource>().clip = null;

        Instantiate(bullet, shootPos.position, Quaternion.identity);
    }

    public void takeDamage(int impact)
    {
        HP -= impact;

        if(HP <= 0 )
        {
            Destroy(gameObject);
        }
    }

    //target enter field, Take target Pos
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagName))
        {
            targetLocal = other.transform;
            canFire = true;
        }
    }

    //target exit field
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagName))
        {
            canFire = false;
        }
    }

}
