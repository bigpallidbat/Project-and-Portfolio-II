using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class itemStats : ScriptableObject
{
    public float shootRate;
    public int shootDamage;
    public int shootDist;
    public int ammoCurr;
    public int ammoMax;
    public bool AreaofEffect;


    public GameObject model;
    public ParticleSystem hitEffect;
    public AudioClip shotSound;
}
