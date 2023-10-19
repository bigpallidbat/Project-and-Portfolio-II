using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour
{
    [SerializeField] itemStats item;
    public Spawner orgin;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            other.GetComponent<PlayerController>().itemPickUpEffect(item);

            if(orgin != null) 
                orgin.heyIDied();
            Destroy(gameObject);

        }
    }

}
