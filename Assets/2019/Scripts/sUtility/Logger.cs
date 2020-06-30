using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger
{

    //static bool isLogging = Rays.Utilities.Congest.DEVELOPMENT;
    static bool isLogging = false;

    /// <summary>
    /// Logger Debug
    /// </summary>
    /// <param name="msg"></param>
	public static void D (string msg) {
        if (isLogging)
            Debug.Log(msg);
    }

    /// <summary>
    /// Logger Warning!!!
    /// </summary>
    /// <param name="msg"></param>
	public static void W (string msg) {
		if (isLogging)
			Debug.LogWarning (msg);
	}

    /// <summary>
    /// Logger Error
    /// </summary>
    /// <param name="msg"></param>
	public static void E (string msg) {
        if (isLogging)
            Debug.LogError(msg);
    }
}
