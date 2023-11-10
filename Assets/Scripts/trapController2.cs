using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapController2 : MonoBehaviour
{
    [SerializeField] Transform targetSpot;
    [SerializeField] Transform origTran;
    [SerializeField] float speed;

    private bool isMoving;
    private bool isExtend;


    private void Start()
    {
        origTran.position = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isMoving)
        {
            StartCoroutine(SpikeTrapExtend());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isExtend == true)
        {
            StartCoroutine(SpikeTrapRetract());
        }
    }

    IEnumerator SpikeTrapExtend()
    {
        isMoving = true;
        if(!isExtend)
        {
            while(!isExtend)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetSpot.position, speed * Time.deltaTime);

                if(transform.position == targetSpot.position)
                {
                    isExtend = true;
                }
                yield return null;
            }
        }
    }

    IEnumerator SpikeTrapRetract()
    {
            while (isExtend)
            {
                transform.position = Vector3.MoveTowards(transform.position, origTran.position, speed * Time.deltaTime);

                if (transform.position == origTran.position)
                {
                    isExtend = false;
                }
                yield return null;
            }
        isMoving = false;
    }
}
