using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionStrength
{
    Excellent,
    Good,
    Normal,
    Low,
    VeryLow,
    NotPlayable,
}

public class PokerManager : MonoBehaviour
{
    private static PokerManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static PokerManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (PokerManager)) as PokerManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an PokerManager object. \n You have to have exactly one PokerManager in the scene.");
            }
            return s_Instance;
        }
    }

    public GameObject objPoker;
    public Canvas canvas;
    public ThrowItemUI uiThrowItem;
    public Box_WaitingNextRound uiWaitingNextRound;
    public Box_RoundRestart uiRoundRestart;
    public Box_Pause uiPause;
    public Box_WaitingPlayers uiWaitingPlayers;

    //for proto only
    public Sprite sprCoin;
    public _SpineObject spLubu;
    public _SpineObject spCleo;

    private bool isInit;

    public void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.POKER;
            isInit = true;
        }
        objPoker.SetActive (true);
        canvas.enabled = true;

        PhotonRoomInfoManager.instance.InitialiseCardGameScripts ();
        RoomInfoManager.instance.JoinRandomRoom ();
    }

    public void Hide ()
    {
        objPoker.SetActive (false);
        canvas.enabled = false;
    }
}
