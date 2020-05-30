using System.Collections;
using Photon;
using PlayRivals;
using UnityEngine;
using UnityEngine.UI;

public class SicboManager : PunBehaviour
{
    private static SicboManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static SicboManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (SicboManager)) as SicboManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an SicboManager object. \n You have to have exactly one SicboManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Text txtTimer;
    public GameObject objOthersUI;
    public Button btnOthers;
    public Button btnLobby;
    public DiceHandler dice;
    public DiceHistoryHandler diceHistory;
    public SicboParasite[] objPlayers;        //reference prefab here

    public SicboPlayer[] unsortedPlayers;   //LOCAL | [0] is myPlayer
    [HideInInspector]
    public bool isPhotonFire;
    [HideInInspector]
    public SicboPlayer[] stockPlayers;  //LOCAL
    [HideInInspector]
    public bool bCanBet;
    [HideInInspector]
    public ApiBridge.SicboPlayer[] apiPlayers;

    private JStartSicbo jStartSicbo;
    private SceneType prevSceneType;
    private bool isInit;
    private Coroutine crStartRound;

    private void Start ()
    {
        btnOthers.onClick.AddListener (OnOthers);
        btnLobby.onClick.AddListener (OnLobby);
    }

    private void OnOthers ()
    {
        objOthersUI.SetActive (!objOthersUI.activeSelf);
    }

    private void OnLobby ()
    {
        if (crStartRound != null)
            StopCoroutine (crStartRound);
        if (PhotonNetwork.room != null)
        {
            PhotonNetwork.LeaveRoom ();
            Debug.LogError ("leaving photon room");
        }
        OnOthers ();
        GlobalVariables.bQuitOnNextRound = false;
        GlobalVariables.bInGame = false;
        _SceneManager.instance.SetActiveScene (SceneType.HOME, true);
        _SceneManager.instance.SetActiveScene (SceneType.SICBO, false);
    }

    public void SetCanvas (bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    private void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.SICBO;
            isInit = true;
        }
        canvas.enabled = true;
        isPhotonFire = true;
        RoomInfoManager.instance.JoinRandomRoom ();
        stockPlayers = new SicboPlayer[10];
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.SICBO;

    }

    private void Hide ()
    {
        canvas.enabled = false;
        isPhotonFire = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }

    public void PrepareGame ()
    {
        InitMyPlayerProperties ();
        photonView.RPC (PhotonEnums.RPC.RequestSicboSlot, PhotonTargets.MasterClient);
    }

    private void InitMyPlayerProperties ()
    {
        apiPlayers = null;
        PhotonPlayer player = PhotonNetwork.player;
        ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;
        properties[PhotonEnums.Player.Active] = false;
        properties[PhotonEnums.Player.NextRoundIn] = true;
        properties[PhotonEnums.Player.ReadyInitialized] = false;
        properties[PhotonEnums.Player.SICBO_BETS] = string.Empty;

        properties[PhotonEnums.Player.SlotIndex] = -1;

        properties[PhotonEnums.Player.Money] = PlayerData.owned_coin;

        properties[PhotonEnums.Player.PlayerID] = PlayerData.id;

        player.SetCustomProperties (properties);
    }

    [PunRPC]
    void RPC_RequestSicboSlot ( PhotonMessageInfo info )
    {
        Logger.E ("rpc_request sicbo slot");
        SyncSlots ();
        int _available_slot = GetAvailableRoomSlotIndex ();

        PhotonUtility.SetPlayerProperties (info.sender, PhotonEnums.Player.SlotIndex, _available_slot);
        photonView.RPC (PhotonEnums.RPC.ReturnSicboSlot, info.sender, _available_slot);
    }

    private void SyncSlots () 
    {
        if (PhotonNetwork.isMasterClient)
        {
            bool[] slots = PhotonUtility.GetRoomProperties<bool[]> (PhotonEnums.Room.Slots);
            for (int i = 0; i < slots.Length; i++)
                slots[i] = false;

            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                int slotIndex = PhotonUtility.GetPlayerProperties<int> (player, PhotonEnums.Player.SlotIndex);

                if (slotIndex != -1)
                    slots[slotIndex] = true;
            }

            PhotonUtility.SetRoomProperties (PhotonEnums.Room.Slots, slots);
        }
    }

    private int GetAvailableRoomSlotIndex ()
    {
        if (PhotonNetwork.room == null)
            return -1;

        //Make the next available slot occupied
        int slot_index = -1;
        bool[] slots = PhotonUtility.GetRoomProperties<bool[]> (PhotonEnums.Room.Slots);

        //debug.LogError("Slot Count : " + slots.Length);

        bool bFoundNewIndex = false;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == false)
            {
                slot_index = i;
                slots[i] = true;
                bFoundNewIndex = true;
                break;
            }
        }

        if (bFoundNewIndex)
            PhotonUtility.SetRoomProperties (PhotonEnums.Room.Slots, slots);

        return slot_index;
    }

    [PunRPC]
    void RPC_ReturnSicboSlot ( int _mySlot )
    {
        Logger.E ("rpc return sicbo slot");
        int mySlot = _mySlot;

        if (mySlot >= 0)
        {
            SortingStockPlayers (mySlot);
            PhotonUtility.SetPlayerProperties (PhotonNetwork.player, PhotonEnums.Player.SlotIndex, mySlot);
        }


        if (!PhotonNetwork.isMasterClient)
        {
            photonView.RPC (PhotonEnums.RPC.RequestSicboSync, PhotonTargets.Others);
            Invoke ("AssignPlayer", 4f);
        }
        else
        {
            AssignPlayer ();
        }

        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine (_PreparingRound ());
        }
    }

    [PunRPC]
    void RPC_RequestSicboSync (PhotonMessageInfo info)
    {
        if (unsortedPlayers[0].parasite != null)
        {
            unsortedPlayers[0].parasite.photonView.TransferOwnership (PhotonNetwork.player); //Re take over avatar
            unsortedPlayers[0].parasite.InitiateSync (info.sender);
        }
    }

    IEnumerator _PreparingRound ()
    {
        PhotonUtility.SetRoomProperties (PhotonEnums.Room.MasterClientID, PhotonNetwork.player.UserId);
        PhotonUtility.SetRoomProperties (PhotonEnums.Room.IS_PLAYING, true);

        int readyCount = 0;
        yield return _WFSUtility.wfs2;
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            bool bReady = PhotonUtility.GetPlayerProperties<bool> (player, PhotonEnums.Player.ReadyInitialized);
            Logger.E (bReady.ToString());
            if (bReady)
                readyCount++;
        }

        PrepareSicboRound ();
    }

    public void SortingStockPlayers ( int myIndex )
    {
        for (int x = 0; x < unsortedPlayers.Length; x++)
        {
            stockPlayers[myIndex] = unsortedPlayers[x];

            myIndex++;
            if (myIndex >= stockPlayers.Length)
                myIndex = 0;
        }
    }

    public void AssignPlayer ()
    {
        int slot_index = PhotonUtility.GetPlayerProperties<int> (PhotonNetwork.player, PhotonEnums.Player.SlotIndex);

        if (slot_index == -1)
        {
            PhotonUtility.SetPlayerProperties (PhotonNetwork.player, PhotonEnums.Player.SlotIndex, GetAvailableRoomSlotIndex ());
            return;
        }

        //Check whether the game is already started
        ExitGames.Client.Photon.Hashtable playerProperties = PhotonNetwork.player.CustomProperties;
        playerProperties[PhotonEnums.Player.ContentURL] = PlayerData.costume_id;
        playerProperties[PhotonEnums.Player.PlayerID] = PlayerData.id;
        playerProperties[PhotonEnums.Player.IsBot] = false;
        playerProperties[PhotonEnums.Player.Name] = PlayerData.display_name;
        playerProperties[PhotonEnums.Player.Gender] = 0;
        playerProperties[PhotonEnums.Player.PictureURL] = string.Empty;
        PhotonNetwork.player.SetCustomProperties (playerProperties);

        objPlayers[slot_index].photonView.TransferOwnership (PhotonNetwork.player);
        objPlayers[slot_index].Initiate ();
    }

    public void PrepareSicboRound ()
    {
        photonView.RPC (PhotonEnums.RPC.PrepareSicboRound, PhotonTargets.AllViaServer);
    }

    [PunRPC]
    protected void RPC_PrepareSicboRound ()
    {
        StartRound ();
        Logger.E ("rpc prepare sicbo round");
    }

    public void StartRound ()
    {
        txtTimer.text = "0";
        if (PhotonUtility.GetPlayerProperties<bool> (PhotonNetwork.player, PhotonEnums.Player.ReadyInitialized))
            PhotonUtility.SetPlayerPropertiesArray (PhotonNetwork.player, new string[] { PhotonEnums.Player.Active, PhotonEnums.Player.NextRoundIn }, new object[] { true, true });
        else
            PhotonUtility.SetPlayerPropertiesArray (PhotonNetwork.player, new string[] { PhotonEnums.Player.Active, PhotonEnums.Player.NextRoundIn }, new object[] { false, false });
        if (crStartRound != null)
            StopCoroutine (crStartRound);
        crStartRound = StartCoroutine (_StartTimer (15f));
    }

    IEnumerator _StartTimer (float timerValue )
    {
        Logger.E ("starting timer");
        bCanBet = true;
        while (timerValue > 0)
        {
            txtTimer.text = timerValue.ToString ();
            yield return new WaitForSecondsRealtime (1f);
            timerValue--;
        }
        txtTimer.text = "0";
        //yield return new WaitForSecondsRealtime (timerValue);
        Logger.E ("end");
        bCanBet = false;
        unsortedPlayers[0].SubmitRecords ();
        yield return _WFSUtility.wfs3;
        jStartSicbo = null;
        PrepareStartSicbo ();
        if (apiPlayers != null)
            unsortedPlayers[0].SetOtherPlayerBets ();
        if (PhotonNetwork.isMasterClient)
        {
            ApiManager.instance.StartSicbo ();
        }
        while (jStartSicbo == null)
        {
            //call bHasRStartSicbo on RPC_SendRStartSicbo NOT on RStartSicbo
            yield return _WFSUtility.wef;
        }

        //dice animation//
        diceHistory.SetHistory (jStartSicbo.dice[0], jStartSicbo.dice[1], jStartSicbo.dice[2]);
        dice.MoveToCenter (jStartSicbo.dice[0], jStartSicbo.dice[1], jStartSicbo.dice[2]);
        //
        yield return _WFSUtility.wfs3;
        for (int i = 0; i < jStartSicbo.players.Length; i++)    //jStartSicbo.players does not contain duplicate player ID so length = number of players
        {
            for (int j = 0; j < jStartSicbo.players[i].bets.Length; j++)
            {
                bool bWin = false;
                if (long.Parse (jStartSicbo.players[i].bets[j].coin_won) > 0)
                    bWin = true;

                unsortedPlayers[0].ShowWinLoseTiles (bWin, jStartSicbo.players[i].bets[j].sicbo_type);
            }
        }

        if (PhotonNetwork.isMasterClient)
        {
            UpdateMoneyProperties (jStartSicbo.players);
        }
        yield return _WFSUtility.wfs2;
        //chips from lose to dealer animation//
        yield return _WFSUtility.wfs2;
        //chips from dealer to win animation//
        yield return _WFSUtility.wfs2;
        UpdateMoneyUI ();
        unsortedPlayers[0].CleanUp ();
        //TransitionToNextRound ();
        PrepareSicboRound ();
        //HAVE NOT DEDUCT COIN FROM PLAYERS*
        //HAVE NOT SHOWN DEDUCTED COIN FROM OTHER PLAYERS*
        //disable bet*
        //send board state to all clients*
        //prepare start sicbo*
        //if master startSicbo*
        //give RStartSicbo to all clients*
        //wait until RStartSicbo != null*
        //dice animation
        //win lose fx*
        //take chip from losing
        //give chip to winning
        //update coin of each players*
        //start new round

    }

    private void PrepareStartSicbo ()
    {
        apiPlayers = null;
        for (int i = 0; i < unsortedPlayers.Length; i++)
        {
            if (unsortedPlayers[i].photonPlayer != null)
            {
                long lCoin = PhotonUtility.GetPlayerProperties<long> (unsortedPlayers[i].photonPlayer, PhotonEnums.Player.Money);
                unsortedPlayers[i].textCoinValue.text = lCoin.toShortCurrency ();
                string strBets = PhotonUtility.GetPlayerProperties<string> (unsortedPlayers[i].photonPlayer, PhotonEnums.Player.SICBO_BETS);
                if (strBets.Length != 0 && strBets != string.Empty)
                    AppendPlayers (FormatBets (strBets));
            }
        }
    }

    private ApiBridge.SicboPlayer[] FormatBets (string strBets )    //not all sicbo players
    {
        //strBets eg: 1001;4;200|1001;6;300
        string[] split1 = strBets.Split ('|');
        ApiBridge.SicboPlayer[] samePlayerBets = new ApiBridge.SicboPlayer[split1.Length];
        for (int i = 0; i < split1.Length; i++)
        {
            string[] split2 = split1[i].Split (';');
            samePlayerBets[i] = new ApiBridge.SicboPlayer (int.Parse (split2[0]), (ApiBridge.SicboBetType) int.Parse (split2[1]), long.Parse (split2[2]));
        }
        return samePlayerBets;
    }

    private void AppendPlayers (ApiBridge.SicboPlayer[] samePlayerBets )
    {
        if (apiPlayers == null)
        {
            apiPlayers = samePlayerBets;
        }
        else
        {
            ApiBridge.SicboPlayer[] appendedPlayers = new ApiBridge.SicboPlayer[apiPlayers.Length + samePlayerBets.Length];
            for (int i = 0; i < apiPlayers.Length; i++)
            {
                appendedPlayers[i] = apiPlayers[i];
            }
            for (int x = 0; x < samePlayerBets.Length; x++)
            {
                appendedPlayers[x + apiPlayers.Length] = samePlayerBets[x];
            }

            apiPlayers = appendedPlayers;
        }
    }

    public void SendRStartSicbo (string strJson )
    {
        photonView.RPC (PhotonEnums.RPC.SendRStartSicbo, PhotonTargets.AllViaServer, strJson);
    }

    [PunRPC]
    protected void RPC_SendRStartSicbo (string strJson )
    {
        jStartSicbo = JsonUtility.FromJson<JStartSicbo> (strJson);
    }

    private void UpdateMoneyProperties (JSicboPlayer[] players )
    {
        for (int i = 0; i < players.Length; i++)
        {
            for (int j = 0; j < unsortedPlayers.Length; j++)
            {
                if (unsortedPlayers[j].photonPlayer != null)
                {
                    Logger.E ("nn: " + unsortedPlayers[j].photonPlayer.NickName);
                    if (unsortedPlayers[j].photonPlayer.NickName == players[i].player_id.ToString ())
                    {
                        Logger.E ("Player id: " + players[i].player_id);
                        PhotonUtility.SetPlayerProperties (unsortedPlayers[j].photonPlayer, PhotonEnums.Player.Money, long.Parse (players[i].coin_after));
                    }
                }
            }
        }
    }

    private void UpdateMoneyUI ()
    {
        for (int i = 0; i < unsortedPlayers.Length; i++)
        {
            if (unsortedPlayers[i].photonPlayer != null)
            {
                long lCoin = PhotonUtility.GetPlayerProperties<long> (unsortedPlayers[i].photonPlayer, PhotonEnums.Player.Money);
                if (unsortedPlayers[i].photonPlayer.NickName == PlayerData.id.ToString ())
                {
                    PlayerData.owned_coin = lCoin;
                    _SceneManager.instance.UpdateAllCoinAndCoupon ();
                }

                unsortedPlayers[i].textCoinValue.text = lCoin.toShortCurrency ();
            }
        }
    }

    //private void TransitionToNextRound ()
    //{
    //    //??
    //}

    //bet timer up => submit record => round timer up => master calls api.StartSicbo() => chips go to dealer => myplayer.ResetBet() [records cleared here] => roll dice 3s or until RStartSicbo => myplayer.ShowWinLose() => myplayer.HideWinLose => next round
    #region Callbacks Photon
    public override void OnDisconnectedFromPhoton ()
    {
        if (isPhotonFire)
        {
            //If the player's in game then quit message
            if (PhotonNetwork.room != null)
                MessageManager.instance.Show (gameObject, "Koneksi bermasalah", ButtonMode.OK, 2);
        }
    }

    public override void OnMasterClientSwitched ( PhotonPlayer newMasterClient )
    {
        //LoginSceneManager.Instance.uiToastBox.Show("Waiting for new host");

        //if (PhotonNetwork.isMasterClient)
            //RespondAssignBot ();
    }

    public override void OnPhotonJoinRoomFailed ( object[] codeAndMsg )
    {
        if (isPhotonFire)
        {
            Photon_Events.FireOnFailedJoinRoomEvent (true);

            Logger.D (codeAndMsg[0] + " OnPhotonRandomJoinFailed : " + codeAndMsg[1]);
            //LoginSceneManager.Instance.uiBusyIndicator.Hide ();

            MessageManager.instance.Show (null, codeAndMsg[0].ToString () == "32765" ? "Ruangan sudah penuh" : "Permainan sudah selesai");
            //32758 Game Doesn't exist
            //32765 Game Full
        }
    }

    public override void OnPhotonRandomJoinFailed ( object[] codeAndMsg )
    {
        if (isPhotonFire)
        {
            Photon_Events.FireOnFailedJoinRandomRoomEvent (true);
        }
    }

    public override void OnJoinedLobby ()
    {
    }

    public override void OnJoinedRoom ()
    {
        if (isPhotonFire)
        {
            Photon_Events.FireOnJoinedRoomEvent (true);
        }
    }

    public override void OnCreatedRoom ()
    {
        if (isPhotonFire)
        {
            Photon_Events.FireOnCreatedRoomEvent (true);
        }
    }

    public override void OnPhotonPlayerConnected ( PhotonPlayer newPlayer )
    {

    }

    // when a player disconnects from the room, update the spawn/position order for all
    public override void OnPhotonPlayerDisconnected ( PhotonPlayer disconnetedPlayer )
    {
        //OtherPlayerDisconnect (disconnetedPlayer);
    }
    #endregion
}
