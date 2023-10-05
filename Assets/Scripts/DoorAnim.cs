using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnim : MonoBehaviour
{
    [Header("----- Anim Components -----")]
    [SerializeField]Animator rightAnim;
    [SerializeField]Animator leftAnim;
    [SerializeField] BoxCollider openDoor;
    [SerializeField] BoxCollider closeDoor;
    private bool isOpen;
    private bool isOpening;
    private bool isClosing;

    private void OnTriggerEnter(Collider other)
    {
        if (!isOpen && !isOpening)
        {
            isOpen = true;
            openDoor.enabled = false;
            closeDoor.enabled = true;
            StartCoroutine(Open());
        }
        else if(isOpen && !isClosing)
        {
            isOpen = false;
            closeDoor.enabled = false;
            openDoor.enabled = true;
            StartCoroutine(Close());
        }
    }

    IEnumerator Open()
    {
        isOpening = true;

        playAnimOpen();
        yield return new WaitForSeconds(4f);
        isOpening = false;
    }
    
    IEnumerator Close()
    {
        isClosing = true;
        playAnimClose();
        yield return new WaitForSeconds(3f);
        AnimStatic();
        isClosing = false;
    }

    void playAnimOpen()
    {
        rightAnim.Play("RightDoor");
        leftAnim.Play("LeftDoor");
    }

    void playAnimClose()
    {
        rightAnim.Play("RightDoorClose");
        leftAnim.Play("LeftDoorClose");
    }

    void AnimStatic()
    {
        rightAnim.Play("Static");
        leftAnim.Play("Static");
    }

}
