﻿[System.Serializable]
public class JGetProfile
{
    public JProfile player;
}

[System.Serializable]
public struct JProfile
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