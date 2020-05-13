using System;
using UnityEngine;

public static class PlayerData
{

    public static string proto_bet;

    public static int id;
    public static string display_name;
    public static long owned_coin;
    public static int costume_id;
    public static int charityCount;

    public static void SetData (string _displayName)
    {
        proto_bet = "20";
        //owned_coin = 40000;

        //id = UnityEngine.Random.Range (1, 10000);
        //display_name = id + "_" + _displayName;
        //display_name = _displayName;
        //if (display_name.Length > 9)
        //{
        //    display_name = display_name.Substring (0, 7);
        //    display_name = display_name + "...";
        //}
        PhotonNetwork.player.NickName = id.ToString ();
        charityCount = 3;
    }

    public static void UpdateData (JHome json )
    {
        proto_bet = "20";
        PhotonNetwork.player.NickName = id.ToString ();
        charityCount = 3;
        id = json.player_id;
        display_name = json.display_name;
        owned_coin = Convert.ToInt64 (json.coin);
        if (json.costume_equiped != 0)
            costume_id = json.costume_equiped;
    }
}
