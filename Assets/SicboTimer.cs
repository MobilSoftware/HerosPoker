using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SicboTimer : MonoBehaviour
{
    private static SicboTimer s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static SicboTimer instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (SicboTimer)) as SicboTimer;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an SicboManager object. \n You have to have exactly one SicboManager in the scene.");
            }
            return s_Instance;
        }
    }

    [HideInInspector]
    public float timer;

    public void SetScaleZ (float zValue )
    {
        //transform.localScale = new Vector3 (1f, 1f, zValue);
        timer = zValue;
    }

    public float GetScaleZ ()
    {
        //return transform.localScale.z;
        return timer;
    }
}
