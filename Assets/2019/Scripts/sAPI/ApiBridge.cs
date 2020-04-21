using UnityEngine;
using Rays.Utilities;
using System;
using System.Text;

public class ApiBridge : MonoBehaviour
{
    public delegate void ResponseDelegate ( string inputJson, string uri );
    public delegate void ErrorDelegate ( int errorCode, string[] errorMsg, string errorUri );

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
        public string error_uri = "";
        public string post_id = "";
        public string post_data = "";
    }


    //Get Version
    public void GetVersion ()
    {
        StartCoroutine (Congest.SendPOST (0, ParseError, GetVersionResponse, "getversion.php"));
    }

    private void GetVersionResponse ( string inputJson, string uri )
    {
        string post_data = ParseResponse (inputJson, uri);
        if (post_data.Length > 0) SendMessage ("RGetVersion", post_data);
    }




    //=======================================================================================================================
    //Error Handling
    private string ParseResponse ( string inputJson, string uri )
    {
        ResponseParam response = new ResponseParam ();
        response = JsonUtility.FromJson<ResponseParam> (inputJson);
        if (response.error_code == 0)
        {
            return PostData (response.post_id, response.post_data);
        }
        else
        {
            ParseError (response.error_code, response.error_msg, uri);
            return "";
        }
    }

    private void ParseError ( int errorCode, string[] errorMsg = null, string uri = "" )
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
        response.error_uri = uri;
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


    public int GetUriEnum ( string uri )
    {
        string[] uriEnum = { "redeemmcashpin.php", "redeemmcashpin.php" };
        for (int i = 0; i < uriEnum.Length; i++)
        {
            if (uri == uriEnum[i]) return i;
        }
        return -1;
    }

    //Set Variable Function
    public void SetPlayerId ( int playerId ) { apiPlayerId = playerId; }
    public void SetToken ( string token ) { apiToken = token; }
    public void SetOtp ( string otp ) { apiOtp = otp; }
}