using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SePokerManager : MonoBehaviour
{
    private static SePokerManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static SePokerManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (SePokerManager)) as SePokerManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an SePokerManager object. \n You have to have exactly one SePokerManager in the scene.");
            }
            return s_Instance;
        }
    }

    public GameObject objPoker;
    public MessageBoxUI uiMessageBox;
    public ThrowItemUI uiThrowItem;
    public Box_WaitingNextRound uiWaitingNextRound;
    public Box_RoundRestart uiRoundRestart;
    public Box_Pause uiPause;
    public Box_WaitingPlayers uiWaitingPlayers;
    public Sprite sprCoin;

    public void Init()
    {
        Debug.LogError ("init sepoker");
    }
}
