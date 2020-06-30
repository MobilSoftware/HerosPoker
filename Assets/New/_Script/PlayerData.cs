using System;
using UnityEngine;

public static class PlayerData
{
    public static int id;
    public static string tag;
    public static string display_name;
    public static string display_picture;
    public static string status_message;
    public static int level;
    public static float exp_percentage;
    public static long owned_coin;
    public static long owned_coupon;
    public static int costume_id;
    public static int vip_level;
    public static int vip_exp;
    public static JHome jHome;

    public static void UpdateData (JHome json )
    {
        jHome = json;
        id = json.player_id;
        PhotonNetwork.player.NickName = id.ToString ();
        display_name = json.display_name;
        display_picture = json.display_picture;
        status_message = json.status_message;
        level = json.level;
        tag = json.player_tag;
        owned_coin = Convert.ToInt64 (json.coin);
        owned_coupon = Convert.ToInt64 (json.coupon);
        if (json.costume_equiped != 0)
            costume_id = json.costume_equiped;
        if (json.exp_percentage != null)
            exp_percentage = float.Parse (json.exp_percentage);
        vip_level = json.vip_level;
        vip_exp = json.vip_exp;
    }
}
