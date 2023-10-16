using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource PlayerSounds;

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
            gameManager.Instance.updateAmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoMax);

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootdist))
            {
                IDamage damgable = hit.collider.GetComponent<IDamage>();

                if (hit.collider.transform.position != transform.position && damgable != null)
                {
                    damgable.takeDamage(shootDamage);
                    Instantiate(gunList[selectedGun].hitEffectEnemy, hit.point, Quaternion.identity);
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
        if (HP > 0)
            HPMax = HP;
        else HP = curHP;
        UpdatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.Instance.playerSpawnPoint.transform.position;
        controller.enabled = true;
    }
    
    public void spawnPlayer(quaternion rot)
    {
        HP = HPMax;
        UpdatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.Instance.playerSpawnPoint.transform.position;
        transform.rotation = rot;
        controller.enabled = true;
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

        selectedGun = gunList.Count - 1;

        gameManager.Instance.updateAmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoMax);
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

        gameManager.Instance.updateAmmo(gunList[selectedGun].ammoCur, gunList[selectedGun].ammoMax);

        isShooting = false;
    }

    void Reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            gunList[selectedGun].ammoCur = gunList[selectedGun].ammoMax;
            PlayerSounds.PlayOneShot(audReload[UnityEngine.Random.Range(0, audReload.Length)], audReloadVol);
        }
    }
}
