using UnityEngine;
using Rays.Utilities;
using System;

public class ApiBridge : MonoBehaviour
{
    public class API
    {
        public int seed;
        public string uri;
        public string rFuncName;
        public string jsonData;
        public int isDigested; //-1. Temp File, 0. False, 1. True
        public string otp;
        public int cacheTime;
        public string jsonReturn;
        public int errorCode;
        public string[] errorMsg;

        public API ( string _uri, string _rFuncName, string _jsonData = "", int _isDigested = 0, string _otp = "" )
        {
            seed = UnityEngine.Random.Range (1, int.MaxValue);
            uri = _uri;
            rFuncName = _rFuncName;
            jsonData = _jsonData;
            isDigested = _isDigested;
            otp = _otp;
            cacheTime = 0;
        }

        public API ( string _uri, int _errorCode )
        {
            uri = _uri;
            errorCode = _errorCode;
        }
    }

    public delegate void ResponseDelegate ( string inputJson, string uri );
    public delegate void ErrorDelegate ( int errorCode, string[] errorMsg, string errorUri );

    public class PokerPlayer
    {
        public int seater_id;
        public int player_id;
        public long coin_before;
        public long coin_bet;
        public long coin_won;
        public long coin_after;
        public long coin_server;
        public bool kick;

        public void Start ( int _seater_id, int _player_id, long _coin_before )
        {
            seater_id = _seater_id;
            player_id = _player_id;
            coin_before = _coin_before;
        }

        public void Update ( long _coin_server, bool _kick )
        {
            coin_server = _coin_server;
            kick = _kick;
        }

        public void End ( long _coin_bet, long _coin_won, long _coin_after )
        {
            coin_bet = _coin_bet;
            coin_won = _coin_won;
            coin_after = _coin_after;
        }

        public string ToJson ()
        {
            return JsonUtility.ToJson (this);
        }
    }

    public class SicboPlayer
    {
        public int player_id;
        public int sicbo_type;
        public long coin_bet;

        public SicboPlayer ( int _player_id, SicboBetType _sicbo_type, long _coin_bet )
        {
            player_id = _player_id;
            sicbo_type = (int) _sicbo_type;
            coin_bet = _coin_bet;
        }

        public string ToJson ()
        {
            return JsonUtility.ToJson (this);
        }
    }

    public enum LoginType
    {
        None, Guest, Google, Facebook
    }

    public enum ScoringType
    {
        None,
        Dragon_King, Dragon, All_Royal, Super_Straight_Flush, Triple_Quartet,
        All_Big, All_Small, Single_Color, Four_Thrice, Six_Pair,
        Super_Straight, Super_Flush, Royal_Flush_Middle, Royal_Flush_Bottom, Straight_Flush_Middle,
        Straight_Flush_Bottom, Four_Of_Kind_Middle, Four_Of_Kind_Bottom, Full_House_Middle, Full_House_Bottom,
        Flush_Middle, Flush_Bottom, Straight_Middle, Straight_Bottom, Three_Of_Kind_Top,
        Three_Of_Kind_Middle, Three_Of_Kind_Bottom, Two_Pair, One_Pair, High_Card
    }

    public enum SicboBetType
    {
        None,
        Single1, Single2, Single3, Single4, Single5, Single6,
        Double1, Double2, Double3, Double4, Double5, Double6,
        Triple1, Triple2, Triple3, Triple4, Triple5, Triple6, TripleAny,
        Dadu4, Dadu5, Dadu6, Dadu7, Dadu8, Dadu9, Dadu10, Dadu11, Dadu12, Dadu13, Dadu14, Dadu15, Dadu16, Dadu17,
        Couple12, Couple13, Couple14, Couple15, Couple16, Couple23, Couple24, Couple25, Couple26,
        Couple34, Couple35, Couple36, Couple45, Couple46, Couple56,
        Small, Big
    }

    public enum PaymentType
    {
        None, Telkomsel, Tri, Indosat, XL,
        Gopay, Dana, Unipin
    }

    public enum FriendType
    {
        FriendList, FriendRequest, FriendRequestMe, FriendBlock, FriendBlockMe, FriendChatMe, FriendGiftMe
    }

    public enum SendFriendType
    {
        SendFriendRemove, SendFriendRequest, ResponseFriendRequestMeYes, ResponseFriendRequestMeNo,
        SendFriendBlock, SendFriendUnBlock, SendFriendReport
    }

    [Serializable]
    public class ResponseParam
    {
        public int error_code = 0;
        public string[] error_msg = { "", "" };
        public string uri = "";
        public int seed = 0;
        public string post_id = "";
        public string post_data = "";
        public string post_time = "";
        public string request_time = "";
    }

    public void GoodResponse ( API api )
    {
        ResponseParam response = ParseResponse (api);
        if (response != null)
        {
            response.uri = api.uri;
            response.seed = api.seed;
            response.post_id = api.jsonReturn;
            SendMessage (api.rFuncName, response);
        }
    }


    [Serializable]
    private class TokenParam
    {
        public int player_id;
        public string token;
    }

    [Serializable]
    private class DeviceParam
    {
        public string device_id;
        public int device_type;
        public string device_detail;
        public string apk_ver;
    }



    private int apiPlayerId = 0;
    private string apiToken = "";
    private string apiOtp = "";



    //Get Version
    [Serializable]
    private class GetVersionParam : DeviceParam
    {
        public int player_id;
        public string token;
    }

    public int GetVersion ( int playerId = 0, string token = "" )
    {
        if (playerId == 0 && apiPlayerId > 0)
        {
            playerId = apiPlayerId;
            token = apiToken;
        }
        GetVersionParam param = new GetVersionParam ();
        param.player_id = playerId;
        param.token = token;
        param.device_id = SystemInfo.deviceUniqueIdentifier;
        param.device_type = (int) Application.platform;
        param.device_detail = "(" + SystemInfo.operatingSystem + ") " + SystemInfo.deviceModel + " (" + SystemInfo.deviceName + ")";
        param.apk_ver = Application.version + (Congest.DEVELOPMENT ? "d" : "");
        string paramJson = JsonUtility.ToJson (param);
        API api = new API ("getversion.php", "RGetVersion", paramJson, 1);
        param = null; paramJson = "";
        StartCoroutine (Congest.SendPOST (this, api));
        return api.seed;
    }


    //Get OTP
    [Serializable]
    private class GetOtpParam
    {
        public string device_id;
        public int device_type;
        public int otp_type;
    }

    public int GetOtp ( int otpType = 0 )
    {
        GetOtpParam param = new GetOtpParam ();
        param.device_id = SystemInfo.deviceUniqueIdentifier;
        param.device_type = (int) Application.platform;
        param.otp_type = otpType;
        string paramJson = JsonUtility.ToJson (param);
        API api = new API ("getotp.php", "RGetOtp", paramJson, 1);
        param = null; paramJson = "";
        StartCoroutine (Congest.SendPOST (this, api));
        return api.seed;
    }



    //User Login Guest / Google / Facebook
    [Serializable]
    private class UserLoginParam : DeviceParam
    {
        public int login_type;
        public string login_id;
        public string login_email;
        public string login_name;
        public string login_picture;
    }

    public int UserLogin ( LoginType loginType, string loginId = "", string loginEmail = "", string loginName = "", string loginPicture = "", string otp = "" )
    { //1. Guest, 2. Google, 3. Facebook
        UserLoginParam param = new UserLoginParam ();
        param.login_type = (int) loginType;
        param.login_id = loginId;
        param.login_email = loginEmail;
        param.login_name = loginName;
        param.login_picture = loginPicture;
        param.device_id = SystemInfo.deviceUniqueIdentifier;
        param.device_type = (int) Application.platform;
        param.device_detail = "(" + SystemInfo.operatingSystem + ") " + SystemInfo.deviceModel + " (" + SystemInfo.deviceName + ")";
        param.apk_ver = Application.version + (Congest.DEVELOPMENT ? "d" : "");
        string paramJson = JsonUtility.ToJson (param);
        if (otp.Length > 0) apiOtp = otp;
        API api = new API ("userlogin.php", "RUserLogin", paramJson, 1, apiOtp);
        param = null; paramJson = "";
        if (apiOtp.Length > 0)
        {
            StartCoroutine (Congest.SendPOST (this, api));
        }
        else
        {
            api.errorCode = 502;
            ParseError (api);
        }
        return api.seed;
    }



    //Get News
    public int GetNews ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getnews.php", "RGetNews", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get Home
    public int GetHome ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("gethome.php", "RGetHome", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get Profile
    [Serializable]
    private class ProfileParam : TokenParam
    {
        public int friend_id;
    }

    public int GetProfile ( int friendId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ProfileParam param = new ProfileParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getprofile.php", "RGetProfile", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get Event
    [Serializable]
    private class EventParam : TokenParam
    {
        public int event_id;
    }

    public int GetEvent ( int eventId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            EventParam param = new EventParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.event_id = eventId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getevent.php", "RGetEvent", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get Hero
    [Serializable]
    private class HeroParam : TokenParam
    {
        public int hero_gender;
        public int hero_id;
        public int costume_id;
    }

    public int GetHero ( int heroGender = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            HeroParam param = new HeroParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.hero_gender = heroGender;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("gethero.php", "RGetHero", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get Costume
    public int GetCostume ( int heroId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            HeroParam param = new HeroParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.hero_id = heroId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getcostume.php", "RGetCostume", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Set Hero
    public int SetHero ( int heroId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            HeroParam param = new HeroParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.hero_id = heroId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("sethero.php", "RSetHero", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Set Costume
    public int SetCostume ( int costumeId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            HeroParam param = new HeroParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.costume_id = costumeId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("setcostume.php", "RSetCostume", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get Shop
    [Serializable]
    private class ShopParam : GetVersionParam
    {
        public int item_type;
        public int item_id;
        public string invoice_id;
    }

    public int GetShop ( int itemType = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ShopParam param = new ShopParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.item_type = itemType;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getshop.php", "RGetShop", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Buy Shop
    public int BuyShop ( int itemId, int paymentType = 0, string invoiceId = "", int playerId = 0, string token = "", string otp = "" )
    {
        if (ParseToken (playerId, token))
        {
            ShopParam param = new ShopParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.item_id = itemId;
            param.item_type = paymentType;
            param.invoice_id = invoiceId;
            param.device_id = SystemInfo.deviceUniqueIdentifier;
            param.device_type = (int) Application.platform;
            string paramJson = JsonUtility.ToJson (param);
            if (otp.Length > 0) apiOtp = otp;
            API api = new API ("buyshop.php", "RBuyShop", paramJson, 1, (param.item_type == 0 ? apiOtp : ""));
            param = null; paramJson = "";
            if (paymentType == 0 && apiOtp.Length == 0)
            {
                api.errorCode = 502;
                ParseError (api);
            }
            else
            {
                StartCoroutine (Congest.SendPOST (this, api));
            }
            return api.seed;
        }
        return 0;
    }



    //Get Friend
    [Serializable]
    private class FriendParam : TokenParam
    {
        public int friend_id;
        public int friend_type;
        public string notes;
    }

    public int GetFriend ( int friendId = 0, FriendType friendType = FriendType.FriendList, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            FriendParam param = new FriendParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            param.friend_type = (int) friendType;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getfriend.php", "RGetFriend", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Send Friend
    public int SendFriend ( int friendId, SendFriendType friendType, string notes = "", int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            FriendParam param = new FriendParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            param.friend_type = (int) friendType;
            param.notes = notes;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("sendfriend.php", "RSendFriend", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Set Phone Num
    [Serializable]
    private class SetPhoneNumParam : TokenParam
    {
        public string phone_num;
    }

    public int SetPhoneNum ( string phoneNum, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SetPhoneNumParam param = new SetPhoneNumParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.phone_num = phoneNum;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("setphonenum.php", "RSetPhoneNum", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Claim Referal Code
    [Serializable]
    private class ClaimReferalCodeParam : GetVersionParam
    {
        public string referal_code;
    }

    public int ClaimReferalCode ( string referalCode, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ClaimReferalCodeParam param = new ClaimReferalCodeParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.referal_code = referalCode;
            param.device_id = SystemInfo.deviceUniqueIdentifier;
            param.device_type = (int) Application.platform;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("claimreferalcode.php", "RClaimReferalCode", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get Leaderboard
    [Serializable]
    private class GetLeaderboardParam : TokenParam
    {
        public int leaderboard_type;
        public int checked_id;
    }

    public int GetLeaderboard ( int leaderboardType = 0, int checkedId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetLeaderboardParam param = new GetLeaderboardParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.leaderboard_type = leaderboardType;
            param.checked_id = (checkedId == 0 ? apiPlayerId : checkedId);
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getleaderboard.php", "RGetLeaderboard", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get Inbox
    [Serializable]
    private class InboxParam : TokenParam
    {
        public int as_sender;
        public int mail_id;
    }

    public int GetInbox ( bool asSender = false, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            InboxParam param = new InboxParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.as_sender = (asSender ? 1 : 0);
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getinbox.php", "RGetInbox", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Read Inbox
    public int ReadInbox ( int mailId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            InboxParam param = new InboxParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.mail_id = mailId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("readinbox.php", "RReadInbox", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Claim Inbox
    public int ClaimInbox ( int mailId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            InboxParam param = new InboxParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.mail_id = mailId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("claiminbox.php", "RClaimInbox", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get VIP Level
    public int GetVIPLevel ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getviplevel.php", "RGetVIPLevel", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Send Credit
    [Serializable]
    private class SendCoinParam : TokenParam
    {
        public string friend_tag;
        public long coin_amount;
    }

    public int SendCoin ( string friendTag, long coinAmount, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SendCoinParam param = new SendCoinParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_tag = friendTag;
            param.coin_amount = coinAmount;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("sendcoin.php", "RSendCoin", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    public int SendCoinHistory ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("sendcoinhistory.php", "RSendCoinHistory", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get Daily Login
    [Serializable]
    private class DailyLoginParam : TokenParam
    {
        public int claim;
    }

    public int GetDailyLogin ( bool claim = false, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            DailyLoginParam param = new DailyLoginParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.claim = (claim ? 1 : 0);
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getdailylogin.php", "RGetDailyLogin", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }




    //Get Invitee
    [Serializable]
    private class InviteeParam : TokenParam
    {
        public int friend_id;
        public int mission_id;
    }

    public int GetInvitee ( int friendId = -1, int missionId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            InviteeParam param = new InviteeParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            param.mission_id = missionId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getinvitee.php", "RGetInvitee", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }




    //Get Store Param
    [Serializable]
    private class GetStoreParam : TokenParam
    {
        public int item_id;
        public PaymentType payment_type;
        public int is_live;
    }

    //Get Redeem Link
    public string GetRedeemLink ( int playerId = 0, string token = "" )
    {
        string paramOtp = "";
        string paramJson = "";
        if (ParseToken (playerId, token))
        {
            GetStoreParam param = new GetStoreParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.is_live = (Congest.DEVELOPMENT ? 0 : 1);
            paramJson = JsonUtility.ToJson (param);
            paramOtp = Util.RandomChar (32);
            paramJson = Digest.Write (paramJson, paramOtp);
        }
        byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes (paramJson);
        paramJson = Convert.ToBase64String (bytesToEncode);
        return Congest.DOMAIN + "redeem/index.php?postid=" + paramOtp + "&postdata=" + paramJson;
    }




    //Get Restore Key
    public string GetRestoreKey ( string restoreSKU, string restoreInvoice )
    {
        return Digest.Write (restoreSKU, restoreInvoice);
    }




    //Send Client Log
    [Serializable]
    private class ClientLogParam
    {
        public string log;
    }

    public int SendClientLog ( string log )
    {
        ClientLogParam param = new ClientLogParam ();
        param.log = log;
        string paramJson = JsonUtility.ToJson (param);
        API api = new API ("sendclientlog.php", "RSendClientLog", paramJson, 1);
        param = null; paramJson = "";
        StartCoroutine (Congest.SendPOST (this, api));
        return api.seed;
    }




    //Get Chat
    public int GetChat ( int friendId = -1, int playerId = 0, string token = "" )
    {
        if (friendId == -1) //Get Public Chat
        {
            string defaultNotFound = "{\"chat\":[]}";
            API api = new API ("chat/chat.ray", "RGetChatPublic", "", -1);
            StartCoroutine (Congest.ReadTempFile (this, api, defaultNotFound));
            return api.seed;
        }
        else if (friendId == 0) //Get Private Chat List
        {
            if (ParseToken (playerId, token))
            {
                TokenParam param = new TokenParam ();
                param.player_id = apiPlayerId;
                param.token = apiToken;
                string paramJson = JsonUtility.ToJson (param);
                API api = new API ("getchat.php", "RGetChatList", paramJson, 1);
                param = null; paramJson = "";
                StartCoroutine (Congest.SendPOST (this, api));
                return api.seed;
            }
        }
        else //Get Private Chat
        {
            string fromto = (apiPlayerId < friendId ? Mathf.FloorToInt (apiPlayerId / 1000).ToString () : Mathf.FloorToInt (friendId / 1000).ToString ()) + "/" + (apiPlayerId < friendId ? apiPlayerId.ToString () : friendId.ToString ()) + "/" + (apiPlayerId > friendId ? apiPlayerId.ToString () : friendId.ToString ());
            string defaultNotFound = "{\"chat\":[]}";
            API api = new API ("chat/" + fromto + "/chat.ray", "RGetChatPrivate", "", -1);
            StartCoroutine (Congest.ReadTempFile (this, api, defaultNotFound));
            return api.seed;
        }
        return 0;
    }



    //Send Chat
    [Serializable]
    private class ChatParam : TokenParam
    {
        public int friend_id;
        public string chat_msg;
    }

    public int SendChat ( string chatMsg, int friendId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ChatParam param = new ChatParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.chat_msg = chatMsg;
            param.friend_id = friendId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("sendchat.php", (friendId == 0 ? "RSendChatPublic" : "RSendChatPrivate"), paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }




    //Start Poker
    [Serializable]
    private class PokerParam : TokenParam
    {
        public string room_seed;
        public long room_bet_coin;
        public string[] poker_player;
        public int event_id;
        public int poker_round_id;
    }

    public int StartPoker ( string roomSeed, long roomBetCoin, PokerPlayer[] pokerPlayer, int eventId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            PokerParam param = new PokerParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.room_seed = roomSeed;
            param.room_bet_coin = roomBetCoin;
            param.event_id = eventId;
            string[] tempString = new string[pokerPlayer.Length];
            var j = 0;
            for (var i = 0; i < pokerPlayer.Length; i++) if (pokerPlayer[i].seater_id > 0) tempString[j++] = pokerPlayer[i].ToJson ();
            param.poker_player = new string[j];
            for (var i = 0; i < j; i++) param.poker_player[i] = tempString[i];
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("startpoker.php", "RStartPoker", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //End Poker
    public int EndPoker ( int pokerId, PokerPlayer[] pokerPlayer, int playerId = 0, string token = "", string otp = "" )
    {
        if (ParseToken (playerId, token))
        {
            PokerParam param = new PokerParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.poker_round_id = pokerId;
            string[] tempString = new string[pokerPlayer.Length];
            var j = 0;
            for (var i = 0; i < pokerPlayer.Length; i++) if (pokerPlayer[i].seater_id > 0) tempString[j++] = pokerPlayer[i].ToJson ();
            param.poker_player = new string[j];
            for (var i = 0; i < j; i++) param.poker_player[i] = tempString[i];
            string paramJson = JsonUtility.ToJson (param);
            if (otp.Length > 0) apiOtp = otp;
            API api = new API ("endpoker.php", "REndPoker", paramJson, 1, apiOtp);
            param = null; paramJson = "";
            if (apiOtp.Length > 0)
            {
                StartCoroutine (Congest.SendPOST (this, api));
            }
            else
            {
                api.errorCode = 502;
                ParseError (api);
            }
            return api.seed;
        }
        return 0;
    }



    //Start Sicbo
    [Serializable]
    private class SicboParam : TokenParam
    {
        public string[] sicbo_player;
        public int event_id;
    }

    public int StartSicbo ( SicboPlayer[] sicboPlayer, int eventId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SicboParam param = new SicboParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.event_id = eventId;
            string[] tempString = new string[sicboPlayer.Length];
            var j = 0;
            for (var i = 0; i < sicboPlayer.Length; i++) if (sicboPlayer[i].coin_bet > 0) tempString[j++] = sicboPlayer[i].ToJson ();
            param.sicbo_player = new string[j];
            for (var i = 0; i < j; i++) param.sicbo_player[i] = tempString[i];
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("startsicbo.php", "RStartSicbo", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Start Slot
    [Serializable]
    private class SlotParam : TokenParam
    {
        public int slot_type;
        public long slot_cost;
    }

    public int StartSlot ( int slotType, long slotCost = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SlotParam param = new SlotParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.slot_type = slotType;
            param.slot_cost = slotCost;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("startslot.php", "RStartSlot", paramJson, 1);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }




    //=======================================================================================================================
    //Error Handling
    private ResponseParam ParseResponse ( API api )
    {
        ResponseParam response = new ResponseParam ();
        if (api.isDigested == -1) //Return Process For Template File
        {
            response.post_data = api.jsonReturn;
        }
        else
        {
            response = JsonUtility.FromJson<ResponseParam> (api.jsonReturn);
            if (response.error_code > 0)
            {
                api.errorCode = response.error_code;
                api.errorMsg = response.error_msg;
                ParseError (api);
                return null;
            }
            response.post_time = (Convert.ToInt32 (response.post_time) - Convert.ToInt32 (response.request_time)).ToString () + " sec(s)";
            response.post_data = PostData (response.post_id, response.post_data);
        }
        return response;
    }

    public void ParseError ( API api )
    {
        if (api.errorCode == 502)
        {
            api.errorMsg = new string[] { "API OTP is empty.", "API OTP kosong." };
        }
        else if (api.errorCode == 503)
        {
            api.errorMsg = new string[] { "API Player ID is empty.", "API Player ID kosong." };
        }
        else if (api.errorCode == 504)
        {
            api.errorMsg = new string[] { "API Token is empty.", "API Token kosong." };
        }
        ResponseParam response = new ResponseParam ();
        response.error_code = api.errorCode;
        response.error_msg = api.errorMsg;
        response.uri = api.uri;
        response.seed = api.seed;
        SendMessage ("RErrorHandler", response);
    }

    private bool ParseToken ( int playerId, string token, string uri = "ParseToken" )
    {
        if (playerId > 0) apiPlayerId = playerId;
        if (token.Length > 0) apiToken = token;
        if (apiPlayerId > 0 && apiToken.Length > 0)
        {
            return true;
        }
        else
        {
            API api = new API (uri, (apiPlayerId == 0 ? 503 : 504));
            ParseError (api);
            return false;
        }
    }

    private string PostData ( string post_id, string post_data )
    {
        return (post_id.Length == 0 ? post_data : Digest.Read (post_data, post_id));
    }


    //=======================================================================================================================
    //Automation
    private bool runBackground = true;

    //Automation Chat Session
    private long chatSession = -1;
    private string chatSessionUrl = "";
    private float refreshChatTime = 0.0f;

    public void StartChatSession ( int friendId = 0, int index = 0 )
    {
        if (index == 0)
        {
            chatSession = (friendId == 0 ? PlayerPrefs.GetInt ("publicChatSession", 0) : PlayerPrefs.GetInt ("privateChatSession", 0));
        }
        else
        {
            chatSession = index;
        }
        chatSessionUrl = "/";
        if (friendId > 0) chatSessionUrl += (apiPlayerId < friendId ? Mathf.FloorToInt (apiPlayerId / 1000).ToString () : Mathf.FloorToInt (friendId / 1000).ToString ()) + "/" + (apiPlayerId < friendId ? apiPlayerId.ToString () : friendId.ToString ()) + "/" + (apiPlayerId > friendId ? apiPlayerId.ToString () : friendId.ToString ()) + "/";
    }

    public void EndChatSession ()
    {
        chatSession = -1;
        chatSessionUrl = "";
    }

    private int GetChatSession ()
    {
        string defaultNotFound = "0";
        API api = new API ("chat" + chatSessionUrl + "chatindex.ray", "StartChatSessionResponse", "", -1);
        StartCoroutine (Congest.ReadTempFile (this, api, defaultNotFound));
        return api.seed;
    }

    private int StartChatSessionResponse ( ResponseParam response )
    {
        if (response.post_data.Length > 0)
        {
            try
            {
                long index = long.Parse (response.post_data);
                if (index > chatSession)
                {
                    chatSession = index;
                    PlayerPrefs.SetInt ((chatSessionUrl.Length == 1 ? "publicChatSession" : "privateChatSession"), (int) chatSession);
                    string defaultNotFound = "{\"chat\":[]}";
                    API api = new API ("chat" + chatSessionUrl + "chat.ray", "UpdateChatSessionResponse", "", -1);
                    StartCoroutine (Congest.ReadTempFile (this, api, defaultNotFound));
                    return api.seed;
                }
            }
            catch (Exception e)
            {
                Debug.Log (e.Message);
            }
        }
        return 0;
    }

    private void UpdateChatSessionResponse ( ResponseParam response )
    {
        if (response.post_data.Length > 0)
        {
            SendMessage ("Update" + (chatSessionUrl.Length == 1 ? "Public" : "Private") + "ChatSession", response);
        }
    }

    //Automation Announcement Session
    private int announcementSession = -1;
    private float refreshAnnouncementTime = 0.0f;

    private int GetAnnouncementSession ()
    {
        string defaultNotFound = "0";
        API api = new API ("announcementindex.ray", "StartAnnouncementSessionResponse", "", -1);
        StartCoroutine (Congest.ReadTempFile (this, api, defaultNotFound));
        return api.seed;
    }

    private int StartAnnouncementSessionResponse ( ResponseParam response )
    {
        if (response.post_data.Length > 0)
        {
            try
            {
                int index = int.Parse (response.post_data);
                if (index > announcementSession)
                {
                    announcementSession = index;
                    PlayerPrefs.SetInt ("announcementSession", announcementSession);
                    string defaultNotFound = "{\"announcement\":[]}";
                    API api = new API ("announcement.ray", "UpdateAnnouncementSession", "", -1);
                    StartCoroutine (Congest.ReadTempFile (this, api, defaultNotFound));
                    return api.seed;
                }
            }
            catch (Exception e)
            {
                Debug.Log (e.Message);
            }
        }
        return 0;
    }

    // Use this for initialization
    void Start ()
    {
        //PlayerPrefs.DeleteAll();
        announcementSession = PlayerPrefs.GetInt ("announcementSession", 0);
    }

    // Update is called once per frame
    void Update ()
    {
        if (runBackground)
        {
            //Chat Session
            if (chatSession >= 0)
            {
                refreshChatTime += Time.deltaTime;
                if (refreshChatTime >= 10.0f) //Check New Chat every 10 secs
                {
                    refreshChatTime = 0.0f;
                    GetChatSession ();
                }
            }
            //Announcement
            refreshAnnouncementTime += Time.deltaTime;
            if (refreshAnnouncementTime >= 60.0f) //Check Announcement every 60 secs
            {
                refreshAnnouncementTime = 0.0f;
                GetAnnouncementSession ();
            }
        }
    }


    //=======================================================================================================================
    //Set Variable Function
    public void SetPlayerId ( int playerId ) { apiPlayerId = playerId; }
    public void SetToken ( string token ) { apiToken = token; }
    public void SetOtp ( string otp ) { apiOtp = otp; }
}