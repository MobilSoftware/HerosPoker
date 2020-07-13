using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelectHero : MonoBehaviour
{
    public Image imgFrame;
    public Image imgFrameName;
    public RawImage imgHero;
    public Text txtHeroName;
    public Button btnChoose;
    public Button btnBuy;
    public Image imgUsing;

    private JGetShopItem jHero;

    private void Start ()
    {
        btnChoose.onClick.AddListener (OnChoose);
        btnBuy.onClick.AddListener (OnBuy);
    }

    private void OnChoose ()
    {
        ApiManager.instance.SetCostume (jHero.default_item_id);
        if (HeroManager.instance.selectedHero != null)
        {
            HeroManager.instance.UnequipSelected ();
            Equip ();
        }
    }

    private void OnBuy ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.SHOP, true);
        ShopManager.instance.OpenHeroTab ();
        _SceneManager.instance.SetActiveScene (SceneType.HERO, false);
    }

    public void SetData (JGetShopItem json)
    {
        jHero = json;
        txtHeroName.text = json.item_name[0];
        SetOwnedStatus (json.is_hero_owned, (json.default_item_id == PlayerData.costume_id));
        string imagePath = BundleManager.instance.GetItemLoadPath (DownloadType.THUMB, json.item_type_id, json.item_id);
        BundleManager.instance.LoadImage (imgHero, imagePath);
    }

    private void SetOwnedStatus (bool isOwned, bool isEquiped )
    {
        if (isOwned)
        {
            imgFrame.color = Color.white;
            imgFrameName.color = Color.white;
            imgHero.color = Color.white;
            btnChoose.gameObject.SetActive (!isEquiped);
            btnBuy.gameObject.SetActive (false);
            imgUsing.gameObject.SetActive (isEquiped);
            if (isEquiped)
            {
                HeroManager.instance.selectedHero = this;
            }
        }
        else
        {
            imgFrame.color = Color.gray;
            imgFrameName.color = Color.gray;
            imgHero.color = Color.gray;
            btnBuy.gameObject.SetActive (true);
            btnChoose.gameObject.SetActive (false);
            imgUsing.gameObject.SetActive (false);
        }
    }

    private bool GetIsEquiped ()
    {
        bool isEquiped = false;
        if (jHero.item_id == 1 && PlayerData.costume_id == 10)
            isEquiped = true;
        else if (jHero.item_id == 2 && PlayerData.costume_id == 11)
            isEquiped = true;
        else if (jHero.item_id == 3 && PlayerData.costume_id == 12)
            isEquiped = true;
        else if (jHero.item_id == 4 && PlayerData.costume_id == 13)
            isEquiped = true;
        else if (jHero.item_id == 5 && PlayerData.costume_id == 14)
            isEquiped = true;
        else if (jHero.item_id == 6 && PlayerData.costume_id == 15)
            isEquiped = true;
        else if (jHero.item_id == 7 && PlayerData.costume_id == 16)
            isEquiped = true;
        else if (jHero.item_id == 8 && PlayerData.costume_id == 17)
            isEquiped = true;
        else if (jHero.item_id == 9 && PlayerData.costume_id == 18)
            isEquiped = true;

        return isEquiped;
    }

    private void Equip ()
    {
        imgFrame.color = Color.white;
        imgFrameName.color = Color.white;
        imgHero.color = Color.white;
        btnChoose.gameObject.SetActive (false);
        btnBuy.gameObject.SetActive (false);
        imgUsing.gameObject.SetActive (true);
        HeroManager.instance.selectedHero = this;
    }

    public void Unequip ()
    {
        imgFrame.color = Color.white;
        imgFrameName.color = Color.white;
        imgHero.color = Color.white;
        btnChoose.gameObject.SetActive (true);
        btnBuy.gameObject.SetActive (false);
        imgUsing.gameObject.SetActive (false);
    }

}
