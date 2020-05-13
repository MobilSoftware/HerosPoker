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
    };

    public enum SlotIconType
    {
        Joker, Ace,
        King, Queen, Jack, Ten,
        Nine, Eight, Seven, Six,
        Five, Four, Three, Two
    };

    public enum SlotIconRule1
    {
        Clover, Dragon,
        Bell, Spade, Love, Club,
        Diamond, Gem, A, K,
        J, Ten, Nine, Unused
    };

    public enum PaymentType
    {
        None, Telkomsel, Tri, Indosat, XL,
        Gopay, Dana, Unipin
    };

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

    public int UserLogin ( LoginType loginType, string loginId = "", string loginEmail = "", string loginName = "", string loginPicture = "" )
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
            Debug.LogError (paramJson);
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