using System.Linq;

public static class PokerData
{
    public static int poker_round_id;
    public static string room_bet;
    public static int[] cards;
    public static string str_cards;
    public static string otp;

    public static bool is_setup;

    public static void Setup ( int _roundID, string _roomBetCoin, int[] _cards, string _otp )
    {
        poker_round_id = _roundID;
        room_bet = _roomBetCoin;
        cards = _cards;
        otp = _otp;

        is_setup = true;
    }

    private static void RandomizeCards ()
    {
        int[] cardIndices = new int[52];

        for (int i = 0; i < 52; i++)
        {
            cardIndices[i] = i;
        }

        for (int j = 0; j < 52; j++)
        {
            int indexRand = UnityEngine.Random.Range (0, 52);
            cardIndices[j] = indexRand;
            cardIndices[indexRand] = j;
        }

        cardIndices = cardIndices.Take (25).ToArray ();
        cards = cardIndices;
    }

    public static void Reset ()
    {
        poker_round_id = 0;
        room_bet = "0";
        cards = null;
        str_cards = string.Empty;
    }
}
