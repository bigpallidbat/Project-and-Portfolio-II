using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemStats : MonoBehaviour
{
    public float shootRate;
    public int shootDamage;
    public int shootDist;
    public int ammoCurr;
    public int ammoMax;

    public GameObject model;
    public ParticleSystem hitEffect;
    public AudioClip shotSound;
}
