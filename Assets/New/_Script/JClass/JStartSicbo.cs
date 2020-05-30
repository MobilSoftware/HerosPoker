[System.Serializable]
public class JStartSicbo
{
    public int[] dice;
    public JSicboPlayer[] players;
}

[System.Serializable]
public struct JSicboPlayer
{
    public int player_id;
    public JSicboBet[] bets;
    public string total_coin_bet;
    public string total_coin_won;
    public string coin_server;
    public string coin_after;
    public bool kick;
}

[System.Serializable]
public struct JSicboBet
{
    public int sicbo_type;
    public string coin_bet;
    public string coin_won;
}