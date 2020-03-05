using UnityEngine;

public static class PlayerData
{

    public static string proto_bet;

    public static int id;
    public static string display_name;
    public static long owned_gold;
    public static int hero_id;

    public static void SetData (string _displayName)
    {
        proto_bet = "20";
        owned_gold = 40000;

        id = Random.Range (1, 10000);
        display_name = id + "_" + _displayName;
        if (display_name.Length > 9)
        {
            display_name = display_name.Substring (0, 7);
            display_name = display_name + "...";
        }
        PhotonNetwork.player.NickName = id.ToString ();
    }
}
