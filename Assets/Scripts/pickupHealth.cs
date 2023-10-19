using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupHealth : MonoBehaviour
{
    [SerializeField] int health = 1;

    UnityEngine.Vector3 startPos;
    UnityEngine.Vector3 endPos;
    int dir = 0;
    int maxHeight = 2;
    int speed = 2;
    float rotationSpeed = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
        health = health * -1;
        startPos = transform.position;
        endPos = new UnityEngine.Vector3(startPos.x, startPos.y + maxHeight, startPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == startPos)
            dir = 0;
        else if (transform.position == endPos)
            dir = 1;

        if (dir == 0)
            transform.position = UnityEngine.Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
        if (dir == 1)
            transform.position = UnityEngine.Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * speed);

        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamage damageable = gameManager.Instance.player.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(health);
                Destroy(gameObject);
            }
        }
    }
}
