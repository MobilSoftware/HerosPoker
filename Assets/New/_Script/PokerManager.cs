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
    public WaitingNextRoundUI uiWaitingNextRound;
    public RoundRestartUI uiRoundRestart;
    public PokerOthersUI uiPause;
    public WaitingPlayersUI uiWaitingPlayers;
    public GameObject uiInject;

    public Sprite sprHighCard;
    public Sprite sprOnePair;
    public Sprite sprTwoPair;
    public Sprite sprThrice;
    public Sprite sprQuad;
    public Sprite sprStraight;
    public Sprite sprFlush;
    public Sprite sprFullHouse;
    public Sprite sprStraightFlush;
    public Sprite sprRoyalFlush;

    //for proto only
    public Sprite sprCoin;
    public _SpineObject spLuBu;
    public _SpineObject spCleo;

    private SceneType prevSceneType;
    private bool isInit;

    public void SetCanvas ( bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

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

        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.POKER;
    }

    public void Hide ()
    {
        objPoker.SetActive (false);
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }
}
