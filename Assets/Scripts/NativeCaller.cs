using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeCaller 
{
    public static void vibrate()
    {
        callNative("vibrate");
    }

    public static void callNative(string method)
    {
        callNative(method, "");
    }

    public static void callNative(string method, string json)
    {
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.harbinger.puzzlelibrary.OverrideUnityActivity");
            AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");

            Debug.Log("method:"+method+" ,json:" + json);
            if (json == null || json == "")
            {
                overrideActivity.Call(method);
            }
            else
            {
                overrideActivity.Call(method, json);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
#endif
    }
}
