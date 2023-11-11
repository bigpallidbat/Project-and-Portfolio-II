using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snipertrail : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    public float speed;
    public Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = dir * speed;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(destroy());
    }
    IEnumerator destroy()
    {
        yield return new WaitForSeconds(0.5f);
    }
}
