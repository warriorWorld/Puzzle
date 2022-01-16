using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareKeys 
{
    public static string SOUND_STATE_KEY = "SOUND_STATE_KEY";

    public static bool isSoundOpen()
    {
        return PlayerPrefs.GetInt(SOUND_STATE_KEY) == 0;
    }
}
