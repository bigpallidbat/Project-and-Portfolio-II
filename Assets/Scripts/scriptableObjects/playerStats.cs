using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class playerStats : ScriptableObject
{
    [Header("----- Player Stats -----")]
    public List<gunStats> startingGunList;
    public List<gunStats> gunList;
    public int gunCount;
    public int selectedGun;
    public int hpcur;
    public int hpmax;
    public int grenadeCount;
    public int medkitCount;

    [Header("----- Player Buffs -----")]
    public int damageBuff;
    public int hpBuff;
    public int speedBuff;
    
}
