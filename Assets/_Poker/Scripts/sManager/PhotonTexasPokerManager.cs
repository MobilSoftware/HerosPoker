using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayRivals;
using Photon;
using System;
using UnityEngine.SceneManagement;

public class PhotonTexasPokerManager : PunBehaviour
{
    public static PhotonTexasPokerManager instance;
    [HideInInspector]
    public int botCount = 0;
    [HideInInspector]
    public List<PhotonPlayer> bots = new List<PhotonPlayer> ();

    /// <summary>
    /// Pesan Terakhir, ini untuk checkpoint supaya tau posisi terakhir sebelum master disconnect dan ganti ke master baru
    /// </summary>
    public static string msgDelayPoker = "";
    /// <summary>
    /// Background Time Out, batas waktu player alt tab dari aplikasi
    /// </summary>
    public static float myBackgroundTimeOut;

    /// <summary>
    /// List Array Player dan Bot, Urutan "BUKAN" dari tempat duduk. tapi urutan player dulu baru bot.
    /// </summary>
    List<PhotonPlayer> pWithBot = new List<PhotonPlayer> ();

    /// <summary>
    /// Array ID untuk keperluan Start Game dan End Game
    /// </summary>
    int[] IDs = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    readonly int maxPlayer = 7;
    bool waitingResponseSync = false;

    private void Awake ()
    {
        instance = this;

        if (PhotonNetwork.player != null)
            ClearPlayerProperties (PhotonNetwork.player, "none");

        //Default timeout is 10 seconds from now
        PhotonNetwork.BackgroundTimeout = 10;
        //HomeSceneManager.Instance.ResetPropertiesServer(GameType.TexasPoker);
    }

    #region photon
    public void PrepareGame ()
    {
        Screen.fullScreen = false;
        _PokerGameManager.startBet = GlobalVariables.MinBetAmount;
        InitialPlayerProperties (PhotonNetwork.player);

        photonView.RPC (PhotonEnums.RPC.RPC_RequestSlot, PhotonTargets.MasterClient);
        //int mySlot = GetAvailableRoomSlotIndex ();

        //if (mySlot >= 0)
        //{
        //    _PokerGameManager.instance.SortingStockPlayers (mySlot);
        //    PhotonUtility.SetPlayerProperties (PhotonNetwork.player, PhotonEnums.Player.SlotIndex, mySlot);
        //}

        //if (PhotonNetwork.isMasterClient)
        //{
        //    PhotonUtility.SetRoomProperties (PhotonEnums.Room.MasterClientID, PhotonNetwork.player.UserId);

        //    if (PhotonNetwork.playerList.Length < 2 && HomeSceneManager.Instance.myConfig.INVITE_NUM_DEFAULT != 0)
        //        StartBotFirst();
        //    else
        //        RespondAssignBot();

        //    StartCoroutine (CheckPlayers ());
        //}

        //if (PhotonNetwork.playerList.Length < 2)
        //{
        //    RequestToSit ();
        //    Box_WaitingPlayers.instance.Show ();
        //}
        //else
        //{
        //    waitingResponseSync = true;
        //    photonView.RPC (PhotonEnums.RPC.RPC_RequestInitializeSync, PhotonTargets.Others);
        //    SyncMatchOnGoingWithProperties ();
        //    Invoke ("RequestToSit", 4f);

        //    Box_WaitingNextRound.instance.Show ();
        //}
    }

    [PunRPC]
    void RPC_RequestSlot ( PhotonMessageInfo info )
    {
        SyncSlots ();
        int _available_slot = GetAvailableRoomSlotIndex ();

        PhotonUtility.SetPlayerProperties (info.sender, PhotonEnums.Player.SlotIndex, _available_slot);
        photonView.RPC (PhotonEnums.RPC.RPC_ReturnSlot, info.sender, _available_slot);
    }

    [PunRPC]
    void RPC_ReturnSlot (int _mySlot )
    {
        int mySlot = _mySlot;

        if (mySlot >= 0)
        {
            _PokerGameManager.instance.SortingStockPlayers (mySlot);
            PhotonUtility.SetPlayerProperties (PhotonNetwork.player, PhotonEnums.Player.SlotIndex, mySlot);
        }

        if (PhotonNetwork.isMasterClient)
        {
            PhotonUtility.SetRoomProperties (PhotonEnums.Room.MasterClientID, PhotonNetwork.player.UserId);

            //if (PhotonNetwork.playerList.Length < 2 && HomeSceneManager.Instance.myConfig.INVITE_NUM_DEFAULT != 0)
            //    StartBotFirst ();
            //else
                RespondAssignBot ();

            StartCoroutine (CheckPlayers ());
        }

        if (PhotonNetwork.playerList.Length < 2)
        {
            RequestToSit ();
            SePokerManager.instance.uiWaitingPlayers.Show ();
        }
        else
        {
            waitingResponseSync = true;
            photonView.RPC (PhotonEnums.RPC.RPC_RequestInitializeSync, PhotonTargets.Others);
            SyncMatchOnGoingWithProperties ();
            Invoke ("RequestToSit", 4f);

            SePokerManager.instance.uiWaitingNextRound.Show ();
        }
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.P))
    //        StartCoroutine(CheckPlayers());
    //}

    public void RequestToSit () //NEW FUNCTION sit down
    {
        CancelInvoke ("RequestToSit");
        AssignAvatar ();
    }

    void AssignAvatar () //The owner Running this
    {
        int slot_index = PhotonUtility.GetPlayerProperties<int> (PhotonNetwork.player, PhotonEnums.Player.SlotIndex);

        //Wait until other players are joining the room
        if (slot_index == -1)
        {
            PhotonUtility.SetPlayerProperties (PhotonNetwork.player, PhotonEnums.Player.SlotIndex, GetAvailableRoomSlotIndex ());
            return;
        }

        //Check whether the game is already started
        ExitGames.Client.Photon.Hashtable playerProperties = PhotonNetwork.player.CustomProperties;
        //playerProperties[PhotonEnums.Player.ContentURL] = DataManager.instance.hero.id;
        playerProperties[PhotonEnums.Player.ContentURL] = PlayerData.hero_id;

        //playerProperties[PhotonEnums.Player.PlayerID] = DataManager.instance.id;
        playerProperties[PhotonEnums.Player.PlayerID] = PlayerData.id;
        playerProperties[PhotonEnums.Player.IsBot] = false;

        //playerProperties[PhotonEnums.Player.Name] = DataManager.instance.displayName;
        playerProperties[PhotonEnums.Player.Name] = PlayerData.display_name;
        //playerProperties[PhotonEnums.Player.Gender] = DataManager.instance.genderID;
        playerProperties[PhotonEnums.Player.Gender] = 0;
        //playerProperties[PhotonEnums.Player.PictureURL] = DataManager.instance.displayPictureURL;
        playerProperties[PhotonEnums.Player.PictureURL] = string.Empty;

        //int totalPlayed = HomeSceneManager.Instance.myPlayerData.player.winning + HomeSceneManager.Instance.myPlayerData.player.losing;
        //if (totalPlayed == 0)
        //    playerProperties[PhotonEnums.Player.WinRate] = (float)0;
        //else
        //playerProperties[PhotonEnums.Player.WinRate] = (int)(((float)HomeSceneManager.Instance.myPlayerData.player.winning / totalPlayed) * 100f);

        //playerProperties[PhotonEnums.Player.TotalPlayed] = totalPlayed;
        playerProperties[PhotonEnums.Player.TotalPlayed] = 0;
        //playerProperties[PhotonEnums.Player.LevelChar] = HomeSceneManager.Instance.myPlayerData.player.level;
        playerProperties[PhotonEnums.Player.LevelChar] = 0;

        //Set the first time player properties
        PhotonNetwork.player.SetCustomProperties (playerProperties);

        _PokerGameManager.instance.stockParasite[slot_index].photonView.TransferOwnership (PhotonNetwork.player);
        _PokerGameManager.instance.stockParasite[slot_index].CallInitialize (false);
    }

    public void RespondAssignBot () //Call Server to Invite BOT
    {
        if (PhotonNetwork.playerList.Length > 1)
            return;

        string password = PhotonUtility.GetRoomProperties<string> (PhotonEnums.Room.Password);
        if (!string.IsNullOrEmpty (password))
            return;

        //if (GlobalVariables.bIsCoins)
            //HomeSceneManager.Instance.InviteRandomPlayer ();
    }

    public void StartBotFirst() // NEW FUNCTION * Call Server to Invite BOT
    {
        if (PhotonNetwork.playerList.Length > 1)
            return;

        string password = PhotonUtility.GetRoomProperties<string>(PhotonEnums.Room.Password);
        if (!string.IsNullOrEmpty(password))
            return;

        //if (GlobalVariables.bIsCoins)
            //HomeSceneManager.Instance.InviteRandomPlayer(UnityEngine.Random.Range(2, 4));
    }

    public void InviteRandomBot()
    {
        //RandomPlayer rBot;

        //if (HomeSceneManager.stockBot.Count > 0)
        //{
        //    if (PhotonNetwork.isMasterClient)
        //    {
        //        for (int x = HomeSceneManager.stockBot.Count - 1; x >= 0; x--)
        //        {
        //            if (botCount >= 3)
        //                return;
        //            if (botCount + PhotonNetwork.playerList.Length > 5)
        //                return;

        //            rBot = HomeSceneManager.stockBot[x];
        //            long lCredit = Convert.ToInt64(rBot.credit);
        //            AssignBot(lCredit, rBot.player_id.ToString(), rBot.display_name, rBot.avatar_equiped, rBot.gender_type_id);

        //            HomeSceneManager.stockBot.RemoveAt(x);
        //        }

        //        RespondAssignBot();
        //    }
        //}
    }

    public void DelayWaitingBot ( int index )
    {
        //if (botCount >= 3)
        //    return;
        //if (botCount + PhotonNetwork.playerList.Length > 5)
        //    return;

        //if (index >= HomeSceneManager.stockBot.Count)
        //    return;

        //StartCoroutine ("DelayAddBot", index);
    }

    IEnumerator DelayAddBot ( int index )
    {
        yield return _WFSUtility.wfs3;
        //RandomPlayer rBot = HomeSceneManager.stockBot[index];

        //if (PhotonNetwork.isMasterClient)
        //{
        //    long lCredit = Convert.ToInt64 (rBot.credit);
        //    AssignBot (lCredit, rBot.player_id.ToString (), rBot.display_name, rBot.avatar_equiped, rBot.gender_type_id);

        //    HomeSceneManager.stockBot.RemoveAt (index);
        //    RespondAssignBot ();
        //}
    }

    /// <summary>
    /// Assign Bot
    /// </summary>
    /// <param name="_money">Player Money</param>
    /// <param name="_botID">Player ID</param>
    /// <param name="_botName">Player Name</param>
    /// <param name="_avatarID">Avatar Equiped</param>
    void AssignBot ( long _money, string _botID, string _botName, int _avatarID, int _genderID )
    {
        if (PhotonNetwork.isMasterClient)
        {
            SyncSlots ();
            int _available_slot = GetAvailableRoomSlotIndex ();

            if (_available_slot == -1)
                return;

            PhotonPlayer myBot = new PhotonPlayer (true, int.Parse (_botID), _botID);
            ExitGames.Client.Photon.Hashtable botProperties = myBot.CustomProperties;

            botProperties[PhotonEnums.Player.Active] = false;
            botProperties[PhotonEnums.Player.NextRoundIn] = true;
            botProperties[PhotonEnums.Player.ReadyInitialized] = false;

            botProperties[PhotonEnums.Player.ContentURL] = _avatarID;
            botProperties[PhotonEnums.Player.PlayerID] = int.Parse (_botID);
            botProperties[PhotonEnums.Player.SlotIndex] = _available_slot;
            botProperties[PhotonEnums.Player.Money] = _money;
            botProperties[PhotonEnums.Player.IsBot] = true;

            botProperties[PhotonEnums.Player.Name] = _botName;
            botProperties[PhotonEnums.Player.Gender] = _genderID;
            botProperties[PhotonEnums.Player.PictureURL] = "";

            botProperties[PhotonEnums.Player.WinRate] = (float) UnityEngine.Random.Range(70, 95); //disimpan sebagai float, tapi randomnya pake integer biar bulat
            botProperties[PhotonEnums.Player.TotalPlayed] = UnityEngine.Random.Range(250, 1078);
            botProperties[PhotonEnums.Player.LevelChar] = UnityEngine.Random.Range(3, 12);

            botProperties[PhotonEnums.Player.Cards] = "";

            botProperties[PhotonEnums.Player.HandRank] = 1; //highcard
            botProperties[PhotonEnums.Player.ValueHand] = 0;
            botProperties[PhotonEnums.Player.SecondValueHand] = 0;

            botProperties[PhotonEnums.Player.RankPoker] = 10;
            botProperties[PhotonEnums.Player.Kicker] = 0;
            botProperties[PhotonEnums.Player.SecondKicker] = 0;

            botProperties[PhotonEnums.Player.TotalBet] = (long) 0;
            botProperties[PhotonEnums.Player.ChipsBet] = (long) 0;

            myBot.SetCustomProperties (botProperties);
            bots.Add (myBot);
            botCount = bots.Count;

            //------Kebutuhan Room * ONLY MASTER
            ExitGames.Client.Photon.Hashtable roomProp = PhotonNetwork.room.CustomProperties;
            roomProp[PhotonEnums.Room.BotCount] = bots.Count;
            PhotonNetwork.room.SetCustomProperties (roomProp);

            PhotonNetwork.room.MaxPlayers = maxPlayer - bots.Count;
            //------

            photonView.RPC (PhotonEnums.RPC.RPC_AssignBot, PhotonTargets.AllViaServer, _botID, _available_slot, botProperties);
        }
    }

    [PunRPC]
    public void RPC_AssignBot ( string _botID, int _slot, ExitGames.Client.Photon.Hashtable _botProperties, PhotonMessageInfo info )
    {
        if (!info.sender.IsLocal) // Others Player, except Sender (Setup Bot Properties)
        {
            //Check Contains Duplicate Bot from new player joined sync bot
            foreach (PhotonPlayer bot in bots)
                if (bot.NickName == _botID)
                    return;

            AddNewBotToArray(_botID, _botProperties);
        }
        else if (info.sender.IsLocal) //I'm The Sender of this RPC
        {
            _PokerGameManager.instance.stockParasite[_slot].photonView.TransferOwnership (PhotonNetwork.player);
            _PokerGameManager.instance.stockParasite[_slot].CallInitialize (true);
        }
    }

    public void AddNewBotToArray(string _botID, ExitGames.Client.Photon.Hashtable _botProperties)
    {
        PhotonPlayer myBot = new PhotonPlayer(true, int.Parse(_botID), _botID);

        myBot.SetCustomProperties(_botProperties);
        bots.Add(myBot);
        botCount = bots.Count;
    }

    public PhotonPlayer GetPhotonBotFromID ( int userid )
    {
        foreach (PhotonPlayer bot in bots)
            if (bot.ID == userid)
                return bot;

        return null;
    }

    public void SyncBot ()
    {
        botCount = 0;

        foreach (_PlayerPokerActor pa in _PokerGameManager.instance.stockPlayers)
            if (pa.isBot)
                botCount++;
    }

    public void InitialPlayerProperties ( PhotonPlayer player ) //Only Initialize First Time
    {
        ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;
        properties[PhotonEnums.Player.Active] = false;
        properties[PhotonEnums.Player.NextRoundIn] = true;
        properties[PhotonEnums.Player.ReadyInitialized] = false;

        properties[PhotonEnums.Player.SlotIndex] = -1;

        long _money = (long) PlayerUtility.GetPlayerCreditsLeft ();
        long _buyIn = _money > GlobalVariables.MinBetAmount * 200 ? GlobalVariables.MinBetAmount * 200 : _money;
        PlayerUtility.BuyInFromBankAccount (_buyIn);

        properties[PhotonEnums.Player.Money] = _buyIn;

        properties[PhotonEnums.Player.Cards] = "";
        properties[PhotonEnums.Player.PlayerID] = "";

        properties[PhotonEnums.Player.HandRank] = 1; //highcard
        properties[PhotonEnums.Player.ValueHand] = 0;
        properties[PhotonEnums.Player.SecondValueHand] = 0;

        properties[PhotonEnums.Player.RankPoker] = 10;
        properties[PhotonEnums.Player.Kicker] = 0;
        properties[PhotonEnums.Player.SecondKicker] = 0;

        properties[PhotonEnums.Player.TotalBet] = (long) 0;
        properties[PhotonEnums.Player.ChipsBet] = (long) 0;

        player.SetCustomProperties (properties);
    }

    protected void SyncSlots () //NEW FUNCTION Syncing Slot Available
    {
        if (PhotonNetwork.isMasterClient)
        {
            bool[] slots = PhotonUtility.GetRoomProperties<bool[]> (PhotonEnums.Room.Slots);
            for (int i = 0; i < slots.Length; i++)
                slots[i] = false;

            RefreashPWithBot ();
            foreach (PhotonPlayer player in pWithBot)
            {
                int slotIndex = PhotonUtility.GetPlayerProperties<int> (player, PhotonEnums.Player.SlotIndex);

                if (slotIndex != -1)
                    slots[slotIndex] = true;
            }

            PhotonUtility.SetRoomProperties (PhotonEnums.Room.Slots, slots);
        }
    }
    #endregion

    #region Flow Game
    //-------------------------PRE MATCH START----------------------------------------------//

    public void PrepareRound () //Only Master
    {
        photonView.RPC (PhotonEnums.RPC.PrepareRoundRPC, PhotonTargets.AllViaServer);
    }

    [PunRPC]
    protected void PrepareRoundRPC ()
    {
        msgDelayPoker = ""; //From Next Game Round
        SePokerManager.instance.uiRoundRestart.Show ();
    }

    public virtual void StartRound () //Only Masterr
    {
        RefreashPWithBot ();
        if (pWithBot.Count < 2)
        {
            ForceStopTheMatch ();
            return;
        }

        if (checkDistributeRoutine != null)
            StopCoroutine (checkDistributeRoutine);

        checkDistributeRoutine = StartCoroutine (CheckDistributeCards ());
    }

    Coroutine checkDistributeRoutine = null;
    IEnumerator CheckDistributeCards () //ONLY MASTER
    {
        //Wait 1 second to wait for delay
        yield return _WFSUtility.wfs1;

        int counter = 0;
        //while (!DataManager.instance.pokerHandler.isSetup)
        while (!PokerData.is_setup)
        {

            if (counter >= 5)
                break;

            Logger.E ("Shuffling The Card " + counter);
            yield return _WFSUtility.wfs1;
            counter++;
        }

        if (counter >= 5)
        {
            int[] playerIDs = GetOtherPlayerID (PhotonEnums.Player.Active);

            //DataManager.instance.pokerHandler.Generate ();
            PokerData.Generate ();
            //HomeSceneManager.Instance.StartPoker(PhotonNetwork.room.Name, GlobalVariables.bIsCoins ? GlobalVariables.MinBetAmount : 0, GlobalVariables.bIsCoins ? 0 : GlobalVariables.MinBetAmount, playerIDs[0], playerIDs[1], playerIDs[2], playerIDs[3], playerIDs[4], playerIDs[5], playerIDs[6], 0, 0, 0, HomeSceneManager.Instance.myPlayerData.player.player_id);

            yield return _WFSUtility.wfs3;

            counter = 0;
            //while (!DataManager.instance.pokerHandler.isSetup)
            while (!PokerData.is_setup)
            {
                if (counter >= 5)
                    break;

                yield return _WFSUtility.wfs1;
                counter++;
            }

            if (counter >= 5)
            {
                photonView.RPC(PhotonEnums.RPC.RPC_ForceQuitMatch, PhotonTargets.AllViaServer);
                yield break;
            }

        }

        if (GetNumActivePlayers () < 2)
        {
            ForceStopTheMatch ();
            yield break;
        }


        //if (DataManager.instance.pokerHandler.cards.Length == 0)
        if (PokerData.cards.Length == 0)
        {
            ForceStopTheMatch();
            yield break;
        }

        photonView.RPC (PhotonEnums.RPC.GameStartedRPC, PhotonTargets.AllViaServer);
    }

    [PunRPC]
    void GameStartedRPC ()
    {
        StopAllCoroutines ();

        msgDelayPoker = ""; //From StartRound
        //int[] cardPack = DataManager.instance.pokerHandler.cards;
        int[] cardPack = PokerData.cards;
        int[] cardTable = new int[5] { cardPack[0], cardPack[1], cardPack[2], cardPack[3], cardPack[4] };
        _PokerGameManager.instance.InstallCardTable (cardTable);

        if (PhotonNetwork.isMasterClient)
            PhotonUtility.SetRoomProperties (PhotonEnums.Room.PokerCardTable, cardPack[0] + "," + cardPack[1] + "," + cardPack[2] + "," + cardPack[3] + "," + cardPack[4]);

        int curCardIndex = 5;

        string strCards = "";
        for (int x = 0; x < _PokerGameManager.instance.stockPlayers.Length; x++)
        {
            _PlayerPokerActor actor = _PokerGameManager.instance.stockPlayers[x];

            if (actor.myPlayer == null)
                continue;

            bool bActive = PhotonUtility.GetPlayerProperties<bool> (actor.myPlayer, PhotonEnums.Player.Active);
            if (bActive)
            {
                strCards = "";
                strCards = cardPack[curCardIndex] + "," + cardPack[curCardIndex + 1];

                string[] cardTypes = strCards.Split (',');
                actor.cardTypes = cardTypes;
                actor.InstallHandCard (cardPack[curCardIndex], cardPack[curCardIndex + 1]);

                if (actor.isBot || actor.isMine)
                    PhotonUtility.SetPlayerProperties (actor.myPlayer, PhotonEnums.Player.Cards, strCards);

                curCardIndex += 2;
            }
        }

        if (animDrawRoutine != null)
            StopCoroutine (animDrawRoutine);

        animDrawRoutine = StartCoroutine (AnimationDrawCard ());
    }

    Coroutine animDrawRoutine = null;
    IEnumerator AnimationDrawCard ()
    {
        _PlayerPokerActor[] playersActive = GetAllPlayerActive ();

        SoundManager.instance.PlaySFX (SFXType.SFX_CardDistributeOnce, Vector3.zero, false, 2);
        //_PokerGameManager.instance.npcDealer.PlayDealAnimation ();

        for (int x = 0; x < playersActive.Length; x++)
        {
            playersActive[x].myCardBack.DealFirstCard ();
            yield return _WFSUtility.wfs03;
        }

        for (int x = 0; x < playersActive.Length; x++)
        {
            playersActive[x].myCardBack.DealSecondCard ();
            yield return _WFSUtility.wfs03;
        }

        SoundManager.instance.StopSFX2 ();
        //_PokerGameManager.instance.npcDealer.StopDealAnimation ();
        yield return _WFSUtility.wfs1;

        if (PhotonUtility.GetPlayerProperties<bool> (PhotonNetwork.player, PhotonEnums.Player.Active))
            _PokerGameManager.instance.unsortedPlayers[0].OpenMyCard ();

        msgDelayPoker = "Distribute Card Complete";
        if (PhotonNetwork.isMasterClient)
        {
            photonView.RPC ("RPC_StartMatchTogether", PhotonTargets.AllViaServer);
        }
    }

    [PunRPC]
    void RPC_StartMatchTogether ()
    {
        //debug.Log("Receive Start Match Together");

        msgDelayPoker = "";
        _PokerGameManager.instance.StartMatch ();
    }
    //-------------------------PRE MATCH END----------------------------------------------

    public void Master_CheckTurn ()
    {
        _PokerGameManager.turnManager.CheckTurn ();

    }

    public void CalculateEndRoundTogether ()
    {
        photonView.RPC (PhotonEnums.RPC.RPC_SyncCalculateEndRound, PhotonTargets.AllViaServer);
    }

    public void NextTurnTogether ( int slotNow, int slotNext )
    {
        photonView.RPC (PhotonEnums.RPC.RPC_SyncNextTurn, PhotonTargets.AllViaServer, slotNow, slotNext);
    }

    [PunRPC]
    void RPC_SyncNextTurn ( int slotNow, int slotNext )
    {
        msgDelayPoker = "";
        _PokerGameManager.turnManager.NextTurnTogether (slotNow, slotNext);
    }

    [PunRPC]
    void RPC_SyncCalculateEndRound ()
    {
        msgDelayPoker = "";
        _PokerGameManager.instance.CalculateEndRound ();
    }

    public void TakeYourPotTogether (bool fromFold = false)
    {
        msgDelayPoker = fromFold ? "Take Our Pot": "Take Our Pot Fold";

        if (PhotonNetwork.isMasterClient)
            photonView.RPC (PhotonEnums.RPC.RPC_TakeYourPotTogether, PhotonTargets.AllViaServer, fromFold);
    }

    [PunRPC]
    void RPC_TakeYourPotTogether (bool fromFold)
    {
        msgDelayPoker = "";
        _PokerGameManager.instance.TakeOurPotTogether (fromFold);
    }

    //-------------------------POST MATCH START-------------------------------------------

    public void RoutineNextMatch ()
    {
        StartCoroutine ("RoutineTransitionToNewMatch");
    }

    IEnumerator RoutineTransitionToNewMatch ()
    {
        bool bNextRoundIn;
        long bMoneyEnuf;
        int bId;

        if (bots.Count > 0)
        {
            for (int x = bots.Count - 1; x >= 0; x--)
            {
                bNextRoundIn = PhotonUtility.GetPlayerProperties<bool> (bots[x], PhotonEnums.Player.NextRoundIn);
                bMoneyEnuf = PhotonUtility.GetPlayerProperties<long> (bots[x], PhotonEnums.Player.Money);
                bId = int.Parse(bots[x].NickName);

                if (bId >= 1000 || !bNextRoundIn || bMoneyEnuf <= _PokerGameManager.startBet)
                    BotLeaving(bots[x]);
            }
        }

        bNextRoundIn = PhotonUtility.GetPlayerProperties<bool> (PhotonNetwork.player, PhotonEnums.Player.NextRoundIn);
        bMoneyEnuf = PhotonUtility.GetPlayerProperties<long> (PhotonNetwork.player, PhotonEnums.Player.Money);

        //debug.LogError("Next Round In ? " + bNextRoundIn + " ----------- " +bMoneyEnuf);

        if (GlobalVariables.bQuitOnNextRound)
        {
            GlobalVariables.bQuitOnNextRound = false;
            ImLeaving();
            StartCoroutine(SePokerManager.instance.uiPause.LoadMenu ());
            yield break;
        }
        else if (GlobalVariables.bSwitchTableNextRound)
        {
            GlobalVariables.bSwitchTableNextRound = false;
            ImLeaving();
            StartCoroutine(SePokerManager.instance.uiPause.LoadSwitchTable());
            yield break;
        }

        if (!bNextRoundIn)
        {
            GlobalVariables.bQuitOnNextRound = false;
            ImLeaving ();
            StartCoroutine (SePokerManager.instance.uiPause.LoadMenu ());
            yield break;
        }
        else if (bMoneyEnuf <= _PokerGameManager.startBet)
        {
            if (PlayerUtility.GetPlayerCreditsLeft () > _PokerGameManager.startBet * 10)
                Bankrupt ();
            else
            {
                GlobalVariables.bQuitOnNextRound = false;
                ImLeaving ();
                StartCoroutine (SePokerManager.instance.uiPause.LoadMenu ());

                //PhotonNetwork.Disconnect();
                //Application.LoadLevel("Menu");
            }
        }

        bNextRoundIn = PhotonUtility.GetPlayerProperties<bool> (PhotonNetwork.masterClient, PhotonEnums.Player.NextRoundIn);
        bMoneyEnuf = PhotonUtility.GetPlayerProperties<long> (PhotonNetwork.masterClient, PhotonEnums.Player.Money);

        if (!bNextRoundIn || bMoneyEnuf <= _PokerGameManager.startBet)
        {
            //LoginSceneManager.Instance.uiToastBox.Show("Waiting for new host");
            yield return _WFSUtility.wfs3;
        }

        yield return _WFSUtility.wfs2;

        msgDelayPoker = "Next Game Round";
        if (PhotonNetwork.isMasterClient)
            NextGameRound ();
    }

    public void NextGameRound ()
    {
        photonView.RPC (PhotonEnums.RPC.RestartGameRoundRPC, PhotonTargets.AllViaServer);
    }

    [PunRPC]
    protected virtual void RestartGameRoundRPC ()
    {
        #region Force Sync
        foreach (_CardActor _c in _PokerGameManager.instance.tableCard)
            _c.gameObject.SetActive (false);

        foreach (_PlayerPokerActor _p in _PokerGameManager.instance.unsortedPlayers)
            _p.SyncMoney ();
        #endregion


        Resources.UnloadUnusedAssets ();
        GC.Collect ();
        ResetPlayerRoundProperties ();

        //HomeSceneManager.Instance.ResetPropertiesServer(GameType.TexasPoker);

        if (PhotonNetwork.isMasterClient)
        {
            RefreashPWithBot();
            if (pWithBot.Count < 2)
                ForceStopTheMatch();
            else
                PrepareRound();
        }
    }

    //-------------------------POST MATCH END---------------------------------------------//

    #endregion

    #region Utils
    public int GetAvailableRoomSlotIndex ()
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

        //debug.LogError("SLOTTTTTT  " + slots[0] + ", " + slots[1] + ", " + slots[2] + ", " + slots[3] + ", " + slots[4] + ", " + slots[5] + ", " + slots[6]);

        if (bFoundNewIndex)
            PhotonUtility.SetRoomProperties (PhotonEnums.Room.Slots, slots);

        return slot_index;
    }

    public PhotonPlayer GetBotwithIndex(int slot_idx)
    {
        if (bots.Count > 0)
        {
            foreach(PhotonPlayer bot in bots)
                if (PhotonUtility.GetPlayerProperties<int>(bot, PhotonEnums.Player.SlotIndex) == slot_idx)
                    return bot;
        }
        
        return null;
    }

    public virtual void ClearPlayerProperties ( PhotonPlayer player, string excludedKey = "slot_index" )
    {
        ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;
        List<string> keys = new List<string> ();

        foreach (DictionaryEntry entry in properties)
        {
            string strKey = entry.Key.ToString ();
            keys.Add (strKey);
        }

        foreach (string strKey in keys)
        {
            if (strKey != excludedKey)
            {
                if (excludedKey == "none")
                    properties["slot_index"] = -1;
                else
                    properties[strKey] = null;
            }
        }

        player.SetCustomProperties (properties);
    }

    public void RemovePlayerFromSlot ( int index )
    {
        if (PhotonNetwork.room == null)
            return;

        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.room.CustomProperties;
        if (index != -1 && properties.ContainsKey (PhotonEnums.Room.Slots))
        {
            ////debug.Log("Removing player from slot index : " + index);
            bool[] slots = (bool[]) properties[PhotonEnums.Room.Slots];

            if (slots.Length > index)
                slots[index] = false;

            properties[PhotonEnums.Room.Slots] = slots;
            PhotonNetwork.room.SetCustomProperties (properties);
        }
    }

    public void ForceStopTheMatch ()// Only Master Client Running This
    {
        msgDelayPoker = "";

        //if (!PhotonNetwork.room.IsOpen)
        //    StartCoroutine (Box_Pause.instance.LoadMenu ());
        //else
            StartCoroutine (RestartGame ());
    }

    private IEnumerator RestartGame () // Only Master Client Running This
    {
        if (checkDistributeRoutine != null)
            StopCoroutine (checkDistributeRoutine);

        if (animDrawRoutine != null)
            StopCoroutine (animDrawRoutine);

        yield return _WFSUtility.wfs05;

        ResetPlayerRoundProperties (); //Reset Player Round (local)
        RespondAssignBot (); //Get Invite Bot

        if (checkPlayersRoutine != null)
            StopCoroutine (checkPlayersRoutine);

        checkPlayersRoutine = StartCoroutine (CheckPlayers ());
    }

    Coroutine checkPlayersRoutine;
    protected IEnumerator CheckPlayers ()  //Only Master
    {
        if (PhotonNetwork.room == null)
            yield break;

        SePokerManager.instance.uiRoundRestart.Hide ();
        yield return _WFSUtility.wfs1;

        int readycount = CheckPlayerReady ();

        if (readycount < 2)
            if (GlobalVariables.bInGame)
                SePokerManager.instance.uiWaitingPlayers.Show ();

        while (PhotonNetwork.room != null && readycount < 2)
        {
            yield return _WFSUtility.wfs3;
            readycount = CheckPlayerReady ();
        }

        if (PhotonNetwork.room != null) //player ready to start
        {
            SePokerManager.instance.uiWaitingPlayers.Hide ();
            PrepareRound ();
        }
    }

    public void ResetPlayerRoundProperties ()
    {
        if (PhotonNetwork.player == null)
            return;

        ExitGames.Client.Photon.Hashtable properties;
        foreach (PhotonPlayer bot in bots)
        {
            properties = bot.CustomProperties;
            properties[PhotonEnums.Player.Active] = false;
            properties[PhotonEnums.Player.Cards] = "";

            properties[PhotonEnums.Player.HandRank] = 1; //highcard
            properties[PhotonEnums.Player.ValueHand] = 0;
            properties[PhotonEnums.Player.SecondValueHand] = 0;
            properties[PhotonEnums.Player.RankPoker] = 10;
            properties[PhotonEnums.Player.Kicker] = 0;
            properties[PhotonEnums.Player.SecondKicker] = 0;

            properties[PhotonEnums.Player.TotalBet] = (long) 0;
            properties[PhotonEnums.Player.ChipsBet] = (long) 0;

            bot.SetCustomProperties (properties);
        }

        properties = PhotonNetwork.player.CustomProperties;
        properties[PhotonEnums.Player.Active] = false;
        properties[PhotonEnums.Player.Cards] = "";

        properties[PhotonEnums.Player.HandRank] = 1; //highcard
        properties[PhotonEnums.Player.ValueHand] = 0;
        properties[PhotonEnums.Player.SecondValueHand] = 0;

        properties[PhotonEnums.Player.RankPoker] = 10;
        properties[PhotonEnums.Player.Kicker] = 0;
        properties[PhotonEnums.Player.SecondKicker] = 0;

        properties[PhotonEnums.Player.TotalBet] = (long) 0;
        properties[PhotonEnums.Player.ChipsBet] = (long) 0;
        PhotonNetwork.player.SetCustomProperties (properties);
    }

    void RefreashPWithBot ()
    {
        pWithBot.Clear ();
        pWithBot.AddRange (PhotonNetwork.playerList);
        pWithBot.AddRange (bots);
    }

    public int[] GetOtherPlayerID (string varFilter = "")
    {
        int idx = 0;

        for (int x = 0; x < IDs.Length; x++)
            IDs[x] = 0;

        RefreashPWithBot ();
        foreach (PhotonPlayer player in pWithBot)
        {
            bool bActive = PhotonUtility.GetPlayerProperties<bool> (player, varFilter);
            if (bActive)
            {
                IDs[idx] = (int.Parse (player.NickName));
                idx++;
            }
        }

        return IDs;
    }

    public int[] GetIndexesInMatch () //Get Slot Index Player In Match
    {
        int idx = 0;
        int i;

        for (int x = 0; x < IDs.Length; x++)
            IDs[x] = -1;

        RefreashPWithBot ();
        foreach (PhotonPlayer player in pWithBot)
        {
            bool bActive = PhotonUtility.GetPlayerProperties<bool> (player, PhotonEnums.Player.Active);
            if (bActive)
            {
                i = PhotonUtility.GetPlayerProperties<int>(player, PhotonEnums.Player.SlotIndex);
                if (i != -1)
                {
                    if (_PokerGameManager.instance.stockPlayers[i].myPlayer != null)
                    {
                        IDs[idx] = i;
                        idx++;
                    }
                }                
            }
        }

        return IDs;
    }

    _PlayerPokerActor[] GetAllPlayerActive ()
    {
        List<_PlayerPokerActor> playersActive = new List<_PlayerPokerActor> ();

        for (int x = 0; x < _PokerGameManager.instance.stockPlayers.Length; x++)
        {
            _PlayerPokerActor actor = _PokerGameManager.instance.stockPlayers[x];
            if (actor.myPlayer != null)
            {
                bool bActive = PhotonUtility.GetPlayerProperties<bool> (actor.myPlayer, PhotonEnums.Player.Active);
                if (bActive)
                    playersActive.Add (actor);
            }
        }

        return playersActive.ToArray ();
    }

    int CheckPlayerReady ()
    {
        int readycount = 0;
        bool bReady;

        RefreashPWithBot ();
        foreach (PhotonPlayer player in pWithBot)
        {
            bReady = false;
            bReady = PhotonUtility.GetPlayerProperties<bool> (player, PhotonEnums.Player.ReadyInitialized);
            if (bReady)
                readycount++;
        }

        return readycount;
    }

    public int GetNumActivePlayers ()
    {
        int numActivePlayers = 0;

        RefreashPWithBot ();
        foreach (PhotonPlayer player in pWithBot)
        {
            bool bActive = false;
            bActive = PhotonUtility.GetPlayerProperties<bool> (player, PhotonEnums.Player.Active);

            if (bActive)
                numActivePlayers++;
        }

        return numActivePlayers;
    }
    #endregion

    public void SetMyStartPoker ( int roundId, string roomBet, string strCards )
    {
        photonView.RPC (PhotonEnums.RPC.SetMyStartPokerRPC, PhotonTargets.Others, roundId, roomBet, strCards);
    }

    [PunRPC]
    protected virtual void SetMyStartPokerRPC ( int roundID, string roomBet, string strCards)
    {
        //DataManager.instance.pokerHandler.Setup (roundID, roomBet, strCards);
        PokerData.Setup (roundID, roomBet, strCards);
    }


    #region Player Leaving Handler
    void BotLeaving ( PhotonPlayer _b )
    {
        _PlayerPokerActor _ppa;
        int _slot = PhotonUtility.GetPlayerProperties<int> (_b, PhotonEnums.Player.SlotIndex);

        if (_slot < 0)
        {
            _ppa = FindPokerTakBerpenghuni (_b.NickName);
            _slot = _ppa.slotIndex;
        }
        else
            _ppa = _PokerGameManager.instance.stockPlayers[_slot];

        if(_ppa._myParasitePlayer !=null)
            _ppa._myParasitePlayer.CleanWithHolyWater ();

        _ppa.CleanWithHolyWater ();

        RemovePlayerFromSlot (_slot);
        bots.Remove (_b);
        SyncBot ();
    }

    _PlayerPokerActor FindPokerTakBerpenghuni ( string nickname )
    {
        _PlayerPokerActor ppa = null;

        foreach (_PlayerPokerActor p in _PokerGameManager.instance.unsortedPlayers)
        {
            if (p._myParasitePlayer.nickname == nickname)
            {
                ppa = p;
                break;
            }
        }

        return ppa;
    }

    void OtherPlayerDisconnect ( PhotonPlayer disconnectedPlayer )
    {
        ExitGames.Client.Photon.Hashtable properties = disconnectedPlayer.CustomProperties;

        if (!properties.ContainsKey (PhotonEnums.Player.Active))
            return;

        int slot_index = (int) properties[PhotonEnums.Player.SlotIndex];
        _PlayerPokerActor _ppa;

        if (slot_index < 0)
            _ppa = FindPokerTakBerpenghuni (disconnectedPlayer.NickName);
        else
            _ppa = _PokerGameManager.instance.stockPlayers[slot_index];

        bool bActive = (bool) properties[PhotonEnums.Player.Active];

        if (msgDelayPoker == "Next Game Round" || msgDelayPoker == "Start Round") // Karena kalo start round, master disconnect harus di kick
        {
            OtherPlayerLeaveRoom (disconnectedPlayer);
        }
        else if (bActive && _ppa != null)
        {
            int _botID = (int) properties[PhotonEnums.Player.PlayerID];
            PhotonPlayer newBot = new PhotonPlayer (true, _botID, _botID + "");
            bots.Add (newBot);

            properties[PhotonEnums.Player.NextRoundIn] = false;
            properties[PhotonEnums.Player.IsBot] = true;
            newBot.SetCustomProperties (properties);

            _ppa._myParasitePlayer.isBot = true;
            _ppa.isBot = true;
            _ppa.myPlayer = newBot;

            if (botCount == 0)
                SyncBot ();
            Debug.LogError ("actor ID: " + newBot.ID);
        }
        else
        {
            OtherPlayerLeaveRoom (disconnectedPlayer);
        }

        if (msgDelayPoker != "")
            StartCoroutine (WaitingStuckMasterDC ());
    }

    protected void OtherPlayerLeaveRoom ( PhotonPlayer disconnectedPlayer )
    {
        if (disconnectedPlayer.IsMasterClient)
            ChangeMasterClient (disconnectedPlayer);

        //Clear disconnected player from local player avater list
        int player_slot_index = PhotonUtility.GetPlayerProperties<int> (disconnectedPlayer, PhotonEnums.Player.SlotIndex);

        //debug.LogError("SLOT INDEX yang Leave : " +player_slot_index);
        _PlayerPokerActor _ppa = _PokerGameManager.instance.stockPlayers[player_slot_index];

        //debug.LogError("Clean HER : " + _ppa.gameObject.name);
        if (_ppa != null)
        {
            if (_ppa._myParasitePlayer != null)
                _ppa._myParasitePlayer.CleanWithHolyWater ();
            _ppa.CleanWithHolyWater ();
        }

        RemovePlayerFromSlot (player_slot_index);

        if (PhotonNetwork.isMasterClient)
        {
            Debug.LogError ("in otehr p leave room");
            PhotonUtility.SetPlayerPropertiesArray (disconnectedPlayer, new string[] { PhotonEnums.Player.Active, PhotonEnums.Player.NextRoundIn, PhotonEnums.Player.SlotIndex }, new object[] { false, false, -1 });
            //HomeSceneManager.Instance.SetOtp (HomeSceneManager.Instance.otp);

            int numActivePlayers = GetNumActivePlayers ();

            if (numActivePlayers < 2)
                StartCoroutine (RestartGame ());

            SyncSlots ();
        }

        if (PhotonUtility.GetPlayerProperties<bool> (disconnectedPlayer, PhotonEnums.Player.IsBot))
        {
            bots.Remove (disconnectedPlayer);
            SyncBot ();
            PhotonUtility.SetRoomProperties (PhotonEnums.Room.BotCount, botCount);
            PhotonNetwork.room.MaxPlayers = maxPlayer - bots.Count;
        }
    }

    protected void OtherPlayerLeaveMiddleGame(PhotonPlayer disconnectedPlayer)
    {
        ExitGames.Client.Photon.Hashtable properties = disconnectedPlayer.CustomProperties;

        if (!properties.ContainsKey(PhotonEnums.Player.Active))
            return;

        int slot_index = (int)properties[PhotonEnums.Player.SlotIndex];
        _PlayerPokerActor _ppa;

        if (slot_index < 0)
            _ppa = FindPokerTakBerpenghuni(disconnectedPlayer.NickName);
        else
            _ppa = _PokerGameManager.instance.stockPlayers[slot_index];

        bool bActive = (bool)properties[PhotonEnums.Player.Active];

        if (msgDelayPoker == "Next Game Round" || msgDelayPoker == "Start Round") // Karena kalo start round, master disconnect harus di kick
        {
            OtherPlayerLeaveRoom(disconnectedPlayer);
        }
        else if (bActive && _ppa != null)
        {
            int _botID = (int)properties[PhotonEnums.Player.PlayerID];
            PhotonPlayer newBot = new PhotonPlayer(true, _botID, _botID + "");
            bots.Add(newBot);

            properties[PhotonEnums.Player.NextRoundIn] = false;
            properties[PhotonEnums.Player.IsBot] = true;
            properties[PhotonEnums.Player.Name] = "";

            newBot.SetCustomProperties(properties);

            _ppa._myParasitePlayer.isBot = true;
            _ppa.isBot = true;
            _ppa.myPlayer = newBot;
            _ppa.forceFold = true;
            _ppa._myName = "";
            _ppa.txtName.text = "";        
            //_ppa.avater3D.ChangeSkinColor(Color.gray);

            if (botCount == 0)
                SyncBot();
        }
        else
        {
            OtherPlayerLeaveRoom(disconnectedPlayer);
        }

        if (msgDelayPoker != "")
            StartCoroutine(WaitingStuckMasterDC());
    }

    protected void OtherPlayerBankrupt ( PhotonPlayer disconnectedPlayer )
    {
        //Clear disconnected player from local player avater list
        int player_slot_index = PhotonUtility.GetPlayerProperties<int> (disconnectedPlayer, PhotonEnums.Player.SlotIndex);
        _PlayerPokerActor _ppa = _PokerGameManager.instance.stockPlayers[player_slot_index];
        _ppa._myParasitePlayer.CleanWithHolyWater ();
        _ppa.CleanWithHolyWater ();

        if (PhotonNetwork.isMasterClient)
        {
            PhotonUtility.SetPlayerPropertiesArray (disconnectedPlayer, new string[] { PhotonEnums.Player.Active, PhotonEnums.Player.NextRoundIn, PhotonEnums.Player.ReadyInitialized }, new object[] { false, false, false });
            //HomeSceneManager.Instance.SetOtp (HomeSceneManager.Instance.otp);
            SyncSlots ();
        }
    }

    IEnumerator WaitingStuckMasterDC ()
    {
        ////debug.LogError("Message Delay Cuk : " +msgDelay);

        if (PhotonNetwork.isNonMasterClientInRoom)
            yield break;

        switch (msgDelayPoker)
        {
            case "Start Round":
                ForceStopTheMatch ();
                break;
            case "Next Game Round":
                yield return _WFSUtility.wfs2;
                NextGameRound ();
                break;
            case "Distribute Card Complete":
                photonView.RPC (PhotonEnums.RPC.RPC_StartMatchTogether, PhotonTargets.AllViaServer);
                break;
            case "Wait Check Turn":
                Master_CheckTurn ();
                break;
            case "Start Next Turn":
                _PokerGameManager.turnManager.StartNextTurn ();
                break;
            case "Take Our Pot":
                TakeYourPotTogether();
                break;
            case "Take Our Pot Fold":
                TakeYourPotTogether (true);
                break;
            case "Wait To Force Fold":
                _PokerGameManager.turnManager.GetTurnNow.S_FoldAction ();
                break;
        }
    }

    public void ImLeaving ()
    {
        //HomeSceneManager.Instance.fromPlaying = true;
        photonView.RPC (PhotonEnums.RPC.QuitGameRPC, PhotonTargets.Others);

        bots.Clear ();
        SyncBot ();
    }

    public void ImLeavingInTheMiddleOfTheGame ()
    {
        //HomeSceneManager.Instance.fromPlaying = true;
        photonView.RPC (PhotonEnums.RPC.QuitInTheMiddleRPC, PhotonTargets.Others);

        bots.Clear ();
        SyncBot ();
    }

    public void Bankrupt ()
    {
        if (_PokerGameHUD.instance.buyInHUD.toggleAuto.isOn)
        {
            _PokerGameHUD.instance.buyInHUD.AutoBuyIn ();
        }
        else
        {
            photonView.RPC (PhotonEnums.RPC.RPC_IMBankrupt, PhotonTargets.AllViaServer);
            GlobalVariables.bQuitOnNextRound = true;

            _PokerGameHUD.instance.buyInHUD.Show ();
        }
    }

    public void ReJoinBuyIn ( long valBuyIn )
    {
        int mySlot = PhotonUtility.GetPlayerProperties<int>(PhotonNetwork.player, PhotonEnums.Player.SlotIndex);

        if (mySlot >= 0)
        {
            GlobalVariables.bQuitOnNextRound = false;
            ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.player.CustomProperties;
            properties[PhotonEnums.Player.NextRoundIn] = true;
            properties[PhotonEnums.Player.ReadyInitialized] = false;
            properties[PhotonEnums.Player.Money] = valBuyIn;
            properties[PhotonEnums.Player.ChipsBet] = (long) 0;
            PhotonNetwork.player.SetCustomProperties(properties);
            PlayerUtility.BuyInFromBankAccount(valBuyIn);

            RequestToSit ();
            _PokerGameHUD.instance.buyInHUD.Hide ();
        }
        else
        {
            //LoginSceneManager.Instance.uiToastBox.Show("Room Is Full");
            ImLeaving();
        }
    }

    [PunRPC]
    public void QuitGameRPC ( PhotonMessageInfo _info )
    {
        OtherPlayerLeaveRoom (_info.sender);
    }

    [PunRPC]
    public void QuitInTheMiddleRPC(PhotonMessageInfo _info)
    {
        OtherPlayerLeaveMiddleGame (_info.sender);
    }

    [PunRPC]
    public void RPC_IMBankrupt ( PhotonMessageInfo _info )
    {
        OtherPlayerBankrupt (_info.sender);
    }

    public void ChangeMasterClient ( PhotonPlayer prevMaster )
    {
        if (prevMaster.IsLocal && PhotonNetwork.room.PlayerCount < 2)
            return;

        PhotonPlayer newMaster;
        newMaster = prevMaster.GetNext ();

        if (!newMaster.IsLocal)
            return;

        Logger.E ("Change master client from PhotonCapsaSusunManager");
        PhotonUtility.SetRoomProperties (PhotonEnums.Room.MasterClientID, newMaster.UserId);
        PhotonNetwork.SetMasterClient (newMaster);
        PhotonNetwork.networkingPeer.SendOutgoingCommands (); //This one send out final request before the application goes into background

        //Logger.E ("SetOtp : " + HomeSceneManager.Instance.otp);
        //HomeSceneManager.Instance.SetOtp (HomeSceneManager.Instance.otp);
    }

    PhotonPlayer FindNewMaster ( PhotonPlayer _prev )
    {
        PhotonPlayer _p = _prev;
        return _p;
    }
    #endregion

    #region Callbacks Photon
    public override void OnDisconnectedFromPhoton()
    {
        //If the player's in game then quit message
        if (PhotonNetwork.room != null)
            SePokerManager.instance.uiMessageBox.Show(gameObject, "ID_ConnectionError", MessageBoxType.OK, 2, true);
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        //LoginSceneManager.Instance.uiToastBox.Show("Waiting for new host");

        if (PhotonNetwork.isMasterClient)
            RespondAssignBot();
    }

    public override void OnPhotonJoinRoomFailed ( object[] codeAndMsg )
    {
        Photon_Events.FireOnFailedJoinRoomEvent (true);

        Logger.D (codeAndMsg[0] + " OnPhotonRandomJoinFailed : " + codeAndMsg[1]);
        //LoginSceneManager.Instance.uiBusyIndicator.Hide ();

        SePokerManager.instance.uiMessageBox.Show (null, codeAndMsg[0].ToString () == "32765" ? "ID_GameFull" : "ID_GameClosed");
        //32758 Game Doesn't exist
        //32765 Game Full
    }

    public override void OnPhotonRandomJoinFailed ( object[] codeAndMsg )
    {
        Photon_Events.FireOnFailedJoinRandomRoomEvent (true);
    }

    public override void OnJoinedLobby ()
    {
    }

    public override void OnJoinedRoom ()
    {
        Photon_Events.FireOnJoinedRoomEvent (true);
    }

    public override void OnCreatedRoom ()
    {
        Photon_Events.FireOnCreatedRoomEvent (true);
    }

    public override void OnPhotonPlayerConnected ( PhotonPlayer newPlayer )
    {

    }

    // when a player disconnects from the room, update the spawn/position order for all
    public override void OnPhotonPlayerDisconnected ( PhotonPlayer disconnetedPlayer )
    {
        OtherPlayerDisconnect (disconnetedPlayer);
    }

    [PunRPC]
    public void RPC_RequestInitializeSync ( PhotonMessageInfo info ) // NEW PLAYER JOIN, info.sender is new Player
    {
        if (_PokerGameManager.instance.unsortedPlayers[0]._myParasitePlayer != null)
        {
            _PokerGameManager.instance.unsortedPlayers[0]._myParasitePlayer.photonView.TransferOwnership (PhotonNetwork.player); //Re take over avatar
            _PokerGameManager.instance.unsortedPlayers[0]._myParasitePlayer.CallCatchupInitialize (info.sender);
        }

        if (bots.Count > 0)
        {
            foreach (PhotonPlayer bot in bots)
            {
                ExitGames.Client.Photon.Hashtable botProperties = bot.CustomProperties;
                string _botID = bot.ID + "";
                int _slot = PhotonUtility.GetPlayerProperties<int> (bot, PhotonEnums.Player.SlotIndex);
                _PokerGameManager.instance.stockParasite[_slot].photonView.TransferOwnership (PhotonNetwork.player); //Re take over Bot
                _PokerGameManager.instance.stockParasite[_slot].BotCatchUp(_botID, botProperties, info.sender);
            }
        }

        //if (HomeSceneManager.Instance.myStartPoker != null)
        //    photonView.RPC (PhotonEnums.RPC.SetMyStartPokerRPC, info.sender, HomeSceneManager.Instance.jsonStart); //Bombardir JSon Start gpp

        if (_PokerGameManager.matchOnGoing)
        {
            //if (DataManager.instance.pokerHandler.isSetup)
            if (PokerData.is_setup)
            {
                //PokerHandler ph = DataManager.instance.pokerHandler;
                //photonView.RPC (PhotonEnums.RPC.SetMyStartPokerRPC, info.sender, ph.poker_round_id, ph.room_bet, ph.strCards); //Bombardir JSon Start gpp
                photonView.RPC (PhotonEnums.RPC.SetMyStartGameRPC, info.sender, PokerData.poker_round_id, PokerData.room_bet, PokerData.str_cards);
            }

            SendInfoOnMatch (info.sender);
        }
    }

    void SendInfoOnMatch ( PhotonPlayer newP )
    {
        string slotCurrentPlayers = "";
        for (int x = 0; x < _PokerGameManager.turnManager.currentPlayers.Count; x++)
        {
            if (x != _PokerGameManager.turnManager.currentPlayers.Count - 1)
                slotCurrentPlayers += _PokerGameManager.turnManager.currentPlayers[x].slotIndex + ",";
            else
                slotCurrentPlayers += _PokerGameManager.turnManager.currentPlayers[x].slotIndex;
        }

        Hashtable syncInfo = new Hashtable ();
        syncInfo["slotCurrentPlayers"] = slotCurrentPlayers;
        int _phaseTurn = _PokerGameManager.turnManager.phaseTurn;
        int _lastPotIndex = _PokerGameManager.instance.lastPotIndex;
        int _lengthPotIndex = _PokerGameManager.instance.potOnTable.Count;
        int _turnNow = _PokerGameManager.turnManager.GetTurnNow.slotIndex;
        int _turnNext = _PokerGameManager.turnManager.GetturnNext.slotIndex;
        long _lastBet = _PokerGameManager.lastBet;

        string _moneyPot = "";
        for (int x = 0; x < _PokerGameManager.instance.potOnTable.Count; x++)
        {
            syncInfo["pot" + x] = _PokerGameManager.instance.potOnTable[x].GetOwner ();

            if (x != _PokerGameManager.instance.potOnTable.Count - 1)
                _moneyPot += _PokerGameManager.instance.potOnTable[x].GetMoney + ",";
            else
                _moneyPot += _PokerGameManager.instance.potOnTable[x].GetMoney;
        }
        syncInfo["moneyPot"] = _moneyPot;

        photonView.RPC (PhotonEnums.RPC.RPC_SyncCatchupOnTable, newP, syncInfo.toJson (), _phaseTurn, _lastPotIndex, _lengthPotIndex, _turnNow, _turnNext, _lastBet);
    }

    [PunRPC]
    public void RPC_SyncCatchupOnTable ( string syncInfos, int _phaseTurn, int _lastPotIndex, int _lengthPotIndex, int _turnNow, int _turnNext , long _lastBet)
    {
        if (!waitingResponseSync)
            return;

        _PokerGameManager.lastBet = _lastBet;

        string stringIndex;
        string[] playersIndex;
        string[] moneyPots;

        waitingResponseSync = false;
        Hashtable dataInfo = syncInfos.toHashtable ();
        _PokerGameManager.matchOnGoing = true;

        SePokerManager.instance.uiWaitingNextRound.Hide ();

        //Region Player 
        stringIndex = (string) dataInfo["slotCurrentPlayers"];
        playersIndex = stringIndex.Split (',');
        _PokerGameManager.turnManager.SyncCurrentMatchPlayers (playersIndex);

        //Region Pot  
        stringIndex = (string) dataInfo["moneyPot"];
        moneyPots = stringIndex.Split (',');
        for (int x = 0; x < _lengthPotIndex; x++)
        {
            stringIndex = (string) dataInfo["pot" + x];
            playersIndex = stringIndex.Split (',');

            _PokerGameManager.instance.CatchUpPot (x, Convert.ToInt64 (moneyPots[x]), playersIndex);
        }

        _PokerGameManager.instance.lastPotIndex = _lastPotIndex;

        //Region Open Card
        //debug.LogError("Phase Turn -- " +_phaseTurn);
        //_PokerGameManager.turnManager.phaseTurn = _phaseTurn;
        _PokerGameManager.turnManager.SetupSyncTurn (_turnNow, _turnNext);
        _PokerGameManager.turnManager.SyncOpenCard (_phaseTurn);
    }

    public void SyncMatchOnGoingWithProperties ()
    {
        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.room.CustomProperties;
        if (properties.ContainsKey (PhotonEnums.Room.LastIndexDealer))
            _PokerGameManager.turnManager.indexLastDealer = (int) properties[PhotonEnums.Room.LastIndexDealer];

        string strCardTable = PhotonUtility.GetRoomProperties<string> (PhotonEnums.Room.PokerCardTable);
        if (strCardTable == "")
            return;

        string[] arCard = strCardTable.Split (',');

        int[] cardTable = new int[5] { int.Parse (arCard[0]), int.Parse (arCard[1]), int.Parse (arCard[2]), int.Parse (arCard[3]), int.Parse (arCard[4]) };
        _PokerGameManager.instance.InstallCardTable (cardTable);

    }
    #endregion

    #region Application Behaviors
    //void OnApplicationFocus(bool hasFocus)
    //{

    //}

    DateTime lastPausedTime;
    float _timeLeftforPause;
    void OnApplicationPause ( bool pauseStatus )
    {
        _timeLeftforPause = myBackgroundTimeOut;

        if (_timeLeftforPause <= 0)
            _timeLeftforPause = 3f;

        PhotonNetwork.BackgroundTimeout = _timeLeftforPause;

        if (pauseStatus)
        {
            lastPausedTime = DateTime.Now;
        }
        else
        {
            DateTime curTime = DateTime.Now;
            TimeSpan diff = curTime - lastPausedTime;
            float diffVal = (float) Math.Floor (diff.TotalSeconds);

            if (diffVal > _timeLeftforPause)
            {
                SePokerManager.instance.uiMessageBox.Show (gameObject, "ID_ConnectionError", MessageBoxType.OK, 2, true);
            }
        }
    }

    [PunRPC]
    void RPC_ForceQuitMatch()
    {
        SePokerManager.instance.uiMessageBox.Show (gameObject, "ID_Timeout", MessageBoxType.OK, 1, true);        
    }

    private void onMessageBoxOKClicked(int returnedCode)
    {
        if (returnedCode == 1)
        {

            Debug.LogError ("Quit Game 99");
            GlobalVariables.bQuitOnNextRound = false;
            ImLeaving ();
            StartCoroutine (SePokerManager.instance.uiPause.LoadMenu ());
        }
        else if (returnedCode == 2)
            SceneManager.LoadScene ("Menu");
            //Application.LoadLevel("Menu");
    }

    #endregion
}