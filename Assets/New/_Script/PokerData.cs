using System.Linq;

public static class PokerData
{
    public static int poker_round_id;
    public static string room_bet;
    public static int[] cards;
    public static string str_cards;

    public static bool is_setup;

    public static void Setup ( int roundID, string roomBet, string _strCards )
    {
        poker_round_id = roundID;
        room_bet = roomBet;
        str_cards = _strCards;
        string[] splitCards = _strCards.Split (',');
        cards = new int[splitCards.Length];
        for (int i = 0; i < splitCards.Length; i++)
        {
            cards[i] = int.Parse (splitCards[i]);
        }
        is_setup = true;
    }

    public static void Generate ()
    {
        poker_round_id = 99;
        //room_bet = DataManager.instance.prototypeBet;
        room_bet = PlayerData.proto_bet;

        RandomizeCards ();

        str_cards = string.Empty;
        for (int i = 0; i < cards.Length; i++)
        {
            if (i == cards.Length - 1)
                str_cards += cards[i];
            else
                str_cards += cards[i] + ",";
        }
        is_setup = true;
        PhotonTexasPokerManager.instance.SetMyStartPoker (poker_round_id, room_bet, str_cards);
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
}
