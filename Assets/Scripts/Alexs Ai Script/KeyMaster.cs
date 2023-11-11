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
        gameManager.Instance.updateKeyCount(1);
    }

    public bool useSmallKey()
    {
        if (SmallKeys > 0)
        {
            SmallKeys--;
            gameManager.Instance.updateKeyCount(-1);
            return true;
        }
        else return false;
    }

    public void gotGreenGeeKey()
    {
        GreenGeeKey = true;
        gameManager.Instance.SetGKeyActive();
    }

    public void gotCyanGeeKey()
    {
        CyanGeeKey = true;
        gameManager.Instance.SetCKeyActive();
    }

    public void gotMagentaGeeKey()
    {
        MagentaGeeKey = true;
        gameManager.Instance.SetMKeyActive();
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


