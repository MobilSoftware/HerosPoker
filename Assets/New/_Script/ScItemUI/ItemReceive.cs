using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemReceiveData
{
    public int typeID;
    public int itemID;
    public int itemValue;
    public int isCurrency;  //0 = item, 1 = coin, 2 = coupon

    public ItemReceiveData ( int _typeID, int _itemID, int _itemValue, int _isCurrency )
    {
        typeID = _typeID;
        itemID = _itemID;
        itemValue = _itemValue;
        isCurrency = _isCurrency;
    }
}

public class ItemReceive : MonoBehaviour
{
    public RawImage imgItem;
    public TextMeshProUGUI txtItemValue;

    public void SetData (ItemReceiveData data)
    {
        if (data.isCurrency == 1)
            txtItemValue.text = data.itemValue.toShortCurrency ();
        else if (data.isCurrency == 2)
            txtItemValue.text = data.itemValue.toCouponShortCurrency ();
        else
            txtItemValue.text = data.itemValue.ToString ();

        string imagePath = BundleManager.instance.GetItemLoadPath (DownloadType.THUMB, data.typeID, data.itemID);
        BundleManager.instance.LoadImage (imgItem, imagePath);
    }
}
