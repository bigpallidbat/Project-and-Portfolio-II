using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource PlayerSounds;

    [Header("----- player state -----")]
    [Range(1, 10)][SerializeField] int HP;
    int HPMax;
    [Range(1, 10)][SerializeField] float playerSpeed;
    [Range(1, 3)][SerializeField] float SprintMod;
    [Range(1, 3)][SerializeField] int JumpMax;
    [Range(8, 30)][SerializeField] float jumpHeight;
    [Range(-40f, -9.81f)][SerializeField] float gravityValue;
    [Range(1,10)][SerializeField] int Stamina;
    int maxStam;


    [Header("----- Gun States -----")]
    [SerializeField] AudioClip Shot;
    [SerializeField] AudioClip[] Miss;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootdist;
    [SerializeField] GameObject cube;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    private int jumpedTimes;
    bool isSprinting;
    bool isShooting;
    private void Start()
    {
        HPMax = HP;
        maxStam = Stamina;
        spawnPlayer();
    }

    void Update()
    {
        Movement();
        if (Input.GetButton("Shoot") && !isShooting) StartCoroutine(shoot());
    }
    
    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= SprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= SprintMod;
        }
    }
    void Movement()
    {
        Sprint();

        groundedPlayer = controller.isGrounded;

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
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        isShooting = true;
        PlayerSounds.PlayOneShot(Shot);
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootdist))
        {
            IDamage damgable = hit.collider.GetComponent<IDamage>();

            if (damgable != null)
            {
                damgable.takeDamage(shootDamage);
            }
            else AudioRando.PlayRandomClip(PlayerSounds, Miss);
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        UpdatePlayerUI();
        if (HP <= 0) gameManager.Instance.YouLose();
    }


    public void spawnPlayer()
    {
        HP = HPMax;
        UpdatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.Instance.playerSpawnPoint.transform.position;
        controller.enabled = true;
    }

    void UpdatePlayerUI()
    {
        gameManager.Instance.playerHpBar.fillAmount = (float)HP / HPMax;
        gameManager.Instance.playerStamBar.fillAmount = (float)Stamina / maxStam;
    }
}
