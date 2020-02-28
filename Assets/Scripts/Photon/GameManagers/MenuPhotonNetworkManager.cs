using UnityEngine;
using PlayRivals;
using Photon;

public  class MenuPhotonNetworkManager : PunBehaviour
{
    #region Singleton
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static MenuPhotonNetworkManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static MenuPhotonNetworkManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuPhotonNetworkManager object in the scene.
                s_Instance = FindObjectOfType(typeof(MenuPhotonNetworkManager)) as MenuPhotonNetworkManager;
                if (s_Instance == null)
                    Debug.Log("Could not locate an MenuPhotonNetworkManager object. \n You have to have exactly one MenuPhotonNetworkManager in the scene.");
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

    string roomInvitation;

    public void Connect(string _roomInvitation = "")
    {
        //LoginSceneManager.Instance.uiBusyIndicator.Show (true);

        roomInvitation = _roomInvitation;
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.ConnectUsingSettings("v1.0");
    }


    public void Disconnect() { PhotonNetwork.Disconnect(); }

    public override void OnConnectedToPhoton()
    {
        Logger.D("Connected to photon");

        Photon_Events.FirePhotonNetworkConnectedEvent(true);

        //if (!PhotonNetwork.insideLobby)
            //Invoke("DelayJoinLobby", 0.3f);
    }

    public override void OnLeftRoom()
    {
        Photon_Events.FireOnLeftRoomEvent(true);
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby(GlobalVariables.sqlLobbyCapsaSusun);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        //BusyIndicatorBox.instance.Hide();
        //LoginSceneManager.Instance.uiBusyIndicator.Hide ();

        if (roomInvitation != "")
        {
            //LoginSceneManager.Instance.uiBusyIndicator.Show(true);
            Invoke("DelayJoinRoom", 0.3f);
        }
        //Debug.Log("OnJoined Lobby" + PhotonNetwork.lobby.Name);
    }

    void DelayJoinRoom()
    {
        RoomInfoManager.instance.JoinRoom(roomInvitation);
    }

    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate();

        Logger.D("Dapet List");
        //RoomInfo[] photonRoomsList = PhotonNetwork.GetRoomList();
        //Debug.LogError("Custom Room List" + photonRoomsList.Length);

        //MenuRoomListUI_.instance.UpdateRooms();
    }
}
