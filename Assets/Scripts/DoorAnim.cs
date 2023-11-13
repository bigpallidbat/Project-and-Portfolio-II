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

    IEnumerator doorRotation(Quaternion targRot, GameObject obk)
    {
        float times = 0f;
        Quaternion initRot = obk.transform.localRotation;

        while(times <= 1f )
        {
            times += Time.deltaTime * openSpeed;

            Quaternion newRotation = Quaternion.Lerp(initRot, targRot, times);



         
            if (obk = rightDoor)
            {
                rotatorRight.transform.rotation = newRotation;
            }
            else rotatorLeft.transform.rotation = newRotation; 
            yield return null;
        }
    }

    public void Activate()
    {
        

        if (isOpen)
        {
            rightOrig = Quaternion.Euler(0, 0, 0);
            leftOrig = Quaternion.Euler(0, 0, 0);
            StartCoroutine(doorRotation(rightOrig, leftDoor));
            StartCoroutine(doorRotation(leftOrig, leftDoor));
            isOpen = false;
        }
        else
        {
            rightOrig = Quaternion.Euler(0, rightAngle, 0);
            leftOrig = Quaternion.Euler(0,leftAngle, 0);
            StartCoroutine (doorRotation(rightOrig,rightDoor));
            StartCoroutine (doorRotation(leftOrig,leftDoor));

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
