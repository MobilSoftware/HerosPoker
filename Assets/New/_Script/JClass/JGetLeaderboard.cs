[System.Serializable]
public class JGetLeaderboard
{
    public string leaderboard_type;
    public JLeaderboard[] leaderboard;
    public JLeaderboardPlayer player;
}

[System.Serializable]
public struct JLeaderboard
{
    public int player_id;
    public string scoring;
    public int rank_id;
    public JHome profile;
}

[System.Serializable]
public struct JPlayerProfile
{
    public int player_id;
    public string coin;
    public string coupon;
    public int level;
    public int exp;
    public int vip_level;
    public int vip_exp;
    public int costume_equiped;
    public int[] costume_owned;
    public int[] hero_owned;
    public int[] hero_featured;
    public string refer_by;

}

[System.Serializable]
public struct JLeaderboardPlayer
{
    public int rank_id;
    public int player_id;
    public string scoring;
}