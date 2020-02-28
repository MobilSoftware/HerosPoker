using UnityEngine;

public class _ParasitePoker : Photon.MonoBehaviour
{
    public _PlayerPokerActor uiPlayerPoker;
    public string nickname;
    bool isMine; //Ga terlalu guna

    public bool isBot;
    public int IDX_READONLY;

    public void CleanWithHolyWater()
    {
        transform.SetParent(_PokerGameManager.instance.transform);
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(0, 0, 0);

        uiPlayerPoker = null;
        isBot = false;
        isMine = false;
    }

    /// <summary>
    /// If New Player == NULL, Initialize for all player.
    /// </summary>
    /// <param name="_isBot"></param>
    /// <param name="newPlayer"></param>
    /// <param name="_local"></param>
    public void CallInitialize(bool _isBot)
    {
        photonView.RPC("RPC_StartInitialize", PhotonTargets.AllViaServer, _isBot);
    }

    public void CallCatchupInitialize(PhotonPlayer newPlayer)
    {
        photonView.RPC("RPC_InitializeCatchup", newPlayer, uiPlayerPoker.alreadyBet, uiPlayerPoker.isFolded, uiPlayerPoker.isAllIn);
    }

    public void BotCatchUp(string _botID, ExitGames.Client.Photon.Hashtable _botProperties, PhotonPlayer newPlayer)
    {
        photonView.RPC("RPC_BotCatchup", newPlayer, _botID, _botProperties, uiPlayerPoker.alreadyBet, uiPlayerPoker.isFolded, uiPlayerPoker.isAllIn);
    }

    [PunRPC]
    void RPC_BotCatchup(string _botID, ExitGames.Client.Photon.Hashtable _botProperties, bool _alreadyBet, bool _isFold, bool _isAllIn)
    {
        foreach (PhotonPlayer bot in PhotonTexasPokerManager.instance.bots)
            if (bot.NickName == _botID)
                return;

        PhotonTexasPokerManager.instance.AddNewBotToArray(_botID, _botProperties);

        RPC_StartInitialize(true);
        uiPlayerPoker.alreadyBet = _alreadyBet;
        uiPlayerPoker.isFolded = _isFold;
        uiPlayerPoker.isAllIn = _isAllIn;
    }

    [PunRPC]
    void RPC_InitializeCatchup(bool _alreadyBet, bool _isFold, bool _isAllIn)
    {
        RPC_StartInitialize(false);
        uiPlayerPoker.alreadyBet = _alreadyBet;
        uiPlayerPoker.isFolded = _isFold;
        uiPlayerPoker.isAllIn = _isAllIn;
    }

    [PunRPC]
    private void RPC_StartInitialize(bool _isBot)
    {
        PhotonPlayer botPhoton = null;
        isMine = photonView.isMine;
        isBot = _isBot;

        if (isBot)
            botPhoton = PhotonTexasPokerManager.instance.GetBotwithIndex(IDX_READONLY);// bots[PhotonTexasPokerManager.instance.bots.Count - 1];

        if (botPhoton == null && isBot)
            Debug.Log("GA DAPET BOT PHOTON BOS");

        int ownerSlotIndex = PhotonUtility.GetPlayerProperties<int>(isBot ? botPhoton : this.photonView.owner, PhotonEnums.Player.SlotIndex);

        uiPlayerPoker = _PokerGameManager.instance.stockPlayers[ownerSlotIndex];
        uiPlayerPoker.gameObject.SetActive(true);
        uiPlayerPoker._myParasitePlayer = this;
        uiPlayerPoker.myPlayer = isBot ? botPhoton : this.photonView.owner;
        nickname = isBot ? botPhoton.NickName : this.photonView.owner.NickName;

        transform.SetParent(uiPlayerPoker.transform);
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(0, 0, 0);

        uiPlayerPoker.StartInitialize(ownerSlotIndex, isMine, isBot);

        if (isMine)
        {
            PhotonUtility.SetPlayerPropertiesArray(uiPlayerPoker.myPlayer, new string[] { PhotonEnums.Player.ReadyInitialized, PhotonEnums.Player.NextRoundIn }, new object[] { true, true });
        }
    }

    [PunRPC]
    void RPC_ForceSyncBotMoney(long _money)
    {
        if (uiPlayerPoker.myPlayer != null)
        {
            PhotonUtility.SetPlayerProperties (uiPlayerPoker.myPlayer, PhotonEnums.Player.Money, _money);
            uiPlayerPoker.SyncMoney ();
        }
    }

    #region Pindahan RPC dari ACS
    public void SendAvaterExpression(int expres)
    {
        photonView.RPC("RPC_ReceiveAvaterExpression", PhotonTargets.AllViaServer, expres);
    }

    [PunRPC]
    void RPC_ReceiveAvaterExpression(int expres)
    {
        //uiPlayerPoker.avater3D.PlayAvaterExpression(expres);
    }

    public void SendChat(string text)
    {
        photonView.RPC("RPC_SendRoomChat", PhotonTargets.AllViaServer, text);
    }

    [PunRPC]
    void RPC_SendRoomChat(string _text)
    {
        uiPlayerPoker.ReceiveRoomChat(_text);
    }

    public void SendThrowItem(int idxItem, int to)
    {
        photonView.RPC("RPC_SendThrowItem", PhotonTargets.AllViaServer, idxItem, to);
    }

    [PunRPC]
    void RPC_SendThrowItem(int _idxItem, int to)
    {
        _PokerGameHUD.instance.boxThrow.ThrowTheItemPoker(_idxItem, IDX_READONLY, to);
    }

    public void SendAllIn(long val, long _bet, long _lastbet)
    {
        photonView.RPC("RPC_SendAllIn", PhotonTargets.AllViaServer, val, _bet, _lastbet);
    }

    [PunRPC]
    void RPC_SendAllIn(long val, long _bet, long _lastbet)
    {
        uiPlayerPoker.RPC_AllIn(val, _bet, _lastbet);
    }

    public void SendCheckCall(long val, long _bet, long _lastbet)
    {
        photonView.RPC("RPC_SendCheckCall", PhotonTargets.AllViaServer, val, _bet, _lastbet);
    }

    [PunRPC]
    void RPC_SendCheckCall(long val, long _bet, long _lastbet)
    {
        if (uiPlayerPoker != null)
            uiPlayerPoker.RPC_CheckCall(val, _bet, _lastbet);
    }

    public void SendRaise(long val, long _bet, long _lastbet)
    {
        photonView.RPC("RPC_SendRaise", PhotonTargets.AllViaServer, val, _bet, _lastbet);
    }

    [PunRPC]
    void RPC_SendRaise(long val, long _bet, long _lastbet)
    {
        if (uiPlayerPoker != null)
            uiPlayerPoker.RPC_Raise(val, _bet, _lastbet);
    }

    public void SendFold()
    {
        photonView.RPC("RPC_SendFold", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    void RPC_SendFold()
    {
        if (uiPlayerPoker != null)
            uiPlayerPoker.RPC_Fold();
    }
    #endregion
}
