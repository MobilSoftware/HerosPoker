using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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
    public InputField txtStatus;
    public Text txtLevel;
    public Text txtCoinValue;
    public Text txtCouponValue;
    public Text txtVerify;
    public Text txtTag;
    public Image fillExpBar;
    public Button btnClose;
    public Button btnVerify;
    public Button btnVIP;
    public Button btnEditStatus;
    public Button btnEditCards;
    public StandHero standHero;

    public GameObject objMenuCards;
    public Button btnHideMenuCards;
    public Transform parentCards;
    public ItemHeroCard prefabItemHeroCard;
    public ItemHeroCard[] equipedCards;
    [HideInInspector]
    public int[] hero_owned;

    private SceneType prevSceneType;
    private bool isInit;
    private List<ItemHeroCard> ownedCards;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
        btnVerify.onClick.AddListener (OnVerify);
        btnVIP.onClick.AddListener (OnVIP);
        //btnEditStatus.onClick.AddListener (OnEditStatus);
        //btnEditCards.onClick.AddListener (OnEditCards);
        //btnHideMenuCards.onClick.AddListener (HideMenuCards);
    }

    private void OnVerify ()
    {
        //_SceneManager.instance.SetActiveScene (SceneType.VERIFY, true);
    }

    private void OnEditStatus ()
    {
        ApiManager.instance.SetStatus (txtStatus.text);
    }

    private void OnEditCards ()
    {
        objMenuCards.SetActive (true);
    }

    private void HideMenuCards ()
    {
        List<int> featuredHeroes = new List<int> ();
        for (int i = 0; i < equipedCards.Length; i++)
        {
            if (!equipedCards[i].objEmpty.activeSelf)
                featuredHeroes.Add (equipedCards[i].heroID);
        }
        if (featuredHeroes.Count == 0)
            ApiManager.instance.SetHeroFeatured (featuredHeroes.ToArray ());

        objMenuCards.SetActive (false);
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
            ownedCards = new List<ItemHeroCard> ();
            canvas.sortingOrder = (int) SceneType.PROFILE;
            isInit = true;
        }

        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.PROFILE;
        txtDisplayName.text = PlayerData.display_name;
        txtStatus.text = PlayerData.status_message;
        txtLevel.text = "Lv. " + PlayerData.level;
        fillExpBar.fillAmount = PlayerData.exp_percentage;
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

    public void SetOwnedHeroesJson (JGetShopItem[] itemHeroes)
    {
        for (int i = 0; i < itemHeroes.Length; i++)
        {
            for (int j = 0; j < hero_owned.Length; j++)
            {
                if (hero_owned[j] == itemHeroes[i].hero_type_id)
                {
                    ItemHeroCard ihc = Instantiate (prefabItemHeroCard, parentCards);
                    ihc.SetData (itemHeroes[i]);
                    ownedCards.Add (ihc);
                }
            }
        }
    }

    public void ResetOwnedHeroes ()
    {
        for (int i = 0; i < parentCards.childCount; i++)
        {
            Destroy (parentCards.GetChild (i).gameObject);
        }
        ownedCards.Clear ();
    }

    public void Logout ()
    {
        ResetOwnedHeroes ();
    }

    public void EquipHeroCard (ItemHeroCard card)
    {
        if (equipedCards[0].objEmpty.activeSelf)
        {
            equipedCards[0].SetCard (card);
        } else if (equipedCards[1].objEmpty.activeSelf)
        {
            equipedCards[1].SetCard (card);
        } else if (equipedCards[2].objEmpty.activeSelf)
        {
            equipedCards[2].SetCard (card);
        }
    }

    public void UnEquipHeroCard (int heroID )
    {
        for (int i = 0; i < ownedCards.Count; i++)
        {
            if (ownedCards[i].heroID == heroID)
            {
                ownedCards[i].btnEquip.interactable = true;
                break;
            }
        }
    }

}
