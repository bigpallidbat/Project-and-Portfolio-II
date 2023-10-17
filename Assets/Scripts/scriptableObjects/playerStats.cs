using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStats : MonoBehaviour
{
    [Header("----- Player Stats -----")]
    public List<gunStats> gunList;
    public int gunCount;
    public int selectedGun;
    public int hpcur;
    public int hpmax;

    [Header("----- Player Buffs -----")]
    public int damageBuff;
    public int hpBuff;
    public int speedBuff;
    
}