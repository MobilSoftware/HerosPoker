using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//using Newtonsoft.Json.Linq;

public enum GameMode
{
    Challenge,
    Levels,
}

public class GlobalVariables
{
    /// <summary>
    /// BuildSettings
    /// </summary>
    /// 
    //PlayRivals

    public static List<string> mySellableItemIDs = new List<string>();
    public static List<string> sellableItemIDs = new List<string>();
    public static int paymentAmount;
    public static int curAds = 0, curPromotions = 0;
    public static string curPromotionID = "";
    public static bool bDontRefresh, canButtonClick = true, bInGame = false, bQuitOnNextRound = false, bSwitchTableNextRound;
    public static string type = "", secondary_type = "";
    public static Hashtable custom_data, keyvalue_data, avaters_data = new Hashtable();

    public static bool bPaid = false;
    public static bool bAdMob = false;
    public static bool bAmazonStore = false;
    public static bool bDingaStore = true;
    public static bool bReduceAds = false;
    public static bool bIAP = true;
    public static bool bFacebookEnabled = true;
    public static bool bFontChanged = false;    
    public static bool bShowDebug = false;
    public static bool bUseWinMyanmarFont = false;
    
    public static bool bPrivateRoomChosen = false, bPlayWithFriendsChosen = false;
    public static float commision = 0.02f;
    public static string tournament_id = "", clanwar_id = "";

    public static ShopItemType shopItemType = ShopItemType.Gems;

    //Playrivals
    public static string curOoredooNum = "", curTelenorNum = "", curMPTNum = "", temp_code = "";
    public static bool bAuthorised = false, bUpdatedChecked = false;
    public static string dailyLoginBonusTime = "retrieving from the server";
    public static int timeElapsedInSeconds = 0;
    
    public static bool bShowingChallengeBox = false;
    public static bool bShowingMoreGamesBox = false;
    public static bool bShowingPopupBox = false;
    public static bool bShowingSecondaryPopupBox = false;
    public static bool bBannerAdsExists = false;

    public static List<int> twoToOne01 = new List<int>();
    public static List<int> twoToOne02 = new List<int>();
    public static List<int> twoToOne03 = new List<int>();
    public static List<int> redNumbers = new List<int>();
    public static List<int> blackNumbers = new List<int>();

    //Global room options
    public static ControllerType controllerType = ControllerType.Player;
    public static RoomOptions roomOptions = new RoomOptions();
    public static ExitGames.Client.Photon.Hashtable roomData = new ExitGames.Client.Photon.Hashtable();
    public static EnvironmentType environment = EnvironmentType.None;
    
    public static ScreenAspectRatio aspectRatio = ScreenAspectRatio.WideScreen_16x9;
    public static int touchIndex = -1;

    public static float ScreenWidth = 19.20f;
    public static float ScreenHeight = 10.80f;
    public static float transitionTime = 0.5f;

    public static int curLevel = 0, mySellableItemIndex = -1;
    public static bool bGameStarted = false;
    public static bool bGamePaused = false;
    public static bool bPlayerDead = false;
    public static bool bChoosingBadges = false;

    //Betting Settings
    public static int playerBetMoney = 0;
    public static int maxMoney = 0;
    public static long MinBetAmount = 20;
    public static int multiplierMinBet = 30;
    public static int curBetAmount = 0;
    public static int finalBetAmunt = 0;
    public static int moneyPaidForTheRound = 0;
    public static int giftPointsWon = 0, finalMoney = 0;

    //Game Global Settings
    public static bool hasDebtForRound = false;
    public static bool bIsCoins = true;
    public static bool bIsPassword = false;
    public static bool bSoundMute = false;
    public static bool showRewards = false;

    public static GameMode gameMode = GameMode.Levels;
    public static bool FBLoggingIn = false;
    public static ShopItemType lastPurchaseType = ShopItemType.None;
   
    public static GameType gameType = GameType.TexasPoker;

    public static TypedLobby sqlLobbyCapsaSusun = new TypedLobby("Poker", LobbyType.SqlLobby);
}

