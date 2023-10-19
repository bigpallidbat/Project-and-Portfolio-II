using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gun;


    void Start()
    {
        gun.ammoCur = gun.ammoMax;
        gun.ammoReserve = gun.ammoReserveStart;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.Instance.playerScript.setGunStats(gun);
            Destroy(gameObject);
        }
    }
}
