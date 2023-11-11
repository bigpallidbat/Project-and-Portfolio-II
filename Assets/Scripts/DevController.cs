using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class DevController : MonoBehaviour
{
    [Header("-----Componenets-----")]
    [SerializeField] CharacterController controller;

    [Header("-----Stats-----")]
    [SerializeField] float speed;
    [SerializeField] float SprintMod;
    [SerializeField] float jumpHeight;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;


    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.Instance.isPaused)
        {
            Movement();
        }
    }


    void Movement()
    {

        groundedPlayer = controller.isGrounded;

        move = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(move * Time.deltaTime * (speed));

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump"))
        {
            playerVelocity.y = jumpHeight;
        }

        if (Input.GetButtonDown("Descend"))
        {
            playerVelocity.y = -jumpHeight;
        }

        playerVelocity.y = Mathf.Lerp(playerVelocity.y, 0, Time.deltaTime);

       // playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
