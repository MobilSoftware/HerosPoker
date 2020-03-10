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
                    Debug.Log ("Could not locate an HomeManager object. \n You have to have exactly one HomeManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Button btnQuickPlay;
    public Button btnPoker;
    public Button btnSloto;
    public TextMeshProUGUI tmpDisplayName;
    public TextMeshProUGUI tmpCoin;
    public Image imgHero;

    public Sprite sprCleo;
    public Sprite sprLubu;

    private void Start ()
    {
        btnQuickPlay.onClick.AddListener (OnQuickPlay);
        btnPoker.onClick.AddListener (OnQuickPlay);
        btnSloto.onClick.AddListener (OnSloto);
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

    public void Show ()
    {
        tmpCoin.text = PlayerData.owned_gold.toShortCurrency ();
        canvas.enabled = true;
    }

    public void Init ()
    {
        PhotonNetwork.ConnectUsingSettings ("v1.0");
        canvas.sortingOrder = (int) SceneType.HOME;
        tmpDisplayName.text = PlayerData.display_name;
        tmpCoin.text = PlayerData.owned_gold.toShortCurrency ();

        if (PlayerData.hero_id == 100)
            imgHero.sprite = sprLubu;
        else if (PlayerData.hero_id == 200)
            imgHero.sprite = sprCleo;
        imgHero.gameObject.SetActive (true);
    }

    public void Hide ()
    {
        canvas.enabled = false;
    }
}
