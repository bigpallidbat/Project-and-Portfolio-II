using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombWall : MonoBehaviour, IDestroy
{
    [SerializeField] GameObject DumbWall;
    [SerializeField] GameObject OpenWall;
    [SerializeField] GameObject pops;

    public void Destroy()
    {
        OpenWall.SetActive(true);
        pops.SetActive(true);
        DumbWall.SetActive(false);
    }
}
