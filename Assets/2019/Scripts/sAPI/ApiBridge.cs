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
        public bool isDigested;
        public string otp;
        public int cacheTime;
        public string jsonReturn;
        public int errorCode;
        public string[] errorMsg;

        public API ( string _uri, string _rFuncName, string _jsonData = "", bool _isDigested = false, string _otp = "" )
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

    private bool runBackground = true;

    public void RunBackGround ( bool isRun )
    {
        runBackground = isRun;
    }



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
        API api = new API ("getversion.php", "RGetVersion", paramJson, true);
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
        API api = new API ("getotp.php", "RGetOtp", paramJson, true);
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
        API api = new API ("userlogin.php", "RUserLogin", paramJson, true, apiOtp);
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
            API api = new API ("getnews.php", "RGetNews", paramJson, true);
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
            API api = new API ("gethome.php", "RGetHome", paramJson, true);
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
            API api = new API ("getprofile.php", "RGetProfile", paramJson, true);
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
            API api = new API ("getevent.php", "RGetEvent", paramJson, true);
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
            API api = new API ("gethero.php", "RGetHero", paramJson, true);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Get Costume
    [Serializable]
    private class CostumeParam : TokenParam
    {
        public int hero_id;
    }

    public int GetCostume ( int heroId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            CostumeParam param = new CostumeParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.hero_id = heroId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("getcostume.php", "RGetCostume", paramJson, true);
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
            CostumeParam param = new CostumeParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.hero_id = heroId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("sethero.php", "RSetHero", paramJson, true);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Set Costume
    [Serializable]
    private class SetCostumeParam : TokenParam
    {
        public int costume_id;
    }

    public int SetCostume ( int costumeId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SetCostumeParam param = new SetCostumeParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.costume_id = costumeId;
            string paramJson = JsonUtility.ToJson (param);
            API api = new API ("setcostume.php", "RSetCostume", paramJson, true);
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
            API api = new API ("getshop.php", "RGetShop", paramJson, true);
            param = null; paramJson = "";
            StartCoroutine (Congest.SendPOST (this, api));
            return api.seed;
        }
        return 0;
    }



    //Buy Shop
    public int BuyShop ( int itemId, int itemType = 0, string invoiceId = "", int playerId = 0, string token = "", string otp = "" )
    {
        if (ParseToken (playerId, token))
        {
            ShopParam param = new ShopParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.item_id = itemId;
            param.item_type = itemType;
            param.invoice_id = invoiceId;
            param.device_id = SystemInfo.deviceUniqueIdentifier;
            param.device_type = (int) Application.platform;
            string paramJson = JsonUtility.ToJson (param);
            if (otp.Length > 0) apiOtp = otp;
            API api = new API ("buyshop.php", "RBuyShop", paramJson, true, (param.item_type == 0 ? apiOtp : ""));
            param = null; paramJson = "";
            if (itemType == 0 && apiOtp.Length == 0)
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
            API api = new API ("startpoker.php", "RStartPoker", paramJson, true);
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
            API api = new API ("endpoker.php", "REndPoker", paramJson, true, apiOtp);
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
            API api = new API ("startsicbo.php", "RStartSicbo", paramJson, true);
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
            API api = new API ("startslot.php", "RStartSlot", paramJson, true);
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
        response = JsonUtility.FromJson<ResponseParam> (api.jsonReturn);
        if (response.error_code == 0)
        {
            response.post_time = (Convert.ToInt32 (response.post_time) - Convert.ToInt32 (response.request_time)).ToString () + " sec(s)";
            response.post_data = PostData (response.post_id, response.post_data);
            return response;
        }
        else
        {
            api.errorCode = response.error_code;
            api.errorMsg = response.error_msg;
            ParseError (api);
            return null;
        }
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
            API api = new API (uri, (apiPlayerId == 0 ? 502 : 504));
            ParseError (api);
            return false;
        }
    }

    private string PostData ( string post_id, string post_data )
    {
        return (post_id.Length == 0 ? post_data : Digest.Read (post_data, post_id));
    }


    //Set Variable Function
    public void SetPlayerId ( int playerId ) { apiPlayerId = playerId; }
    public void SetToken ( string token ) { apiToken = token; }
    public void SetOtp ( string otp ) { apiOtp = otp; }
}