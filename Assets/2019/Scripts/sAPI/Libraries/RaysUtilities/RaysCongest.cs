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

        private static bool useNew = true;

        public static IEnumerator SendPOST ( int cacheTime, ApiBridge.ErrorDelegate returnError, ApiBridge.ResponseDelegate returnPost, string uri, string jsonData = "", bool isDigested = false, string otp = "" )
        {
            System.DateTime epochStart = new System.DateTime (2019, 11, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int curTime = (int) (System.DateTime.UtcNow - epochStart).TotalSeconds;
            if (curTime - PlayerPrefs.GetInt (uri + "_time", 0) < cacheTime && PlayerPrefs.GetString (uri + "_value", "") != "")
            {
                //Debug.Log("Masuk 1");
                PlayerPrefs.SetInt (uri + "_time", curTime);
                returnPost (PlayerPrefs.GetString (uri + "_value", ""), uri);
            }
            else
            {
                //Debug.Log("Masuk 2");
                WWWForm form = new WWWForm ();
                if (jsonData.Length > 0)
                {
                    if (isDigested)
                    {
                        string postid = (otp.Length == 0 ? Util.RandomChar (32) : otp);
                        form.AddField ("post_id", postid.Trim ());
                        form.AddField ("post_time", System.DateTime.Now.ToString ("yyMMdd-HHmmss").Trim ());
                        jsonData = Digest.Write (jsonData, postid);
                    }
                    form.AddField ("post_data", jsonData.Trim ());
                }

                if (useNew)
                {
                    using (UnityWebRequest www = UnityWebRequest.Post (APIURI + uri, form))
                    {
                        www.timeout = 30;
                        www.chunkedTransfer = false;
                        yield return www.SendWebRequest ();
                        if (www.isNetworkError || www.isHttpError)
                        {
                            string[] tempError = ParseUnityError (www.error);
                            if (DEVELOPMENT) tempError[0] = uri + " = " + www.error;
                            if (DEVELOPMENT) Debug.Log (tempError[0]);
                            if (returnError != null) returnError ((www.isNetworkError ? 500 : 501), new string[] { tempError[0], tempError[1] }, uri);
                        }
                        else
                        {
                            if (DEVELOPMENT) Debug.Log (uri + " = " + www.downloadHandler.text);
                            if (returnPost != null)
                            {
                                curTime = (int) (System.DateTime.UtcNow - epochStart).TotalSeconds;
                                PlayerPrefs.SetInt (uri + "_time", curTime);
                                PlayerPrefs.SetString (uri + "_value", www.downloadHandler.text);
                                returnPost (www.downloadHandler.text, uri);
                            }
                        }
                    }
                }
                else
                {
                    using (WWW www = new WWW (APIURI + uri, form))
                    {
                        yield return www;
                        if (!string.IsNullOrEmpty (www.error))
                        {
                            string[] tempError = ParseUnityError (www.error);
                            if (DEVELOPMENT) tempError[0] = uri + " = " + www.error;
                            if (DEVELOPMENT) Debug.Log (tempError[0]);
                            if (returnError != null) returnError (501, new string[] { tempError[0], tempError[1] }, uri);
                        }
                        else
                        {
                            if (DEVELOPMENT) Debug.Log (uri + " = " + www.text);
                            if (returnPost != null)
                            {
                                curTime = (int) (System.DateTime.UtcNow - epochStart).TotalSeconds;
                                PlayerPrefs.SetInt (uri + "_time", curTime);
                                PlayerPrefs.SetString (uri + "_value", www.text);
                                returnPost (www.text, uri);
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerator ReadChatText ( ApiBridge.ErrorDelegate returnError, ApiBridge.ResponseDelegate returnPost, string uri, string defaultNotFound = "" )
        {
            if (useNew)
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
            else
            {
                using (WWW www = new WWW (CHATURI + uri))
                {
                    yield return www;
                    if (!string.IsNullOrEmpty (www.error))
                    {
                        if (www.error != "404 Not Found")
                        {
                            string[] tempError = ParseUnityError (www.error);
                            returnError (501, new string[] { tempError[0], tempError[1] }, uri);
                        }
                        else
                        {
                            returnPost (defaultNotFound, uri);
                        }
                    }
                    else
                    {
                        returnPost (www.text, uri);
                    }
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