[System.Serializable]
public class JGetCostume
{
    public JCostume[] costumes;
}

[System.Serializable]
public struct JCostume
{
    public int hero_type_id;
    public string[] item_name;
    public string[] item_desc;
    public int item_id;
    public string price_coin;
    public string price_coupon;
    public string price_idr;
    public bool is_hero_owned;
    public bool is_costume_owned;
}