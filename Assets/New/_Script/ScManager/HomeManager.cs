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
    public Button btnQuickPlay;
    public Button btnPoker;
    public Button btnSloto;
    public TextMeshProUGUI tmpDisplayName;
    public TextMeshProUGUI tmpCoin;
    public StandHero standHero;

    public _SpineObject spStandCleo;
    public _SpineObject spStandLubu;

    private void Start ()
    {
        btnProfile.onClick.AddListener (OnProfile);
        btnQuickPlay.onClick.AddListener (OnQuickPlay);
        btnPoker.onClick.AddListener (OnQuickPlay);
        btnSloto.onClick.AddListener (OnSloto);
    }

    private void OnProfile ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.PROFILE, true);
    }

    private void OnQuickPlay ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.POKER, true);
        Hide ();
    }

    private void OnSloto ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.SLOTO, true);
        Hide ();
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
        tmpCoin.text = PlayerData.owned_coin.toShortCurrency ();
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
}
