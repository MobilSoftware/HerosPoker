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
    private bool isSettingJson;
    private SceneType prevSceneType;
    private List<JGetShopItem> jsonItemCoins;
    private JGetShopItem[] sortedItemHeroes;

    private void Start ()
    {
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
                togCoin.image.sprite = sprToggleOn;
                togHero.image.sprite = sprToggleOff;
                objCoinContainer.SetActive (true);
                objHeroContainer.SetActive (false);
            } else if (togHero.isOn)
            {
                togCoin.image.sprite = sprToggleOff;
                togHero.image.sprite = sprToggleOn;
                objCoinContainer.SetActive (false);
                objHeroContainer.SetActive (true);
            }
        }
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
            StartCoroutine (_WaitSetJson ());
        }

        canvas.enabled = true;
        tmpCoin.text = Convert.ToInt64 (PlayerData.owned_coin).toShortCurrency ();
        tmpCoupon.text = Convert.ToInt64 (PlayerData.owned_coupon).toCouponShortCurrency ();
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.SHOP;

    }

    public void SetJson (JGetShop json )
    {
        isSettingJson = true;
        jsonItemCoins = new List<JGetShopItem> ();
        List<JGetShopItem> jsonItemHeroes = new List<JGetShopItem> ();
        for (int i = 0; i < json.items.Length; i++)
        {
            if (json.items[i].item_type_id == 1)
                jsonItemCoins.Add (json.items[i]);
            else if (json.items[i].item_type_id == 5)
                jsonItemHeroes.Add (json.items[i]);
        }

        //sort here
        sortedItemHeroes = jsonItemHeroes.OrderBy (x => x.is_hero_owned ? 1 : 0).ThenBy (x => (x.is_new == 0) ? 0 : 1).ThenBy (x => int.Parse (x.price_idr)).ThenBy (x => long.Parse (x.price_coin)).ToArray ();
        //

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

        for (int i = 0; i < jsonItemCoins.Count; i++)
        {
            ItemCoin ic = Instantiate (prefabItemCoin, parentCoin);
            ic.SetData (jsonItemCoins[i]);
        }
    }

    private void SetItemHero ()
    {
        if (sortedItemHeroes == null)
            return;

        for (int i = 0; i < sortedItemHeroes.Length; i++)
        {
            ItemHero ih = Instantiate (prefabItemHero, parentCoin);
            ih.SetData (sortedItemHeroes[i]);
        }
    }

    public void Hide ()
    {
        togCoin.isOn = true;
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }

    public void UpdateCoinAndCoupon ()
    {
        tmpCoin.text = PlayerData.owned_coin.toShortCurrency ();
        tmpCoupon.text = PlayerData.owned_coupon.toCouponShortCurrency ();
    }
}
