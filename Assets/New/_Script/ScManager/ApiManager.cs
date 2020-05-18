using System;
using System.Collections;
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
        Debug.LogError ("RErrorHandler from " + error.uri + " (Seed #" + error.seed.ToString ()
                    + ")\n(Code #" + error.error_code + ") {" + error.error_msg[0] + " || " + error.error_msg[1] + "}");
        MessageManager.instance.Show (this.gameObject, error.error_msg[1], ButtonMode.OK);
    }

    public void GetVersion ()
    {
        int playerID = PlayerPrefs.GetInt (PrefEnum.PLAYER_ID.ToString (), 0);
        string token = PlayerPrefs.GetString (PrefEnum.TOKEN.ToString (), string.Empty);
        if (playerID != 0 && token != string.Empty)
            api.GetVersion (playerID, token);
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
        api.UserLogin (ApiBridge.LoginType.Guest);
    }

    private void RUserLogin ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return User Login: " + response.post_data);
        JUserLogin json = JsonUtility.FromJson<JUserLogin> (response.post_data);
        api.SetPlayerId (json.player.player_id);
        api.SetToken (json.player.token);
        PlayerData.display_name = json.player.display_name;
        PlayerData.id = json.player.player_id;
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
        Logger.E (json.player.costume_equiped + " costume");
        if (json.player.costume_equiped == 0)
            BeginManager.instance.Show ();
        else
        {
            HomeManager.instance.Init ();
            HomeManager.instance.Show ();
        }
    }

    public void GetProfile (int friendID = 0)
    {
        //friendID = 0 means get my profile
        api.GetProfile (friendID);
    }

    private void RGetProfile ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Get Profile: " + response.post_data);
        JGetProfile json = JsonUtility.FromJson<JGetProfile> (response.post_data);
    }

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
        BeginManager.instance.Hide ();
        HomeManager.instance.Init ();
        HomeManager.instance.Show ();
    }

    public void SetCostume (int costumeID = 0 )
    {
        //costumeID = 0 means default costume
        api.SetCostume (costumeID);
    }

    private void RSetCostume ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Set Costume: " + response.post_data);
    }

    #region gameplay
    public void StartPoker (string _photonRoomID, long _roomBetCoin)
    {
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
        PokerData.Setup (json.poker_round_id, json.room_bet_coin, json.cards, json.otp);
        api.SetOtp (json.otp);

        for (int i = 0; i < json.players.Length; i++)
        {
            if (json.players[i].player_id == PlayerData.id)
            {
                if (json.players[i].kick)
                    PhotonTexasPokerManager.instance.KickFromServer ();
                else
                {
                    long lCoin = Convert.ToInt64 (json.players[i].coin_server);
                    if (lCoin <= GlobalVariables.MinBetAmount * 200)
                        PhotonTexasPokerManager.instance.SyncCoinFromServer (lCoin);
                }
            }
        }

        //sync poker players among master and non masters
        ApiBridge.PokerPlayer[] serverPlayers = new ApiBridge.PokerPlayer[8];
        for (int x = 0; x < json.players.Length; x++)
        {
            serverPlayers[x] = new ApiBridge.PokerPlayer ();
            long coinPlayer = Convert.ToInt64 (json.players[x].coin_before);
            serverPlayers[x].Start (json.players[x].seater_id, json.players[x].player_id, coinPlayer);
            long coinServer = Convert.ToInt64 (json.players[x].coin_server);
            serverPlayers[x].Update (coinServer, json.players[x].kick);
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
    }

    public void StartSicbo ()
    {
        api.StartSicbo (sicboPlayers);
    }

    private void RStartSicbo ( ApiBridge.ResponseParam response )
    {
        Logger.E ("Return Start Sicbo: " + response.post_data);
    }
    #endregion
}
