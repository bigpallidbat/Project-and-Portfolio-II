using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorController : MonoBehaviour
{
    [Header("----- Elevator Components -----")]
    [SerializeField] BoxCollider trigger;
    [SerializeField] BoxCollider barrier;
    [SerializeField] Transform elevatorDestination;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    private bool isRising;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(gameObject.transform, true);

            trigger.enabled = false;
            barrier.enabled = true;
            if(!isRising)
            StartCoroutine(elevatorAnimate());

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(null, true);
        }
    }

    IEnumerator elevatorAnimate()
    {
        isRising = true;
        anim.Play("Elevator");
        yield return new WaitForSeconds(13f);
        barrier.enabled = false;
        yield return new WaitForSeconds(17.05f);
        anim.Play("Static");
        isRising = false;
        trigger.enabled = true;
    }
}
