[System.Serializable]
public class JClaimPromoCode
{
    public JPromo promo;
    public JHome player;
}

[System.Serializable]
public class JPromo
{
    public string promo_code;
    public int item_type;
    public int item_id;
    public string[] item_name;
    public string item_amount;
    public int[] package;
}