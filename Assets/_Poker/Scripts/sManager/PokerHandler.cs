using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokerHandler : MonoBehaviour
{
    //equivalent of StartPoker
    public int poker_round_id;
    public string room_bet;
    public int[] cards;
    public string strCards;

    public bool isSetup;

    public void Setup (int roundID, string roomBet, string _strCards)
    {
        poker_round_id = roundID;
        room_bet = roomBet;
        strCards = _strCards;
        string[] splitCards = _strCards.Split (',');
        cards = new int[splitCards.Length];
        for (int i = 0; i < splitCards.Length; i++)
        {
            cards[i] = int.Parse (splitCards[i]);
        }
        isSetup = true;
    }

    public void Generate ()
    {
        poker_round_id = 99;
        room_bet = DataManager.instance.prototypeBet;

        RandomizeCards ();

        strCards = string.Empty;
        for (int i = 0; i < cards.Length; i++)
        {
            if (i == cards.Length - 1)
                strCards += cards[i];
            else
                strCards += cards[i] + ",";
        }
        isSetup = true;
        PhotonTexasPokerManager.instance.SetMyStartPoker (poker_round_id, room_bet, strCards);
    }

    public void RandomizeCards ()
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
