[System.Serializable]
public class JStartPoker
{
    public int poker_round_id;
    public string room_seed;
    public string room_bet_coin;
    public JPokerPlayer[] players;
    public int[] cards;
    public string otp;
}

[System.Serializable]
public struct JPokerPlayer
{
    public int player_id;
    public int seater_id;
    public string coin_before;
    public string coin_server;
    public bool kick;
}