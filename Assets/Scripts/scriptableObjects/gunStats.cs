using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public float shootRate;
    public int shootDamage;
    public int shootdist;
    public int ammoCur;
    public int ammoMax;

    public GameObject model;
    public ParticleSystem hitEffect;
    public ParticleSystem hitEffectEnemy;
    public AudioClip shootSound;
    [Range(0, 1)] public float shootSoundVol;
}
