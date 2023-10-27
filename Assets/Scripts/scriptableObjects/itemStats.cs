using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class itemStats : ScriptableObject
{
    public string itemName;
    public int id;
    public int amount;


    public enum itemType {healing =1, Damage, Speed, Ammo, Health ,grenade };
    public itemType type;


    
}
