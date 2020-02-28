using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private static MenuManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static MenuManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (MenuManager)) as MenuManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an MenuManager object. \n You have to have exactly one MenuManager in the scene.");
            }
            return s_Instance;
        }
    }

    public MessageBoxUI uiMessageBox;
    public ThrowItemUI uiThrowItem;
    public Box_WaitingNextRound uiWaitingNextRound;
    public Box_RoundRestart uiRoundRestart;
    public Box_Pause uiPause;
    public Box_WaitingPlayers uiWaitingPlayers;
    public Sprite sprCoin;
}
