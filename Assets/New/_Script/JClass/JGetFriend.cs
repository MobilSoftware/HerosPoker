[System.Serializable]
public class JGetFriend
{
    public JListFriendType list;
    public JFriend[] friends;
}

[System.Serializable]
public struct JListFriendType
{
    public int[] friend_list;
    public int[] friend_request;
    public int[] friend_request_me;
    public int[] friend_block;
    public int[] friend_block_me;
    public int[] friend_chat_me;
    public int[] friend_gift_me;
}

[System.Serializable]
public class JFriend
{
    public int player_id;
    public string coin;
    public string coupon;
    public string display_name;
    public string status_message;
    public string display_picture;
    public int[] hero_owned;
    public int[] costume_owned;
    public int costume_equiped;
    public int level;
    public int exp;
    public int vip_level;
    public string player_tag;
    public int[] hero_featured;
    public string exp_percentage;
    public bool verified;
    public bool on_friend_list;
    public bool on_friend_request;
    public bool on_friend_request_me;
    public bool on_friend_block;
    public bool on_friend_block_me;
    public bool chat_notify;
    public bool gift_notify;
}