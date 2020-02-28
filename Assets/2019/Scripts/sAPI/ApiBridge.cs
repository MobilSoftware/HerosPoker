using UnityEngine;
using Rays.Utilities;
using System;
using System.Text;

public class ApiBridge : MonoBehaviour
{
    public delegate void ResponseDelegate ( string inputJson );
    public delegate void ErrorDelegate ( int errorCode, string[] errorMsg );

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

    private int apiPlayerId = 0;
    private string apiToken = "";
    private string apiOtp = "";

    private bool runBackground = true;

    public void RunBackGround ( bool isRun )
    {
        runBackground = isRun;
    }

    [Serializable]
    public class ResponseParam
    {
        public int error_code = 0;
        public string[] error_msg = { "", "" };
        public string post_id = "";
        public string post_data = "";
    }


    //Get Version
    public void GetVersion ()
    {
        StartCoroutine (Congest.SendPOST (0, ParseError, GetVersionResponse, "getversion.php"));
    }

    private void GetVersionResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetVersion", post_data);
    }




    //Get News
    [Serializable]
    private class GetNewsParam
    {
        public int player_id;
        public string token;
        public string device_id;
        public int device_type;
        public string apk_ver;
    }

    public void GetNews ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetNewsParam param = new GetNewsParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.device_id = SystemInfo.deviceUniqueIdentifier;
            param.device_type = (int) Application.platform;
            param.apk_ver = Application.version + (Congest.DEVELOPMENT ? "d" : "");
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (60, ParseError, GetNewsResponse, "getnews.php", paramJson, true));
        }
    }

    private void GetNewsResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetNews", post_data);
    }



    //User Register
    [Serializable]
    private class UserRegisterParam
    {
        public string email;
        public string password;
        public string device_id;
        public int device_type;
    }

    public void UserRegister ( string email, string password )
    {
        UserRegisterParam param = new UserRegisterParam ();
        param.email = email;
        param.password = password;
        param.device_id = SystemInfo.deviceUniqueIdentifier;
        param.device_type = (int) Application.platform;
        string paramJson = JsonUtility.ToJson (param);
        StartCoroutine (Congest.SendPOST (0, ParseError, UserRegisterResponse, "userregister.php", paramJson, true));
    }

    private void UserRegisterResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RUserRegister", post_data);
    }




    //User Bind
    [Serializable]
    private class UserBindParam
    {
        public int bind_via;
        public int is_bind; //0. Not Bind, > 0. Player_ID
        public string social_id;
        public string social_email;
        public string social_name;
        public string social_picture;
        public string device_id;
        public int device_type;
    }

    public void UserBind ( int bindVia = 0, int isBind = 0, string social_id = "", string social_email = "", string social_name = "", string social_picture = "", string otp = "" )
    {
        UserBindParam param = new UserBindParam ();
        param.bind_via = bindVia;
        param.is_bind = isBind;
        param.social_id = social_id;
        param.social_email = social_email;
        param.social_name = social_name;
        param.social_picture = social_picture;
        param.device_id = SystemInfo.deviceUniqueIdentifier;
        param.device_type = (int) Application.platform;
        string paramJson = JsonUtility.ToJson (param);
        if (otp.Length > 0) apiOtp = otp;
        if (apiOtp.Length > 0)
        {
            StartCoroutine (Congest.SendPOST (0, ParseError, UserBindResponse, "userbind.php", paramJson, true, apiOtp));
        }
        else
        {
            ParseError (502);
        }
    }

    private void UserBindResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RUserBind", post_data);
    }



    //User Login
    [Serializable]
    private class UserLoginParam
    {
        public string email;
        public string password;
        public string device_id;
        public int device_type;
    }

    public void UserLogin ( string email, string password )
    {
        UserLoginParam param = new UserLoginParam ();
        param.email = email;
        param.password = password;
        param.device_id = SystemInfo.deviceUniqueIdentifier;
        param.device_type = (int) Application.platform;
        string paramJson = JsonUtility.ToJson (param);
        StartCoroutine (Congest.SendPOST (0, ParseError, UserLoginResponse, "userlogin.php", paramJson, true));
    }

    private void UserLoginResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RUserLogin", post_data);
    }


    //Get OTP
    [Serializable]
    private class GetOtpParam
    {
        public string device_id;
        public int device_type;
        public int otp_type;
    }

    public void GetOtp ( int otpType = 0 )
    {
        GetOtpParam param = new GetOtpParam ();
        param.device_id = SystemInfo.deviceUniqueIdentifier;
        param.device_type = (int) Application.platform;
        param.otp_type = otpType;
        string paramJson = JsonUtility.ToJson (param);
        StartCoroutine (Congest.SendPOST (0, ParseError, GetOtpResponse, "getotp.php", paramJson, true));
    }

    private void GetOtpResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetOtp", post_data);
    }


    public void GetIapOtp ( int otpType = 0 )
    {
        GetOtpParam param = new GetOtpParam ();
        param.device_id = SystemInfo.deviceUniqueIdentifier;
        param.device_type = (int) Application.platform;
        param.otp_type = otpType;
        string paramJson = JsonUtility.ToJson (param);
        StartCoroutine (Congest.SendPOST (0, ParseError, GetIapOtpResponse, "getotp.php", paramJson, true));
    }

    private void GetIapOtpResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetIapOtp", post_data);
    }


    //User Login FB/Google
    [Serializable]
    private class UserLoginSocialParam
    {
        public string social_id;
        public string social_email;
        public string social_name;
        public string social_picture;
        public string device_id;
        public int device_type;
    }

    private void UserLoginSocial ( int socialType, string social_id, string social_email, string social_name, string social_picture, string otp )
    { //0. FB, 1. Google
        UserLoginSocialParam param = new UserLoginSocialParam ();
        param.social_id = social_id;
        param.social_email = social_email;
        param.social_name = social_name;
        param.social_picture = social_picture;
        param.device_id = SystemInfo.deviceUniqueIdentifier;
        param.device_type = (int) Application.platform;
        string paramJson = JsonUtility.ToJson (param);
        if (otp.Length > 0) apiOtp = otp;
        if (apiOtp.Length > 0)
        {
            if (socialType == 0) //Facebook
            {
                StartCoroutine (Congest.SendPOST (0, ParseError, UserLoginFBResponse, "userloginfb.php", paramJson, true, apiOtp));
            }
            else //Google
            {
                StartCoroutine (Congest.SendPOST (0, ParseError, UserLoginGoogleResponse, "userlogingoogle.php", paramJson, true, apiOtp));
            }
        }
        else
        {
            ParseError (502);
        }
    }

    public void UserLoginFB ( string social_id, string social_email, string social_name, string social_picture, string otp = "" )
    {
        UserLoginSocial (0, social_id, social_email, social_name, social_picture, otp);
    }

    private void UserLoginFBResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RUserLoginFB", post_data);
    }

    public void UserLoginGoogle ( string social_id, string social_email, string social_name, string social_picture, string otp = "" )
    {
        UserLoginSocial (1, social_id, social_email, social_name, social_picture, otp);
    }

    private void UserLoginGoogleResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RUserLoginGoogle", post_data);
    }



    //Get Home
    [Serializable]
    private class TokenParam
    {
        public int player_id;
        public string token;
        public int is_live;
    }

    public void GetHome ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetHomeResponse, "gethome.php", paramJson, true));
        }
        //string post_data = "{\"player\":{\"player_id\":1001,\"friend_search_id\":\"13E9\",\"referal_code\":\"G10RT9\",\"credit\":\"11270\",\"gem\":\"75495\",\"keylock\":\"517\",\"coupon\":\"4975\",\"score\":\"152626\",\"scoring_type_id\":\"1\",\"scoring_card\":\"4,8,12,16,20,24,28,32,36,40,44,48,0\",\"winning\":746,\"losing\":1098,\"level\":50,\"display_name\":\"DevMichael\",\"display_picture\":null,\"status_message\":\"_klsubububininininibibib\",\"gender_type_id\":\"1\",\"avatar_equiped\":\"28\",\"vehicle_equiped\":\"5\",\"vehicle_name\":[\"Honda Civic\",\"Honda Civic\"],\"vehicle_bonus_credit\":\"285\",\"vehicle_bonus_gem\":\"0\",\"ppicture_equiped\":\"0\",\"pbadge_owned\":\"161,164,162,165,158,166,134,135,136,137,138,151,154,142,159,139,140,145\",\"ptitle_equiped\":\"123\",\"ptitle\":[\"Stylish\",\"Stylish\"],\"house\":\"22\",\"rank\":\"Rank Amateur\",\"daily_bonus_countdown\":0,\"videoads_bonus_left\":10,\"bankrupt_bonus_left\":5,\"daily_login_reward_taken\":true,\"daily_login_log\":\"\",\"lucky_spin_taken\":true,\"friend_request_notif\":0,\"friend_chat_notif\":0,\"friend_gift_notif\":0,\"bind_via\":0,\"refer_by\":\"0\",\"verified\":1}}";
        //SendMessage("RGetHome", post_data);
    }

    private void GetHomeResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetHome", post_data);
    }



    //Get Daily Bonus
    public void GetDailyBonus ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetDailyBonusResponse, "getdailybonus.php", paramJson, true));
        }
    }

    private void GetDailyBonusResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetDailyBonus", post_data);
    }



    //Get Video Ads Bonus
    public void GetVideoAdsBonus ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetVideoAdsBonusResponse, "getvideoadsbonus.php", paramJson, true));
        }
    }

    private void GetVideoAdsBonusResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetVideoAdsBonus", post_data);
    }



    //Get Bankrupt Bonus
    public void GetBankruptBonus ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetBankruptBonusResponse, "getbankruptbonus.php", paramJson, true));
        }
    }

    private void GetBankruptBonusResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetBankruptBonus", post_data);
    }



    //Get Shop List
    [Serializable]
    private class GetShopListParam
    {
        public int player_id;
        public string token;
        public int item_type;
    }

    public void GetShopList ( int typeId = 1, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetShopListParam param = new GetShopListParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.item_type = typeId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetShopListResponse, "getshoplist.php", paramJson, true));
        }
    }

    private void GetShopListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetShopList", post_data);
    }


    //Buy Shop
    [Serializable]
    private class ItemParam
    {
        public int player_id;
        public string token;
        public int item_id;
    }

    public void BuyShop ( int itemId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ItemParam param = new ItemParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.item_id = itemId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, BuyShopResponse, "buyshop.php", paramJson, true));
        }
    }

    private void BuyShopResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RBuyShop", post_data);
    }



    //Get IAP List
    public void GetIapList ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (60, ParseError, GetIapListResponse, "getiaplist.php", paramJson, true));
        }
    }

    private void GetIapListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetIapList", post_data);
    }



    //Buy IAP
    [Serializable]
    private class BuyIapParam
    {
        public int player_id;
        public string token;
        public int item_id;
        public string invoice_id;
        public string device_id;
        public int device_type;
    }

    public void BuyIap ( int itemId, string invoiceId, int playerId = 0, string token = "", string otp = "" )
    {
        if (ParseToken (playerId, token))
        {
            BuyIapParam param = new BuyIapParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.item_id = itemId;
            param.invoice_id = invoiceId;
            param.device_id = SystemInfo.deviceUniqueIdentifier;
            param.device_type = (int) Application.platform;
            string paramJson = JsonUtility.ToJson (param);
            if (otp.Length > 0) apiOtp = otp;
            if (apiOtp.Length > 0)
            {
                StartCoroutine (Congest.SendPOST (0, ParseError, BuyIapResponse, "buyiap.php", paramJson, true, apiOtp));
            }
            else
            {
                ParseError (502);
            }
        }
    }

    private void BuyIapResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RBuyIap", post_data);
    }



    //Get Wardrobe
    public void GetWardrobe ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetWardrobeResponse, "getwardrobe.php", paramJson, true));
        }
    }

    private void GetWardrobeResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetWardrobe", post_data);
    }



    //Get Garage
    public void GetGarage ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetGarageResponse, "getgarage.php", paramJson, true));
        }
    }

    private void GetGarageResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetGarage", post_data);
    }



    //Get Title
    public void GetTitle ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetTitleResponse, "gettitle.php", paramJson, true));
        }
    }

    private void GetTitleResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetTitle", post_data);
    }



    //Set Title
    public void SetTitle ( int itemId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ItemParam param = new ItemParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.item_id = itemId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SetTitleResponse, "settitle.php", paramJson, true));
        }
    }

    private void SetTitleResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSetTitle", post_data);
    }



    //Use Item
    public void UseItem ( int itemId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ItemParam param = new ItemParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.item_id = itemId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, UseItemResponse, "useitem.php", paramJson, true));
        }
    }

    private void UseItemResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RUseItem", post_data);
    }



    //Send Item
    [Serializable]
    private class SendItemParam
    {
        public int player_id;
        public string token;
        public int friend_id;
        public int item_id;
    }

    public void SendItem ( int friendId, int itemId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SendItemParam param = new SendItemParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            param.item_id = itemId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendItemResponse, "senditem.php", paramJson, true));
        }
    }

    private void SendItemResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendItem", post_data);
    }



    //Send Credit
    [Serializable]
    private class SendCreditParam
    {
        public int player_id;
        public string token;
        public string friend_tag;
        public long credit_amount;
    }

    public void SendCredit ( string friendTag, long creditAmount, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SendCreditParam param = new SendCreditParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_tag = friendTag;
            param.credit_amount = creditAmount;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendCreditResponse, "sendcredit.php", paramJson, true));
        }
    }

    private void SendCreditResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendCredit", post_data);
    }



    public void SendCreditHistory ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendCreditHistoryResponse, "sendcredithistory.php", paramJson, true));
        }
    }

    private void SendCreditHistoryResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendCreditHistory", post_data);
    }




    //Send Support
    [Serializable]
    private class SendSupportParam
    {
        public int player_id;
        public string token;
        public string ticket_id;
        public string email;
        public string message;
    }

    public void SendSupport ( string ticketId, string email, string message, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SendSupportParam param = new SendSupportParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.ticket_id = ticketId;
            param.email = email;
            param.message = message;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendSupportResponse, "sendsupport.php", paramJson, true));
        }
    }

    private void SendSupportResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendSupport", post_data);
    }




    //Send Report
    [Serializable]
    private class SendReportParam
    {
        public int player_id;
        public string token;
        public string device_id;
        public int device_type;
        public string message;
    }

    public void SendReport ( string message = "", int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SendReportParam param = new SendReportParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.device_id = SystemInfo.deviceUniqueIdentifier;
            param.device_type = (int) Application.platform;
            param.message = message;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, null, null, "sendreport.php", paramJson, true));
        }
    }

    public void SendMail ( int friendId, int itemId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SendItemParam param = new SendItemParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            param.item_id = itemId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendMailResponse, "sendmail.php", paramJson, true));
        }
    }

    private void SendMailResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendMail", post_data);
    }



    //Read Mail
    [Serializable]
    private class ReadMailParam
    {
        public int player_id;
        public string token;
        public int mail_id;
    }

    public void ReadMail ( int mailId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ReadMailParam param = new ReadMailParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.mail_id = mailId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, ReadMailResponse, "readmail.php", paramJson, true));
        }
    }

    private void ReadMailResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RReadMail", post_data);
    }



    //Claim Mail
    public void ClaimMail ( int mailId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ReadMailParam param = new ReadMailParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.mail_id = mailId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, ClaimMailResponse, "claimmail.php", paramJson, true));
        }
    }

    private void ClaimMailResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RClaimMail", post_data);
    }



    //Get Friend List
    public void GetFriendList ( int itemId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ItemParam param = new ItemParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.item_id = itemId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetFriendListResponse, "getfriendlist.php", paramJson, true));
        }
    }

    private void GetFriendListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetFriendList", post_data);
    }



    //Get Friend Detail
    [Serializable]
    private class GetFriendDetailParam
    {
        public int player_id;
        public string token;
        public int friend_id;
    }

    public void GetFriendDetail ( int friendId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetFriendDetailParam param = new GetFriendDetailParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetFriendDetailResponse, "getfrienddetail.php", paramJson, true));
        }
    }

    private void GetFriendDetailResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetFriendDetail", post_data);
    }



    //Get Friend Request Me
    public void GetFriendRequestMe ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (20, ParseError, GetFriendRequestMeResponse, "getfriendrequestme.php", paramJson, true));
        }
    }

    private void GetFriendRequestMeResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetFriendRequestMe", post_data);
    }



    //Get Friend Request
    public void GetFriendRequest ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (20, ParseError, GetFriendRequestResponse, "getfriendrequest.php", paramJson, true));
        }
    }

    private void GetFriendRequestResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetFriendRequest", post_data);
    }



    //Send Friend Request
    public void SendFriendRequest ( int friendId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetFriendDetailParam param = new GetFriendDetailParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendFriendRequestResponse, "sendfriendrequest.php", paramJson, true));
        }
    }

    private void SendFriendRequestResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendFriendRequest", post_data);
    }



    //Send Friend Delete
    public void SendFriendDelete ( int friendId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetFriendDetailParam param = new GetFriendDetailParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendFriendDeleteResponse, "sendfrienddelete.php", paramJson, true));
        }
    }

    private void SendFriendDeleteResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendFriendDelete", post_data);
    }



    //Send Friend Report
    [Serializable]
    private class SendFriendReportParam
    {
        public int player_id;
        public string token;
        public int friend_id;
        public string reason;
    }

    public void SendFriendReport ( int friendId, string reason, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SendFriendReportParam param = new SendFriendReportParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            param.reason = reason;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendFriendReportResponse, "sendfriendreport.php", paramJson, true));
        }
    }

    private void SendFriendReportResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendFriendReport", post_data);
    }



    //Send Friend Block
    public void SendFriendBlock ( int friendId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetFriendDetailParam param = new GetFriendDetailParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendFriendBlockResponse, "sendfriendblock.php", paramJson, true));
        }
    }

    private void SendFriendBlockResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendFriendBlock", post_data);
    }



    //Get Friend Block
    public void GetFriendBlock ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetFriendBlockResponse, "getfriendblock.php", paramJson, true));
        }
    }

    private void GetFriendBlockResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetFriendBlock", post_data);
    }



    //Get Friend Block Me
    public void GetFriendBlockMe ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetFriendBlockMeResponse, "getfriendblockme.php", paramJson, true));
        }
    }

    private void GetFriendBlockMeResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetFriendBlockMe", post_data);
    }



    //Send Friend Response
    [Serializable]
    private class SendFriendResponseParam
    {
        public int player_id;
        public string token;
        public int friend_id;
        public int is_accept;
    }

    public void SendFriendResponse ( int friendId, bool isAccept, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SendFriendResponseParam param = new SendFriendResponseParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            param.is_accept = (isAccept ? 1 : 0);
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendFriendResponseResponse, "sendfriendresponse.php", paramJson, true));
        }
    }

    private void SendFriendResponseResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendFriendResponse", post_data);
    }



    //Search Friend By Id
    [Serializable]
    private class SearchFriendParam
    {
        public int player_id;
        public string token;
        public string search_key;
        public int is_live;
    }

    public void SearchFriendById ( string searchKey, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SearchFriendParam param = new SearchFriendParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.search_key = searchKey;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SearchFriendByIdResponse, "searchfriendbyid.php", paramJson, true));
        }
    }

    private void SearchFriendByIdResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSearchFriendById", post_data);
    }



    //Search Friend By Name
    public void SearchFriendByName ( string searchKey, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SearchFriendParam param = new SearchFriendParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.search_key = searchKey;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SearchFriendByNameResponse, "searchfriendbyname.php", paramJson, true));
        }
    }

    private void SearchFriendByNameResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSearchFriendByName", post_data);
    }



    //Get Fragment
    public void GetFragment ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetFragmentResponse, "getfragment.php", paramJson, true));
        }
    }

    private void GetFragmentResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetFragment", post_data);
    }


    //Get Config
    /*public void GetConfig(int playerId = 0, string token = "")
    {
        if (ParseToken(playerId, token))
        {
            TokenParam param = new TokenParam();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson(param);
            StartCoroutine(Congest.SendPOST(0, ParseError, GetConfigResponse, "getconfig.php", paramJson, true));
        }
    }

    private void GetConfigResponse(string inputJson)
    {
        string post_data = ParseResponse(inputJson);
        if (post_data.Length > 0) SendMessage("RGetConfig", post_data);
    }*/

    public void GetConfig ()
    {
        StartCoroutine (Congest.ReadChatText (ParseError, GetConfigResponse, "config.txt"));
    }

    private void GetConfigResponse ( string post_data )
    {
        if (post_data.Length > 0) SendMessage ("RGetConfig", Encoding.UTF8.GetString (Convert.FromBase64String (post_data)));
    }



    //Get New Notif
    public void GetNewNotif ()
    {
        StartCoroutine (Congest.ReadChatText (ParseError, GetNewNotifResponse, "newnotif.txt"));
    }

    private void GetNewNotifResponse ( string post_data )
    {
        if (post_data.Length > 0) SendMessage ("RGetNewNotif", Encoding.UTF8.GetString (Convert.FromBase64String (post_data)));
    }



    //Get VIP Level
    public void GetVIPLevel ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetVIPLevelResponse, "getviplevel.php", paramJson, true));
        }
    }

    private void GetVIPLevelResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetVIPLevel", post_data);
    }


    //Get Chest List
    public void GetChestList ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (60, ParseError, GetChestListResponse, "getchestlist.php", paramJson, true));
        }
    }

    private void GetChestListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetChestList", post_data);
    }


    //Get Reward List
    [Serializable]
    private class GetRewardParam
    {
        public int player_id;
        public string token;
        public int chest_type;
        public int key_type;
    }

    public void GetRewardList ( int chestId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetRewardParam param = new GetRewardParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.chest_type = chestId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetRewardListResponse, "getrewardlist.php", paramJson, true));
        }
    }

    private void GetRewardListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetRewardList", post_data);
    }


    //Open Chest
    public void OpenChest ( int chestId, int keyType = 3, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetRewardParam param = new GetRewardParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.chest_type = chestId;
            param.key_type = keyType;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, OpenChestResponse, "openchest.php", paramJson, true));
        }
    }

    private void OpenChestResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("ROpenChest", post_data);
    }



    //Slot Test
    [Serializable]
    private class SlotTestParam
    {
        public int set_win;
        public string set_matrix;
    }

    public void SlotTest ( int setWin = 2, string setMatrix = "" )
    {
        SlotTestParam param = new SlotTestParam ();
        param.set_win = setWin;
        param.set_matrix = setMatrix;
        string paramJson = JsonUtility.ToJson (param);
        StartCoroutine (Congest.SendPOST (0, ParseError, SlotTestResponse, "slottest.php", paramJson));
    }

    private void SlotTestResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSlotTest", post_data);
    }



    //Spin Slot
    [Serializable]
    private class SpinSlotParam
    {
        public int player_id;
        public string token;
        public int slot_type;
        public long slot_cost;
    }

    public void SpinSlot ( int slotType = 1, long slotCost = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SpinSlotParam param = new SpinSlotParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.slot_type = slotType;
            param.slot_cost = slotCost;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SpinSlotResponse, "spinslot.php", paramJson, true));
        }
    }

    private void SpinSlotResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSpinSlot", post_data);
    }



    //Get Daily Login Reward List
    public void GetDailyLoginRewardList ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (20, ParseError, GetDailyLoginRewardListResponse, "getdailyloginrewardlist.php", paramJson, true));
        }
    }

    private void GetDailyLoginRewardListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetDailyLoginRewardList", post_data);
    }


    //Get Daily Login Reward
    public void GetDailyLoginReward ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetDailyLoginRewardResponse, "getdailyloginreward.php", paramJson, true));
        }
    }

    private void GetDailyLoginRewardResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetDailyLoginReward", post_data);
    }


    //Get Lucky Spin
    public void GetLuckySpin ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetLuckySpinResponse, "getluckyspin.php", paramJson, true));
        }
    }

    private void GetLuckySpinResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetLuckySpin", post_data);
    }




    //Get Gift History
    [Serializable]
    private class GetGiftHistoryParam
    {
        public int player_id;
        public string token;
        public int as_sender;
    }

    public void GetGiftHistory ( bool asSender, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetGiftHistoryParam param = new GetGiftHistoryParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.as_sender = (asSender ? 1 : 0);
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetGiftHistoryResponse, "getgifthistory.php", paramJson, true));
        }
    }

    private void GetGiftHistoryResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetGiftHistory", post_data);
    }



    public void GetMailbox ( bool asSender, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetGiftHistoryParam param = new GetGiftHistoryParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.as_sender = (asSender ? 1 : 0);
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetMailboxResponse, "getmailbox.php", paramJson, true));
        }
    }

    private void GetMailboxResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetMailbox", post_data);
    }




    //Get Leaderboard
    [Serializable]
    private class GetLeaderboardParam
    {
        public int player_id;
        public string token;
        public int leaderboard_type;
        public int event_id;
    }

    public void GetLeaderboard ( int typeId = 1, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetLeaderboardParam param = new GetLeaderboardParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.leaderboard_type = typeId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetLeaderboardResponse, "getleaderboard.php", paramJson, true));
        }
    }

    private void GetLeaderboardResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetLeaderboard", post_data);
    }



    public void GetEventLeaderboard ( int eventId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetLeaderboardParam param = new GetLeaderboardParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.event_id = eventId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetEventLeaderboardResponse, "geteventleaderboard.php", paramJson, true));
        }
    }

    private void GetEventLeaderboardResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetEventLeaderboard", post_data);
    }



    //Get Daily Quest List
    public void GetDailyQuestList ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (20, ParseError, GetDailyQuestListResponse, "getdailyquestlist.php", paramJson, true));
        }
    }

    private void GetDailyQuestListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetDailyQuestList", post_data);
    }



    //Claim Daily Quest
    [Serializable]
    private class QuestParam
    {
        public int player_id;
        public string token;
        public int quest_id;
    }

    public void ClaimDailyQuest ( int questId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            QuestParam param = new QuestParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.quest_id = questId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, ClaimDailyQuestResponse, "claimdailyquest.php", paramJson, true));
        }
    }

    private void ClaimDailyQuestResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RClaimDailyQuest", post_data);
    }



    //Claim Invitee Mission
    [Serializable]
    private class ClaimInviteeMissionParam
    {
        public int player_id;
        public string token;
        public int friend_id;
        public int mission_id;
    }

    public void ClaimInviteeMission ( int friendId = 0, int missionId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ClaimInviteeMissionParam param = new ClaimInviteeMissionParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            param.mission_id = missionId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, ClaimInviteeMissionResponse, "claiminviteemission.php", paramJson, true));
        }
    }

    private void ClaimInviteeMissionResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RClaimInviteeMission", post_data);
    }



    //Claim Achievement
    public void ClaimAchievement ( int questId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            QuestParam param = new QuestParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.quest_id = questId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, ClaimAchievementResponse, "claimachievement.php", paramJson, true));
        }
    }

    private void ClaimAchievementResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RClaimAchievement", post_data);
    }



    //Get Achievement List
    public void GetAchievementList ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (20, ParseError, GetAchievementListResponse, "getachievementlist.php", paramJson, true));
        }
    }

    private void GetAchievementListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetAchievementList", post_data);
    }



    //Get One Time Quest List
    public void GetOneTimeQuestList ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (20, ParseError, GetOneTimeQuestListResponse, "getonetimequestlist.php", paramJson, true));
        }
    }

    private void GetOneTimeQuestListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetOneTimeQuestList", post_data);
    }



    //Claim One Time Quest
    public void ClaimOneTimeQuest ( int questId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            QuestParam param = new QuestParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.quest_id = questId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, ClaimOneTimeQuestResponse, "claimonetimequest.php", paramJson, true));
        }
    }

    private void ClaimOneTimeQuestResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RClaimOneTimeQuest", post_data);
    }



    //ClaimReferalCode
    [Serializable]
    private class ClaimReferalCodeParam
    {
        public int player_id;
        public string token;
        public string referal_code;
        public string device_id;
        public int device_type;
    }

    public void ClaimReferalCode ( string referalCode, int playerId = 0, string token = "" )
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
            StartCoroutine (Congest.SendPOST (0, ParseError, ClaimReferalCodeResponse, "claimreferalcode.php", paramJson, true));
        }
    }

    private void ClaimReferalCodeResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RClaimReferalCode", post_data);
    }




    //ClaimDealerCode
    public void ClaimDealerCode ( string referalCode, int playerId = 0, string token = "" )
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
            StartCoroutine (Congest.SendPOST (0, ParseError, ClaimDealerCodeResponse, "claimdealercode.php", paramJson, true));
        }
    }

    private void ClaimDealerCodeResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RClaimDealerCode", post_data);
    }




    //ClaimPromoCode
    [Serializable]
    private class ClaimPromoCodeParam
    {
        public int player_id;
        public string token;
        public string promo_code;
    }

    public void ClaimPromoCode ( string promoCode, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            ClaimPromoCodeParam param = new ClaimPromoCodeParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.promo_code = promoCode;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, ClaimPromoCodeResponse, "claimpromocode.php", paramJson, true));
        }
    }

    private void ClaimPromoCodeResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RClaimPromoCode", post_data);
    }




    //Invite Random Player
    [Serializable]
    private class InviteRandomPlayerParam
    {
        public int player_id;
        public string token;
        public int room_type_id;
        public int num_of_player;
    }

    public void InviteRandomPlayer ( int roomTypeId = 1, int numOfPlayer = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            InviteRandomPlayerParam param = new InviteRandomPlayerParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.room_type_id = roomTypeId;
            param.num_of_player = numOfPlayer;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, InviteRandomPlayerResponse, "inviterandomplayer.php", paramJson, true));
        }
    }

    private void InviteRandomPlayerResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RInviteRandomPlayer", post_data);
    }



    //Invite Known Player
    [Serializable]
    private class InviteKnownPlayerParam
    {
        public int player_id;
        public string token;
        public string photon_id;
        public int friend_id;
        public int min_credit;
        public int min_gem;
        public int num_player;
        public int game_type_id;
    }

    public void InviteKnownPlayer ( int friendId, string photonId, int minCredit = 0, int minGem = 0, int numPlayer = 0, int gameTypeId = 1, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            InviteKnownPlayerParam param = new InviteKnownPlayerParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.photon_id = photonId;
            param.friend_id = friendId;
            param.min_credit = minCredit;
            param.min_gem = minGem;
            param.num_player = numPlayer;
            param.game_type_id = gameTypeId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, InviteKnownPlayerResponse, "inviteknownplayer.php", paramJson, true));
        }
    }

    private void InviteKnownPlayerResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RInviteKnownPlayer", post_data);
    }



    //=======================================================================================================================
    //Send Public Chat
    [Serializable]
    private class SendPublicChatParam
    {
        public int player_id;
        public string token;
        public string chat_msg;
    }

    public void SendPublicChat ( string chatMsg, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SendPublicChatParam param = new SendPublicChatParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.chat_msg = chatMsg;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendPublicChatResponse, "sendpublicchat.php", paramJson, true));
        }
    }

    private void SendPublicChatResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendPublicChat", post_data);
    }



    //Get Public Chat
    public void GetPublicChat ()
    {
        StartCoroutine (Congest.ReadChatText (ParseError, GetPublicChatResponse, "content.txt", "{\"chat\":[]}"));
    }

    private void GetPublicChatResponse ( string post_data )
    {
        if (post_data.Length > 0) SendMessage ("RGetPublicChat", post_data);
    }



    //Get Announcement
    public void GetAnnouncement ()
    {
        StartCoroutine (Congest.ReadChatText (ParseError, GetAnnouncementResponse, "announcement.txt"));
    }

    private void GetAnnouncementResponse ( string post_data )
    {
        if (post_data.Length > 0) SendMessage ("RGetAnnouncement", post_data);
    }



    //Get Private Chat List
    public void GetPrivateChatList ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetPrivateChatListResponse, "getprivatechatlist.php", paramJson, true));
        }
    }

    private void GetPrivateChatListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetPrivateChatList", post_data);
    }



    //Send Private Chat
    [Serializable]
    private class SendPrivateChatParam
    {
        public int player_id;
        public string token;
        public string chat_msg;
        public int friend_id;
    }

    public void SendPrivateChat ( int friendId, string chatMsg, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SendPrivateChatParam param = new SendPrivateChatParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.chat_msg = chatMsg;
            param.friend_id = friendId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SendPrivateChatResponse, "sendprivatechat.php", paramJson, true));
        }
    }

    private void SendPrivateChatResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSendPrivateChat", post_data);
    }



    //Get Private Chat
    public void GetPrivateChat ( int friendId )
    {
        string fromto = (apiPlayerId < friendId ? Mathf.FloorToInt (apiPlayerId / 1000).ToString () : Mathf.FloorToInt (friendId / 1000).ToString ()) + "/" + (apiPlayerId < friendId ? apiPlayerId.ToString () : friendId.ToString ()) + "/" + (apiPlayerId > friendId ? apiPlayerId.ToString () : friendId.ToString ());
        StartCoroutine (Congest.ReadChatText (ParseError, GetPrivateChatResponse, fromto + "/content.txt", "{\"chat\":[]}"));
    }

    private void GetPrivateChatResponse ( string post_data )
    {
        if (post_data.Length > 0) SendMessage ("RGetPrivateChat", post_data);
    }



    //=======================================================================================================================
    //Set Gender
    [Serializable]
    private class SetGenderParam
    {
        public int player_id;
        public string token;
        public int gender;
        public string display_name;
        public string status_msg;
    }

    public void SetGender ( int gender, string display_name, string status_msg = "", int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SetGenderParam param = new SetGenderParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.gender = gender;
            param.display_name = display_name;
            param.status_msg = status_msg;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SetGenderResponse, "setgender.php", paramJson, true));
        }
    }

    private void SetGenderResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSetGender", post_data);
    }


    //Leave Game Pinalty
    [Serializable]
    private class LeaveGamePinaltyParam
    {
        public int player_id;
        public string token;
        public int minus_credit;
        public int minus_gem;
        public int minus_keylock;
    }

    public void LeaveGamePinalty ( int minCredit, int minGem = 0, int minKeylock = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            LeaveGamePinaltyParam param = new LeaveGamePinaltyParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.minus_credit = minCredit;
            param.minus_gem = minGem;
            param.minus_keylock = minKeylock;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, LeaveGamePinaltyResponse, "pinaltygame.php", paramJson, true));
        }
    }

    private void LeaveGamePinaltyResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RLeaveGamePinalty", post_data);
    }


    public void ChangeGender ( int gender = 1, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SetGenderParam param = new SetGenderParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.gender = gender;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, ChangeGenderResponse, "changegender.php", paramJson, true));
        }
    }

    private void ChangeGenderResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RChangeGender", post_data);
    }



    //Set Activity
    [Serializable]
    private class SetActivityParam
    {
        public int player_id;
        public string token;
        public int activity_id;
    }

    public void SetActivity ( int activityId, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SetActivityParam param = new SetActivityParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.activity_id = activityId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SetActivityResponse, "setactivity.php", paramJson, true));
        }
    }

    private void SetActivityResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSetActivity", post_data);
    }



    //Set Status
    [Serializable]
    private class SetStatusParam
    {
        public int player_id;
        public string token;
        public string status_msg;
    }

    public void SetStatus ( string status_msg = "", int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SetStatusParam param = new SetStatusParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.status_msg = status_msg;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SetStatusResponse, "setstatus.php", paramJson, true));
        }
    }

    private void SetStatusResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSetStatus", post_data);
    }



    //Set Phone Num
    [Serializable]
    private class SetPhoneNumParam
    {
        public int player_id;
        public string token;
        public string phone_num;
    }

    public void SetPhoneNum ( string phoneNum, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            SetPhoneNumParam param = new SetPhoneNumParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.phone_num = phoneNum;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, SetPhoneNumResponse, "setphonenum.php", paramJson, true));
        }
    }

    private void SetPhoneNumResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RSetPhoneNum", post_data);
    }



    //Get Temporary Profile
    public void GetTemporaryProfile ( int friendId )
    {
        StartCoroutine (Congest.ReadChatText (ParseError, GetTemporaryProfileResponse, Mathf.FloorToInt (friendId / 1000).ToString () + "/" + friendId.ToString () + "/profile.txt"));
    }

    private void GetTemporaryProfileResponse ( string post_data )
    {
        if (post_data.Length > 0) SendMessage ("RGetTemporaryProfile", post_data);
    }



    public void GetTemporaryProfileChat ( int friendId )
    {
        StartCoroutine (Congest.ReadChatText (ParseError, GetTemporaryProfileChatResponse, Mathf.FloorToInt (friendId / 1000).ToString () + "/" + friendId.ToString () + "/profile.txt"));
    }

    private void GetTemporaryProfileChatResponse ( string post_data )
    {
        if (post_data.Length > 0) SendMessage ("RGetTemporaryProfileChat", post_data);
    }



    public void GetTemporaryProfileLeaderboard ( int friendId )
    {
        StartCoroutine (Congest.ReadChatText (ParseError, GetTemporaryProfileLeaderboardResponse, Mathf.FloorToInt (friendId / 1000).ToString () + "/" + friendId.ToString () + "/profile.txt"));
    }

    private void GetTemporaryProfileLeaderboardResponse ( string post_data )
    {
        if (post_data.Length > 0) SendMessage ("RGetTemporaryProfileLeaderboard", post_data);
    }



    //=======================================================================================================================
    //Get Room
    [Serializable]
    private class GetRoomParam
    {
        public int player_id;
        public string token;
        public int game_type_id;
    }

    public void GetRoom ( int gameTypeId = 1, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetRoomParam param = new GetRoomParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.game_type_id = gameTypeId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (60, ParseError, GetRoomResponse, "getroom.php", paramJson, true));
        }
    }

    private void GetRoomResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetRoom", post_data);
    }



    //Get Slot
    [Serializable]
    private class GetSlotListParam
    {
        public int player_id;
        public string token;
        public int slot_currency;
    }

    public void GetSlotList ( int slotCurrency = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetSlotListParam param = new GetSlotListParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.slot_currency = slotCurrency;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetSlotListResponse, "getslotlist.php", paramJson, true));
        }
    }

    private void GetSlotListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetSlotList", post_data);
    }



    //Start Game
    [Serializable]
    private class StartGameParam
    {
        public int player_id;
        public string token;
        public string photon_room_id;
        public long room_bet_credit;
        public long room_bet_gem;
        public int player2_id;
        public int player3_id;
        public int player4_id;
    }

    public void StartGame ( string photonId, long roomBetCredit, long roomBetGem, int player2Id, int player3Id = 0, int player4Id = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            StartGameParam param = new StartGameParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.photon_room_id = photonId;
            param.room_bet_credit = roomBetCredit;
            param.room_bet_gem = roomBetGem;
            param.player2_id = player2Id;
            param.player3_id = player3Id;
            param.player4_id = player4Id;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, StartGameResponse, "startgame.php", paramJson, true));
        }
    }

    private void StartGameResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RStartGame", post_data);
    }


    //End Game
    [Serializable]
    private class EndGameParam
    {
        public int player_id;
        public string token;
        public int game_round_id;
        public long player_bet_credit;
        public long player_bet_gem;
        public int player_score;
        public ScoringType player_best_hand;
        public string player_card;
        public int player2_id;
        public long player2_bet_credit;
        public long player2_bet_gem;
        public int player2_score;
        public ScoringType player2_best_hand;
        public string player2_card;
        public int player3_id;
        public long player3_bet_credit;
        public long player3_bet_gem;
        public int player3_score;
        public ScoringType player3_best_hand;
        public string player3_card;
        public int player4_id;
        public long player4_bet_credit;
        public long player4_bet_gem;
        public int player4_score;
        public ScoringType player4_best_hand;
        public string player4_card;
        public int room_type_id;
        public int player_all_kill_id;
        public int event_id;
    }

    public void EndGame ( int gameRoundId, int player1Id, long playerBetCredit, long playerBetGem, int playerScore, ScoringType playerBestHand, string playerCard,
            int player2Id, long player2BetCredit, long player2BetGem, int player2Score, ScoringType player2BestHand, string player2Card,
            int roomTypeId = 0, int playerAllKillId = 0,
            int player3Id = 0, long player3BetCredit = 0, long player3BetGem = 0, int player3Score = 0, ScoringType player3BestHand = ScoringType.None, string player3Card = "",
            int player4Id = 0, long player4BetCredit = 0, long player4BetGem = 0, int player4Score = 0, ScoringType player4BestHand = ScoringType.None, string player4Card = "",
            int eventId = 0, int playerId = 0, string token = "", string otp = "" )
    {
        if (ParseToken (playerId, token))
        {
            if (playerId == 0) playerId = apiPlayerId;
            if (playerId == player2Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerScore, playerBestHand, playerCard,
                    player2Id, player2BetCredit, player2BetGem, player2Score, player2BestHand, player2Card,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerScore, out playerBestHand, out playerCard,
                    out player2Id, out player2BetCredit, out player2BetGem, out player2Score, out player2BestHand, out player2Card);
            }
            else if (playerId == player3Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerScore, playerBestHand, playerCard,
                    player3Id, player3BetCredit, player3BetGem, player3Score, player3BestHand, player3Card,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerScore, out playerBestHand, out playerCard,
                    out player3Id, out player3BetCredit, out player3BetGem, out player3Score, out player3BestHand, out player3Card);
            }
            else if (playerId == player4Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerScore, playerBestHand, playerCard,
                    player4Id, player4BetCredit, player4BetGem, player4Score, player4BestHand, player4Card,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerScore, out playerBestHand, out playerCard,
                    out player4Id, out player4BetCredit, out player4BetGem, out player4Score, out player4BestHand, out player4Card);
            }
            EndGameParam param = new EndGameParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.game_round_id = gameRoundId;
            param.player_bet_credit = playerBetCredit;
            param.player_bet_gem = playerBetGem;
            param.player_score = playerScore;
            param.player_best_hand = playerBestHand;
            param.player_card = playerCard;
            param.player2_id = player2Id;
            param.player2_bet_credit = player2BetCredit;
            param.player2_bet_gem = player2BetGem;
            param.player2_score = player2Score;
            param.player2_best_hand = player2BestHand;
            param.player2_card = player2Card;
            param.player3_id = player3Id;
            param.player3_bet_credit = player3BetCredit;
            param.player3_bet_gem = player3BetGem;
            param.player3_score = player3Score;
            param.player3_best_hand = player3BestHand;
            param.player3_card = player3Card;
            param.player4_id = player4Id;
            param.player4_bet_credit = player4BetCredit;
            param.player4_bet_gem = player4BetGem;
            param.player4_score = player4Score;
            param.player4_best_hand = player4BestHand;
            param.player4_card = player4Card;
            param.room_type_id = roomTypeId;
            param.player_all_kill_id = playerAllKillId;
            param.event_id = eventId;
            string paramJson = JsonUtility.ToJson (param);
            if (otp.Length > 0) apiOtp = otp;
            if (apiOtp.Length > 0)
            {
                StartCoroutine (Congest.SendPOST (0, ParseError, EndGameResponse, "endgame.php", paramJson, true, apiOtp));
            }
            else
            {
                ParseError (502);
            }
        }
    }

    private void EndGameResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("REndGame", post_data);
    }


    private void SwapMasterClient ( int _player1Id, long _playerBetCredit, long _playerBetGem, int _playerScore, ScoringType _playerBestHand, string _playerCard,
        int _player2Id, long _player2BetCredit, long _player2BetGem, int _player2Score, ScoringType _player2BestHand, string _player2Card,
        out int player1Id, out long playerBetCredit, out long playerBetGem, out int playerScore, out ScoringType playerBestHand, out string playerCard,
        out int player2Id, out long player2BetCredit, out long player2BetGem, out int player2Score, out ScoringType player2BestHand, out string player2Card )
    {
        player1Id = _player2Id;
        player2Id = _player1Id;
        playerBetCredit = _player2BetCredit;
        player2BetCredit = _playerBetCredit;
        playerBetGem = _player2BetGem;
        player2BetGem = _playerBetGem;
        playerScore = _player2Score;
        player2Score = _playerScore;
        playerBestHand = _player2BestHand;
        player2BestHand = _playerBestHand;
        playerCard = _player2Card;
        player2Card = _playerCard;
    }

    private void SwapMasterClient ( int _player1Id, long _playerBetCredit, long _playerBetGem, ScoringType _playerBestHand,
        int _player2Id, long _player2BetCredit, long _player2BetGem, ScoringType _player2BestHand,
        out int player1Id, out long playerBetCredit, out long playerBetGem, out ScoringType playerBestHand,
        out int player2Id, out long player2BetCredit, out long player2BetGem, out ScoringType player2BestHand )
    {
        player1Id = _player2Id;
        player2Id = _player1Id;
        playerBetCredit = _player2BetCredit;
        player2BetCredit = _playerBetCredit;
        playerBetGem = _player2BetGem;
        player2BetGem = _playerBetGem;
        playerBestHand = _player2BestHand;
        player2BestHand = _playerBestHand;
    }



    //Start Poker
    [Serializable]
    private class StartPokerParam
    {
        public int player_id;
        public string token;
        public string photon_room_id;
        public long room_bet_credit;
        public long room_bet_gem;
        public int player1_id;
        public int player2_id;
        public int player3_id;
        public int player4_id;
        public int player5_id;
        public int player6_id;
        public int player7_id;
        public int player8_id;
        public int player9_id;
        public int player10_id;
    }

    public void StartPoker ( string photonId, long roomBetCredit, long roomBetGem, int player1Id, int player2Id, int player3Id = 0, int player4Id = 0,
        int player5Id = 0, int player6Id = 0, int player7Id = 0, int player8Id = 0, int player9Id = 0, int player10Id = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            StartPokerParam param = new StartPokerParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.photon_room_id = photonId;
            param.room_bet_credit = roomBetCredit;
            param.room_bet_gem = roomBetGem;
            param.player1_id = player1Id;
            param.player2_id = player2Id;
            param.player3_id = player3Id;
            param.player4_id = player4Id;
            param.player5_id = player5Id;
            param.player6_id = player6Id;
            param.player7_id = player7Id;
            param.player8_id = player8Id;
            param.player9_id = player9Id;
            param.player10_id = player10Id;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, StartPokerResponse, "startpoker.php", paramJson, true));
        }
    }

    private void StartPokerResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RStartPoker", post_data);
    }



    //End Poker
    [Serializable]
    private class EndPokerParam
    {
        public int player_id;
        public string token;
        public int poker_round_id;
        public long player_bet_credit;
        public long player_bet_gem;
        public ScoringType player_best_hand;
        public int player2_id;
        public long player2_bet_credit;
        public long player2_bet_gem;
        public ScoringType player2_best_hand;
        public int player3_id;
        public long player3_bet_credit;
        public long player3_bet_gem;
        public ScoringType player3_best_hand;
        public int player4_id;
        public long player4_bet_credit;
        public long player4_bet_gem;
        public ScoringType player4_best_hand;
        public int player5_id;
        public long player5_bet_credit;
        public long player5_bet_gem;
        public ScoringType player5_best_hand;
        public int player6_id;
        public long player6_bet_credit;
        public long player6_bet_gem;
        public ScoringType player6_best_hand;
        public int player7_id;
        public long player7_bet_credit;
        public long player7_bet_gem;
        public ScoringType player7_best_hand;
        public int player8_id;
        public long player8_bet_credit;
        public long player8_bet_gem;
        public ScoringType player8_best_hand;
        public int player9_id;
        public long player9_bet_credit;
        public long player9_bet_gem;
        public ScoringType player9_best_hand;
        public int player10_id;
        public long player10_bet_credit;
        public long player10_bet_gem;
        public ScoringType player10_best_hand;
        public int room_type_id;
        public int event_id;
    }

    public void EndPoker ( int pokerRoundId, int player1Id, long playerBetCredit, long playerBetGem, ScoringType playerBestHand,
            int player2Id, long player2BetCredit, long player2BetGem, ScoringType player2BestHand, int roomTypeId = 0,
            int player3Id = 0, long player3BetCredit = 0, long player3BetGem = 0, ScoringType player3BestHand = ScoringType.None,
            int player4Id = 0, long player4BetCredit = 0, long player4BetGem = 0, ScoringType player4BestHand = ScoringType.None,
            int player5Id = 0, long player5BetCredit = 0, long player5BetGem = 0, ScoringType player5BestHand = ScoringType.None,
            int player6Id = 0, long player6BetCredit = 0, long player6BetGem = 0, ScoringType player6BestHand = ScoringType.None,
            int player7Id = 0, long player7BetCredit = 0, long player7BetGem = 0, ScoringType player7BestHand = ScoringType.None,
            int player8Id = 0, long player8BetCredit = 0, long player8BetGem = 0, ScoringType player8BestHand = ScoringType.None,
            int player9Id = 0, long player9BetCredit = 0, long player9BetGem = 0, ScoringType player9BestHand = ScoringType.None,
            int player10Id = 0, long player10BetCredit = 0, long player10BetGem = 0, ScoringType player10BestHand = ScoringType.None,
            int eventId = 0, int playerId = 0, string token = "", string otp = "" )
    {
        if (ParseToken (playerId, token))
        {
            if (playerId == 0) playerId = apiPlayerId;
            if (playerId == player2Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerBestHand,
                    player2Id, player2BetCredit, player2BetGem, player2BestHand,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerBestHand,
                    out player2Id, out player2BetCredit, out player2BetGem, out player2BestHand);
            }
            else if (playerId == player3Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerBestHand,
                    player3Id, player3BetCredit, player3BetGem, player3BestHand,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerBestHand,
                    out player3Id, out player3BetCredit, out player3BetGem, out player3BestHand);
            }
            else if (playerId == player4Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerBestHand,
                    player4Id, player4BetCredit, player4BetGem, player4BestHand,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerBestHand,
                    out player4Id, out player4BetCredit, out player4BetGem, out player4BestHand);
            }
            else if (playerId == player5Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerBestHand,
                    player5Id, player5BetCredit, player5BetGem, player5BestHand,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerBestHand,
                    out player5Id, out player5BetCredit, out player5BetGem, out player5BestHand);
            }
            else if (playerId == player6Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerBestHand,
                    player6Id, player6BetCredit, player6BetGem, player6BestHand,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerBestHand,
                    out player6Id, out player6BetCredit, out player6BetGem, out player6BestHand);
            }
            else if (playerId == player7Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerBestHand,
                    player7Id, player7BetCredit, player7BetGem, player7BestHand,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerBestHand,
                    out player7Id, out player7BetCredit, out player7BetGem, out player7BestHand);
            }
            else if (playerId == player8Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerBestHand,
                    player8Id, player8BetCredit, player8BetGem, player8BestHand,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerBestHand,
                    out player8Id, out player8BetCredit, out player8BetGem, out player8BestHand);
            }
            else if (playerId == player9Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerBestHand,
                    player9Id, player9BetCredit, player9BetGem, player9BestHand,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerBestHand,
                    out player9Id, out player9BetCredit, out player9BetGem, out player9BestHand);
            }
            else if (playerId == player10Id)
            {
                SwapMasterClient (player1Id, playerBetCredit, playerBetGem, playerBestHand,
                    player10Id, player10BetCredit, player10BetGem, player10BestHand,
                    out player1Id, out playerBetCredit, out playerBetGem, out playerBestHand,
                    out player10Id, out player10BetCredit, out player10BetGem, out player10BestHand);
            }
            EndPokerParam param = new EndPokerParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.poker_round_id = pokerRoundId;
            param.player_bet_credit = playerBetCredit;
            param.player_bet_gem = playerBetGem;
            param.player_best_hand = playerBestHand;
            param.player2_id = player2Id;
            param.player2_bet_credit = player2BetCredit;
            param.player2_bet_gem = player2BetGem;
            param.player2_best_hand = player2BestHand;
            param.player3_id = player3Id;
            param.player3_bet_credit = player3BetCredit;
            param.player3_bet_gem = player3BetGem;
            param.player3_best_hand = player3BestHand;
            param.player4_id = player4Id;
            param.player4_bet_credit = player4BetCredit;
            param.player4_bet_gem = player4BetGem;
            param.player4_best_hand = player4BestHand;
            param.player5_id = player5Id;
            param.player5_bet_credit = player5BetCredit;
            param.player5_bet_gem = player5BetGem;
            param.player5_best_hand = player5BestHand;
            param.player6_id = player6Id;
            param.player6_bet_credit = player6BetCredit;
            param.player6_bet_gem = player6BetGem;
            param.player6_best_hand = player6BestHand;
            param.player7_id = player7Id;
            param.player7_bet_credit = player7BetCredit;
            param.player7_bet_gem = player7BetGem;
            param.player7_best_hand = player7BestHand;
            param.player8_id = player8Id;
            param.player8_bet_credit = player8BetCredit;
            param.player8_bet_gem = player8BetGem;
            param.player8_best_hand = player8BestHand;
            param.player9_id = player9Id;
            param.player9_bet_credit = player9BetCredit;
            param.player9_bet_gem = player9BetGem;
            param.player9_best_hand = player9BestHand;
            param.player10_id = player10Id;
            param.player10_bet_credit = player10BetCredit;
            param.player10_bet_gem = player10BetGem;
            param.player10_best_hand = player10BestHand;
            param.room_type_id = roomTypeId;
            param.event_id = eventId;
            string paramJson = JsonUtility.ToJson (param);
            if (otp.Length > 0) apiOtp = otp;
            if (apiOtp.Length > 0)
            {
                StartCoroutine (Congest.SendPOST (0, ParseError, EndPokerResponse, "endpoker.php", paramJson, true, apiOtp));
            }
            else
            {
                ParseError (502);
            }
        }
    }

    private void EndPokerResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("REndPoker", post_data);
    }



    //=======================================================================================================================
    //Error Handling
    private string ParseResponse ( string inputJson )
    {
        ResponseParam response = new ResponseParam ();
        response = JsonUtility.FromJson<ResponseParam> (inputJson);
        if (response.error_code == 0)
        {
            return PostData (response.post_id, response.post_data);
        }
        else
        {
            ParseError (response.error_code, response.error_msg);
            return "";
        }
    }

    private void ParseError ( int errorCode, string[] errorMsg = null )
    {
        if (errorCode == 502)
        {
            errorMsg = new string[] { "API OTP is empty.", "API OTP kosong." };
        }
        else if (errorCode == 503)
        {
            errorMsg = new string[] { "API Player ID is empty.", "API Player ID kosong." };
        }
        else if (errorCode == 504)
        {
            errorMsg = new string[] { "API Token is empty.", "API Token kosong." };
        }
        ResponseParam response = new ResponseParam ();
        response.error_code = errorCode;
        response.error_msg = errorMsg;
        SendMessage ("RErrorHandler", response);
    }

    private bool ParseToken ( int playerId, string token )
    {
        if (playerId > 0) apiPlayerId = playerId;
        if (token.Length > 0) apiToken = token;
        if (apiPlayerId > 0 && apiToken.Length > 0)
        {
            return true;
        }
        else
        {
            if (apiPlayerId == 0)
            {
                ParseError (503);
            }
            else
            {
                ParseError (504);
            }
            return false;
        }
    }

    private string PostData ( string post_id, string post_data )
    {
        return (post_id.Length == 0 ? post_data : Digest.Read (post_data, post_id));
    }


    //=======================================================================================================================
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
        chatSessionUrl = "";
        if (friendId > 0) chatSessionUrl = (apiPlayerId < friendId ? Mathf.FloorToInt (apiPlayerId / 1000).ToString () : Mathf.FloorToInt (friendId / 1000).ToString ()) + "/" + (apiPlayerId < friendId ? apiPlayerId.ToString () : friendId.ToString ()) + "/" + (apiPlayerId > friendId ? apiPlayerId.ToString () : friendId.ToString ());
    }

    private void StartChatSessionResponse ( string post_data )
    {
        if (post_data.Length > 0)
        {
            try
            {
                long index = long.Parse (post_data);
                if (index > chatSession)
                {
                    chatSession = index;
                    if (chatSessionUrl.Length == 0)
                    {
                        PlayerPrefs.SetInt ("publicChatSession", (int) chatSession);
                        StartCoroutine (Congest.ReadChatText (ParseError, UpdateChatSessionResponse, "content.txt", "{\"chat\":[]}"));
                    }
                    else
                    {
                        PlayerPrefs.SetInt ("privateChatSession", (int) chatSession);
                        StartCoroutine (Congest.ReadChatText (ParseError, UpdateChatSessionResponse, chatSessionUrl + "content.txt", "{\"chat\":[]}"));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log (e.Message);
            }
        }
    }

    private void UpdateChatSessionResponse ( string post_data )
    {
        if (post_data.Length > 0)
        {
            string temp_data = "";
            string temp_data2 = "";
            int strpos = post_data.IndexOf ((chatSessionUrl.Length == 0 ? "{\"chat_public_id\":\"" : "{\"chat_private_id\":\"") + chatSession + "\"");
            if (strpos > 0)
            {
                temp_data2 = post_data.Substring (0, post_data.IndexOf ("{\"chat\":["));
                temp_data = post_data.Substring (strpos);
                post_data = temp_data2 + "{\"chat\":[" + temp_data;
            }
            SendMessage ("Update" + (chatSessionUrl.Length == 0 ? "Public" : "Private") + "ChatSession", post_data);
        }
    }

    private void GetChatSession ()
    {
        StartCoroutine (Congest.ReadChatText (ParseError, StartChatSessionResponse, chatSessionUrl + "/index.txt"));
    }

    public void EndChatSession ()
    {
        chatSession = -1;
        chatSessionUrl = "";
    }



    //Automation Announcement Session
    private int announcementSession = -1;
    private float refreshAnnouncementTime = 0.0f;

    private void StartAnnouncementSessionResponse ( string post_data )
    {
        if (post_data.Length > 0)
        {
            try
            {
                int index = int.Parse (post_data);
                if (index > announcementSession)
                {
                    announcementSession = index;
                    PlayerPrefs.SetInt ("announcementSession", announcementSession);
                    SendMessage ("UpdateAnnouncementSession", post_data);
                }
            }
            catch (Exception e)
            {
                Debug.Log (e.Message);
            }
        }
    }

    private void GetAnnouncementSession ()
    {
        StartCoroutine (Congest.ReadChatText (ParseError, StartAnnouncementSessionResponse, "iannouncement.txt"));
    }



    //Automation Set Absent
    private float refreshAbsentTime = 86400.0f;
    private float refreshInviteeTime = 86400.0f;
    private string absentRoom = "";

    public void RoomAbsent ( string photonId = "" )
    {
        absentRoom = photonId;
        refreshAbsentTime = 86400.0f;
    }

    private void SetAbsent ()
    {
        if (apiPlayerId > 0 && apiToken.Length > 0)
        {
            SearchFriendParam param = new SearchFriendParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.search_key = absentRoom;
            param.is_live = (Congest.DEVELOPMENT ? 0 : 1);
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, null, null, "setabsent.php", paramJson));
        }
    }

    //Get Absent
    public void GetAbsent ()
    {
        StartCoroutine (Congest.ReadChatText (ParseError, GetAbsentResponse, "absent.txt"));
    }

    private void GetAbsentResponse ( string post_data )
    {
        if (post_data.Length > 0) SendMessage ("RGetAbsent", "{\"absent\":" + post_data + "}");
    }

    //Get Invitee
    private void GetInvitee ()
    {
        if (apiPlayerId > 0) StartCoroutine (Congest.ReadChatText (ParseError, GetInviteeResponse, Mathf.FloorToInt (apiPlayerId / 1000).ToString () + "/" + apiPlayerId.ToString () + "/invitee.txt"));
    }

    private void GetInviteeResponse ( string post_data )
    {
        if (post_data.Length > 0)
        {
            SendMessage ("UpdateInviteeSession", post_data);
            ClearInvitee ();
        }
    }

    //Clear Invitee
    private void ClearInvitee ()
    {
        if (apiPlayerId > 0 && apiToken.Length > 0)
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.is_live = (Congest.DEVELOPMENT ? 0 : 1);
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, null, null, "clearinvitee.php", paramJson));
        }
    }



    //Get Invitee List
    public void GetInviteeList ( int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            TokenParam param = new TokenParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, GetInviteeListResponse, "getinviteelist.php", paramJson, true));
        }
    }

    private void GetInviteeListResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetInviteeList", post_data);
    }



    public void GetInviteeMission ( int friendId = 0, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetFriendDetailParam param = new GetFriendDetailParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.friend_id = friendId;
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (20, ParseError, GetInviteeMissionResponse, "getinviteemission.php", paramJson, true));
        }
    }

    private void GetInviteeMissionResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RGetInviteeMission", post_data);
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
                if (refreshChatTime >= 10.0f) //Set Refresh Rate
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
            //Absent Session
            refreshInviteeTime += Time.deltaTime;
            if (refreshInviteeTime >= 10.0f) //Check if got any invitation to join a room every 10 secs
            {
                refreshInviteeTime = 0.0f;
                GetInvitee ();
            }
            refreshAbsentTime += Time.deltaTime;
            if (refreshAbsentTime >= 60.0f) //Set your online status every 1 min
            {
                refreshAbsentTime = 0.0f;
                SetAbsent ();
            }
        }
    }



    //=======================================================================================================================
    //Get Store Param
    [Serializable]
    private class GetStoreParam
    {
        public int player_id;
        public string token;
        public int item_id;
        public PaymentType payment_type;
        public int is_live;
    }

    public string GetStoreLink ( int itemId, PaymentType paymentType, int playerId = 0, string token = "" )
    {
        string paramOtp = "";
        string paramJson = "";
        if (ParseToken (playerId, token))
        {
            GetStoreParam param = new GetStoreParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.item_id = itemId;
            param.payment_type = paymentType;
            param.is_live = (Congest.DEVELOPMENT ? 0 : 1);
            paramJson = JsonUtility.ToJson (param);
            paramOtp = Util.RandomChar (32);
            paramJson = Digest.Write (paramJson, paramOtp);
        }
        byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes (paramJson);
        paramJson = Convert.ToBase64String (bytesToEncode);
        return Congest.STOREDOMAIN + "?postid=" + paramOtp + "&postdata=" + paramJson;
    }


    //Get MCash Param
    [Serializable]
    private class GetMCashLinkParam
    {
        public int player_id;
        public string token;
        public string pin;
        public string serial;
        public string user_hp;
        public int is_live;
    }

    public string GetMCashLink ( string pin, string serial, string userHp, int playerId = 0, string token = "" )
    {
        string paramOtp = "";
        string paramJson = "";
        if (ParseToken (playerId, token))
        {
            GetMCashLinkParam param = new GetMCashLinkParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.pin = pin;
            param.serial = serial;
            param.user_hp = userHp;
            param.is_live = (Congest.DEVELOPMENT ? 0 : 1);
            paramJson = JsonUtility.ToJson (param);
            paramOtp = Util.RandomChar (32);
            paramJson = Digest.Write (paramJson, paramOtp);
        }
        byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes (paramJson);
        paramJson = Convert.ToBase64String (bytesToEncode);
        return Congest.DOMAIN + "mcash/mcashredeempin.php?postid=" + paramOtp + "&postdata=" + paramJson;
    }



    public void RedeemMCashPin ( string pin, string serial, string userHp, int playerId = 0, string token = "" )
    {
        if (ParseToken (playerId, token))
        {
            GetMCashLinkParam param = new GetMCashLinkParam ();
            param.player_id = apiPlayerId;
            param.token = apiToken;
            param.pin = pin;
            param.serial = serial;
            param.user_hp = userHp;
            param.is_live = (Congest.DEVELOPMENT ? 0 : 1);
            string paramJson = JsonUtility.ToJson (param);
            StartCoroutine (Congest.SendPOST (0, ParseError, RedeemMCashPinResponse, "redeemmcashpin.php", paramJson, true));
        }
    }

    private void RedeemMCashPinResponse ( string inputJson )
    {
        string post_data = ParseResponse (inputJson);
        if (post_data.Length > 0) SendMessage ("RRedeemMCashPin", post_data);
    }



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



    public string GetRestoreKey ( string restoreSKU, string restoreInvoice )
    {
        return Digest.Write (restoreSKU, restoreInvoice);
    }



    //Set Variable Function
    public void SetPlayerId ( int playerId ) { apiPlayerId = playerId; }
    public void SetToken ( string token ) { apiToken = token; }
    public void SetOtp ( string otp ) { apiOtp = otp; }
}

/*Notes
20  GetHome
60  GetNews     (awal login)
60  GetRoom(0)
20  GetDailyLoginRewardList (kalo belom claim hari ini dan awal login)
20  GetOneTimeQuestList => RGetOneTimeQuestList manggil GetDailyQuestList
20  GetDailyQuestList
20  GetAchievementList
20  GetFriendList
0   GetFriendDetail (kalo buka app dari deep link)
20  GetFriendRequest
20  GetFriendRequestMe
20  GetInviteeMission   (awal login)
60  GetChestList    (awal login)
60  GetIAPList (2)
0   GetShopList (1)
0   GetShopList (4) (awal login)
0   GetShopList (5) (awal login)
0   GetMailbox
-   GetPublicChat
-   GetConfiguration
-   GetAnnouncement
-   GetNewNotif     (awal login)
 */
