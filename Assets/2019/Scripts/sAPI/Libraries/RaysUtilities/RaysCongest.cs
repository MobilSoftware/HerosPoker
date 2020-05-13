using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Rays.Utilities
{
    public static class Congest
    {
        public const bool DEVELOPMENT = true;

        public const string DOMAIN = "http://194.59.165.164/";
        public const string APIVER = "v1.0";
        public const string APIURI = DOMAIN + (DEVELOPMENT ? "dev" : "api") + "/" + APIVER + "/";
        public const string CHATURI = DOMAIN + (DEVELOPMENT ? "dev" : "api") + "/temp/";
        //public const string STOREDOMAIN = "http://myplay.id/store_webview.php";

        public static IEnumerator SendPOST ( ApiBridge apiBridge, ApiBridge.API api )
        {
            System.DateTime epochStart = new System.DateTime (2019, 11, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int curTime = (int) (System.DateTime.UtcNow - epochStart).TotalSeconds;
            if (curTime - PlayerPrefs.GetInt (api.uri + "_time", 0) < api.cacheTime && PlayerPrefs.GetString (api.uri + "_value", "") != "")
            {
                PlayerPrefs.SetInt (api.uri + "_time", curTime);
                api.jsonReturn = PlayerPrefs.GetString (api.uri + "_value", "");
                apiBridge.GoodResponse (api);
            }
            else
            {
                WWWForm form = new WWWForm ();
                if (api.jsonData.Length > 0)
                {
                    if (api.isDigested)
                    {
                        string postid = (api.otp.Length == 0 ? Util.RandomChar (32) : api.otp);
                        form.AddField ("post_id", postid.Trim ());
                        form.AddField ("post_time", System.DateTime.Now.ToString ("yyMMdd-HHmmss").Trim ());
                        api.jsonData = Digest.Write (api.jsonData, postid);
                    }
                    form.AddField ("post_data", api.jsonData.Trim ());
                }

                using (UnityWebRequest www = UnityWebRequest.Post (APIURI + api.uri, form))
                {
                    www.timeout = 30;
                    www.chunkedTransfer = false;
                    yield return www.SendWebRequest ();
                    if (www.isNetworkError || www.isHttpError)
                    {
                        string[] tempError = ParseUnityError (www.error);
                        if (DEVELOPMENT) tempError[0] = api.uri + " = " + www.error;
                        if (DEVELOPMENT) Debug.Log (tempError[0]);
                        api.errorCode = (www.isNetworkError ? 500 : 501);
                        api.errorMsg = new string[] { tempError[0], tempError[1] };
                        apiBridge.ParseError (api);
                    }
                    else
                    {
                        //if (DEVELOPMENT) Debug.Log(api.uri + " response return = " + www.downloadHandler.text);
                        curTime = (int) (System.DateTime.UtcNow - epochStart).TotalSeconds;
                        PlayerPrefs.SetInt (api.uri + "_time", curTime);
                        PlayerPrefs.SetString (api.uri + "_value", www.downloadHandler.text);
                        api.jsonReturn = www.downloadHandler.text;
                        apiBridge.GoodResponse (api);
                    }
                }
            }
        }

        public static IEnumerator ReadChatText ( ApiBridge.ErrorDelegate returnError, ApiBridge.ResponseDelegate returnPost, string uri, string defaultNotFound = "" )
        {
            using (UnityWebRequest www = UnityWebRequest.Get (CHATURI + uri))
            {
                www.timeout = 30;
                www.chunkedTransfer = false;
                yield return www.SendWebRequest ();
                if (www.isNetworkError || www.isHttpError)
                {
                    if (www.isHttpError && !www.isNetworkError)
                    {
                        returnPost (defaultNotFound, uri);
                    }
                    else
                    {
                        string[] tempError = ParseUnityError (www.error);
                        returnError ((www.isNetworkError ? 500 : 501), new string[] { tempError[0], tempError[1] }, uri);
                    }
                }
                else
                {
                    returnPost (www.downloadHandler.text, uri);
                }
            }
        }

        private static string[] ParseUnityError ( string msg )
        {
            string[] returnMsg = new string[2];
            if (msg == "Cannot resolve destination host")
            {
                returnMsg[0] = "_No Internet Connection";
                returnMsg[1] = "Tidak ada koneksi Internet";
            }
            else
            {
                returnMsg[0] = msg;
                returnMsg[1] = msg;
            }
            return returnMsg;
        }
    }
}