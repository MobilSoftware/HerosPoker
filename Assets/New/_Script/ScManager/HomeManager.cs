using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public TextMeshProUGUI tmpDisplayName;
    public TextMeshProUGUI tmpCoin;
    public TextMeshProUGUI tmpCoupon;
    public StandHero standHero;


    //public _SpineObject spStandCleo;
    //public _SpineObject spStandLubu;

    private void Start ()
    {
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
    }

    private void OnFriend ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.FRIEND, true);
    }

    private void OnLeaderboard ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.LEADERBOARD, true);
    }

    private void OnHero ()
    {
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
        _SceneManager.instance.SetActiveScene (SceneType.SLOTO, true);
        Hide ();
    }

    private void OnWestern ()
    {
        //_SceneManager.instance.SetActiveScene (SceneType.SLOTO, true);
        //Hide ();
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
}
