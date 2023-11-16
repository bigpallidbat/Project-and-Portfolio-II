using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource PlayerSounds;

    


    [SerializeField] GameObject nade;
    [SerializeField] GameObject throwPos;
    [SerializeField] GameObject flashLight;
    

    [Header("----- player state -----")]
    [Range(1, 15)][SerializeField] int HP;
    int HPMax;
    public static int curHP;
    [Range(1, 10)][SerializeField] float playerSpeed;
    [Range(1, 3)][SerializeField] float SprintMod;
    [Range(1, 3)][SerializeField] int JumpMax;
    [Range(8, 30)][SerializeField] float jumpHeight;
    [Range(-40f, -9.81f)][SerializeField] float gravityValue;
    [SerializeField] float Stamina;
    float maxStam;
    [SerializeField] float runCost;
    [SerializeField] float ChargeRate;
    float origPlayerSpeed;
    [SerializeField] playerStats stats;
    [SerializeField] int grenadeCount;
    [SerializeField] int medkitCount;
    [SerializeField] int medkitHeal;


    [Header("----- Gun States -----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject gunModel2;
    [SerializeField] GameObject gunModel3;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootdist;
    [SerializeField] Transform shootPos;
    [SerializeField] snipertrail SniperTrail;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] audDamage;
    [Range(0, 1)][SerializeField] float audDamageVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audReload;
    [Range(0, 1)][SerializeField] float audReloadVol;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    private int jumpedTimes;
    bool isSprinting;
    bool isShooting;
    private Coroutine recharge;
    int selectedGun;
    bool footstepsPlaying;
    private IInteract actionable;

    private void Start()
    {
        HPMax = HP;
        maxStam = Stamina;
        origPlayerSpeed = playerSpeed;
        if (!sceneManager.scenechange)
        {
            spawnPlayer();
        }
        else
        {
            spawnPlayer(Quaternion.identity);
        }


    }

    void Update()
    {
        if (!gameManager.Instance.isPaused)
        {
            if (gunList.Count > 0)
            {
                selectGun();

                if (Input.GetButton("Shoot") && !isShooting)
                    StartCoroutine(shoot());
            }
            Movement();
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
            playerSpeed *= SprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            playerSpeed /= SprintMod;
        }
        if (isSprinting)
        {
            Stamina -= runCost * Time.deltaTime;
            if (Stamina < 0)
            {
                Stamina = 0;
            }

            if(recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(ReachargeStamina());
        }
        if(Stamina == 0)
        {
            playerSpeed = origPlayerSpeed;
        }
        UpdatePlayerUI();
    }

    void Movement()
    {
        Sprint();
        Reload();
        inputs();

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && move.normalized.magnitude > 0.3f && !footstepsPlaying)
            StartCoroutine(playFootsteps());

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }

        move = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(move * Time.deltaTime * (playerSpeed + stats.speedBuff));

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && jumpedTimes < JumpMax)
        {
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
            PlayerSounds.PlayOneShot(audJump[UnityEngine.Random.Range(0, audJump.Length)], audJumpVol);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator playFootsteps()
    {
        footstepsPlaying = true;

        PlayerSounds.PlayOneShot(audSteps[UnityEngine.Random.Range(0, audSteps.Length)], audStepsVol);
        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);
        footstepsPlaying = false;
    }

    IEnumerator shoot()
    {
        if (gunList[selectedGun].ammoCur > 0)
        {
            isShooting = true;

            RaycastHit hit;
            if (!gunList[selectedGun].IsRaycast)
            {
                shootdist = 10000;
            }

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootdist))
            {
                IDamage damgable = hit.collider.GetComponent<IDamage>();

                    Vector3 toTarget =  (hit.point - shootPos.position).normalized;

                    gunList[selectedGun].projectile.GetComponent<Bullet>().dir = toTarget;
                    gunList[selectedGun].ammoCur--;
                    PlayerSounds.PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].shootSoundVol);
                    gameManager.Instance.updateAmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoReserve);
                if (!gunList[selectedGun].IsRaycast)
                    Instantiate(gunList[selectedGun].projectile, shootPos.position, shootPos.transform.rotation);
                else
                {
                    SniperTrail.dir = toTarget;
                    SniperTrail.speed = 1000;
                    Instantiate(SniperTrail, shootPos.position, shootPos.transform.rotation);
                    if (hit.collider.transform.position != transform.position && damgable != null)
                    {
                        


                        damgable.takeDamage(shootDamage + stats.damageBuff);
                        if (!hit.collider.GetComponent<spawnerDestroyable>())
                        {
                            
                            Instantiate(gunList[selectedGun].hitEffectEnemy, hit.point, Quaternion.identity);
                        }
                    }
                    else
                    {
                        Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);
                    }
                }
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        PlayerSounds.PlayOneShot(audDamage[UnityEngine.Random.Range(0, audDamage.Length)], audDamageVol);
        StartCoroutine(gameManager.Instance.playerFlash());

        UpdatePlayerUI();

        if (HP <= 0)
            gameManager.Instance.YouLose();
    }

    public void spawnPlayerminor()
    {
        HP = HPMax + stats.hpBuff;
        UpdatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.Instance.playerSpawnPoint.transform.position;
        controller.enabled = true;
    }

    public void spawnPlayer()
    {
        HP = HPMax + stats.hpBuff;
        UpdatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.Instance.playerSpawnPoint.transform.position;
        controller.enabled = true;
        getSpawnStats();
    }
    
    public void spawnPlayer(quaternion rot)
    {
        if (HP > 0)
            stats.hpcur = HP;
        else HP = curHP;

        UpdatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.Instance.playerSpawnPoint.transform.position;
        transform.rotation = rot;
        controller.enabled = true;
        getSpawnStats(true);
        sceneManager.scenechange = false;
    }

    void UpdatePlayerUI()
    {
        gameManager.Instance.playerHpBar.fillAmount = (float)HP / HPMax;
        gameManager.Instance.playerStamBar.fillAmount = (float)Stamina / maxStam;
    }

    private IEnumerator ReachargeStamina()
    {
        yield return new WaitForSeconds(1f);

        while(Stamina < maxStam)
        {
            Stamina += ChargeRate / 10f;
            if (Stamina > maxStam) Stamina = maxStam;
            UpdatePlayerUI();
            yield return new WaitForSeconds(.1f);
        }
    }
    public void setHP()
    {
        curHP = HP;
    }

    void updateHP()
    {
        HPMax = stats.hpmax + stats.hpBuff;
        HP += stats.hpBuff;
        UpdatePlayerUI();
    }

    public void setGunStats(gunStats gun)
    {
        gunList.Add(gun);
        if(gun.IsRaycast) shootDamage = gun.shootDamage ;
        
        else gun.projectile.GetComponent<Bullet>().damage = gun.shootDamage + stats.damageBuff;
       
        shootdist = gun.shootdist;
        shootRate = gun.shootRate;
        if (!gun.IsRaycast) { gun.projectile.GetComponent<Bullet>().speed = gun.projectileSpeed; gun.projectile.GetComponent<Bullet>().DestroyTime = (gun.shootdist) / (gun.projectileSpeed); }



        if (gun.ID == 1)
        {
            gunModel3.SetActive(false);
            gunModel2.SetActive(false);
            gunModel.SetActive(true);
            gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
            gunModel.transform.localScale = gun.model.transform.localScale;
        }

        if(gun.ID == 2)
        {
            gunModel3.SetActive(false);
            gunModel.SetActive(false);
            gunModel2.SetActive(true);
            shootPos.localPosition = gun.ShootPos;
            gunModel2.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
            gunModel2.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
            gunModel2.transform.localScale = gun.model.transform.localScale;
        }

        if(gun.ID == 3)
        {
            gunModel2.SetActive(false);
            gunModel.SetActive(false);
            gunModel3.SetActive(true);
            gunModel3.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
            gunModel3.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
            gunModel3.transform.localScale = gun.model.transform.localScale;
        }
      
        selectedGun = gunList.Count - 1;

        gameManager.Instance.updateAmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoReserve);
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count)
        {
            selectedGun++;
            if (selectedGun <= -1)
            {
                selectedGun = gunList.Count - 1;
            }
            else if (selectedGun >= gunList.Count)
            {
                selectedGun = 0;
            }
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > -1)
        {
            selectedGun--;
            if (selectedGun <= -1)
            {
                selectedGun = gunList.Count - 1;
            }
            else if (selectedGun >= gunList.Count)
            {
                selectedGun = 0;
            }
            changeGun();
        }
    }

    void changeGun()
    {


        if(gunList[selectedGun].IsRaycast) shootDamage = gunList[selectedGun].shootDamage;
        else gunList[selectedGun].projectile.GetComponent<Bullet>().damage = gunList[selectedGun].shootDamage+ stats.damageBuff;
        shootdist = gunList[selectedGun].shootdist;
        shootRate = gunList[selectedGun].shootRate;

        if (!gunList[selectedGun].IsRaycast)
        {
            gunList[selectedGun].projectile.GetComponent<Bullet>().speed = gunList[selectedGun].projectileSpeed;
            gunList[selectedGun].projectile.GetComponent<Bullet>().DestroyTime = (gunList[selectedGun].shootdist) / (gunList[selectedGun].projectileSpeed);
        }

        if (gunList[selectedGun].ID == 1)
        {
            gunModel3.SetActive(false);
            gunModel2.SetActive(false);
            gunModel.SetActive(true);
            gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
            gunModel.transform.localScale = gunList[selectedGun].model.transform.localScale;
        }

        if (gunList[selectedGun].ID == 2)
        {
            gunModel3.SetActive(false);
            gunModel.SetActive(false);
            gunModel2.SetActive(true);
            gunModel2.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
            gunModel2.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
            gunModel2.transform.localScale = gunList[selectedGun].model.transform.localScale;
            shootPos.localPosition = gunList[selectedGun].ShootPos;
        }   

        if (gunList[selectedGun].ID == 3)
        {
            gunModel2.SetActive(false);
            gunModel.SetActive(false);
            gunModel3.SetActive(true);
            gunModel3.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
            gunModel3.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
            gunModel3.transform.localScale = gunList[selectedGun].model.transform.localScale;
        }



        gameManager.Instance.updateAmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoReserve);

        isShooting = false;
    }

    void Reload()
    {
        if (Input.GetButtonDown("Reload") && gunList[selectedGun].ammoCur != gunList[selectedGun].ammoMax)
        {
            if (gunList[selectedGun].ammoReserve > 0)
            {
                gunList[selectedGun].ammoCur = gunList[selectedGun].ammoMax;
                PlayerSounds.PlayOneShot(audReload[UnityEngine.Random.Range(0, audReload.Length)], audReloadVol);
                gunList[selectedGun].ammoReserve--;
                gameManager.Instance.updateAmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoReserve);

            }

        }
    }

 

    void inputs()
    {
        if (Input.GetButtonDown("Action"))
        {
            if(actionable != null)
            {
                actionable.Activate();
            }
            
        }
        if (Input.GetButtonDown("Grenade") && grenadeCount > 0)
        {
            nade.GetComponent<grenade>().player = throwPos;
            nade.GetComponent<grenade>().ThrowGrenade();
            grenadeCount--;
            gameManager.Instance.updateGrenade(grenadeCount);
        }
        if (Input.GetButtonDown("Heal") && medkitCount > 0)
        {
            medkitCount--;

            int amountToHeal = (HPMax - HP);
            

            if (amountToHeal >= medkitHeal && HP + medkitHeal !> HPMax)
                HP += medkitHeal;
            else HP = HPMax;
            gameManager.Instance.updateMedkit(medkitCount);
            UpdatePlayerUI();

        }
        if (Input.GetButtonDown("Flashlight"))
        {
            if(flashLight.GetComponent<Light>().enabled == false)
            {
                flashLight.GetComponent<Light>().enabled = true;
            }
            else
            {
                flashLight.GetComponent<Light>().enabled = false;
            }
        }
    }


    private void getSpawnStats()
    {
        gunList.Clear();
        for (int i = 0; i < stats.startingGunList.Count; ++i)
        {
            gunList.Add(stats.startingGunList[i]);
        }
        selectedGun = 0;
        gunList[selectedGun].ammoCur = gunList[selectedGun].ammoMax;
        gunList[selectedGun].ammoReserve = gunList[selectedGun].ammoReserveStart;
        changeGun();
        grenadeCount = stats.grenadeCount;
        medkitCount = stats.medkitCount;
        gameManager.Instance.updateGrenade(grenadeCount);
        gameManager.Instance.updateMedkit(medkitCount);

    }

    public void getSpawnStats(bool check)
    {
        gunList.Clear();
        for(int i = 0; i < stats.gunCount; ++i)
        {
            gunList.Add(stats.gunList[i]);
        }
        selectedGun = 0;
        gunList[selectedGun].ammoCur = gunList[selectedGun].ammoMax;
        changeGun();
        HP = stats.hpcur;
        HPMax = stats.hpmax;
        medkitCount = stats.medkitCount;
        grenadeCount = stats.grenadeCount;
        gameManager.Instance.updateGrenade(grenadeCount);
        gameManager.Instance.updateMedkit(medkitCount);
    }

    public void setStats(bool check)
    {
        stats.gunList.Clear();

        for(int i = 0;i < gunList.Count; ++i)
        {
            stats.gunList.Add(gunList[i]);
        }

        

        stats.gunCount = gunList.Count;
        stats.hpcur = HP;
        stats.hpmax = HPMax;
        stats.grenadeCount = grenadeCount;
        stats.medkitCount = medkitCount;
        stats.hpBuff = 0;

    }
    public void setStats()
    {
        if(stats.gunCount > 1)
            stats.gunList.Clear();
        for(int i = 0; i  < gunList.Count; ++i)
            {
            stats.gunList.Add(stats.startingGunList[i]);
            }
        gunList.Clear();
        stats.hpcur = 0;
        stats.hpmax = HPMax;
        stats.gunCount = 1;
        stats.grenadeCount = 2;
        stats.hpBuff = 0;
        stats.speedBuff = 0;
        stats.damageBuff = 0;
        stats.medkitCount = 2;
    }

    public void setBuff(int amount, itemStats.itemType type)
    {

        switch (type)
        {
            case itemStats.itemType.healing:

                medkitHeal = amount;
                medkitCount++;
                gameManager.Instance.updateMedkit(medkitCount);

                break;
            case itemStats.itemType.Damage:

                stats.damageBuff = amount; //Damage buff add damage

                break;

            case itemStats.itemType.Speed:

                stats.speedBuff = amount; //Speed buff add speed

                break;
            case itemStats.itemType.Ammo:

                restoreAmmo(amount); //Restore Ammo

                break;
            case itemStats.itemType.Health: //Health buffs add HP

                stats.hpBuff = amount;
                updateHP();

                break;

            default: break;
        }
    }

    public void itemPickUpEffect(itemStats item)
    {

        if (item.type == itemStats.itemType.grenade) {
            grenadeCount++;
            gameManager.Instance.updateGrenade(grenadeCount);
        }
        else
        {
            setBuff(item.amount, item.type);
        }
    }

    void restoreAmmo(int amount)
    {
        for(int i = 0; i < gunList.Count; i++)
        {
            gunList[i].ammoReserve += amount;
        }
        gameManager.Instance.updateAmmo(gunList[selectedGun].ammoCur,gunList[selectedGun].ammoReserve);
    }

    public void SetActionable(IInteract obj)
    {
        actionable = obj;
    }

}
