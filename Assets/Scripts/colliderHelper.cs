using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliderHelper : MonoBehaviour
{
    [SerializeField] int id;

    public delegate void enterDelegate(int id);
    public static event enterDelegate entered;

    public delegate void exitDelegate(int id);
    public static event exitDelegate exited;

    public void CollisionEnter()
    {
        entered?.Invoke(id);
    }

    public void CollisionExited()
    {
        exited?.Invoke(id);
    }

    private void OnTriggerEnter(Collider other)
    {
        

        if (other.CompareTag("Player"))
        {
            CollisionEnter();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollisionExited();
        }
    }
}
