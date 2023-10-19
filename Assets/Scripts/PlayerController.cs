using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource PlayerSounds;

    [SerializeField] Collider actionRange;
    private bool canActivate;
    private IInteract actionable;
    [SerializeField] GameObject nade;
    [SerializeField] GameObject throwPos;
    

    [Header("----- player state -----")]
    [Range(1, 10)][SerializeField] int HP;
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
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootdist;

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

    private void Start()
    {
        HPMax = HP;
        maxStam = Stamina;
        origPlayerSpeed = playerSpeed;
        spawnPlayer();

        Debug.Log(HP);
        Debug.Log(HPMax);
        Debug.Log(gunList);
    }

    void Update()
    {
        if (gunList.Count > 0)
        {
            selectGun();

            if (Input.GetButton("Shoot") && !isShooting)
                StartCoroutine(shoot());
        }
        Movement();
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

        controller.Move(move * Time.deltaTime * playerSpeed);

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
            gunList[selectedGun].ammoCur--;
            PlayerSounds.PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].shootSoundVol);
            gameManager.Instance.updateAmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoReserve);

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootdist))
            {
                IDamage damgable = hit.collider.GetComponent<IDamage>();

                if (hit.collider.transform.position != transform.position && damgable != null)
                {
                    damgable.takeDamage(shootDamage);
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

    public void setGunStats(gunStats gun)
    {
        gunList.Add(gun);

        shootDamage = gun.shootDamage;
        shootdist = gun.shootdist;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
        gunModel.transform.localScale = gun.model.transform.localScale;

        //needs to set model rotation to force gun models to face forward
        //if (gun.ID == 1)
        //{
        //    gunModel.transform.rotation = gunList[selectedGun].model.transform.rotation;
        //}
        //else if(gun.ID == 2)
        //{
        //    gunModel.transform.rotation = Quaternion.Euler(0, 0, 0);
        //}

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
        shootDamage = gunList[selectedGun].shootDamage;
        shootdist = gunList[selectedGun].shootdist;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
        //gunModel.transform.localScale = gunList[selectedGun].size.localScale;
        gunModel.transform.localScale = gunList[selectedGun].model.transform.localScale;

        ////needs to set model rotation to force gun models to face forward
        //if (gunList[selectedGun].ID == 1)
        //{
        //    gunModel.transform.rotation = gunList[selectedGun].model.transform.rotation;
        //}
        //else if (gunList[selectedGun].ID == 2)
        //{
        //    gunModel.transform.rotation = Quaternion.Euler(0, 0, 0);
        //}

        gameManager.Instance.updateAmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoReserve);

        isShooting = false;
    }

    void Reload()
    {
        if (Input.GetButtonDown("Reload"))
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
        if (Input.GetButtonDown("Action") && canActivate)
        {
            actionable.Activate();
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

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        actionable = other.GetComponent<IInteract>();

        if (actionable != null)
        {
            canActivate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        actionable = null;
        canActivate = false;
    }

    private void getSpawnStats()
    {
        gunList.Add(stats.gunList[0]);
        selectedGun = 0;
        changeGun();
        grenadeCount = stats.grenadeCount;
        medkitCount = stats.medkitCount;
        gunList[selectedGun].ammoReserve = gunList[selectedGun].ammoReserveStart;

    }

    public void getSpawnStats(bool check)
    {

        for(int i = 0; i < stats.gunCount; i++)
        {
            gunList.Add(stats.gunList[i]);
        }
        selectedGun = 0; changeGun();
        HP = stats.hpcur;
        HPMax = stats.hpmax;
        medkitCount = stats.medkitCount;
        grenadeCount = stats.grenadeCount;

    }

    public void setStats(bool check)
    {

        for(int i = 0;i < gunList.Count; i++)
        {
            stats.gunList[i] = gunList[i];
        }

        

        stats.gunCount = gunList.Count;
        stats.hpcur = HP;
        stats.hpmax = HPMax;
        stats.grenadeCount = grenadeCount;
        stats.medkitCount = medkitCount;

    }
    public void setStats()
    {
        if(stats.gunCount > 1)
            stats.gunList.Clear();
        stats.gunList[0] = stats.startingGunList[0];
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
    }

}
