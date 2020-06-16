using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHeroCard : MonoBehaviour
{
    public RawImage imgHero;
    public Text txtHeroName;
    public Button btnEquip;
    public Button btnUnEquip;
    public GameObject objEmpty;

    [HideInInspector]
    public int heroID;

    private void Start ()
    {
        if (btnEquip != null)
            btnEquip.onClick.AddListener (OnEquip);
        if (btnUnEquip != null)
            btnUnEquip.onClick.AddListener (OnUnEquip);
    }

    private void OnEquip () //on menu card
    {
        ProfileManager.instance.EquipHeroCard (this);
        btnEquip.interactable = false;
    }

    private void OnUnEquip ()   //not on menu card
    {
        ProfileManager.instance.UnEquipHeroCard (heroID);
        objEmpty.SetActive (true);
    }

    public void SetData (JGetShopItem json)
    {
        heroID = json.hero_type_id;
        txtHeroName.text = json.item_name[0];
        string imagePath = BundleManager.instance.GetItemLoadPath (DownloadType.THUMB, json.item_type_id, json.item_id);
        BundleManager.instance.LoadImage (imgHero, imagePath);
        objEmpty.SetActive (false);
    }

    public void SetCard (ItemHeroCard card )
    {
        heroID = card.heroID;
        txtHeroName.text = card.txtHeroName.text;
        imgHero.texture = card.imgHero.texture;
        objEmpty.SetActive (false);
    }


}
