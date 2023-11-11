using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMaster : MonoBehaviour
{
    int SmallKeys;
    bool GreenGeeKey;
    bool CyanGeeKey;
    bool MagentaGeeKey;

    public void addKey()
    {
        SmallKeys++;
    }

    public bool useSmallKey()
    {
        if (SmallKeys > 0)
        {
            SmallKeys--;
            return true;
        }
        else return false;
    }

    public void gotGreenGeeKey()
    {
        GreenGeeKey = true;
    }

    public void gotCyanGeeKey()
    {
        CyanGeeKey = true;
    }

    public void gotMagentaGeeKey()
    {
        MagentaGeeKey = true;
    }

    public bool useCyanGeeKey()
    {
        return CyanGeeKey;
    }

    public bool useGreenGeeKey()
    {
        return GreenGeeKey;
    }

    public bool useMagentaGeeKey()
    {
        return MagentaGeeKey;
    }
}


