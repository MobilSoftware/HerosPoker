using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System.Text.RegularExpressions;

public class FacebookManager : MonoBehaviour
{
    private static FacebookManager s_Instance = null;

    public static FacebookManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (FacebookManager)) as FacebookManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an FacebookManager object. \n You have to have exactly one FacebookManager in the scene.");
            }
            return s_Instance;
        }
    }

    private Dictionary<string, object> FBUserDetails;

    private void Awake ()
    {
        if (!FB.IsInitialized)
        {
            FB.Init (InitCallbackFB, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp ();
        }
    }

    private void InitCallbackFB ()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp ();
        }
    }

    private void OnHideUnity ( bool isGameShown )
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void Logout ()
    {
        if (FB.IsLoggedIn)
            FB.LogOut ();
    }

    public void Login ()
    {
        var perms = new List<string> () { "public_profile", "email" };
        FB.LogInWithReadPermissions (perms, AuthCallbackFB);
    }

    private void AuthCallbackFB ( ILoginResult result )
    {
        if (FB.IsLoggedIn)
        {
            FB.API ("/me?fields=id,name,email,picture{url}", HttpMethod.GET, FetchProfileCallbackFB, new Dictionary<string, string> ());
        }
    }

    private void FetchProfileCallbackFB ( IGraphResult result )
    {
        //uiLogin.Hide ();
        Logger.E ("FB: " + result.RawResult);
        FBUserDetails = (Dictionary<string, object>) result.ResultDictionary;
        Dictionary<string, object> pictureDetails = (Dictionary<string, object>) FBUserDetails["picture"];
        Dictionary<string, object> pictureData = (Dictionary<string, object>) pictureDetails["data"];

        string fb_id = FBUserDetails["id"].ToString ();
        string fb_email = string.Empty;
        if (FBUserDetails.ContainsKey ("email"))
        {
            fb_email = FBUserDetails["email"].ToString ();
        }
        else
        {
            string deviceID = SystemInfo.deviceUniqueIdentifier;
            string cleanDeviceID = RemoveSpecialCharacters (deviceID);
            if (cleanDeviceID.Length > 50)
                cleanDeviceID = cleanDeviceID.Substring (0, 50);

            fb_email = cleanDeviceID + "@empty.fb";
        }

        string fb_name = FBUserDetails["name"].ToString ();
        string fb_url = "https://graph.facebook.com/{0}/picture?type=normal";
        fb_url = string.Format (fb_url, fb_id);
        ApiManager.instance.FacebookLogin (fb_id, fb_email, fb_name, fb_url);
    }

    public string RemoveSpecialCharacters ( string str )
    {
        return Regex.Replace (str, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
    }
}
