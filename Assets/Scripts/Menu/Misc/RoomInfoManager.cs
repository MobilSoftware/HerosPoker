using UnityEngine;
using System.Collections;
using PlayRivals;
using System.Collections.Generic;

public class RoomInfoManager : MonoBehaviour
{

    #region Singleton
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static RoomInfoManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static RoomInfoManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first RoomInfoManager object in the scene.
                s_Instance = FindObjectOfType(typeof(RoomInfoManager)) as RoomInfoManager;
                if (s_Instance == null)
                    Debug.Log("Could not locate an RoomInfoManager object. \n You have to have exactly one RoomInfoManager in the scene.");
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

    public string curRoomName = "";
    public MeshRenderer tableMat;
    public MeshRenderer backdropMat;

    [SerializeField] List<Transform> chairParents;

    public List<GameObject> chairRanks;
    public List<Texture2D> tableRanks;
    public List<Texture2D> backdropRanks;
    public Sprite[] spriteCity;

    [SerializeField] Transform[] chairsPoker;
    [SerializeField] Transform[] chairsCapsa;

    [SerializeField] GameObject[] gameEnvi;
    [SerializeField] MeshRenderer[] gameTable;
    [SerializeField] MeshRenderer[] gameBackdrop;
    [SerializeField] GameObject[] gameCanvas;

    protected void OnEnable()
    {
        Photon_Events.onJoinedRoom += onSuccessJoinRoom;
        Photon_Events.onFailedJoinRandomRoom += onFailedJoinRandomRoom;
        Photon_Events.onLeftRoom += onLeftRoom;
    }

    protected void OnDisable()
    {
        Photon_Events.onJoinedRoom -= onSuccessJoinRoom;
        Photon_Events.onFailedJoinRandomRoom -= onFailedJoinRandomRoom;
        Photon_Events.onLeftRoom -= onLeftRoom;
    }

    #region Joining And Create Room

    public void JoinRoom(string roomName)
    {
        Debug.Log("Joining room");
        //LoginSceneManager.Instance.uiBusyIndicator.Show(true);

        Debug.Log("room : "+roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    public void CreatePrivateRoom(GameType gameType, string password)// bool bGem, int betAmount, string password)
    {
        string city = GlobalVariables.environment.ToString();
        string gametype = GlobalVariables.gameType.ToString();
        string strPrefix = (int)gameType == 1 ?"PK_" : "CS_";
        string roomName = strPrefix + Random.Range(100, 9999);

        //LoginSceneManager.Instance.uiBusyIndicator.Show(true);

        byte expectedPlayer = 8;

        GlobalVariables.roomOptions.MaxPlayers = expectedPlayer;
        GlobalVariables.roomOptions.IsOpen = true;
        GlobalVariables.roomOptions.IsVisible = true;
        GlobalVariables.roomOptions.PublishUserId = true;

        //Search and join any dice room
        string[] strRoomProperties = { "C0", "C1", "C2", PhotonEnums.Room.BetType, PhotonEnums.Room.MinimumBet, PhotonEnums.Room.GameType, PhotonEnums.Room.Environment, PhotonEnums.Room.Password};
        GlobalVariables.roomOptions.CustomRoomPropertiesForLobby = strRoomProperties;

        //Add global minimum bet amount
        GlobalVariables.roomData.Clear();
        //GlobalVariables.roomData[PhotonEnums.Room.FBID] = GlobalVariables.Player.fb_id;
        GlobalVariables.roomData[PhotonEnums.Room.MinimumBet] = (int)GlobalVariables.MinBetAmount;
        GlobalVariables.roomData[PhotonEnums.Room.GameType] = (int)gameType;

        GlobalVariables.roomData[PhotonEnums.Room.Password] = password;
        if (password.Equals (string.Empty))
            GlobalVariables.bIsPassword = false;
        else
            GlobalVariables.bIsPassword = true;

        if (!GlobalVariables.bIsCoins)
            GlobalVariables.roomData[PhotonEnums.Room.BetType] = "gems";
        else
            GlobalVariables.roomData[PhotonEnums.Room.BetType] = "coins";

        GlobalVariables.roomData[PhotonEnums.Room.Environment] = city;
        GlobalVariables.roomData["C0"] = city;
        GlobalVariables.roomData["C1"] = password == "" ? "Public" : "Private";
        GlobalVariables.roomData["C2"] = gametype;

        GlobalVariables.roomOptions.CustomRoomProperties = GlobalVariables.roomData; 

        curRoomName = roomName;

        if (!PhotonNetwork.connected)
        {
            if (PhotonNetwork.connectionState == ConnectionState.Connected)
                MenuPhotonNetworkManager.instance.Disconnect();

            StartCoroutine(ReconnectBehaviour());
            return;
        }

        //Now Create room

        PhotonNetwork.CreateRoom(curRoomName, GlobalVariables.roomOptions, GlobalVariables.sqlLobbyCapsaSusun);
    }

    public void JoinRandomRoom()
    {
        //string city = GlobalVariables.environment.ToString();
        string city = "prototype";
        string gametype = GlobalVariables.gameType.ToString();
        //LoginSceneManager.Instance.uiBusyIndicator.Show(true);

        byte expectedPlayer = 8;

        GlobalVariables.roomOptions.MaxPlayers = expectedPlayer;
        GlobalVariables.roomOptions.IsOpen = true;
        GlobalVariables.roomOptions.IsVisible = true;
        GlobalVariables.roomOptions.PublishUserId = true;

        //Search and join any room
        string[] strRoomProperties = { "C0", "C1", "C2", PhotonEnums.Room.BetType, PhotonEnums.Room.MinimumBet, PhotonEnums.Room.GameType, PhotonEnums.Room.Environment, PhotonEnums.Room.Password};
        GlobalVariables.roomOptions.CustomRoomPropertiesForLobby = strRoomProperties;

        //Add global minimum bet amount
        GlobalVariables.roomData.Clear();

        GlobalVariables.roomData[PhotonEnums.Room.BetType] = "coins";

        GlobalVariables.roomData[PhotonEnums.Room.MinimumBet] = (int) GlobalVariables.MinBetAmount;
        GlobalVariables.roomData[PhotonEnums.Room.GameType] = (int)GlobalVariables.gameType;
        GlobalVariables.roomData[PhotonEnums.Room.Environment] = city;

        GlobalVariables.roomData["C0"] = city;
        GlobalVariables.roomData["C1"] = "Public";
        GlobalVariables.roomData["C2"] = gametype;

        GlobalVariables.roomOptions.CustomRoomProperties = GlobalVariables.roomData;

        //if ((PhotonNetwork.countOfPlayersInRooms % expectedPlayer) == 0)
        //{
        //    string strPrefix = (int)GlobalVariables.gameType == 1 ? "PK_" : "CS_";
        //    curRoomName = strPrefix + Random.Range(100, 9999) +"0" +PhotonNetwork.countOfRooms;

        //    PhotonNetwork.CreateRoom(curRoomName, GlobalVariables.roomOptions, GlobalVariables.sqlLobbyCapsaSusun);
        //}
        //else
        //{
            string sqlLobbyFilter = "C0 = '" + city + "' AND C1 = 'Public' AND C2 = '" + gametype + "'";
            PhotonNetwork.JoinRandomRoom (null, 0, MatchmakingMode.FillRoom, GlobalVariables.sqlLobbyCapsaSusun, sqlLobbyFilter);
        //}
    }

    #endregion

    #region Photon Callback

    private void onFailedJoinRandomRoom(bool val)
    {
        //LoginSceneManager.Instance.uiBusyIndicator.Hide ();
        Debug.LogError ("failed room");
        string strPrefix = (int)GlobalVariables.gameType == 1 ? "PK_": "CS_";
        curRoomName = strPrefix + Random.Range(100, 9999) + "0" + PhotonNetwork.countOfRooms;
        PhotonNetwork.CreateRoom(curRoomName, GlobalVariables.roomOptions, GlobalVariables.sqlLobbyCapsaSusun);
    }

    private void onSuccessJoinRoom(bool val)
    {
        //HomeSceneManager.Instance.myHomeMenuReference.uiHome.myAvatar.HideAvatar();
        Debug.LogError ("joined room");
        //DataManager.instance.btnPlay.gameObject.SetActive (false);
        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.room.CustomProperties;
        if (!properties.ContainsKey(PhotonEnums.Room.Slots))
        {
            if (GlobalVariables.gameType == GameType.TexasPoker)
                PhotonUtility.SetRoomProperties(PhotonEnums.Room.Slots, new bool[8] { false, false, false, false, false, false, false, false });
        }        

        string strBetType = PhotonUtility.GetRoomProperties<string>(PhotonEnums.Room.BetType);
        string strPassword = PhotonUtility.GetRoomProperties<string> (PhotonEnums.Room.Password);
        Logger.E ("join room pass: " + strPassword);
        if (strPassword.Equals (string.Empty))
            GlobalVariables.bIsPassword = false;
        else
            GlobalVariables.bIsPassword = true;
        Logger.E ("bIsPassword: " + GlobalVariables.bIsPassword);
        if (strBetType == "gems")
            GlobalVariables.bIsCoins = false;
        else
            GlobalVariables.bIsCoins = true;

        StartCoroutine(LoadGame());
        //HomeSceneManager.Instance.HitAbsent(PhotonNetwork.room.Name);
    }

    private void onLeftRoom(bool val)
    {
        waitingSwitch = false;
    }

    #endregion

    bool waitingSwitch = false;
    public IEnumerator SwitchingRoom()
    {
        waitingSwitch = true;
        if (PhotonNetwork.room != null)
            PhotonNetwork.LeaveRoom();

        if (!PhotonNetwork.connected)
        {
            DisconnectFromSwitchRoom();
            yield break;
        }

        int cd = 6;
        while (cd >= 0)
        {
            yield return _WFSUtility.wfs1;

            if (!waitingSwitch)
                cd = -1;

            cd--;
        }

        if (waitingSwitch)
        {
            DisconnectFromSwitchRoom();
        }
        else if(!waitingSwitch)
        {
            if (GlobalVariables.bIsCoins)
            {
                //long lCredit = DataManager.instance.ownedGold;
                long lCredit = PlayerData.owned_gold;
                if (lCredit < GlobalVariables.MinBetAmount)
                {
                    string strTitle = LocalisationManager.instance.GetText("ID_Warning");
                    string strDesc = string.Format("Koin tidak cukup untuk masuk ke ruangan. Anda butuh minimum {0} koin untuk bermain di room ini.", GlobalVariables.MinBetAmount.toShortCurrency());
                    PokerManager.instance.uiMessageBox.Show(gameObject, strDesc, MessageBoxType.OK);

                    DisconnectFromSwitchRoom();

                    yield break;
                }
            }

            CreatePrivateRoom(GlobalVariables.gameType, "");
        }
    }

    void DisconnectFromSwitchRoom()
    {
        //CameraManager.instance.HideGameCamera();

        //DeactiveGameEnvi();
        //HomeSceneManager.Instance.GetHome();

    }

    private IEnumerator ReconnectBehaviour()
    {
        bool connectAtemp = true;
        int countAtemp = 0;

        //ToastBox.instance.Show("Connecting to server");
        //LoginSceneManager.Instance.uiToastBox.Show ("ID_ConnectServer");

        //LoginSceneManager.Instance.uiBusyIndicator.Show (true);

        MenuPhotonNetworkManager.instance.Connect();

        while (connectAtemp)
        {
            yield return _WFSUtility.wfs1;

            if (PhotonNetwork.connectionState == ConnectionState.Connected)
            {
                PhotonNetwork.CreateRoom(curRoomName, GlobalVariables.roomOptions, GlobalVariables.sqlLobbyCapsaSusun);
                connectAtemp = false;
            }
            else
                countAtemp++;

            if (countAtemp > 5)
            {
                connectAtemp = false;
            }
        }
    }

    private IEnumerator LoadGame()
    {
        Resources.UnloadUnusedAssets ();

        yield return _WFSUtility.wfs05;

        if (GlobalVariables.gameType == GameType.TexasPoker)
        {
            _PokerGameHUD.instance.Show();

            PhotonTexasPokerManager.instance.PrepareGame();
        }


    }
}
