[System.Serializable]
public class JGetHero
{
    public JHero[] heroes;
}

[System.Serializable]
public struct JHero
{
    public int hero_type_id;
    public string hero_name;
    public int hero_gender;
    public int item_id;
    public string price_coin;
    public string price_coupon;
    public string price_idr;
    public bool is_hero_owned;
}