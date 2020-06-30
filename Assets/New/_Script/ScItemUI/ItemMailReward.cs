using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMailReward : MonoBehaviour
{
    public RawImage imgReward;
    public Text txtQuantity;
    public GameObject objDimmer;
    [HideInInspector]
    public bool isInit;

    public void SetData (int itemTypeId, int itemID, string strQuantity )
    {
        isInit = true;
        if (itemTypeId == 1)
        {
            txtQuantity.text = long.Parse (strQuantity).toShortCurrency ();
            imgReward.texture = HomeManager.instance.sprCoin.texture;
        } else if (itemTypeId == 2)
        {
            txtQuantity.text = long.Parse (strQuantity).toCouponShortCurrency ();
            imgReward.texture = HomeManager.instance.sprCoupon.texture;
        }
        else
        {
            txtQuantity.text = strQuantity;
            string imagePath = BundleManager.instance.GetItemLoadPath (DownloadType.THUMB, itemTypeId, itemID);
            BundleManager.instance.LoadImage (imgReward, imagePath);
        }
        gameObject.SetActive (true);
    }

    public void Reset ()
    {
        objDimmer.SetActive (false);
        gameObject.SetActive (false);
        isInit = false;
    }
}
