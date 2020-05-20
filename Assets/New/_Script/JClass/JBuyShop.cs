[System.Serializable]
public class JBuyShop
{
    public JBuyShopItem item;
    public JBuyShopPlayer player;
}

[System.Serializable]
public struct JBuyShopItem
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
}

[System.Serializable]
public struct JBuyShopPlayer
{
    public int player_id;
    public string coin;
    public string coupon;
    public string display_name;
    public string display_picture;
    public int[] hero_owned;
    public int[] costume_owned;
    public int costume_equiped;
    public string player_tag;
    public bool verified;

}