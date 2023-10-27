using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public float shootRate;
    public int shootDamage;
    public int shootdist;
    public int ammoCur;
    public int ammoMax;
    public int ammoReserve;
    public int ammoReserveStart;
    public int ammoReserveMax;
    public Vector3 ShootPos;
    public GameObject projectile;
    public float projectileSpeed;

    public GameObject model;
    public ParticleSystem hitEffect;
    public ParticleSystem hitEffectEnemy;
    public AudioClip shootSound;
    [Range(0, 1)] public float shootSoundVol;
    public int ID;
    public bool IsRaycast;
}
