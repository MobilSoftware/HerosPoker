using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public TMP_InputField ipfNickname;
    public Button btnSet;
    public Button btnPlay;

    public string prototypeBet;

    public int id;
    public string displayName;
    public long ownedGold;
    public Hero hero;
    public PokerHandler pokerHandler;

    private void Start ()
    {
        btnSet.onClick.AddListener (OnSet);
        btnPlay.gameObject.SetActive (false);
    }

    public void OnSet ()
    {
        if (ipfNickname.text == string.Empty)
        {
            displayName = "NoName";
        }
        else
        {
            displayName = ipfNickname.text;
        }

        id = Random.Range (1, 10000);
        displayName = id + "_" + displayName;
        if (displayName.Length > 9)
        {
            displayName = displayName.Substring (0, 7);
            displayName = displayName + "...";
        }
        ownedGold = 50000;
        PhotonNetwork.player.NickName = id.ToString ();

        PhotonNetwork.ConnectUsingSettings ("v1.0");
        PhotonRoomInfoManager.instance.InitialiseCardGameScripts ();
        prototypeBet = "20";
        btnSet.gameObject.SetActive (false);
        btnPlay.gameObject.SetActive (true);
    }
}
