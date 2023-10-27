using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorController : MonoBehaviour, IInteract
{
    [Header("----- Elevator Components -----")]
    [SerializeField] BoxCollider trigger;
    [SerializeField] BoxCollider barrier;
    [SerializeField] Transform elevatorDestination;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    private Transform originalPos;
    private bool isRising;
    private bool toRise;
    private bool isTop;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform;
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
            if (!isRising)
            {
                toRise = true;
            }
            //StartCoroutine(elevatorAnimate());

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(null, true);
            toRise = false;
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

    private void RISE()
    {
        if (!isTop)
        {
            transform.position = Vector3.MoveTowards(transform.position, elevatorDestination.position, speed * Time.deltaTime);
            if(transform.position == elevatorDestination.position)
            {
                isTop = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPos.position, speed * Time.deltaTime);
            if(transform.position == originalPos.position)
            {
                isTop = false;
            }
        }
    }

    public void Activate()
    {
        if (toRise)
        {
            RISE();
        }
    }
}
