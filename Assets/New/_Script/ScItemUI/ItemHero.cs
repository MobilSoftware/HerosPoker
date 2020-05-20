using UnityEngine;
using UnityEngine.UI;

public class ItemHero : MonoBehaviour
{
    public RawImage imgHero;
    public Text txtHeroName;
    public Text txtPriceIDR;
    public Text txtPriceCoin;
    public GameObject objImageCoin;
    public Button btnBuy;

    private int itemID;
    private int paymentType;
    private long priceIDR;
    private long priceCoin;

    private void Start ()
    {
        btnBuy.onClick.AddListener (OnBuy);
    }

    private void OnBuy ()
    {
        ApiManager.instance.BuyShop (itemID, paymentType);
    }

    public void SetData (JGetShopItem json)
    {
        txtHeroName.text = json.item_name[0];
        //load image hero
        string imagePath = BundleManager.instance.GetItemLoadPath (DownloadType.THUMB, json.item_type_id, json.item_id);
        BundleManager.instance.LoadImage (imgHero, imagePath);
        priceIDR = long.Parse (json.price_idr);
        priceCoin = long.Parse (json.price_coin);
        if (priceIDR != 0)
        {
            paymentType = 0;
            txtPriceCoin.gameObject.SetActive (false);
            objImageCoin.SetActive (false);
            txtPriceIDR.gameObject.SetActive (true);
            txtPriceIDR.text = "IDR " + priceIDR.ToString ("N0");
        } else if (priceCoin != 0)
        {
            paymentType = 1;
            txtPriceIDR.gameObject.SetActive (false);
            objImageCoin.SetActive (true);
            txtPriceCoin.gameObject.SetActive (true);
            txtPriceCoin.text = priceCoin.toShortCurrency ();
        }
    }
}
