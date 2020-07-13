using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroManager : MonoBehaviour
{
    private static HeroManager s_Instance = null;
    public static HeroManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (HeroManager)) as HeroManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an HeroManager object. \n You have to have exactly one HeroManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Transform parentItems;
    public ItemSelectHero prefabItemSelectHero;
    public TextMeshProUGUI tmpCoin;
    public TextMeshProUGUI tmpCoupon;
    public Button btnClose;
    [HideInInspector]
    public ItemSelectHero selectedHero;

    private bool isInit;
    private SceneType prevSceneType;
    [HideInInspector]
    public bool isSettingJson;
    private JGetShopItem[] sortedHeroes;
    private List<ItemSelectHero> selectHeroes;
    private Coroutine crUpdateStatus;

    private void Start ()
    {
        isSettingJson = true;
        btnClose.onClick.AddListener (Hide);
    }

    public void SetCanvas (bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    private void Show ()
    {
        if (!isInit)
        {
            isInit = true;
            canvas.sortingOrder = (int) SceneType.HERO;
            StartCoroutine (_WaitSetJson ());
        }

        canvas.enabled = true;
        tmpCoin.text = Convert.ToInt64 (PlayerData.owned_coin).toShortCurrency ();
        tmpCoupon.text = Convert.ToInt64 (PlayerData.owned_coupon).toCouponShortCurrency ();
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.HERO;
    }

    public void UpdateCoinAndCoupon ()
    {
        tmpCoin.text = PlayerData.owned_coin.toShortCurrency ();
        tmpCoupon.text = PlayerData.owned_coupon.toCouponShortCurrency ();
    }

    public void SetJson ( List<JGetShopItem> unsortedHeroes )
    {
        sortedHeroes = unsortedHeroes.OrderBy (x => (x.default_item_id == PlayerData.costume_id) ? 0 : 1).ThenBy(x => x.is_hero_owned ? 0 : 1).ToArray ();
        isSettingJson = false;
        isInit = false;
        if (parentItems.childCount > 0)
        {
            for (int i = 0; i < parentItems.childCount; i++)
            {
                Destroy (parentItems.GetChild (i).gameObject);
            }
        }
    }

    IEnumerator _WaitSetJson ()
    {
        while (isSettingJson)
        {
            yield return _WFSUtility.wef;
        }
        selectHeroes = new List<ItemSelectHero> ();
        SetItemSelectHero ();
    }

    private void Hide ()
    {
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
        //_SceneManager.instance.SetActiveScene (SceneType.HOME, true);
        HomeManager.instance.Show ();
        HomeManager.instance.Init ();
    }

    private void SetItemSelectHero()
    {
        if (sortedHeroes == null)
            return;

        selectHeroes.Clear ();
        for (int i = 0; i < sortedHeroes.Length; i++)
        {
            ItemSelectHero ish = Instantiate (prefabItemSelectHero, parentItems);
            ish.SetData (sortedHeroes[i]);
            selectHeroes.Add (ish);
        }
    }

    //call after choosing new hero
    public void UpdateStatusOwned(List<JGetShopItem> unsortedHeroes)
    {
        SetJson (unsortedHeroes);

        if (selectHeroes == null)
            return;

        for (int i = 0; i < selectHeroes.Count; i++)
        {
            selectHeroes[i].SetData (sortedHeroes[i]);
        }
    }

    //public void UpdateStatusEquipped ()
    //{
    //    if (crUpdateStatus != null)
    //        StopCoroutine (crUpdateStatus);
    //    crUpdateStatus = StartCoroutine (_UpdateStatusEquipped ());
    //}

    //IEnumerator _UpdateStatusEquipped()
    //{
    //    //if (selectedHero != null)
    //    //{
    //    //    selectedHero.Unequip ();
    //    //}
    //    for (int i = 0; i < selectHeroes.Count; i++)
    //    {
    //        selectHeroes[i].SetData (sortedHeroes[i]);
    //        yield return _WFSUtility.wef;
    //    }
    //}

    public void Logout ()
    {
        if (crUpdateStatus != null)
        {
            StopCoroutine (crUpdateStatus);
            crUpdateStatus = null;
        }
        isInit = false;
        isSettingJson = true;
    }

    public void UnequipSelected ()
    {
        selectedHero.Unequip ();
    }
}
