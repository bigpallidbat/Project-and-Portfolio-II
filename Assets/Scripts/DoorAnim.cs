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
        rightOrig = rotatorRight.transform.rotation; leftOrig = rotatorLeft.transform.rotation;
    }

    IEnumerator doorRotation(Quaternion targRot, GameObject obk)
    {
        float times = 0f;
        Quaternion newRotation;

        while (times <= 1f )
        {
            times += Time.deltaTime * openSpeed;
            if(obk == rotatorRight)  newRotation = Quaternion.Lerp(rightOrig, targRot, times);

            else  newRotation = Quaternion.Lerp(leftOrig, targRot, times);




            if (obk == rotatorRight)
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
            
            StartCoroutine(doorRotation(rightOrig, rotatorRight));
            StartCoroutine(doorRotation(leftOrig, rotatorLeft));
            isOpen = false;
        }
        else
        {
            Quaternion rightNew = Quaternion.Euler(0, rightAngle, 0);
            Quaternion leftNew = Quaternion.Euler(0, leftAngle, 0);
            StartCoroutine (doorRotation(rightNew,rotatorRight));
            StartCoroutine (doorRotation(leftNew,rotatorLeft));

            isOpen = true;
            

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SetActionable(this);
            gameManager.Instance.E.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SetActionable(null);
            gameManager.Instance.E.gameObject.SetActive(false);
        }
    }
    
}
