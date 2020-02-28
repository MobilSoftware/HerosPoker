using UnityEngine;
using System.Collections;

public class UI_FontManager : MonoBehaviour 
{
    #region Singleton
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static UI_FontManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static UI_FontManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first UI_FontManager object in the scene.
                s_Instance = FindObjectOfType(typeof(UI_FontManager)) as UI_FontManager;
                //if (s_Instance == null)
                    //Debug.Log("Could not locate an UI_FontManager object. \n You have to have exactly one UI_FontManager in the scene.");
            }
            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        s_Instance = null;
    }
    #endregion

    public Font unityFontUnicode, unityFontMyanmar, unityBitmapFont;
    //public UIFont nguiFontUnicode, nguiFontMyanmar;
}
