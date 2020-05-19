using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    private static FacebookManager s_Instance = null;

    public static FacebookManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (FacebookManager)) as FacebookManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an FacebookManager object. \n You have to have exactly one FacebookManager in the scene.");
            }
            return s_Instance;
        }
    }

}
