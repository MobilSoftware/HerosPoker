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

public class DataManager : MonoBehaviour
{
    //equivalent of HomeSceneManager
    private static DataManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static DataManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (DataManager)) as DataManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an DataManager object. \n You have to have exactly one DataManager in the scene.");
            }
            return s_Instance;
        }
    }

    public string prototypeBet;

    public int id;
    public string displayName;
    public long ownedGold;
    public Hero hero;
    public PokerHandler pokerHandler;

    private void Start ()
    {
#if UNITY_EDITOR
        id = 10;
        displayName = "ProtoEditor";
        ownedGold = 50000;
        PhotonNetwork.player.NickName = "1000";
#else
        id = 20;
        displayName = "ProtoEmulator";
        ownedGold = 30000;
        PhotonNetwork.player.NickName = "2000";
#endif

        PhotonNetwork.ConnectUsingSettings ("v1.0");
        PhotonRoomInfoManager.instance.InitialiseCardGameScripts ();
    }
}
