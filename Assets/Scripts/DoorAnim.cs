using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DoorAnim : MonoBehaviour , IInteract
{
    [Header("----- Anim Components -----")]
    [SerializeField] BoxCollider Door;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;
    [SerializeField] GameObject rotatorLeft;
    [SerializeField] GameObject rotatorRight;


    [SerializeField] float openSpeed;
    [SerializeField] int leftAngle;
    [SerializeField] int rightAngle;

    [SerializeField] Quaternion rightOrig;
    [SerializeField] Quaternion leftOrig;

    private bool isOpen;


    private void Start()
    {
        isOpen = false;
        rightOrig = Quaternion.identity; leftOrig = Quaternion.identity;
    }

    IEnumerator doorRotation(Quaternion targRot)
    {
        float times = 0f;
        Quaternion initRot = rightDoor.transform.localRotation;

        Vector3 worldPivPointRight = transform.TransformPoint(rotatorRight.transform.position);
        Vector3 worldPivPointLeft = transform.TransformPoint(rotatorLeft.transform.position);

        while(times <= 1f )
        {
            times += Time.deltaTime * openSpeed;

            Quaternion newRotation = Quaternion.Lerp(initRot, targRot, times);

            Vector3 pivotOffSetRight = worldPivPointRight - rightDoor.transform.position;
            Vector3 pivotOffSetLeft = worldPivPointLeft - leftDoor.transform.position;

            //leftDoor.transform.rotation = Quaternion.Inverse(newRotation) * Quaternion.Euler(0, pivotOffSetLeft.y ,0);
            //rightDoor.transform.rotation = Quaternion.Inverse(newRotation) * Quaternion.Euler(0, pivotOffSetRight.y ,0);

            rotatorRight.transform.rotation = newRotation;
            rotatorLeft.transform.rotation = Quaternion.Inverse(newRotation); 
            yield return null;
        }
    }

    public void Activate()
    {
        

        if (isOpen)
        {
            //rightDoor.transform.rotation = Quaternion.Lerp(leftDoor.transform.rotation, rightOrig, openSpeed * Time.deltaTime);
            //leftDoor.transform.rotation = Quaternion.Lerp(leftDoor.transform.rotation, leftOrig , openSpeed * Time.deltaTime);
            rightOrig = Quaternion.Euler(0, 0, 0);
            StartCoroutine(doorRotation(rightOrig));
            isOpen = false;
        }
        else
        {
            //recieved = true;
            //Debug.Log(recieved);
            //Quaternion rotRight = Quaternion.Euler(0, rightAngle, 0);
            //Quaternion rotLeft = Quaternion.Euler(0, leftAngle, 0);

             //rightDoor.transform.rotation = Quaternion.Lerp(rightOrig, Quaternion.Euler(0 , rightAngle, 0)  , openSpeed * Time.deltaTime);
             //leftDoor.transform.rotation = Quaternion.Lerp(leftOrig, Quaternion.Euler(0 , leftAngle, 0), openSpeed * Time.deltaTime);

            rightOrig = Quaternion.Euler(0, 90, 0);
            StartCoroutine (doorRotation(rightOrig));

            isOpen = true;
            

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SetActionable(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SetActionable(null);
        }
    }

}
