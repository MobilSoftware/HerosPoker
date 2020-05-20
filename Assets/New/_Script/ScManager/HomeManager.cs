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
    public Button btnSloto;
    public Button btnTempLogout;
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
        btnSloto.onClick.AddListener (OnSloto);
        btnTempLogout.onClick.AddListener (OnLogout);
        btnShop.onClick.AddListener (OnShop);
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

    private void OnSloto ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.SLOTO, true);
        Hide ();
    }

    private void OnLogout()
    {
        FacebookManager.instance.Logout ();
        Hide ();
        PlayerPrefs.DeleteAll ();
        _SceneManager.instance.SetActiveScene (SceneType.LOGIN, true);
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
