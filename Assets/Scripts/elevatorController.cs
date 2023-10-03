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

    private Transform elevatorOrigPos;
    private bool isRising;

    // Start is called before the first frame update
    void Start()
    {
        elevatorOrigPos = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trigger.enabled = false;
            barrier.enabled = true;
            if(!isRising)
            StartCoroutine(elevatorAnimate());

        }
    }

    IEnumerator elevatorAnimate()
    {
        isRising = true;
        anim.Play("Elevator");
        yield return new WaitForSeconds(30.05f);
        anim.Play("Static");
        isRising = false;
    }
}
