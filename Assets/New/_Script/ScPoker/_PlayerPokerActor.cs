using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class _PlayerPokerActor : MonoBehaviour
{
    #region UI
    [Header("My Player UI Panel")]
    public Text txtBtnCheck;
    public Text txtBtnAllIn;
    public Text txtValueRaise, txtValueRaise2;
    public Button btnCheckCall;
    public Button btnRaise;
    public Button btnFold;
    public Button btnOpenRaise;
    public Button btnAllIn;
    public Button btnThrowPanel;
    public GameObject panelAction, txtObjCheck, txtObjCall, panelPreAction;
    public Slider uiSlider;
    _SliderValue sliderTools;
    [SerializeField] Toggle[] togglePreAct; //Fold, Check Or Fold, Check Only, Call Any;
    public RectTransform trPanelMoney;

    [Header("UI Panel")]
    public Image countDownImg;
    public Text txtBet;
    public Text txtRole;
    public Text txtMyMoney;
    public Text txtChatMessage;
    public Text txtName;
    public Text txtHandRank;
    public Image imgHandRank;

    public GameObject allINobj;
    public GameObject objCD;
    public GameObject panelTxtRankHand;
    [SerializeField] GameObject panelBet;
    //[SerializeField] GameObject btnInvite;
    [SerializeField] GameObject chatItem;

    public Transform chipsD;
    public Transform postPanelThrowItem, postThrowTarget;

    public Image[] imgBetAmount;

    public ChipsDrop[] myChipsBet;
    public ChipFlow myChipsFlow;
    public CardBack myCardBack;
    public GameObject objPlayerActive;

    [SerializeField] Image panelState;

    [SerializeField] Color raiseColor;
    [SerializeField] Color checkColor;
    [SerializeField] Color callColor;
    [SerializeField] Color foldColor;

    private long val;
    private bool isOpen;
    #endregion

    [Space(20)]

    public _CardActor[] _myHandCard = new _CardActor[2];
    public _CardActor[] _myHighlightedCard = new _CardActor[5];
    public HandRankPoker _playerHandRank; //Properties
    public ApiBridge.ScoringType GetScoringTypeAPI()
    {
        ApiBridge.ScoringType scoringType = ApiBridge.ScoringType.None;

        if (isFolded)
            return scoringType;

        switch ((int)_playerHandRank)
        {
            case 1:
                scoringType = ApiBridge.ScoringType.High_Card;
                break;
            case 2:
                scoringType = ApiBridge.ScoringType.One_Pair;
                break;
            case 3:
                scoringType = ApiBridge.ScoringType.Two_Pair;
                break;
            case 4:
                scoringType = ApiBridge.ScoringType.Three_Of_Kind_Bottom;
                break;
            case 5:
                scoringType = ApiBridge.ScoringType.Straight_Bottom;
                break;
            case 6:
                scoringType = ApiBridge.ScoringType.Flush_Bottom;
                break;
            case 7:
                scoringType = ApiBridge.ScoringType.Full_House_Bottom;
                break;
            case 8:
                scoringType = ApiBridge.ScoringType.Four_Of_Kind_Bottom;
                break;
            case 9:
                scoringType = ApiBridge.ScoringType.Straight_Flush_Bottom;
                break;
            case 10:
                scoringType = ApiBridge.ScoringType.Royal_Flush_Bottom;
                break;

        }

        return scoringType;
    }

    public Hero hero;
    public _ParasitePoker _myParasitePlayer;
    public PhotonPlayer myPlayer;

    public int slotIndex = -1;
    public int _kicker, valueHand, secondValueHand, secondKicker; //Properties
    public int RANK; //Properties
    public int heroUsed;

    public long chipsBet; //Properties
    public long totalBet; //Properties

    public Text txtInsta1;
    public Text txtInsta2;
    public Text txtInsta3;

    public Button instaButton1;
    public Button instaButton2;
    public Button instaButton3;
    public Button instaAllIn;
    void SyncBet(long _chipsBet, long _totalBet)
    {
        myMoney = PhotonUtility.GetPlayerProperties<long>(myPlayer, PhotonEnums.Player.Money);
        chipsBet = _chipsBet;
        totalBet = _totalBet;
    }

    public long myMoney; //Properties
    long lastSliderBet;

    float countDown;

    public string _myName;
    public string[] cardTypes;

    public bool alreadyBet; //Once Sync
    public bool isAllIn; //Once Sync
    public bool isBot, isMine;
    public bool isFolded; //Once Sync
    public bool forceFold = false;

    [SerializeField] bool doneAction;

    bool IsChipsCheck(long value) { return chipsBet == value; }
    public bool StopTurn(long value) { return chipsBet == value && alreadyBet; }
    bool NotEnufMoney() { return myMoney <= _PokerGameManager.lastBet - chipsBet; }
    bool NotEnufRaise() { return myMoney <= (_PokerGameManager.lastBet == 0 ? _PokerGameManager.startBet : _PokerGameManager.lastBet * 2) - chipsBet; }

    void OnDisable()
    {
        //ActivateInviteButton(true);

        hero.Reset();
    }

    private void Start()
    {
        sliderTools = new _SliderValue();
        InitializeActor();
        if (instaButton1 != null)
        {
            instaButton1.onClick.AddListener (FirstInstaRaise);
            instaButton2.onClick.AddListener (SecondInstaRaise);
            instaButton3.onClick.AddListener (ThirdInstaRaise);
            instaAllIn.onClick.AddListener (InstaAllIn);
        }
    }

    public void SetMyRole(_GameRoleEnums _role)
    {
        panelState.gameObject.SetActive(true);
        chipsBet = 0;
        totalBet = 0;

        switch (_role)
        {
            case _GameRoleEnums.Dealer:
                _PokerGameManager.instance.dealer = this;
                txtRole.text = "Dealer";

                panelState.color = Color.black;
                break;
            case _GameRoleEnums.SmallBlind:
                _PokerGameManager.instance.smallBlind = this;
                txtRole.text = "Small Blind";
                DoBet(_PokerGameManager.startBet / 2);

                panelState.color = callColor;
                break;
            case _GameRoleEnums.BigBlind:
                _PokerGameManager.instance.bigBlind = this;
                txtRole.text = "Big Blind";
                DoBet(_PokerGameManager.startBet);

                panelState.color = checkColor;
                break;
        }
    }

    void InitializeActor()
    {
        btnCheckCall.onClick.AddListener(() => { S_CheckCall();});
        btnAllIn.onClick.AddListener(() => {S_AllIn(); });

        btnRaise.onClick.AddListener(() => {S_Raise();});
        btnFold.onClick.AddListener(() => {S_FoldAction();});
        btnOpenRaise.onClick.AddListener(() => { OpenPanelRaise(true);});
        uiSlider.onValueChanged.AddListener(ReturnScrollValue);

        btnThrowPanel.onClick.AddListener(() => { _PokerGameHUD.instance.ShowPanelThrow(postPanelThrowItem, this); });
    }

    public void SyncMoney() //In Case Not Sync
    {
        if (myPlayer == null)
            return;

        if (!isMine || isBot)
            myMoney = PhotonUtility.GetPlayerProperties<long>(myPlayer, PhotonEnums.Player.Money);

        if (isBot && PhotonNetwork.isMasterClient)
            _myParasitePlayer.photonView.RPC(PhotonEnums.RPC.RPC_ForceSyncBotMoney, PhotonTargets.Others, myMoney);

        txtMyMoney.text = myMoney.toFlexibleCurrency();
    }

    public void ResetPanel() //Solution Gerak cepet, please Delete later
    {
        panelPreAction.SetActive(false);
        panelState.gameObject.SetActive(false);
        panelBet.SetActive(false);
        panelTxtRankHand.SetActive(false);
        if (imgHandRank != null)
            imgHandRank.gameObject.SetActive (false);
    }

    public void ResetProperties()
    {
        StopCoroutine("MyTurnStart");

        UI_allin(false);

        panelPreAction.SetActive(false);
        panelState.gameObject.SetActive(false);
        panelBet.SetActive(false);
        panelTxtRankHand.SetActive(false);
        if (imgHandRank != null)
            imgHandRank.gameObject.SetActive (false);

        forceFold = false;
        isFolded = false;
        alreadyBet = false;

        chipsBet = 0;
        totalBet = 0;

        if (isMine || isBot)
        {
            if (myPlayer != null)
                PhotonUtility.SetPlayerProperties(myPlayer, PhotonEnums.Player.TotalBet, totalBet);
        }

        txtRole.text = "";
        panelState.color = Color.white;
        txtBet.text = "";
        txtHandRank.text = "";

        objCD.SetActive(false);
        DeactiveChips();

        ClearPreAction();

        hero.Revert ();
        //avater3D.ChangeSkinColor(Color.white);
    }

    public void ResetNextRound()
    {
        chipsBet = 0;

        if (isMine || isBot)
        {
            if (myPlayer != null)
                PhotonUtility.SetPlayerProperties(myPlayer, PhotonEnums.Player.ChipsBet, chipsBet);
        }

        alreadyBet = false;
        txtRole.text = "";
        panelState.color = Color.white;

        ClearPreAction();

        if (!isFolded)
            panelState.gameObject.SetActive(false);
    }

    public void AutoBuyIn(long valMoney)
    {
        myMoney = valMoney;
        txtMyMoney.text = myMoney.toFlexibleCurrency();
        PhotonUtility.SetPlayerProperties(myPlayer, PhotonEnums.Player.Money, myMoney);
    }

    void DoneAction()
    {
        if (doneAction)
            return;

        doneAction = true;
        objPlayerActive.SetActive (false);

        StopCoroutine("MyTurnStart");

        if (isMine && !isBot)
            PhotonTexasPokerManager.myBackgroundTimeOut = 4f;

        if (myMoney == 0)
            UI_allin(true);
        else
            UI_allin(false);

        OpenPanelRaise(false);

        objCD.gameObject.SetActive(false);
        panelAction.SetActive(false);

        if(isFolded || isAllIn)
            panelPreAction.SetActive(false);
        else
            panelPreAction.SetActive(true);

        uiSlider.gameObject.SetActive(false);

        PhotonTexasPokerManager.msgDelayPoker = "Wait Check Turn";
        if (PhotonNetwork.isMasterClient && _PokerGameManager.turnManager.GetTurnNow == this)
            PhotonTexasPokerManager.instance.Master_CheckTurn();
    }

    /// <summary>
    /// Pull the chips Yo!
    /// </summary>
    public void PullTheChips(long chips)
    {
        //Debug.LogError(name + " get : " +chips);
        AnimationFlowChips(chipsBet, true);

        //myMoney += chips;       //pajak
        float tax = chips * 0.1f;
        long chipsAfterTax = chips - Convert.ToInt64 (tax);
        myMoney += chipsAfterTax;

        if (isMine && !isBot)
            StartCoroutine (_UpdateMyUIPlayer (chipsAfterTax));
        else
            UpdateUIPlayer();

        totalBet -= chips;

        ShowCardWinner();

        SoundManager.instance.PlaySFX(_playerHandRank == HandRankPoker.fullHouse ? SFXType.SFX_WinC : SFXType.SFX_WinA, Vector3.zero);
        //avater3D.PlayAvaterExpression(Random.Range(1, 3) > 1 ? (int)AvaterAnimation.SitHappy : (int)AvaterAnimation.SitVeryHappy);
    }

    void ShowCardWinner()
    {
        CancelInvoke("DeactiveCardWinner");

        //for (int x = 0; x < _myHighlightedCard.Length; x++)
            //if(_myHighlightedCard[x] != null)
                //_myHighlightedCard[x].objFxCard.SetActive(true);
    }

    public void DeactiveCardWinner()
    {
        //for (int x = 0; x < _myHighlightedCard.Length; x++)
            //if (_myHighlightedCard[x] != null)
                //_myHighlightedCard[x].objFxCard.SetActive(false);
    }

    public void MyTurn()
    {        
        //Debug.Log("MY TURN : " +gameObject.name);
        SoundManager.instance.PlaySFX(SFXType.SFX_PokerSwitch, Vector3.zero);
        doneAction = false;

        if(forceFold)
            AlwaysFold();

        if (myMoney <= 0 || isFolded)
        {
            SkipNoMoney();
            return;
        }
        else
        {
            _PokerGameManager.SetOtherBigChip(this);
            StartCoroutine("MyTurnStart");

            SetPanelAction();
            objPlayerActive.SetActive (true);

            //_PokerGameManager.instance.fxTableSpotlite.ShowFxSpotlight(System.Array.IndexOf(_PokerGameManager.instance.unsortedPlayers, this));
        }
    }

    void SetPanelAction()
    {
        if (!isMine || isBot)
            return;

        panelAction.SetActive(true);
        panelPreAction.SetActive(false);

        btnCheckCall.gameObject.SetActive(false);
        btnOpenRaise.gameObject.SetActive(false);
        btnAllIn.gameObject.SetActive(false);
        bool isCheck = IsChipsCheck(_PokerGameManager.lastBet);

        if (isMine && !isBot)
            InitiatePreAction(isCheck);

        if (NotEnufMoney()) //Duid ga cukup untuk Raise dan Call
        {
            btnAllIn.gameObject.SetActive(true);
            txtBtnAllIn.text = myMoney.toFlexibleCurrency();
        }
        else
        {

            Logger.E ("set panel action: |last bet: " + _PokerGameManager.lastBet + "|chipsBet: " + chipsBet + "|txtBtnCheck: " + (_PokerGameManager.lastBet - chipsBet));
            if (NotEnufRaise()) //Duid ga cukup untuk Raise tapi cukup untuk Call, karena Raise harus 2 kali dari last chipsbet
            {
                txtObjCheck.SetActive(isCheck);
                txtObjCall.SetActive(!isCheck);

                txtBtnCheck.text = (_PokerGameManager.lastBet - chipsBet).toFlexibleCurrency();

                btnCheckCall.gameObject.SetActive(true);

                btnAllIn.gameObject.SetActive(true);
                txtBtnAllIn.text = myMoney.toFlexibleCurrency();
            }
            else //Duid cukup untuk raise dan call
            {
                txtObjCheck.SetActive(isCheck);
                txtObjCall.SetActive(!isCheck);

                txtBtnCheck.text = (_PokerGameManager.lastBet - chipsBet).toFlexibleCurrency();

                btnCheckCall.gameObject.SetActive(true);
                btnOpenRaise.gameObject.SetActive(true);
            }
        }
    }

    void InitiatePreAction(bool flagCheck)
    {
        if (togglePreAct[0].isOn) //Check Or Fold
        {
            togglePreAct[0].isOn = false;
            if (flagCheck)
                S_CheckCall();
            else
                AlwaysFold();
        }
        else if (togglePreAct[1].isOn) // Check Only
        {
            togglePreAct[1].isOn = false;
            if (flagCheck)
                S_CheckCall();
        }
        else if (togglePreAct[2].isOn) // Call Any
        {
            togglePreAct[2].isOn = false;

            //Debug.Log("ENUF GA? " +NotEnufMoney());

            if (NotEnufMoney())
                S_AllIn();
            else
                S_CheckCall();
        }
    }
    void ClearPreAction()
    {
        foreach (Toggle tgl in togglePreAct)
            tgl.isOn = false;
    }

    IEnumerator MyTurnStart()
    {
        bool isPlayingTimerSFX = false;
        objCD.gameObject.SetActive(true);

        countDownImg.fillAmount = 1;
        countDown = 15;
        int batasActionRemote = 0;

        if (PhotonNetwork.isMasterClient && isBot)
            batasActionRemote = UnityEngine.Random.Range(8, 14);

        if (isMine & !isBot)
            PhotonTexasPokerManager.myBackgroundTimeOut = 8f;

        while (countDown > 0)
        {
            countDownImg.fillAmount = countDown/15;
            yield return null;

            if (countDown < batasActionRemote && isBot)
            {
                batasActionRemote = 0;
                RemoteActionbyMaster();
            }

            countDown -= 1 * Time.fixedDeltaTime;
            if (!isPlayingTimerSFX && countDown < 6f)
            {
                isPlayingTimerSFX = true;
                SoundManager.instance.PlaySFX(SFXType.SFX_Timer, Vector3.zero);
            }

            if (countDown < 10 && countDown > 9 && isMine && !isBot)
                PhotonTexasPokerManager.myBackgroundTimeOut = 4f;

            //Debug.LogError("While Bajingannnnnnnnn 10 " +countDown);
        }

        if (isMine && !isBot)
            S_FoldAction ();


        #region force Fold by Master
        int tolerantTime = 3;
        while (tolerantTime > 0)
        {
            //Debug.LogError("While Bajingannnnnnnnn 2");

            yield return _WFSUtility.wfs1;
            tolerantTime--;
        }
        //Debug.LogError ("after tolerant");

        PhotonTexasPokerManager.msgDelayPoker = "Wait To Force Fold";
        if (PhotonNetwork.isMasterClient)
            S_FoldAction();
        #endregion
    }

    

    #region Check Call and All In
    void S_AllIn()
    {
        _myParasitePlayer.SendAllIn(myMoney, chipsBet, totalBet);
    }

    public void RPC_AllIn(long val, long _bet, long _lastbet)
    {
        //Debug.LogError("Bajingan All In");
        SoundManager.instance.PlaySFX(SFXType.SFX_PokerRaise, Vector3.zero);

        SyncBet(_bet, _lastbet);
        DoBet(val);
        DoneAction();

        hero.CallAction ();
        //avater3D.PlayAvaterExpression((int)AvaterAnimation.SitAllIn);
    }

    void S_CheckCall()
    {
        if (isMine && !isBot)
            Logger.E ("val in S_CheckCall: " + "|Last bet: " + _PokerGameManager.lastBet + "|chipsBet: " + chipsBet + "|val: " + (_PokerGameManager.lastBet - chipsBet));
        _myParasitePlayer.SendCheckCall(_PokerGameManager.lastBet - chipsBet, chipsBet, totalBet);
    }

    public void RPC_CheckCall(long _val, long _bet, long _lastbet)
    {
        //Debug.LogError("Bajingan Check Call");      

        panelState.color = _val == 0 ? checkColor : callColor;
        txtRole.text = _val == 0 ? "Check" : "Call : " + _val.toFlexibleCurrency();

        SyncBet(_bet, _lastbet);
        DoBet(_val);
        DoneAction();

        if (_val == 0)
            hero.CheckAction ();
        else
            hero.CallAction ();
        //avater3D.PlayAvaterExpression(_val == 0 ? (int)AvaterAnimation.SitCheck : (int) AvaterAnimation.SitBet);
        SoundManager.instance.PlaySFX(_val == 0 ? SFXType.SFX_PokerCheck : SFXType.SFX_PokerCall, Vector3.zero);
    }
    #endregion

    #region raise
    void S_Raise()
    {
        //Debug.LogError("Raise WOI " + lastSliderBet +" - " +chipsBet);
        _myParasitePlayer.SendRaise(lastSliderBet - chipsBet, chipsBet, totalBet);
    }

    public void RPC_Raise(long val, long _bet, long _lastbet)
    {
        //Debug.LogError("Bajingan Raise " +val);

        SyncBet(_bet, _lastbet);
        DoBet(val); //value Raise
        DoneAction();

        SoundManager.instance.PlaySFX(SFXType.SFX_PokerRaise, Vector3.zero);
        txtRole.text = "Raise : " + val.toFlexibleCurrency();
        panelState.color = raiseColor;

        hero.CallAction ();
        //avater3D.PlayAvaterExpression((int)AvaterAnimation.SitBet);
    }

    void ReturnScrollValue(float _val)
    {
        //Debug.Log("aaaaaaaaaa " + _PokerGameManager.biggestBet + _PokerGameManager.lastBet);
        lastSliderBet = sliderTools.CalculateScore(_PokerGameManager.biggestBet, _PokerGameManager.lastBet, _val);
        txtValueRaise.text = lastSliderBet.toFlexibleCurrency();
        txtValueRaise2.text = txtValueRaise.text;
    }

    private void SetInstaButtons ()
    {
        val = _PokerGameManager.instance.GetPotValue ();
        if (val == 0)
            val = _PokerGameManager.startBet;
        isOpen = _PokerGameManager.instance.tableCard[2].gameObject.activeSelf;
        if (!isOpen)
        {
            instaButton3.gameObject.SetActive (true);
            txtInsta1.text = (3 * val).toFlexibleCurrency();
            txtInsta2.text = (4 * val).toFlexibleCurrency ();
            txtInsta3.text = (5 * val).toFlexibleCurrency ();
        }
        else
        {
            instaButton3.gameObject.SetActive (false);
            txtInsta1.text = Convert.ToInt64 (0.5f * val).toFlexibleCurrency ();
            txtInsta2.text = Convert.ToInt64 (0.7f * val).toFlexibleCurrency ();
        }
    }

    private void FirstInstaRaise ()
    {
        if (!isOpen)
            lastSliderBet = 3 * val;
        else
            lastSliderBet = Convert.ToInt64 (0.5f * val);
        S_Raise ();
    }

    private void SecondInstaRaise ()
    {
        if (!isOpen)
            lastSliderBet = 4 * val;
        else
            lastSliderBet = Convert.ToInt64 (0.7f * val);
        S_Raise ();
    }

    private void ThirdInstaRaise()
    {
        if (!isOpen)
        {
            lastSliderBet = 5 * val;
            S_Raise ();
        }
    }

    private void InstaAllIn ()
    {
        lastSliderBet = _PokerGameManager.biggestBet;
        S_Raise ();
    }

    void OpenPanelRaise(bool flag)
    {
        uiSlider.gameObject.SetActive(flag);
        btnRaise.gameObject.SetActive(flag);
        btnOpenRaise.gameObject.SetActive(!flag);

        uiSlider.value = 0;
        ReturnScrollValue(0);
        if (isMine && !isBot)
            SetInstaButtons ();
        //add instant buttons here
    }
    #endregion

    public void S_FoldAction()
    {
        _myParasitePlayer.SendFold();
    }

    //RPC
    public void RPC_Fold()
    {
        //Debug.LogError("Bajingan Fold");
        SoundManager.instance.PlaySFX(SFXType.SFX_PokerFold, Vector3.zero);

        isFolded = true;
        alreadyBet = true;

        DoneAction();

        RANK = 10;

        txtRole.text = "Fold";
        panelState.color = foldColor;


        hero.FoldAction ();
        //avater3D.ChangeSkinColor(Color.gray);
        //avater3D.PlayAvaterExpression((int)AvaterAnimation.SitFold);
    }

    void SkipNoMoney()
    {
        DoneAction();
    }

    void DoBet(long val) //All Client Running this
    {
        alreadyBet = true;
        //Debug.Log(gameObject.name + " Bet >>>> " +val);

        //Debug.Log("Val " +val + " __ chips " +chipsBet +" __totalBet" +totalBet);

        chipsBet += val;
        totalBet += val;
        myMoney -= val;
        if (myMoney < 0)
            myMoney = 0;

        
        if (isMine && !isBot)
        Logger.E ("my money in do bet: " + myMoney);

        if (_PokerGameManager.lastBet <= chipsBet)
            _PokerGameManager.lastBet = chipsBet;

        if (val != 0)
            ShowChips(val);

        UpdateUIPlayer();
    }

    void UI_allin(bool flag)
    {
        isAllIn = flag;

        allINobj.SetActive(flag);
        panelState.gameObject.SetActive(!flag);
    }

    void UpdateUIPlayer()
    {
        panelBet.SetActive(totalBet <= 0 ? false : true);
        txtBet.text = totalBet.toFlexibleCurrency();
        txtMyMoney.text = myMoney.toFlexibleCurrency();

        if (isMine || isBot)
            PhotonUtility.SetPlayerPropertiesArray(myPlayer, new string[] { PhotonEnums.Player.Money, PhotonEnums.Player.TotalBet, PhotonEnums.Player.ChipsBet }, new object[] { myMoney, totalBet, chipsBet });
    }

    IEnumerator _UpdateMyUIPlayer (long chips )
    {
        yield return _WFSUtility.wfs1;
        long prevMoney = myMoney - chips;
        trPanelMoney.SetAsLastSibling ();

        float originalX = trPanelMoney.sizeDelta.x;
        float originalY = trPanelMoney.sizeDelta.y;
        trPanelMoney.sizeDelta = new Vector2 (originalX + 500f, originalY);
        LeanTween.scale (trPanelMoney.gameObject, new Vector3 (1.2f, 1.2f, 1.2f), 0.25f).setEaseOutBounce ();
        long currentMoney = prevMoney;
        long counter = 10;
        if (GlobalVariables.bIsCoins)
        {
            currentMoney *= 1000;
            chips *= 1000;
        }
        long betPer = chips / counter;
        while (counter != 0)
        {
            currentMoney += betPer;
            txtMyMoney.text = currentMoney.ToString ("N0");

            counter--;
            yield return _WFSUtility.wfs003;
        }
        LeanTween.scale (trPanelMoney.gameObject, Vector3.one, 0.25f).setEaseOutBounce ();
        trPanelMoney.sizeDelta = new Vector2 (originalX, originalY);
        trPanelMoney.SetSiblingIndex (1);
        if (myMoney <= 0)
            myMoney = 0;

        txtMyMoney.text = myMoney.toFlexibleCurrency ();

        PhotonUtility.SetPlayerProperties(myPlayer, PhotonEnums.Player.Money, myMoney);
    }


    #region Initialize
    public void CleanWithHolyWater()
    {
        UI_allin(false);

        ClearPreAction();
        panelPreAction.SetActive(false);
        if (imgHandRank != null)
            imgHandRank.gameObject.SetActive (false);
        panelState.gameObject.SetActive(false);
        panelBet.SetActive(false);
        panelTxtRankHand.SetActive(false);

        isFolded = false;
        alreadyBet = false;

        chipsBet = 0;
        totalBet = 0;
        txtRole.text = "";
        panelState.color = Color.white;
        txtBet.text = "";
        txtHandRank.text = "";

        myCardBack.Hide();
        objCD.SetActive(false);
        DeactiveChips();

        _myHandCard[0].gameObject.SetActive(false);
        _myHandCard[1].gameObject.SetActive(false);

        _myHandCard[0].objFxCard.gameObject.SetActive(false);
        _myHandCard[1].objFxCard.gameObject.SetActive(false);

        panelAction.SetActive(false);
        panelPreAction.SetActive(false);

        myPlayer = null;
        _myParasitePlayer = null;
        isMine = false;
        isBot = false;
        slotIndex = -1;
        RANK = 10;
        heroUsed = 0;
        myMoney = 0;
        forceFold = false;

        gameObject.SetActive(false);
    }

    public void StartInitialize(int _slot, bool _flagMine, bool _isBot = false)
    {
        forceFold = false;

        isBot = _isBot;
        slotIndex = _slot;
        isMine = _flagMine;
        totalBet = 0;

        //ActivateInviteButton(false);
        chatItem.SetActive(false);

        foreach(Image i in imgBetAmount)
            i.sprite = PokerManager.instance.sprCoin;

        ExitGames.Client.Photon.Hashtable properties = myPlayer.CustomProperties;
        #region change name & picture & Avatar 3D
        _myName = (string) properties[PhotonEnums.Player.Name];
        myMoney = (long)properties[PhotonEnums.Player.Money];

        txtName.text = _myName;
        txtMyMoney.text = myMoney.toFlexibleCurrency();
        
        //Load Avatar 3D
        if (isMine && !isBot)
            //avatarEquiped = DataManager.instance.hero.id;
            heroUsed = PlayerData.costume_id;
        else
            heroUsed = PhotonUtility.GetPlayerProperties<int> (myPlayer, PhotonEnums.Player.ContentURL);

        //hero.LoadSpine (heroUsed, isMine && !isBot);
        hero.LoadFromBundle (heroUsed, isMine && !isBot);
        #endregion

        _playerHandRank = (HandRankPoker)((int)properties[PhotonEnums.Player.HandRank]);
        RANK = (int)properties[PhotonEnums.Player.RankPoker];

        valueHand = (int)properties[PhotonEnums.Player.ValueHand];
        secondValueHand = (int)properties[PhotonEnums.Player.SecondValueHand];

        _kicker = (int)properties[PhotonEnums.Player.Kicker];
        secondKicker = (int)properties[PhotonEnums.Player.SecondKicker];

        totalBet = (long)properties[PhotonEnums.Player.TotalBet];
        chipsBet = (long)properties[PhotonEnums.Player.ChipsBet];

        string strCard = (string)properties[PhotonEnums.Player.Cards];
        if (strCard != "")
        {
            string[] cardTypes = strCard.Split(',');
            this.cardTypes = cardTypes;
            InstallHandCard(int.Parse(cardTypes[0]), int.Parse(cardTypes[1]));
        }

        if (isBot)
            if (PhotonTexasPokerManager.instance.botCount == 0)
                PhotonTexasPokerManager.instance.SyncBot();

        UpdateUIPlayer();
    }
    #endregion

    #region Remote
    void RemoteActionbyMaster()
    {
        int randomRange = UnityEngine.Random.Range(1, RANK * 10);

        if (RANK == 1)
            randomRange = 0;

        if (randomRange < 6) //randomRange % RANK == 0
        {
            if (NotEnufMoney())
                S_AllIn();
            else if (NotEnufRaise())
                CallAny();
            else
            {
                int r = UnityEngine.Random.Range(1, 10);
                if (r < 5)
                    RemoteRaise();
                else
                    CallAny();
            }
        }
        else
        {
            if (_PokerGameManager.turnManager.phaseTurn < 3 && _PokerGameManager.lastBet <= _PokerGameManager.startBet*4)
                CallAny();
            else
                CheckOnly();
        }
    }

    void RemoteRaise()
    {
        lastSliderBet = sliderTools.CalculateScore(_PokerGameManager.biggestBet, _PokerGameManager.lastBet, UnityEngine.Random.Range(0, 1f));
        S_Raise();
    }

    void CallAny()
    {
        S_CheckCall();
    }

    void CheckOnly()
    {
        if (IsChipsCheck(_PokerGameManager.lastBet))
            S_CheckCall();
        else
            AlwaysFold();
    }

    void AlwaysFold()
    {
        S_FoldAction();
    }
    #endregion

    //void ActivateInviteButton(bool flag)
    //{
    //    btnInvite.SetActive(flag);
    //}

    public void ShowMyCard(bool flag)
    {
        if (flag)
        {
            myCardBack.Hide();
            for (int x = 0; x < _myHandCard.Length; x++)
                _myHandCard[x].gameObject.SetActive(true);

            if (imgHandRank == null)
            {
                panelTxtRankHand.SetActive (true);
                txtHandRank.text = _playerHandRank.ToString ();
            }
            else
            {
                imgHandRank.gameObject.SetActive (true);
                switch (_playerHandRank)
                {
                    case HandRankPoker.highCard:
                        imgHandRank.sprite = PokerManager.instance.sprHighCard;
                        break;
                    case HandRankPoker.onePair:
                        imgHandRank.sprite = PokerManager.instance.sprOnePair;
                        break;
                    case HandRankPoker.twoPairs:
                        imgHandRank.sprite = PokerManager.instance.sprTwoPair;
                        break;
                    case HandRankPoker.threeOfaKind:
                        imgHandRank.sprite = PokerManager.instance.sprThrice;
                        break;
                    case HandRankPoker.fourOfAKind:
                        imgHandRank.sprite = PokerManager.instance.sprQuad;
                        break;
                    case HandRankPoker.straight:
                        imgHandRank.sprite = PokerManager.instance.sprStraight;
                        break;
                    case HandRankPoker.flush:
                        imgHandRank.sprite = PokerManager.instance.sprFlush;
                        break;
                    case HandRankPoker.fullHouse:
                        imgHandRank.sprite = PokerManager.instance.sprFullHouse;
                        break;
                    case HandRankPoker.straightFlush:
                        imgHandRank.sprite = PokerManager.instance.sprStraightFlush;
                        break;
                    case HandRankPoker.royalFlush:
                        imgHandRank.sprite = PokerManager.instance.sprRoyalFlush;
                        break;
                }
            }
        }
        else
        {
            for (int x = 0; x < _myHandCard.Length; x++)
                _myHandCard[x].FlipCardDown();

            panelTxtRankHand.SetActive(false);
            if (imgHandRank != null)
                imgHandRank.gameObject.SetActive (false);
        }
    }

    public void InstallHandCard(int id1, int id2)
    {
        _myHandCard[0].RefreshCard(id1);
        _myHandCard[1].RefreshCard(id2);
    }

    public void OpenMyCard()
    {
        _myHandCard[0].gameObject.SetActive(true);
        _myHandCard[1].gameObject.SetActive(true);
    }

    public void ReceiveRoomChat(string _text)
    {
        txtChatMessage.text = _text;
        chatItem.SetActive(true);
        Invoke("StartChatDestroyTimer", 5f);
    }

    void StartChatDestroyTimer()
    {
        txtChatMessage.text = "";
        chatItem.SetActive(false);
    }

    #region Util Card and FX
    public void SetMyInfoRank()
    {
        if (!PhotonUtility.GetPlayerProperties<bool>(myPlayer, PhotonEnums.Player.Active))
            return;

        if (imgHandRank == null)
            panelTxtRankHand.SetActive (true);
        else
            imgHandRank.gameObject.SetActive (true);
        System.Collections.Generic.List<_CardActor> a = new System.Collections.Generic.List<_CardActor>();

        foreach (_CardActor ca in _PokerGameManager.instance.tableCard)
            if (ca.gameObject.activeSelf)
                a.Add(ca);

        if (a.Count >= 2)
        {
            HandRankPoker hrp = _PokerGameManager.cardManager.EvaluatPlayerHand (a.ToArray (), _myHandCard);
            if (imgHandRank == null)
                txtHandRank.text = hrp.ToString ();
            else
            {
                switch (hrp)
                {
                    case HandRankPoker.highCard:
                        imgHandRank.sprite = PokerManager.instance.sprHighCard;
                        break;
                    case HandRankPoker.onePair:
                        imgHandRank.sprite = PokerManager.instance.sprOnePair;
                        break;
                    case HandRankPoker.twoPairs:
                        imgHandRank.sprite = PokerManager.instance.sprTwoPair;
                        break;
                    case HandRankPoker.threeOfaKind:
                        imgHandRank.sprite = PokerManager.instance.sprThrice;
                        break;
                    case HandRankPoker.fourOfAKind:
                        imgHandRank.sprite = PokerManager.instance.sprQuad;
                        break;
                    case HandRankPoker.straight:
                        imgHandRank.sprite = PokerManager.instance.sprStraight;
                        break;
                    case HandRankPoker.flush:
                        imgHandRank.sprite = PokerManager.instance.sprFlush;
                        break;
                    case HandRankPoker.fullHouse:
                        imgHandRank.sprite = PokerManager.instance.sprFullHouse;
                        break;
                    case HandRankPoker.straightFlush:
                        imgHandRank.sprite = PokerManager.instance.sprStraightFlush;
                        PokerManager.instance.bStraightFlush = true;
                        break;
                    case HandRankPoker.royalFlush:
                        imgHandRank.sprite = PokerManager.instance.sprRoyalFlush;
                        PokerManager.instance.bRoyalFlush = true;
                        break;
                }
            }
        }

    }

    public void SaveMyCardProperties()
    {
        if (myPlayer == null)
            return;

        if (!isBot && !isMine)
            return;

        ExitGames.Client.Photon.Hashtable properties = myPlayer.CustomProperties;
        properties[PhotonEnums.Player.HandRank] = (int)_playerHandRank;
        properties[PhotonEnums.Player.ValueHand] = valueHand;
        properties[PhotonEnums.Player.SecondValueHand] = secondValueHand;

        properties[PhotonEnums.Player.RankPoker] = RANK;
        properties[PhotonEnums.Player.Kicker] = _kicker;
        properties[PhotonEnums.Player.SecondKicker] = secondKicker;

        myPlayer.SetCustomProperties(properties);
    }

    void ShowChips(long val)
    {
        DeactiveChips();

        myChipsBet[0].PlayDropAnimation();

        if (val > 1000)
            myChipsBet[1].PlayDropAnimation();

        if (val >= 10000)
            myChipsBet[2].PlayDropAnimation();
    }

    void DeactiveChips()
    {
        myChipsBet[0].gameObject.SetActive(false);
        myChipsBet[1].gameObject.SetActive(false);
        myChipsBet[2].gameObject.SetActive(false);
    }

    public void AnimationFlowChips(long val, bool pull)
    {
        //FlowType flowT = FlowType.Ten;

        //if (val < 1000)
        //    flowT = FlowType.Ten;
        //else if (val < 10000)
        //    flowT = FlowType.HundredAndThousand;
        //else
            //flowT = FlowType.All;

        //myChipsFlow.SetFlowType(flowT);

        if (pull)
            myChipsFlow.ShowFx(1, 0);
        else
            myChipsFlow.ShowFx(0, 1);

        DeactiveChips();
    }
    #endregion
}
