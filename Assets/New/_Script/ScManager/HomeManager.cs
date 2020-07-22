using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HomeManager : MonoBehaviour
{
    private static HomeManager s_Instance = null;
    public static HomeManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (HomeManager)) as HomeManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an HomeManager object. \n You have to have exactly one HomeManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Sprite sprCoin;
    public Sprite sprCoupon;
    public Button btnProfile;
    public Button btnShop;
    public Button btnQuickPlay;
    public Button btnPoker;
    public Button btnMinigame;
    public Button btnHideMinigame;
    public Button btnOriental;
    public Button btnWestern;
    public Button btnSicbo;
    public Button btnOthers;
    public Button btnParentOthers;
    public Button btnSettings;
    public Button btnHero;
    public Button btnFreeCoin;
    public Button btnHideFreeCoin;
    public Button btnFriend;
    public Button btnLeaderboard;
    public Button btnPromoCode;
    public Button btnInbox;
    public Button btnTransfer;
    public Button btnDailyRewards;
    public Button btnWeeklyRewards;
    public Button btnMoneySlot;
    public Button btnWatchAds;
    public Button btnDailyQuest;
    public Button btnChat;
    public GameObject objNotifInbox;
    public GameObject objNotifFriend;
    public TextMeshProUGUI tmpDisplayName;
    public TextMeshProUGUI tmpCoin;
    public TextMeshProUGUI tmpCoupon;
    public StandHero standHero;
    public Transform parentMinichat;
    public ItemMinichat prefabItemMinichat;
    public InputField invisIPF;
    public ScrollRect scrRect;
    public RunningText runningText;

    [HideInInspector]
    public JGetChatPublic json;
    private Coroutine crSetMinichat;
    private Coroutine crAddPublicChat;

    //public _SpineObject spStandCleo;
    //public _SpineObject spStandLubu;

    private void Start ()
    {
        btnChat.onClick.AddListener (OnChat);
        invisIPF.onEndEdit.AddListener (OnSubmit);
        btnProfile.onClick.AddListener (OnProfile);
        btnQuickPlay.onClick.AddListener (OnQuickPlay);
        btnPoker.onClick.AddListener (OnPokerRoom);
        btnMinigame.onClick.AddListener (OnMinigame);
        btnShop.onClick.AddListener (OnShop);
        btnHideMinigame.onClick.AddListener (OnHideGames);
        btnOriental.onClick.AddListener (OnOriental);
        btnWestern.onClick.AddListener (OnWestern);
        btnSicbo.onClick.AddListener (OnSicbo);
        btnParentOthers.onClick.AddListener (OnHideOthers);
        btnSettings.onClick.AddListener (OnSettings);
        btnOthers.onClick.AddListener (OnOthers);
        btnFreeCoin.onClick.AddListener (OnFreeCoin);
        btnHideFreeCoin.onClick.AddListener (OnHideFreeCoin);
        btnHero.onClick.AddListener (OnHero);
        btnFriend.onClick.AddListener (OnFriend);
        btnLeaderboard.onClick.AddListener (OnLeaderboard);
        btnPromoCode.onClick.AddListener (OnPromoCode);
        btnInbox.onClick.AddListener (OnInbox);
        btnTransfer.onClick.AddListener (OnTransfer);
        btnDailyRewards.onClick.AddListener (OnDailyRewards);
        btnWeeklyRewards.onClick.AddListener (OnWeeklyRewards);
        btnWatchAds.onClick.AddListener (OnWatchAds);
        btnDailyQuest.onClick.AddListener (OnDailyQuest);
        btnMoneySlot.onClick.AddListener (OnMoneySlot);
    }

    private void OnWatchAds ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.WATCH_ADS, true);
    }

    private void OnDailyQuest ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.DAILY_QUEST, true);
    }

    private void OnChat ()
    {
        invisIPF.ActivateInputField ();
    }

    private void OnSubmit (string strContent )
    {
        if (strContent.Length > 0)
        {
            ApiManager.instance.SendChat (strContent);
        }
    }

    private void OnMoneySlot ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.MONEY_SLOT, true);
    }

    private void OnDailyRewards ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.DAILY_REWARDS, true);
    }

    private void OnWeeklyRewards ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.WEEKLY_REWARDS, true);
    }

    private void OnFriend ()
    {
        objNotifFriend.SetActive (false);
        _SceneManager.instance.SetActiveScene (SceneType.FRIEND, true);
    }

    private void OnLeaderboard ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.LEADERBOARD, true);
    }

    private void OnTransfer()
    {
        _SceneManager.instance.SetActiveScene (SceneType.TRANSFER, true);
    }

    private void OnPromoCode ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.REDEEM, true);
    }

    private void OnInbox ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.INBOX, true);
    }

    private void OnHero ()
    {
        Hide ();
        _SceneManager.instance.SetActiveScene (SceneType.HERO, true);
    }

    private void OnFreeCoin ()
    {
        btnHideFreeCoin.gameObject.SetActive (true);
    }

    private void OnHideFreeCoin ()
    {
        btnHideFreeCoin.gameObject.SetActive (false);
    }

    private void OnProfile ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.PROFILE, true);
    }

    private void OnShop()
    {
        _SceneManager.instance.SetActiveScene (SceneType.SHOP, true);
    }

    private void OnQuickPlay ()
    {
        //_SceneManager.instance.SetActiveScene (SceneType.POKER, true);
        //Hide ();
        PokerRoomManager.instance.OnQuickPlay ();
    }

    private void OnPokerRoom ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.POKER_ROOM, true);
    }

    private void OnMinigame ()
    {
        //_SceneManager.instance.SetActiveScene (SceneType.SLOTO, true);
        //Hide ();

        btnHideMinigame.gameObject.SetActive (true);
    }

    private void OnHideGames()
    {
        btnHideMinigame.gameObject.SetActive (false);
    }

    private void OnOriental ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.SLOTO, true, 1);
        Hide ();
    }

    private void OnWestern ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.SLOTO, true, 2);
        Hide ();
    }

    private void OnSicbo ()
    {
        GlobalVariables.gameType = GameType.Sicbo;
        OnHideGames ();
        Hide ();
        _SceneManager.instance.SetActiveScene (SceneType.SICBO, true);
    }

    private void OnOthers()
    {
        btnParentOthers.gameObject.SetActive (true);
    }

    private void OnHideOthers()
    {
        btnParentOthers.gameObject.SetActive (false);
    }

    private void OnSettings()
    {
        _SceneManager.instance.SetActiveScene (SceneType.SETTINGS, true);
    }

    public void SetCanvas (bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    public void Show ()
    {
        //PhotonTexasPokerManager.instance.isPhotonFire = false;
        SicboManager.instance.isPhotonFire = false;
        UpdateCoinAndCoupon ();
        canvas.enabled = true;
        _SceneManager.instance.activeSceneType = SceneType.HOME;
    }

    public void Init ()
    {
        canvas.sortingOrder = (int) SceneType.HOME;
        tmpDisplayName.text = PlayerData.display_name;

        //standHero.LoadSpine (PlayerData.hero_id);
        standHero.LoadFromBundle (PlayerData.costume_id);
    }

    public void Hide ()
    {
        canvas.enabled = false;
    }

    public void UpdateCoinAndCoupon ()
    {
        tmpCoin.text = PlayerData.owned_coin.toShortCurrency ();
        tmpCoupon.text = PlayerData.owned_coupon.toCouponShortCurrency ();
    }

    public void SetJson (JGetChatPublic _json)
    {
        parentMinichat.gameObject.SetActive (false);
        json = _json;
        for (int i = 0; i < parentMinichat.childCount; i++)
        {
            Destroy (parentMinichat.GetChild (i).gameObject);
        }
        parentMinichat.gameObject.SetActive (true);
        if (crSetMinichat != null)
            StopCoroutine (crSetMinichat);
        crSetMinichat = StartCoroutine (_SetMinichat ());
    }

    IEnumerator _SetMinichat ()
    {

        for (int x = 0; x < json.chat.Length; x++)
        {
            ItemMinichat imc = Instantiate (prefabItemMinichat, parentMinichat);
            yield return _WFSUtility.wef;
            imc.SetData (json.chat[x]);
        }

        scrRect.verticalNormalizedPosition = 0f;
        yield return _WFSUtility.wef;
        parentMinichat.gameObject.SetActive (false);
        yield return _WFSUtility.wef;
        scrRect.verticalNormalizedPosition = 0f;
        parentMinichat.gameObject.SetActive (true);
        yield return _WFSUtility.wef;
        scrRect.verticalNormalizedPosition = 0f;
    }

    public void AddPublicChat (JPublicChat[] chat )
    {
        if (crAddPublicChat != null)
            StopCoroutine (crAddPublicChat);
        crAddPublicChat = StartCoroutine (_AddPublicChat (chat));
    }

    IEnumerator _AddPublicChat (JPublicChat[] chat )
    {
        Logger.E ("adding chat");
        for (int i = 0; i < chat.Length; i++)
        {
            ItemMinichat imc = Instantiate (prefabItemMinichat, parentMinichat);
            yield return _WFSUtility.wef;
            imc.SetData (chat[i]);
        }

        scrRect.verticalNormalizedPosition = 0f;
        yield return _WFSUtility.wef;
        parentMinichat.gameObject.SetActive (false);
        yield return _WFSUtility.wef;
        scrRect.verticalNormalizedPosition = 0f;
        parentMinichat.gameObject.SetActive (true);
        yield return _WFSUtility.wef;
        scrRect.verticalNormalizedPosition = 0f;
    }

    public int GetLastPublicChatID ()
    {
        return json.chat[json.chat.Length - 1].chat_public_id;
    }
}
