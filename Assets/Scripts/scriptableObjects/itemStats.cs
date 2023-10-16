using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class itemStats : ScriptableObject
{
    public string itemName;
    public int id;
    public enum strength{small = 1, medium, large, full};
    public Sprite icon;
    public enum itemType {healing =1, Damage, Speed, Ammo };

    
}
