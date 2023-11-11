using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabKey : MonoBehaviour
{
    [SerializeField] GameObject KeyMaster;
    [SerializeField] bool Gkey;
    [SerializeField] bool MKey;
    [SerializeField] bool CKey;

    [SerializeField] float rotationSpeed;
    [SerializeField] float bobbingSpeed;
    [SerializeField] float bobbingAmount;

     float originalY;

    void Start()
    {
        originalY = transform.position.y;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        float bobbingOffset = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = new Vector3(transform.position.x, originalY + bobbingOffset, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Gkey) KeyMaster.GetComponent<KeyMaster>().gotGreenGeeKey();
            else if (MKey) KeyMaster.GetComponent<KeyMaster>().gotMagentaGeeKey();
            else if (CKey) KeyMaster.GetComponent<KeyMaster>().gotCyanGeeKey();
            else KeyMaster.GetComponent<KeyMaster>().addKey();
            Destroy(gameObject);
        }

    }

}
