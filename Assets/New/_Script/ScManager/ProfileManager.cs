using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    private static ProfileManager s_Instance = null;
    public static ProfileManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (ProfileManager)) as ProfileManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an ProfileManager object. \n You have to have exactly one ProfileManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Text txtDisplayName;
    public Text txtLevel;
    public Text txtCoinValue;
    public Text txtCouponValue;
    public Text txtVerify;
    public Text txtTag;
    public Button btnClose;
    public Button btnVerify;
    public Button btnVIP;
    public StandHero standHero;

    private SceneType prevSceneType;
    private bool isInit;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
        btnVerify.onClick.AddListener (OnVerify);
        btnVIP.onClick.AddListener (OnVIP);
    }

    private void OnVerify ()
    {
        //_SceneManager.instance.SetActiveScene (SceneType.VERIFY, true);
    }

    private void OnVIP ()
    {
        //_SceneManager.instance.SetActiveScene (SceneType.VIP, true);
    }

    public void SetCanvas ( bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    public void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.PROFILE;
            isInit = true;
        }

        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.PROFILE;
        txtDisplayName.text = PlayerData.display_name;
        UpdateCoinAndCoupon ();
        txtTag.text = "Tag: " + PlayerData.tag;
        standHero.LoadFromBundle (PlayerData.costume_id);
    }

    public void Hide ()
    {
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }

    public void UpdateCoinAndCoupon ()
    {
        txtCoinValue.text = PlayerData.owned_coin.toShortCurrency ();
        txtCouponValue.text = PlayerData.owned_coupon.toCouponShortCurrency ();
    }
}
