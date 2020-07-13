using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApiManager : MonoBehaviour
{
    private static ApiManager s_Instance = null;

    public static ApiManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (ApiManager)) as ApiManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an ApiManager object. \n You have to have exactly one ApiManager in the scene.");
            }
            return s_Instance;
        }
    }

    [SerializeField]
    private ApiBridge api;

    [HideInInspector]
    public bool bOtpSet;
    [HideInInspector]
    public ApiBridge.PokerPlayer[] pokerPlayers;
    [HideInInspector]
    public ApiBridge.SicboPlayer[] sicboPlayers;

    public void RErrorHandler ( ApiBridge.ResponseParam error )
    {
        if (LoginManager.instance != null)
            LoginManager.instance.btnGuest.interactable = true;
        if (error.error_code == 19)
        {
            SettingsManager.instance.Logout ();
        }
        Debug.LogError ("RErrorHandler from " + error.uri + " (Seed #" + error.seed.ToString ()
                    + ")\n(Code #" + error.error_code + ") {" + error.error_msg[0] + " || " + error.error_msg[1] + "}");
        if (Rays.Utilities.Congest.DEVELOPMENT)
            MessageManager.instance.Show (this.gameObject, "(" + error.error_code + ") " + error.error_msg[1], ButtonMode.OK, -1, "OK", "Batal", error.uri);
        else
            MessageManager.instance.Show (this.gameObject, error.error_msg[0], ButtonMode.OK);
    }

    public void GetVersion ()
    {
        int playerID = PlayerPrefs.GetInt (PrefEnum.PLAYER_ID.ToString (), 0);
        string token = PlayerPrefs.GetString (PrefEnum.TOKEN.ToString (), string.Empty);
        if (playerID != 0 && token != string.Empty)
        {
            Logger.E ("pID: " + playerID);
            Logger.E ("token: " + token);
            api.SetPlayerId (playerID);
            api.SetToken (token);
            api.GetVersion (playerID, token);
        }
        else
            api.GetVersion ();
    }

    private void RGetVersion ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Version: " + response.post_data);
        JGetVersion json = JsonUtility.FromJson<JGetVersion> (response.post_data);
        BundleManager.instance.ProcessGetVersion (json);
    }

    public void GetOtp ( int _type = 0)
    {
        api.GetOtp (_type);
    }

    private void RGetOtp ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Otp: " + response.post_data);
        JGetOtp json = JsonUtility.FromJson<JGetOtp> (response.post_data);
        //set otp
        api.SetOtp (json.otp.otp_key);
        bOtpSet = true;
    }

    public void GuestLogin ()
    {
        LoginManager.instance.btnGuest.interactable = false;
        api.UserLogin (ApiBridge.LoginType.Guest);
    }

    public void FacebookLogin (string fb_id, string fb_email, string fb_name, string fb_picture_url)
    {
        api.UserLogin (ApiBridge.LoginType.Facebook, fb_id, fb_email, fb_name, fb_picture_url);
    }

    private void RUserLogin ( ApiBridge.ResponseParam response )
    {
        LoginManager.instance.btnGuest.interactable = true;
        Logger.E ("Return User Login: " + response.post_data);
        JUserLogin json = JsonUtility.FromJson<JUserLogin> (response.post_data);
        api.SetPlayerId (json.player.player_id);
        api.SetToken (json.player.token);
        PlayerData.display_name = json.player.display_name;
        PlayerData.id = json.player.player_id;
        PlayerData.tag = json.player.player_tag;
        PlayerData.owned_coupon = Convert.ToInt64 (json.player.coupon);
        PlayerData.owned_coin = Convert.ToInt64 (json.player.coin);
        PlayerPrefs.SetInt (PrefEnum.PLAYER_ID.ToString(), json.player.player_id);
        PlayerPrefs.SetString (PrefEnum.TOKEN.ToString (), json.player.token);
        PlayerPrefs.Save ();

        GetHome ();
    }

    public void GetNews ()
    {
        api.GetNews ();
    }

    private void RGetNews ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get News: " + response.post_data);
        JGetNews json = JsonUtility.FromJson<JGetNews> (response.post_data);
    }

    public void GetHome ()
    {
        api.GetHome ();
    }

    private void RGetHome ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Home: " + response.post_data);
        JGetHome json = JsonUtility.FromJson<JGetHome> (response.post_data);
        _SceneManager.instance.SetActiveScene (SceneType.LOGIN, false);
        PlayerData.UpdateData (json.player);
        if (json.player.costume_equiped == 0)
            _SceneManager.instance.SetActiveScene (SceneType.BEGIN, true);
        else
        {
            StartChatSession ();
            GetChat ();
            GetShop ();
            GetInbox ();
            GetLeaderboard ();
            GetFriend ();
            GetDailyTransferLimit ();
            GetWeeklyLogin ();
            MoneySlotManager.instance.json.next_in = json.player.money_slot_next_in;
            MoneySlotManager.instance.StartTimer ();
            //_SceneManager.instance.SetActiveScene (SceneType.HOME, true);
            HomeManager.instance.Show ();
            HomeManager.instance.Init ();
        }

        if (json.player.friend_gift_me.Length > 0)
            HomeManager.instance.objNotifInbox.SetActive (true);
        else
            HomeManager.instance.objNotifInbox.SetActive (false);
        if (json.player.friend_request_me.Length > 0)
            HomeManager.instance.objNotifFriend.SetActive (true);
        else
            HomeManager.instance.objNotifFriend.SetActive (false);
        if (json.player.can_claim_daily)
        {
            DailyRewardsManager.instance.btnClaim.interactable = true;
        }
        else
        {
            DailyRewardsManager.instance.btnClaim.interactable = false;
        }
    }

    //public void GetProfile (int friendID = 0)
    //{
    //    //friendID = 0 means get my profile
    //    api.GetProfile (friendID);
    //}

    //private void RGetProfile ( ApiBridge.ResponseParam response )
    //{
    //    Logger.E ("Return Get Profile: " + response.post_data);
    //    JGetProfile json = JsonUtility.FromJson<JGetProfile> (response.post_data);
    //}

    public void GetEvent (int eventID = 0)
    {
        //eventID = 0 means list of events
        api.GetEvent (eventID);
    }

    private void RGetEvent ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Event: " + response.post_data);
        JGetEvent json = JsonUtility.FromJson<JGetEvent> (response.post_data);
    }

    public void GetHero (int heroGender = 0)
    {
        //heroGender = 0 means list of all heroes
        api.GetHero (heroGender);
    }

    private void RGetHero ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Hero: " + response.post_data);
        JGetHero json = JsonUtility.FromJson<JGetHero> (response.post_data);
    }

    public void GetCostume (int heroID = 0)
    {
        //heroID = 0 means list of all costumes
        api.GetCostume (heroID);
    }

    private void RGetCostume ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Costume: " + response.post_data);
        JGetCostume json = JsonUtility.FromJson<JGetCostume> (response.post_data);
    }

    public void SetHero (int heroID)
    {
        api.SetHero (heroID);
    }

    private void RSetHero ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Set Hero: " + response.post_data);
        JSetHero json = JsonUtility.FromJson<JSetHero> (response.post_data);
        PlayerData.costume_id = json.player.costume_equiped;
        _SceneManager.instance.SetActiveScene (SceneType.BEGIN, false);
        //_SceneManager.instance.SetActiveScene (SceneType.HOME, true);
        HomeManager.instance.Show ();
        HomeManager.instance.Init ();
        GetShop ();
        GetInbox ();
        GetLeaderboard ();
        GetFriend ();
        GetDailyTransferLimit ();
        GetWeeklyLogin ();
    }

    public void SetCostume (int costumeID = 0 )
    {
        //costumeID = 0 means default costume
        api.SetCostume (costumeID);
    }

    private void RSetCostume ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Set Costume: " + response.post_data);
        JSetCostume json = JsonUtility.FromJson<JSetCostume> (response.post_data);
        PlayerData.costume_id = json.player.costume_equiped;
        //HomeManager.instance.Init ();
        //HeroManager.instance.UpdateStatusEquipped ();
    }

    public void GetShop (int itemType = 0)
    {
        api.GetShop (itemType);
        ShopManager.instance.isSettingJson = true;
        HeroManager.instance.isSettingJson = true;

    }

    private void RGetShop (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Shop: " + response.post_data);
        JGetShop json = JsonUtility.FromJson<JGetShop> (response.post_data);
        ShopManager.instance.SetJson (json);
    }

    public void BuyShop (int itemID, int paymentType, string invoice = "" )
    {
        api.BuyShop (itemID, paymentType, invoice);
    }

    private void RBuyShop (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Buy Shop: " + response.post_data);
        JBuyShop json = JsonUtility.FromJson<JBuyShop> (response.post_data);
        if (json.item.item_type_id == 1)
        {
            int itemValue = int.Parse (json.item.bonus_coin);
            ItemReceiveData data = new ItemReceiveData (json.item.item_type_id, json.item.item_id, itemValue);
            ReceiveItemManager.instance.Show (new ItemReceiveData[] { data });
        }
        PlayerData.owned_coin = long.Parse (json.player.coin);
        PlayerData.owned_coupon = long.Parse (json.player.coupon);
        _SceneManager.instance.UpdateAllCoinAndCoupon ();
        ShopManager.instance.UpdateStatus (json);
        MessageManager.instance.Show (this.gameObject, "Anda berhasil membeli " + json.item.item_name[0]);
    }

    public void GetLeaderboard (int eventTypeID = 0 )
    {
        api.GetLeaderboard (eventTypeID);
    }

    private void RGetLeaderboard (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Leaderboard: " + response.post_data);
        JGetLeaderboard json = JsonUtility.FromJson<JGetLeaderboard> (response.post_data);
        LeaderboardManager.instance.SetJson (json);
    }

    public void GetFriend (int friendID = 0, ApiBridge.FriendType friendType = ApiBridge.FriendType.FriendList)
    {
        api.GetFriend (friendID, friendType);
    }

    private void RGetFriend (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Friend: " + response.post_data);
        JGetFriend json = JsonUtility.FromJson<JGetFriend> (response.post_data);
        if (TransferManager.instance.isWaitingJSON)
        {
            if (json.friend.display_name == null)
            {
                MessageManager.instance.Show (this.gameObject, "Pemain tidak ditemukan");
                TransferManager.instance.ResetAll ();
                TransferManager.instance.ToInput ();
                return;
            }
            TransferManager.instance.SetReceiverInfo (json.friend.display_name, json.friend.display_picture);
            return;
        }
        if (FriendManager.instance.isSettingJsonFriendList)
        {
            FriendManager.instance.SetJson (json, ApiBridge.FriendType.FriendList);
            GetFriend (0, ApiBridge.FriendType.FriendRequestMe);
        } 
        else if (FriendManager.instance.isSettingJsonFriendReqMe)
        {
            FriendManager.instance.SetJson (json, ApiBridge.FriendType.FriendRequestMe);
        }
    }

    public void SendFriend (int friendID, ApiBridge.SendFriendType sendFriendType, string notes = "")
    {
        api.SendFriend (friendID, sendFriendType, notes);
    }

    private void RSendFriend (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Send Friend: " + response.post_data);
    }

    public void SearchFriend (string strSearch)
    {
        api.SearchFriend (strSearch);
    }

    private void RSearchFriend (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Search Friend: " + response.post_data);
        JSearchFriend json = JsonUtility.FromJson<JSearchFriend> (response.post_data);
        FriendManager.instance.ShowSearchResult (json.friend);
    }

    public void SetStatus (string strStatus )
    {
        api.SetStatus (strStatus);
    }

    private void RSetStatus (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Set Status: " + response.post_data);
        JSetStatus json = JsonUtility.FromJson<JSetStatus> (response.post_data);
        PlayerData.jHome = json.player;
        PlayerData.status_message = json.player.status_message;
    }

    public void SetHeroFeatured (int[] heroIDs )
    {
        api.SetHeroFeatured (heroIDs);
    }

    private void RSetHeroFeatured (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Set Hero Featured: " + response.post_data);
        JSetHeroFeatured json = JsonUtility.FromJson<JSetHeroFeatured> (response.post_data);
        PlayerData.jHome = json.player;
        PlayerData.UpdateData (json.player);
    }

    public void GetDailyTransferLimit ()
    {
        api.SendCoin ();
    }

    public void SendCoin (string playerTag, long coinAmount )
    {
        api.SendCoin (playerTag, coinAmount);
    }

    private void RSendCoin (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Send Coin: " + response.post_data);
        JSendCoin json = JsonUtility.FromJson<JSendCoin> (response.post_data);
        long lCoin = long.Parse (json.transfer.coin_limit) * 1000;
        TransferManager.instance.txtCoinLimitAmount.text = lCoin.ToString ("N0");
        if (TransferManager.instance.isInit)
        {
            MessageManager.instance.Show (this.gameObject, "Proses transfer telah berhasil");
            _SceneManager.instance.SetActiveScene (SceneType.TRANSFER, false);
            PlayerData.owned_coin = Convert.ToInt64 (json.player.coin);
            PlayerData.owned_coupon = Convert.ToInt64 (json.player.coupon);
            _SceneManager.instance.UpdateAllCoinAndCoupon ();
        }
    }

    public void ClaimPromoCode (string strCode )
    {
        api.ClaimPromoCode (strCode);
    }

    private void RClaimPromoCode (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Claim Promo Code: " + response.post_data);
        JClaimPromoCode json = JsonUtility.FromJson<JClaimPromoCode> (response.post_data);
        if (json.promo.item_type == 1 || json.promo.item_type == 2)
        {
            int itemValue = int.Parse (json.promo.item_amount);
            ItemReceiveData data = new ItemReceiveData (json.promo.item_type, json.promo.item_id, itemValue);
            ReceiveItemManager.instance.Show (new ItemReceiveData[] { data });
            PlayerData.owned_coin = Convert.ToInt64 (json.player.coin);
            PlayerData.owned_coupon = Convert.ToInt64 (json.player.coupon);
            _SceneManager.instance.UpdateAllCoinAndCoupon ();
        }
        RedeemManager.instance.ipfCode.text = string.Empty;
    }

    public void GetInbox (bool isSender = false )
    {
        api.GetInbox (isSender);
        InboxManager.instance.isSettingJSON = true;
    }

    private void RGetInbox (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Inbox: " + response.post_data);
        JGetInbox json = JsonUtility.FromJson<JGetInbox> (response.post_data);
        InboxManager.instance.SetJson (json);
    }

    public void ReadInbox (int mailID)
    {
        api.ReadInbox (mailID);
    }

    private void RReadInbox (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Read Inbox: " + response.post_data);
    }

    public void ClaimInbox (int mailID)
    {
        api.ClaimInbox (mailID);
    }

    private void RClaimInbox (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Claim Inbox: " + response.post_data);
        JClaimInbox json = JsonUtility.FromJson<JClaimInbox> (response.post_data);
        int itemValue = int.Parse (json.inbox.item_amount);
        ItemReceiveData data = new ItemReceiveData (json.inbox.item_type_id, json.inbox.item_id, itemValue);
        ReceiveItemManager.instance.itemsData.Add (data);
        if (InboxManager.instance.countClaim == 0)
        {
            ReceiveItemManager.instance.ShowCombined ();
            PlayerData.owned_coin = Convert.ToInt64 (json.player.coin);
            PlayerData.owned_coupon = Convert.ToInt64 (json.player.coupon);
            _SceneManager.instance.UpdateAllCoinAndCoupon ();
        }
        else
        {
            InboxManager.instance.countClaim--;
        }
    }

    public void HideInbox (int mailID )
    {
        api.HideInbox (mailID);
    }

    private void RHideInbox (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Hide Inbox: " + response.post_data);
    }

    public void GetDailyLogin ()
    {
        api.GetDailyLogin ();
    }

    private void RGetDailyLogin (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Daily Login: " + response.post_data);
        DailyRewardsManager.instance.btnClaim.interactable = false;
        JGetDailyLogin json = JsonUtility.FromJson<JGetDailyLogin> (response.post_data);
        int itemValue = int.Parse (json.reward.item_amount);
        ItemReceiveData data = new ItemReceiveData (json.reward.item_type, json.reward.item_id, itemValue);
        ReceiveItemManager.instance.Show (new ItemReceiveData[] { data });
        PlayerData.jHome.can_claim_daily = false;
        PlayerData.owned_coin = Convert.ToInt64 (json.player.coin);
        PlayerData.owned_coupon = Convert.ToInt64 (json.player.coupon);
        _SceneManager.instance.UpdateAllCoinAndCoupon ();
    }

    public void GetWeeklyLogin (bool isClaim = false)
    {
        if (!isClaim)
            WeeklyRewardsManager.instance.isSettingJson = true;
        api.GetWeeklyLogin (isClaim);
    }

    private void RGetWeeklyLogin (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return GetWeeklyLogin: " + response.post_data);
        JGetWeeklyLogin json = JsonUtility.FromJson<JGetWeeklyLogin> (response.post_data);

        if (json.reward.item_amount == null)
        {
            WeeklyRewardsManager.instance.SetJson (json);
        }
        else
        {
            int itemValue = int.Parse (json.reward.item_amount);
            ItemReceiveData data = new ItemReceiveData (json.reward.item_type, json.reward.item_id, itemValue);
            ReceiveItemManager.instance.itemsData.Add (data);
            if (json.bonus_reward.item_amount != null)
            {
                int bonusItemValue = int.Parse (json.bonus_reward.item_amount);
                ItemReceiveData bonusData = new ItemReceiveData (json.bonus_reward.item_type, json.bonus_reward.item_id, bonusItemValue);
                ReceiveItemManager.instance.itemsData.Add (bonusData);
            }
            ReceiveItemManager.instance.ShowCombined ();
            PlayerData.jHome.can_claim_weekly = false;
            PlayerData.owned_coin = Convert.ToInt64 (json.player.coin);
            PlayerData.owned_coupon = Convert.ToInt64 (json.player.coupon);
            _SceneManager.instance.UpdateAllCoinAndCoupon ();
            WeeklyRewardsManager.instance.SetWeeklyDaysStatus (json);
            WeeklyRewardsManager.instance.IncrementProgressBar ();
            WeeklyRewardsManager.instance.json.login_count++;
        }
    }

    public void GetMoneySlot ()
    {
        api.GetMoneySlot ();
    }

    private void RGetMoneySlot (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Money Slot: " + response.post_data);
        JGetMoneySlot json = JsonUtility.FromJson<JGetMoneySlot> (response.post_data);
        MoneySlotManager.instance.SetJson (json);
    }

    public void UpdateAnnouncementSession (ApiBridge.ResponseParam response )
    {
        Logger.E ("Update Announcement Session: " + response.post_data);
        JUpdateAnnouncement json = JsonUtility.FromJson<JUpdateAnnouncement> (response.post_data);
        HomeManager.instance.runningText.AddQueue (json.announcement.announcement_text);
    }

    public void GetChat (int friendID = -1)
    {
        api.GetChat (friendID);
    }

    private void RGetChatPublic (ApiBridge.ResponseParam response )
    {
        //when friend ID = -1, get public chat content
        Logger.E ("Return Get Chat Public: " + response.post_data);
        JGetChatPublic json = JsonUtility.FromJson<JGetChatPublic> (response.post_data);
        HomeManager.instance.SetJson (json);
    }

    private void RGetChatList (ApiBridge.ResponseParam response )
    {
        //when friend ID = 0, get private chat list
        Logger.E ("Return Get Chat List: " + response.post_data);
    }

    private void RGetChatPrivate (ApiBridge.ResponseParam response )
    {
        //when friend ID else, get private chat content with [friendID]
        Logger.E ("Return Get Chat Private: " + response.post_data);
    }

    public void StartChatSession (int friendID = 0)
    {
        api.StartChatSession (friendID);
        Logger.E ("start chat session");
    }

    public void EndChatSession ()
    {
        api.EndChatSession ();
    }

    public void UpdatePublicChatSession (ApiBridge.ResponseParam response )
    {
        Logger.E ("Update Public Chat Session: " + response.post_data);
        if (HomeManager.instance.json != null)
        {
            JGetChatPublic json = JsonUtility.FromJson<JGetChatPublic> (response.post_data);
            List<JPublicChat> updateChat = new List<JPublicChat> ();
            int lastChatID = HomeManager.instance.GetLastPublicChatID ();
            for (int i = 0; i < json.chat.Length; i++)
            {
                if (json.chat[i].chat_public_id > lastChatID)
                    updateChat.Add (json.chat[i]);
            }
            HomeManager.instance.AddPublicChat (updateChat.ToArray());
        }

    }

    public void UpdatePrivateChatSession (ApiBridge.ResponseParam response )
    {
        Logger.E ("Update Private Chat Session: " + response.post_data);
    }

    public void SendChat (string strContent, int friendID = 0 )
    {
        api.SendChat (strContent, friendID);
    }

    private void RSendChatPublic (ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Send Chat Public: " + response.post_data);

    }

    #region gameplay
    public void StartPoker (string _photonRoomID, long _roomBetCoin)
    {
        Logger.E ("room: " + _photonRoomID);
        Logger.E ("bet: " + _roomBetCoin);
        Logger.E ("playerIDs: " + pokerPlayers[0].player_id + " | " + pokerPlayers[1].player_id);
        api.StartPoker (_photonRoomID, _roomBetCoin, pokerPlayers);
    }

    private void RStartPoker ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Start Poker: " + response.post_data);
        SetPokerData (response.post_data);
        PhotonTexasPokerManager.instance.SetOthersPokerData (response.post_data);
    }

    public void SetPokerData (string jStartPoker )
    {
        //called by every client
        JStartPoker json = JsonUtility.FromJson<JStartPoker> (jStartPoker);
        PokerData.Setup (json.poker.poker_round_id, json.poker.room_bet_coin, json.poker.cards, json.poker.otp);
        api.SetOtp (json.poker.otp);

        for (int i = 0; i < json.poker.players.Length; i++)
        {
            if (json.poker.players[i].player_id == PlayerData.id)
            {
                if (json.poker.players[i].kick)
                    PhotonTexasPokerManager.instance.KickFromServer ();
                else
                {
                    long lCoin = Convert.ToInt64 (json.poker.players[i].coin_server);
                    if (lCoin <= GlobalVariables.MaxBuyIn)
                        PhotonTexasPokerManager.instance.SyncCoinFromServer (lCoin);
                }
            }
        }

        //sync poker players among master and non masters
        ApiBridge.PokerPlayer[] serverPlayers = new ApiBridge.PokerPlayer[8];
        for (int a = 0; a < 8; a++)
        {
            serverPlayers[a] = new ApiBridge.PokerPlayer ();
        }
        for (int x = 0; x < json.poker.players.Length; x++)
        {
            serverPlayers[x] = new ApiBridge.PokerPlayer ();
            long coinPlayer = Convert.ToInt64 (json.poker.players[x].coin_before);
            serverPlayers[x].Start (json.poker.players[x].seater_id, json.poker.players[x].player_id, coinPlayer);
            long coinServer = Convert.ToInt64 (json.poker.players[x].coin_server);
            serverPlayers[x].Update (coinServer, json.poker.players[x].kick);
        }
        pokerPlayers = serverPlayers;
    }

    //private void UpdatePlayerPokers (string )

    public void EndPoker (int _roundID)
    {
        api.EndPoker (_roundID, pokerPlayers);
    }

    private void REndPoker ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return End Poker: " + response.post_data);
        //UPDATE COIN AND COUPON(?)
    }

    public void StartSicbo ()
    {
        //sicboPlayers = new ApiBridge.SicboPlayer[1];
        //sicboPlayers[0] = new ApiBridge.SicboPlayer (1016, ApiBridge.SicboBetType.Small, 100);
        //api.StartSicbo (sicboPlayers);
        if (SicboManager.instance.apiPlayers != null)
        {
            api.StartSicbo (SicboManager.instance.apiPlayers);
        }
        else
            api.StartSicbo (new ApiBridge.SicboPlayer[] { });
    }

    private void RStartSicbo ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Start Sicbo: " + response.post_data);
        SicboManager.instance.SendRStartSicbo (response.post_data);
    }
    #endregion
}
