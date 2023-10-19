using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject gate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.miniGoalAcquired)
        {
            anim.SetBool("isOpen", gameManager.miniGoalAcquired);

        }

    }
}
