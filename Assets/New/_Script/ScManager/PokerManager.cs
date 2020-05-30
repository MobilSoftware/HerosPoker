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
    public PokerOthersUI uiOthers;
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

    public Sprite sprCoin;
    public Sprite sprLubu;
    public Sprite sprCleo;
    public Sprite sprMusashi;
    public Sprite sprNapoleon;
    public Sprite sprGenghis;
    public Sprite sprAlexander;

    public GameObject objFxStraightFlush;
    public UIParticleSystem psStraightFlush1;
    public UIParticleSystem psStraightFlush2;
    public GameObject objFxRoyalFlush;
    public UIParticleSystem psRoyalFlush1;
    public UIParticleSystem psRoyalFlush2;

    [HideInInspector]
    public bool bStraightFlush;
    [HideInInspector]
    public bool bRoyalFlush;

    //proto only
    //public _SpineObject spLuBu;
    //public _SpineObject spCleo;

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
        //PhotonTexasPokerManager.instance.isPhotonFire = true;
        objPoker.SetActive (true);
        canvas.enabled = true;

        PhotonRoomInfoManager.instance.InitialiseCardGameScripts ();
        RoomInfoManager.instance.JoinRandomRoom ();

        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.POKER;
    }

    public void Hide ()
    {
        //PhotonTexasPokerManager.instance.isPhotonFire = false;
        objPoker.SetActive (false);
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }

    public void SetActivePokerFx (bool val)
    {
        if (val && (bRoyalFlush || bStraightFlush))
        {
            Texture2D t2d = GetHeroTexture ();
            if (bRoyalFlush)
            {
                psRoyalFlush1.particleTexture = t2d;
                psRoyalFlush2.particleTexture = t2d;
                objFxRoyalFlush.SetActive (true);
            } else if (bStraightFlush)
            {
                psStraightFlush1.particleTexture = t2d;
                psStraightFlush2.particleTexture = t2d;
                objFxStraightFlush.SetActive (true);
            }
        }
        else
        {
            bStraightFlush = false;
            bRoyalFlush = false;
            objFxRoyalFlush.SetActive (false);
            objFxStraightFlush.SetActive (false);
        }
    }

    private Texture2D GetHeroTexture ()
    {
        Texture2D texture = sprLubu.texture;
        switch (PlayerData.costume_id)
        {
            case 3: texture = sprLubu.texture; break;
            case 7: texture = sprCleo.texture; break;
            case 8: texture = sprMusashi.texture; break;
            case 9: texture = sprNapoleon.texture; break;
            case 10: texture = sprGenghis.texture; break;
            case 18: texture = sprAlexander.texture; break;
        }

        return texture;
    }
}
