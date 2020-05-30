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
    public string display_name;
    public string display_picture;
    public bool verified;
}

[System.Serializable]
public struct JLeaderboardPlayer
{
    public int rank_id;
    public int player_id;
    public string scoring;
}