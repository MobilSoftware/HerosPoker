using System;
using UnityEngine;

public static class PlayerData
{

    public static string proto_bet;

    public static int id;
    public static string tag;
    public static string display_name;
    public static long owned_coin;
    public static int owned_coupon;
    public static int costume_id;
    public static int charityCount;

    public static void UpdateData (JHome json )
    {
        proto_bet = "20";
        PhotonNetwork.player.NickName = id.ToString ();
        charityCount = 3;
        id = json.player_id;
        display_name = json.display_name;
        tag = json.player_tag;
        owned_coin = Convert.ToInt64 (json.coin);
        owned_coupon = Convert.ToInt16 (json.coupon);
        if (json.costume_equiped != 0)
            costume_id = json.costume_equiped;
    }
}
