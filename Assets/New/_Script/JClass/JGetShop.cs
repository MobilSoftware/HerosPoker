[System.Serializable]
public class JGetShop
{
    public JGetShopItem[] items;
}

[System.Serializable]
public struct JGetShopItem
{
    public int item_id;
    public string[] item_name;
    public string[] item_desc;
    public int item_type_id;
    public int hero_type_id;
    public string price_coin;
    public string price_coupon;
    public string price_idr;
    public string playstore_id;
    public string appstore_id;
    public string bonus_coin;       //coin added after purchase
    public int one_time_only;
    public int is_new;
    public bool is_hero_owned;
    public bool is_costume_owned;

}