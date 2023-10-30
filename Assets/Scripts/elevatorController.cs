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

    [SerializeField] Transform originalPos;
    private bool isRising;
    private bool toRise;
    private bool isTop;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        originalPos.localPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SetActionable(this);

            //other.gameObject.transform.SetParent(gameObject.transform);

            trigger.enabled = false;
            barrier.enabled = true;
            
            //StartCoroutine(elevatorAnimate());

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.Instance.playerScript.SetActionable(null);
            //other.gameObject.transform.SetParent(null);
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
          while (!isTop)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, elevatorDestination.localPosition, speed * Time.deltaTime);
                if (transform.localPosition == elevatorDestination.localPosition)
                {
                    isTop = true;
                }

            }
        }
        else
        {
            while (isTop)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPos.localPosition, speed * Time.deltaTime);
                if (transform.localPosition == originalPos.localPosition)
                {
                    isTop = false;
                }
            }
        }
    }

    public void Activate()
    {
        //gameManager.Instance.playerScript.gameObject.transform.SetParent(gameObject.transform, true);
        RISE();
        
    }
}
