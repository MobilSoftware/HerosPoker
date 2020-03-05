using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class _PokerGameManager : MonoBehaviour
{
    public static _PokerGameManager instance;

    public Mesh chipTen;
    public Mesh chipHundred;
    public Mesh chipThousand;
    public FxTableCard fxTableCard;
    public FxTableSpotlite fxTableSpotlite;
    //public GenericAvater npcDealer;

    #region Game Manager
    public static _CardManager cardManager;
    public static _TurnManager turnManager;

    /// <summary>
    /// Unsorted Players [0] is My Player
    /// </summary>
    public _PlayerPokerActor[] unsortedPlayers = new _PlayerPokerActor[7]; //Local UNSORTED [0] is MyPlayer
    public _PlayerPokerActor[] stockPlayers = new _PlayerPokerActor[7]; //Local
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

    public _ParasitePoker[] stockParasite;
    public _CardActor[] tableCard = new _CardActor[5]; //Local
    public ChipsDrop[] chipDealer = new ChipsDrop[3];

    public Transform[] positionPot = new Transform[8]; //Local
    public GameObject objPot; //Local
    public GameObject chipD;

    public List<_PotObject> potOnTable = new List<_PotObject> (); //Sync
    public int lastPotIndex; //Sync

    public _PlayerPokerActor dealer, smallBlind, bigBlind; //Sync

    public static long biggestBet;
    public static long lastBet; //Sync
    public static long startBet; //Local
    public static bool matchOnGoing = false; //Sync

    public static void SetOtherBigChip ( _PlayerPokerActor exceptThis )
    {
        List<_PlayerPokerActor> sortedPlayer = new List<_PlayerPokerActor> (turnManager.currentPlayers.OrderByDescending (x => x.myMoney));
        sortedPlayer.Remove (exceptThis);

        long mine = exceptThis.myMoney + exceptThis.chipsBet;

        for (int x = sortedPlayer.Count - 1; x >= 0; x--)
            if (sortedPlayer[x].isFolded)
                sortedPlayer.RemoveAt (x);

        if (sortedPlayer.Count > 0)
        {
            biggestBet = sortedPlayer[0].myMoney + sortedPlayer[0].chipsBet;

            if (biggestBet > mine || biggestBet == 0)
                biggestBet = mine;
        }
        else
            biggestBet = mine;

        ////debug.Log("Set Biggest Bet : " + biggestBet);
    }

    #endregion

    private void Awake ()
    {
        instance = this;
        turnManager = new _TurnManager ();
        turnManager.indexLastDealer = -1;

        cardManager = GetComponent<_CardManager> ();

        //npcDealer.LoadAvatarDealer ();
    }

    #region GamePlay

    public void StartMatch ()
    {
        //debug.Log("Start Match");
        matchOnGoing = true;

        turnManager.UpdatePlayerForMatch ();
        cardManager.SetupRanks ();
        SavePropertiesCardPlayer ();

        turnManager.StartFirstTurn ();
    }

    void SavePropertiesCardPlayer ()
    {
        foreach (_PlayerPokerActor _p in unsortedPlayers)
            _p.SaveMyCardProperties ();
    }

    public void InstallCardTable ( int[] idx )
    {
        for (int a = 0; a < tableCard.Length; a++)
            tableCard[a].RefreshCard (idx[a]);
    }

    public void CatchUpPot ( int idx, long _money, string[] slotsOwner )
    {
        _PlayerPokerActor[] _owners = new _PlayerPokerActor[slotsOwner.Length];

        for (int a = 0; a < _owners.Length; a++)
            _owners[a] = instance.stockPlayers[int.Parse (slotsOwner[a])];

        GameObject o = Instantiate (objPot, positionPot[idx]);
        o.transform.localPosition = Vector3.zero;
        potOnTable.Add (o.GetComponent<_PotObject> ());

        potOnTable[potOnTable.Count - 1].SetPotOwner (_owners);
        potOnTable[potOnTable.Count - 1].SetPotValue (_money);

        ShowChips ();
    }

    public void AddPot ( int indexPot, long _startPot, _PlayerPokerActor[] owners )
    {
        Logger.E("Owner Pot : " +owners.Length);

        if (owners.Length <= 0)
            return;

        lastPotIndex = indexPot;

        GameObject o = Instantiate (objPot, positionPot[lastPotIndex]);
        o.transform.localPosition = Vector3.zero;
        potOnTable.Add (o.GetComponent<_PotObject> ());

        potOnTable[lastPotIndex].SetPotOwner (owners);
        potOnTable[lastPotIndex].SetPotValue (_startPot);
    }

    public void AddMoneyToPot ( long _val )
    {
        potOnTable[lastPotIndex].AddMoneyToPot (_val);

        ShowChips ();
    }

    public void CalculateEndRound ()
    {
        foreach (_PlayerPokerActor _p in turnManager.currentPlayers)
            _p.AnimationFlowChips (_p.chipsBet, false);


        if (turnManager.currentPlayers.Count > 0) //Takutnya player baru masuk dapet RPC manggil ini padahal belom initiate
        {
            StopCoroutine (RoutineCalculatePot ());
            StartCoroutine (RoutineCalculatePot ());
        }
    }

    IEnumerator RoutineCalculatePot ()
    {
        List<_PlayerPokerActor> pSort = new List<_PlayerPokerActor> (turnManager.currentPlayers.OrderBy (x => x.chipsBet));

        //debug.Log("Jumlah player Sort : " + pSort.Count);

        while (pSort.Count > 0)
        {
            long totalBet = 0;
            long batasBet = 0;

            for (int x = pSort.Count - 1; x >= 0; x--)
            {
                if (pSort[x].isFolded)
                {
                    totalBet += pSort[x].chipsBet;
                    pSort[x].chipsBet = 0;
                    pSort.RemoveAt (x);
                }
            }

            batasBet = pSort[0].chipsBet;

            if (pSort.Count > 0)
            {
                for (int x = pSort.Count - 1; x >= 0; x--)
                {
                    totalBet += batasBet;
                    pSort[x].chipsBet -= batasBet;

                    if (pSort[x].chipsBet <= 0)
                        pSort.RemoveAt (x);
                }
            }

            AddMoneyToPot (totalBet);

            if (pSort.Count > 0)
                AddPot (lastPotIndex + 1, 0, pSort.ToArray ());

            yield return null;
        }

        _PlayerPokerActor _p;
        _PlayerPokerActor _allInPlayer = null;
        for (int x = turnManager.currentPlayers.Count - 1; x >= 0; x--)
        {
            _p = turnManager.currentPlayers[x];

            if (_p.isAllIn || _p.isFolded)
            {
                if (turnManager.ThisIsNextPlayer (_p))
                    turnManager.UpdatePlayerNextTurn (_p);

                if (_p.isAllIn && _allInPlayer == null)
                    _allInPlayer = _p;

                turnManager.currentPlayers.RemoveAt (x);
            }
        }

        if (turnManager.currentPlayers.Count > 1)
        {
            if (_allInPlayer != null)
                if (potOnTable[lastPotIndex].owner.Contains (_allInPlayer))
                    AddPot (lastPotIndex + 1, 0, turnManager.currentPlayers.ToArray ());

            turnManager.OpenCard ();
        }
        else
        {
            int jumlahFolded = 0;

            for (int x = 0; x < turnManager.matchPlayers.Count; x++) //untuk ngitung jumlah player Fold
                if (turnManager.matchPlayers[x].isFolded)
                    jumlahFolded++;

            if (jumlahFolded >= turnManager.matchPlayers.Count - 1)
                PhotonTexasPokerManager.instance.TakeYourPotTogether (true); // Go To Take Our Pot Together
            else
                turnManager.OpenAllCard ();

        }
    }

    IEnumerator OpenCardRoutine ( int from, int to )
    {
        SoundManager.instance.PlaySFX (SFXType.SFX_CardDistributeOnce, Vector3.zero, false, 2);
        for (int t = from; t <= to; t++)
        {
            turnManager.phaseTurn++;
            if (t >= 5)
                break;

            fxTableCard.DealTableCard (tableCard[t].gameObject);
            yield return _WFSUtility.wfs08;
        }
        SoundManager.instance.StopSFX2 ();

        unsortedPlayers[0].SetMyInfoRank ();

        yield return _WFSUtility.wfs1;

        if (turnManager.phaseTurn >= 6)
            PhotonTexasPokerManager.instance.TakeYourPotTogether ();
        else
            turnManager.StartNextTurn ();
    }

    public void TakeOurPotTogether (bool fromFold)
    {
        StartCoroutine (TakeOurPot (fromFold));
    }

    IEnumerator TakeOurPot (bool fromFold)
    {
        matchOnGoing = false;

        foreach (_PlayerPokerActor _p in turnManager.matchPlayers)
        {
            if (!fromFold)
            {                
                if (_p.isBot || !_p.isMine)
                {
                    if (_p.isFolded)
                        _p.myCardBack.Hide();
                    else
                        _p.ShowMyCard(true);
                }
            }
            else
            {
                _p.myCardBack.Hide();
            }            
        }

        yield return _WFSUtility.wfs2;

        potOnTable.Reverse ();

        for (int x = potOnTable.Count - 1; x >= 0; x--)
        {
            if (x == potOnTable.Count - 1)
                DeactiveChips ();

            potOnTable[x].GiveThePotToWinner ();
            SoundManager.instance.PlaySFX (SFXType.SFX_CoinSpread, Vector3.zero);
            Destroy (potOnTable[x].gameObject);
            yield return _WFSUtility.wfs3;

            foreach (_CardActor _c in tableCard)
                _c.objFxCard.SetActive (false);

            foreach (_PlayerPokerActor _p in turnManager.matchPlayers)
            {
                _p.DeactiveCardWinner ();
                //if (_p.isMine && !_p.isBot)
                //{
                //    HomeSceneManager.Instance.myHomeMenuReference.uiMyCouponPoker.SetData ();
                //    switch (_p.GetScoringTypeAPI())
                //    {
                //        case ApiBridge.ScoringType.Royal_Flush_Bottom:
                //            long lCoupon = Convert.ToInt64 (HomeSceneManager.Instance.myHomeMenuReference.uiMyCouponPoker.txtJackpotValue.text);
                //            Box_ItemReceived.instance.ShowRewardCoupon (lCoupon);
                //            //jackpot
                //            break;
                //        case ApiBridge.ScoringType.Four_Of_Kind_Bottom:
                //        case ApiBridge.ScoringType.Straight_Flush_Bottom:
                //            long lCoupon1 = Convert.ToInt64 (HomeSceneManager.Instance.myHomeMenuReference.uiMyCouponPoker.txtMyCouponValue1.text);
                //            Box_ItemReceived.instance.ShowRewardCoupon(lCoupon1);
                //            break;
                //        case ApiBridge.ScoringType.Full_House_Bottom:
                //        case ApiBridge.ScoringType.Straight_Bottom:
                //        case ApiBridge.ScoringType.Flush_Bottom:
                //            long lCoupon2 = Convert.ToInt64 (HomeSceneManager.Instance.myHomeMenuReference.uiMyCouponPoker.txtMyCouponValue2.text);
                //            Box_ItemReceived.instance.ShowRewardCoupon (lCoupon2);
                //            break;
                //    }
                //}
            }
        }

        yield return _WFSUtility.wfs1;
        potOnTable.Clear ();
        lastPotIndex = 0;
        yield return _WFSUtility.wfs1;

        for (int x = 0; x < tableCard.Length; x++)
        {
            tableCard[x].FlipCardDown ();
            yield return _WFSUtility.wfs03;
        }

        foreach (_PlayerPokerActor _p in turnManager.matchPlayers)
            _p.ShowMyCard (false);

        foreach (_PlayerPokerActor _p in stockPlayers)
            _p.ResetPanel();

        turnManager.ResetMatch();
        PhotonTexasPokerManager.instance.RoutineNextMatch();
    }

    public class _TurnManager
    {
        public int indexLastDealer, phaseTurn; //Sync
        public List<_PlayerPokerActor> matchPlayers = new List<_PlayerPokerActor> (); //Sync
        public List<_PlayerPokerActor> currentPlayers = new List<_PlayerPokerActor> (); //Sync
        _PlayerPokerActor turnNow;
        _PlayerPokerActor nextTurn;

        public _PlayerPokerActor GetTurnNow
        {
            get
            {
                return turnNow;
            }
        }

        public _PlayerPokerActor GetturnNext
        {
            get
            {
                return nextTurn;
            }
        }

        public bool ThisIsNextPlayer ( _PlayerPokerActor _player )
        {
            return _player == nextTurn;
        }

        public void UpdatePlayerNextTurn ( _PlayerPokerActor _player = null )
        {
            if (_player == null)
                _player = turnNow;

            int val = currentPlayers.IndexOf (_player);

            //debug.LogError("Count Player " + currentPlayers.Count);
            //debug.LogError("Current Index " +val);
            val = GetIndexValueRound (val, currentPlayers.Count);

            nextTurn = currentPlayers[val];
        }

        /// <summary>
        /// Get Value Index in Round,  0,1,2,3,4,5,6,0,1,2,3,4,5,......
        /// </summary>
        /// <param name="cur"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        int GetIndexValueRound ( int cur, int max )
        {
            cur++;
            if (cur >= max)
                cur = 0;

            return cur;
        }

        public void ResetMatch ()
        {
            foreach (_PlayerPokerActor player in matchPlayers)
                player.ResetProperties ();
        }

        public void UpdatePlayerForMatch ()
        {
            currentPlayers.Clear ();
            matchPlayers.Clear ();

            //debug.Log("stock : " + instance.stockPlayers.Length);

            foreach (_PlayerPokerActor pA in instance.stockPlayers)
            {
                bool bActive = PhotonUtility.GetPlayerProperties<bool> (pA.myPlayer, PhotonEnums.Player.Active);
                if (bActive)
                {
                    currentPlayers.Add (pA);
                    matchPlayers.Add (pA);
                }
            }

            //debug.Log("CURRENT PLAYERS : " + currentPlayers.Count);
        }


        public void SyncCurrentMatchPlayers ( string[] sCp ) //, string[] sMp
        {
            currentPlayers.Clear ();
            matchPlayers.Clear ();

            foreach (_PlayerPokerActor pA in instance.stockPlayers)
            {
                bool bActive = PhotonUtility.GetPlayerProperties<bool> (pA.myPlayer, PhotonEnums.Player.Active);
                if (bActive)
                    matchPlayers.Add (pA);
            }
            
            for (int a = 0; a < sCp.Length; a++)
            {
                currentPlayers.Add (instance.stockPlayers[int.Parse (sCp[a])]);
            }
        }

        public void StartFirstTurn ()
        {
            indexLastDealer = GetIndexValueRound (indexLastDealer, matchPlayers.Count); 

            if (PhotonNetwork.isMasterClient)
                PhotonUtility.SetRoomProperties (PhotonEnums.Room.LastIndexDealer, indexLastDealer);

            phaseTurn = 0;

            instance.AddPot (0, 0, currentPlayers.ToArray ());

            if (currentPlayers.Count > 2)
            {
                lastBet = startBet;

                currentPlayers[indexLastDealer].SetMyRole (_GameRoleEnums.Dealer);
                instance.chipD.SetActive (true);
                LeanTween.move (instance.chipD, currentPlayers[indexLastDealer].chipsD, 1f);

                currentPlayers[GetIndexValueRound (indexLastDealer, matchPlayers.Count)].SetMyRole (_GameRoleEnums.SmallBlind);
                currentPlayers[GetIndexValueRound (indexLastDealer + 1, matchPlayers.Count)].SetMyRole (_GameRoleEnums.BigBlind);

                turnNow = currentPlayers[GetIndexValueRound (indexLastDealer + 1, matchPlayers.Count)];
            }
            else if (currentPlayers.Count == 2)
            {
                lastBet = startBet;
                instance.chipD.SetActive (false);
                currentPlayers[indexLastDealer].SetMyRole (_GameRoleEnums.SmallBlind);
                currentPlayers[GetIndexValueRound (indexLastDealer, matchPlayers.Count)].SetMyRole (_GameRoleEnums.BigBlind);

                turnNow = currentPlayers[GetIndexValueRound (indexLastDealer, matchPlayers.Count)];
            }

            UpdatePlayerNextTurn (); //To Initialize the Next Turn Player before Start Game

            if (PhotonUtility.GetPlayerProperties<bool> (PhotonNetwork.player, PhotonEnums.Player.Active))
            {
                if (!nextTurn.isMine || nextTurn.isBot)
                    instance.unsortedPlayers[0].panelPreAction.SetActive (true);
            }

            StartNextTurn ();
        }

        public void OpenAllCard ()
        {
            instance.StartCoroutine (instance.OpenCardRoutine (phaseTurn, 5));
        }

        public void OpenCard ()
        {
            lastBet = 0;
            foreach (_PlayerPokerActor _p in currentPlayers)
                _p.ResetNextRound ();

            if (phaseTurn == 0)
                instance.StartCoroutine (instance.OpenCardRoutine (phaseTurn, 2));
            else
                instance.StartCoroutine (instance.OpenCardRoutine (phaseTurn, phaseTurn));
        }

        public void SyncOpenCard ( int _phase )
        {
            phaseTurn = 0;

            if (_phase != 0)
            {
                for (int t = 0; t <= _phase - 1; t++)
                {
                    turnManager.phaseTurn++;

                    if (t >= 5)
                        break;

                    instance.tableCard[t].gameObject.SetActive (true);
                }
            }
        }

        public void CheckTurn ()
        {
            ////debug.Log(" | " + currentPlayers[GetIndexNextTurn()].chipsBet + " | " + lastBet);
            int avail = 0;
            int foldAll = 0;

            foreach (_PlayerPokerActor p in currentPlayers)
            {
                if (p.myMoney == 0 || p.isAllIn)
                    avail++;

                if (p.isFolded)
                    foldAll++;
            }


            //Debug.Log("available Player : " +avail);

            if (nextTurn.StopTurn (lastBet) || avail >= currentPlayers.Count || foldAll >= currentPlayers.Count - 1)
                PhotonTexasPokerManager.instance.CalculateEndRoundTogether ();
            else
                StartNextTurn ();
        }

        public void StartNextTurn ()
        {
            PhotonTexasPokerManager.msgDelayPoker = "Start Next Turn";

            if (PhotonNetwork.isMasterClient)
                PhotonTexasPokerManager.instance.NextTurnTogether (turnNow.slotIndex, nextTurn.slotIndex);
        }

        public void NextTurnTogether ( int _turnNow, int _nextTurn )
        {
            SetupSyncTurn (_turnNow, _nextTurn);

            turnNow = nextTurn;
            UpdatePlayerNextTurn ();

            turnNow.MyTurn ();
        }

        public void SetupSyncTurn ( int _turnNow, int _nextTurn )
        {
            turnNow = instance.stockPlayers[_turnNow];
            nextTurn = instance.stockPlayers[_nextTurn];
        }
    }
    #endregion

    public void CleanEverything ()
    {
        if (potOnTable.Count > 0)
        {
            for (int x = potOnTable.Count - 1; x >= 0; x--)
                if (potOnTable[x] != null)
                    Destroy (potOnTable[x].gameObject);
        }

        foreach (_CardActor _c in tableCard)
            _c.objFxCard.SetActive (false);

        foreach (_CardActor _c in tableCard)
            _c.gameObject.SetActive (false);

        potOnTable.Clear ();

        foreach (_ParasitePoker _p in stockParasite)
            _p.CleanWithHolyWater ();

        foreach (_PlayerPokerActor _a in unsortedPlayers)
            _a.CleanWithHolyWater ();

        for (int x = 0; x < stockPlayers.Length; x++)
            stockPlayers[x] = null;

        turnManager.currentPlayers.Clear ();
        turnManager.matchPlayers.Clear ();
        turnManager.indexLastDealer = -1;

        DeactiveChips ();
    }

    void ShowChips ()
    {
        DeactiveChips ();

        chipDealer[0].PlayDropAnimation ();
        chipDealer[1].PlayDropAnimation ();
        chipDealer[2].PlayDropAnimation ();
    }

    void DeactiveChips ()
    {
        chipDealer[0].gameObject.SetActive (false);
        chipDealer[1].gameObject.SetActive (false);
        chipDealer[2].gameObject.SetActive (false);
    }

}