using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int senitivity;
    [SerializeField] int lockvertmin;
    [SerializeField] int lockvertmax;



    [SerializeField] bool invertY;

    float xRot;
    //float yRot;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * senitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * senitivity;
        if (invertY) xRot += mouseY;
        else xRot -= mouseY;

        // clamp the rot on the X-axis
        xRot = Mathf.Clamp(xRot, lockvertmin, lockvertmax);

        // rotate the camera on x axis
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        // rotate the player y axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
