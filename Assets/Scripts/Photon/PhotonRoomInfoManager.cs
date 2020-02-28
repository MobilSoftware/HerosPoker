using UnityEngine;
using System.Collections;
using PlayRivals;
using Photon;
using UnityEngine.UI;

public class PhotonRoomInfoManager : PunBehaviour
{
    #region Singleton
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static PhotonRoomInfoManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static PhotonRoomInfoManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first PhotonRoomInfoManager object in the scene.
                s_Instance = FindObjectOfType(typeof(PhotonRoomInfoManager)) as PhotonRoomInfoManager;
                if (s_Instance == null)
                    Debug.Log("Could not locate an PhotonRoomInfoManager object. \n You have to have exactly one PhotonRoomInfoManager in the scene.");
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

    public Hashtable dataTable = new Hashtable();

    private PhotonTexasPokerManager photonTexasPokerManager;

    // Use this for initialization
    void Start () {
        //InvokeRepeating("ParseAllRoomsInfo", 0.0f, 5.0f);
	}

    public void InitialiseCardGameScripts()
    {
        RemoveCardGameScripts();

        photonTexasPokerManager = gameObject.AddComponent<PhotonTexasPokerManager>();

        GlobalVariables.bInGame = true;
    }

    public void RemoveCardGameScripts()
    {
        if (photonTexasPokerManager != null)
            Destroy(photonTexasPokerManager);
    }

    private void ParseAllRoomsInfo()
    {
        //dataTable.Clear();

        //totalPlayers = 0;
        //RoomInfo[] photonRoomsList = PhotonNetwork.GetRoomList();
        //foreach (RoomInfo game in photonRoomsList)
        //{
        //    ExitGames.Client.Photon.Hashtable roomProperties = game.CustomProperties;
        //    string game_type = PhotonUtility.ParseString(roomProperties, PhotonEnums.Room.GameType);
        //    string environment = PhotonUtility.ParseString(roomProperties, PhotonEnums.Room.Environment);
        //    int igame_type = PhotonUtility.ParseInt(roomProperties, PhotonEnums.Room.GameType);
        //    int ienvironment = PhotonUtility.ParseInt(roomProperties, PhotonEnums.Room.Environment);
        //    //Debug.LogError(igame_type + " |||| " + ienvironment);
        //    PR_Utility.IncreaseDataCount(ref dataTable, game_type + "_" + environment, game.PlayerCount);
        //    totalPlayers += game.PlayerCount;
        //}

        //totalRooms = PhotonNetwork.GetRoomList().Length;
        //Messenger<bool>.Invoke("RoomInfoUpdated", true);
    }

    //public int GetPlayersCount(GameType type)
    //{
    //    return PR_Utility.ParseInt(dataTable, type.ToString());
    //}

    //public int GetPlayersCount(GameType type, EnvironmentType environmentType)
    //{
    //    int playersCount = PR_Utility.ParseInt(dataTable, type + "_" + environmentType);
    //    return playersCount;
    //}

    //public void RespondRequestToJoin(string sender_player_id, string receiver_player_id, bool status)
    //{
    //    photonView.RPC(PhotonEnums.RPC.RespondRequestToJoinRPC, PhotonTargets.All, sender_player_id, receiver_player_id, status);
    //}
}
