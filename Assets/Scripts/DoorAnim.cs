using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DoorAnim : MonoBehaviour , IInteract
{
    [Header("----- Anim Components -----")]
    [SerializeField] BoxCollider openDoor;
    [SerializeField] BoxCollider closeDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [SerializeField] float openSpeed;
    [SerializeField] int leftAngle;
    [SerializeField] int rightAngle;

    [SerializeField] Quaternion rightOrig;
    [SerializeField] Quaternion leftOrig;

    private bool isOpen;
    private bool recieved;

    private void Start()
    {
        isOpen = false;
        rightOrig = Quaternion.identity; leftOrig = Quaternion.identity;
    }

    public void Activate()
    {
        

        if (isOpen)
        {
            //rightDoor.transform.rotation = Quaternion.Lerp(leftDoor.transform.rotation, rightOrig, openSpeed * Time.deltaTime);
            //leftDoor.transform.rotation = Quaternion.Lerp(leftDoor.transform.rotation, leftOrig , openSpeed * Time.deltaTime);

        }
        else
        {
            recieved = true;
            Debug.Log(recieved);
            //Quaternion rotRight = Quaternion.Euler(0, rightAngle, 0);
            //Quaternion rotLeft = Quaternion.Euler(0, leftAngle, 0);

            //rightDoor.transform.rotation = Quaternion.Lerp(rightOrig, rotRight, openSpeed * Time.deltaTime);
            //leftDoor.transform.rotation = Quaternion.Lerp(leftOrig, rotLeft, openSpeed * Time.deltaTime);

            rightDoor.transform.Rotate(0, rightAngle * Time.deltaTime, 0);
            leftDoor.transform.Rotate(0, leftAngle * Time.deltaTime, 0);

            

        }
    }
}
