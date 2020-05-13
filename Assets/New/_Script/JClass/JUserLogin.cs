[System.Serializable]
public class JUserLogin
{
    public JPlayer player;
}

[System.Serializable]
public struct JPlayer
{
    public int player_id;
    public string player_tag;
    public string token;
    public string coin;
    public string coupon;
    public string display_name;
    public string display_picture;
    public bool verified;
}