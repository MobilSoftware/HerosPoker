using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private static ShopManager s_Instance = null;
    public static ShopManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (ShopManager)) as ShopManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an ShopManager object. \n You have to have exactly one ShopManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Sprite sprToggleOn;
    public Sprite sprToggleOff;
    public Color colTextToggleOn;
    public Color colTextToggleOff;
    public Text txtTogCoin;
    public Text txtTogHero;
    public Toggle togCoin;
    public Toggle togHero;
    public Button btnClose;
    public GameObject objCoinContainer;
    public GameObject objHeroContainer;
    public Transform parentCoin;
    public Transform parentHero;
    public ItemCoin prefabItemCoin;
    public ItemHero prefabItemHero;
    public TextMeshProUGUI tmpCoin;
    public TextMeshProUGUI tmpCoupon;

    private bool isInit;
    [HideInInspector]
    public bool isSettingJson;
    private SceneType prevSceneType;
    private List<JGetShopItem> jsonItemCoins;
    private List<JGetShopItem> jsonItemHeroes;
    private List<ItemHero> itemHeroes;
    private JGetShopItem[] sortedItemHeroes;
    private Coroutine crUpdateStatus;

    private void Start ()
    {
        isSettingJson = true;
        btnClose.onClick.AddListener (Hide);
        togCoin.onValueChanged.AddListener (OnToggle);
        togHero.onValueChanged.AddListener (OnToggle);
    }

    private void OnToggle (bool val )
    {
        if (val)
        {
            if (togCoin.isOn)
            {
                OpenCoinTab ();
            } else if (togHero.isOn)
            {
                OpenHeroTab ();
            }
        }
    }

    public void OpenHeroTab ()
    {
        togCoin.image.sprite = sprToggleOff;
        togHero.image.sprite = sprToggleOn;
        txtTogCoin.color = colTextToggleOff;
        txtTogHero.color = colTextToggleOn;
        objCoinContainer.SetActive (false);
        objHeroContainer.SetActive (true);
    }

    public void OpenCoinTab ()
    {
        togCoin.image.sprite = sprToggleOn;
        togHero.image.sprite = sprToggleOff;
        txtTogCoin.color = colTextToggleOn;
        txtTogHero.color = colTextToggleOff;
        objCoinContainer.SetActive (true);
        objHeroContainer.SetActive (false);
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
        if (!isInit)
        {
            isInit = true;
            canvas.sortingOrder = (int) SceneType.SHOP;
        }

        StartCoroutine (_WaitSetJson ());
        canvas.enabled = true;
        tmpCoin.text = Convert.ToInt64 (PlayerData.owned_coin).toShortCurrency ();
        tmpCoupon.text = Convert.ToInt64 (PlayerData.owned_coupon).toCouponShortCurrency ();
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.SHOP;

    }

    public void SetJson (JGetShop json )
    {
        jsonItemCoins = new List<JGetShopItem> ();
        jsonItemHeroes = new List<JGetShopItem> ();
        for (int i = 0; i < json.items.Length; i++)
        {
            if (json.items[i].item_type_id == 1)
                jsonItemCoins.Add (json.items[i]);
            else if (json.items[i].item_type_id == 5)
                jsonItemHeroes.Add (json.items[i]);
        }
        HeroManager.instance.SetJson (jsonItemHeroes);
        //sort here
        sortedItemHeroes = jsonItemHeroes.OrderBy (x => x.is_hero_owned ? 1 : 0).ThenBy (x => (x.is_new == 0) ? 0 : 1).ThenBy (x => int.Parse (x.price_idr)).ThenBy (x => long.Parse (x.price_coin)).ToArray ();
        //
        ProfileManager.instance.SetOwnedHeroesJson (sortedItemHeroes);
        isSettingJson = false;
    }

    IEnumerator _WaitSetJson ()
    {
        while (isSettingJson)
        {
            yield return _WFSUtility.wef;
        }

        SetItemCoin ();
        yield return _WFSUtility.wef;
        SetItemHero ();
    }

    private void SetItemCoin ()
    {
        if (jsonItemCoins == null)
            return;

        if (parentCoin.childCount == 0)
        {
            for (int i = 0; i < jsonItemCoins.Count; i++)
            {
                ItemCoin ic = Instantiate(prefabItemCoin, parentCoin);
                ic.SetData(jsonItemCoins[i]);
            }
        }
    }

    private void SetItemHero ()
    {
        if (sortedItemHeroes == null)
            return;

        if (parentHero.childCount == 0)
        {
            itemHeroes = new List<ItemHero>();
            for (int i = 0; i < sortedItemHeroes.Length; i++)
            {
                ItemHero ih = Instantiate(prefabItemHero, parentHero);
                ih.SetData(sortedItemHeroes[i]);
                itemHeroes.Add(ih);
            }
        }
    }

    public void ClearShop()
    {
        for (int i = 0; i < parentCoin.childCount; i++)
        {
            Destroy(parentCoin.GetChild(i).gameObject);
        }

        for (int x = 0; x < parentCoin.childCount; x++)
        {
            Destroy(parentHero.GetChild(x).gameObject);
        }
    }

    public void Hide ()
    {
        isInit = false;
        togCoin.isOn = true;
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }

    public void UpdateCoinAndCoupon ()
    {
        tmpCoin.text = PlayerData.owned_coin.toShortCurrency ();
        tmpCoupon.text = PlayerData.owned_coupon.toCouponShortCurrency ();
    }

    public void UpdateStatus (JBuyShop json )
    {
        if (crUpdateStatus != null)
            StopCoroutine(crUpdateStatus);

        crUpdateStatus = StartCoroutine(_UpdateStatus(json));
    }

    IEnumerator _UpdateStatus(JBuyShop json)
    {
        if (json.item.item_type_id == 5)
        {
            for (int i = 0; i < jsonItemHeroes.Count; i++)
            {
                for (int j = 0; j < json.player.costume_owned.Length; j++)
                {
                    if (jsonItemHeroes[i].default_item_id == json.player.costume_owned[j])
                    {
                        jsonItemHeroes[i].is_costume_owned = true;
                        jsonItemHeroes[i].is_hero_owned = true;
                    }

                    yield return _WFSUtility.wef;
                }
            }

            sortedItemHeroes = jsonItemHeroes.OrderBy(x => x.is_hero_owned ? 1 : 0).ThenBy(x => (x.is_new == 0) ? 0 : 1).ThenBy(x => int.Parse(x.price_idr)).ThenBy(x => long.Parse(x.price_coin)).ToArray();
            for (int x = 0; x < itemHeroes.Count; x++)
            {
                itemHeroes[x].SetData(sortedItemHeroes[x]);
                yield return _WFSUtility.wef;
            }

            HeroManager.instance.UpdateStatusOwned(jsonItemHeroes);
        }
    }

    public void Logout()
    {
        isInit = false;
        isSettingJson = true;
    }
}
