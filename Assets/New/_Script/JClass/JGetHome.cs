[System.Serializable]
public class JGetHome
{
    public JHome player;
}

[System.Serializable]
public struct JHome
{
    public int player_id;
    public string coin;
    public string coupon;
    public int level;
    public int exp;
    public int vip_level;
    public int vip_exp;
    public string display_name;
    public string display_picture;
    public int[] hero_owned;
    public int[] costume_owned;
    public int[] hero_featured;
    public int refer_by;
    public int win_after_refer;
    public int dealer_by;
    public string status_message;
    public string daily_login;
    public string player_referal_code;
    public int costume_equiped;
    public string player_tag;
    public bool verified;
    public bool level_up;
    public string exp_percentage;
    public bool can_claim_daily;
    public bool can_claim_weekly;
    public int money_slot_next_in;
    public int[] friend_list;
    public int[] friend_request;
    public int[] friend_request_me;
    public int[] friend_block;
    public int[] friend_block_me;
    public int[] friend_chat_me;
    public int[] friend_gift_me;

}