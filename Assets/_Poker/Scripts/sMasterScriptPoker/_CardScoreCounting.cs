using System;
using System.Collections.Generic;
using System.Linq;

public class _CardScoreCounting
{
    public bool IsRoyalFlush(_CardActor[] cards)
    {
        int count = 0;
        List<_CardActor> packHighlight = new List<_CardActor>();

        for (int j = 0; j < 4; j++)
        {
            count = 0;
            packHighlight.Clear();

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i]._numberCard == CardNumber.King && (Suite)j == cards[i]._suite)
                {
                    packHighlight.Add(cards[i]);
                    count++;
                }

                if (cards[i]._numberCard == CardNumber.Queen && (Suite)j == cards[i]._suite)
                {
                    packHighlight.Add(cards[i]);
                    count++;
                }

                if (cards[i]._numberCard == CardNumber.Jack && (Suite)j == cards[i]._suite)
                {
                    packHighlight.Add(cards[i]);
                    count++;
                }

                if (cards[i]._numberCard == CardNumber.Ten && (Suite)j == cards[i]._suite)
                {
                    packHighlight.Add(cards[i]);
                    count++;
                }

                if (cards[i]._numberCard == CardNumber.Ace && (Suite)j == cards[i]._suite)
                {
                    packHighlight.Add(cards[i]);
                    count++;
                }
            }

            if (count >= 5)
            {
                _cardHighlight = packHighlight.ToArray();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check Straight Flush
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public bool IsStraightFlush(_CardActor[] cards)
    {
        int count = 0;

        Dictionary<Suite, int> suit = new Dictionary<Suite, int>(); // in Hand h,s,c,h,c,s,h >> that will be {(h,3), (s,2), (c,2)}
        cards = cards.OrderByDescending(x => x._numberCard).ToArray();

        foreach (_CardActor card in cards)
        {
            if (suit.ContainsKey(card._suite))
                suit[card._suite] += 1; // Increase value of the same suit
            else
                suit.Add(card._suite, 1); // Add new unique suit in dictionary
        }
        suit = suit.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value); //{(h,3), (s,2), (c,2)}

        if (suit.First().Value >= 5)
        {
            Suite flushSuite = suit.First().Key;
            List<_CardActor> packStraightFlush = new List<_CardActor>();

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i]._suite == flushSuite)
                    packStraightFlush.Add(cards[i]);
            }

            List<CardNumber> listNumber = new List<CardNumber>();
            for (int i = 0; i < cards.Length; i++)
                listNumber.Add(cards[i]._numberCard);

            if (listNumber.Contains(CardNumber.Two))
                count++;
            if (listNumber.Contains(CardNumber.Three))
                count++;
            if (listNumber.Contains(CardNumber.Four))
                count++;
            if (listNumber.Contains(CardNumber.Five))
                count++;
            if (listNumber.Contains(CardNumber.Ace))
                count++;

            if (count == 5)
            {
                tieBrakerValue = 3;
                return true;
            }

            _CardActor[] cardsSort = packStraightFlush.OrderBy(x => x._numberCard).ToArray();

            count = 0;
            _CardActor lastRow = cardsSort[0];
            for (int i = 0; i < cardsSort.Length; i++)
            {
                if ((int)cardsSort[i]._numberCard - (int)lastRow._numberCard == 0)
                    continue;

                if ((int)cardsSort[i]._numberCard - (int)lastRow._numberCard == 1)
                {
                    if (count == 0)
                        packStraightFlush.Add(lastRow);

                    count++;
                    packStraightFlush.Add(lastRow);

                    if (count >= 4)
                        break;
                }
                else
                {
                    count = 0;
                    packStraightFlush.Clear();
                }

                lastRow = cardsSort[i];
            }

            if (count >= 4)
            {
                _cardHighlight = packStraightFlush.ToArray();
                SetTieBreaker(packStraightFlush);
                return true;
            }
            else
                return false;

        }
        else
            return false;
    }

    /// <summary>
    /// Check Straight
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public bool IsStraight(_CardActor[] cards)
    {
        int count = 0;

        List<CardNumber> listNumber = new List<CardNumber>();
        for (int i = 0; i < cards.Length; i++)
            listNumber.Add(cards[i]._numberCard);

        cards = cards.OrderBy(x => x._numberCard).ToArray();
        List<_CardActor> packStraight = new List<_CardActor>();

        if (listNumber.Contains(CardNumber.Two))
            count++;
        if(listNumber.Contains(CardNumber.Three))
            count++;
        if(listNumber.Contains(CardNumber.Four))
            count++;
        if(listNumber.Contains(CardNumber.Five))
            count++;
        if(listNumber.Contains(CardNumber.Ace))
            count++;

        if (count == 5)
        {
            tieBrakerValue = 3;
            return true;
        }

        count = 0;
        _CardActor lastRow = cards[0];
        for (int i = 0; i < cards.Length; i++)
        {
            if ((int)cards[i]._numberCard - (int)lastRow._numberCard == 0)
                continue;

            if ((int)cards[i]._numberCard - (int)lastRow._numberCard == 1)
            {
                if (count == 0)
                    packStraight.Add(lastRow);

                count++;
                packStraight.Add(lastRow);

                if (count >= 4)
                    break;
            }
            else
            {
                count = 0;
                packStraight.Clear();
            }

            lastRow = cards[i];
        }

        if (count >= 4)
        {
            _cardHighlight = packStraight.ToArray();
            SetTieBreaker(packStraight);
            return true;
        }
        else
            return false;

    }
    
    /// <summary>
    /// Check Flush
    /// </summary>
    /// <returns></returns>
    public bool IsFlush(_CardActor[] cards)
    {
        Dictionary<Suite, int> suit = new Dictionary<Suite, int>(); // in Hand h,s,c,h,c,s,h >> that will be {(h,3), (s,2), (c,2)}
        cards = cards.OrderByDescending(x => x._numberCard).ToArray();

        foreach (_CardActor card in cards)
        {
            if (suit.ContainsKey(card._suite))
                suit[card._suite] += 1; // Increase value of the same suit
            else
                suit.Add(card._suite, 1); // Add new unique suit in dictionary
        }
        suit = suit.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value); //{(h,3), (s,2), (c,2)}

        if (suit.First().Value >= 5)
        {
            Suite flushSuite = suit.First().Key;
            List<_CardActor> packFlush = new List<_CardActor>();

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i]._suite == flushSuite)
                {
                    packFlush.Add(cards[i]);

                    if (cards[i].handCard && kickerValue == -1)
                        kickerValue = (int)cards[i]._numberCard;
                }
            }

            _cardHighlight = packFlush.ToArray();

            if (packFlush.Count >= 5)
                SetTieBreakerFlush(packFlush);

            return true;
        }
        else
            return false;
    }
    
    private int[] rankcount;//shows array, if player have 3 king in hand,rankcount[0]=3
    private CardNumber[] rankcard;  //shows array, if player have 3 king in hand,rankcount[0]=king (11)
    private _CardActor[] _cardHighlight;

    public int kickerValue; // Only for pair, 2 pairs, 3 pairs, for of a kind
    public int secondKickerValue;

    public int tieBrakerValue; // tie braker for straight and flash
    public int secondTieBrakerValue;

    public int[] Rankcount
    {
        get
        {
            return rankcount;
        }

        set
        {
            rankcount = value;
        }
    }
    public CardNumber[] Rankcard
    {
        get
        {
            return rankcard;
        }

        set
        {
            rankcard = value;
        }
    }
    public _CardActor[] CardHighlight
    {
        get
        {
            return _cardHighlight;
        }

        set
        {
            _cardHighlight = value;
        }
    }

    /// <summary>
    /// To Check Pair, 2 Pairs, Triple, Four of a Kind
    /// </summary>
    /// <param name="cards">Final Card : Table + Hand</param>
    /// <param name="_hand">Card on Hand</param>
    public void SetHandRanker(List<_CardActor> cards)
    {        
        Dictionary<CardNumber, int> rank = new Dictionary<CardNumber, int>(); // in Hand 3,5,7,6,5,j,4 >> that will be {(3,1), (5,2), (7,1), (6,1), (j,1), (4,1) }

        foreach (_CardActor card in cards)
        {
            if (rank.ContainsKey(card._numberCard))
                rank[card._numberCard] += 1; //Increase value of the same number
            else
                rank.Add(card._numberCard, 1); //Add new unique number in dictionary
        }

        rank = rank.OrderByDescending(x => x.Value).ThenByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value); //{(5,2), (3,1) , (7,1), (6,1), (j,1), (4,1) }
        
        //--------------------------------- Check Rank

        rankcount = rank.Values.ToArray<int>();
        rankcard = rank.Keys.ToArray<CardNumber>();
    }

    void SetTieBreaker(List<_CardActor> cardsPacket)
    {
        _CardActor[] pack = cardsPacket.OrderByDescending(x => x._numberCard).ToArray();
        int front = (int)pack[0]._numberCard;
        int back = pack.Length > 4 ? (int)pack[4]._numberCard : 1;

        tieBrakerValue = front + back;
    }

    void SetTieBreakerFlush(List<_CardActor> cardsPacket)
    {
        _CardActor[] pack = cardsPacket.OrderByDescending(x => x._numberCard).ToArray();
        tieBrakerValue = 0;

        for (int i = 0; i < 5; i++)
            tieBrakerValue += (int)((int)pack[i]._numberCard * Math.Pow(10, 5 - i));
    }

    public void SetKicker(_CardActor[] _hand, int totalPair = 1, int arrKicker = 1) // pair K dan 7, Hand 2,7
    {
        _hand = _hand.OrderBy(x => x._numberCard).ToArray();
        int lastKicker = -1;
        kickerValue = -1;

        for (int x = 0; x < 2; x++)
        {
            if (totalPair == 2)
            {
                if (_hand[x]._numberCard != rankcard[0] && _hand[x]._numberCard != rankcard[1])
                    kickerValue = (int)_hand[x]._numberCard;
                else
                    kickerValue = lastKicker;
            }
            else
            {
                if (_hand[x]._numberCard != rankcard[0])
                    kickerValue = (int)_hand[x]._numberCard;
                else
                    kickerValue = lastKicker;
            }

            lastKicker = kickerValue;
        }

        lastKicker = -1;
        secondKickerValue = -1;
        if (arrKicker == 2)
        {
            for (int x = 0; x < 2; x++)
            {
                if (totalPair == 2)
                {
                    if (_hand[x]._numberCard != rankcard[0] && _hand[x]._numberCard != rankcard[1] && _hand[x]._numberCard != (CardNumber)kickerValue)
                        secondKickerValue = (int)_hand[x]._numberCard;
                    else
                        secondKickerValue = lastKicker;
                }
                else
                {
                    if (_hand[x]._numberCard != rankcard[0] && _hand[x]._numberCard != (CardNumber)kickerValue)
                        secondKickerValue = (int)_hand[x]._numberCard;
                    else
                        secondKickerValue = lastKicker;
                }

                lastKicker = secondKickerValue;
            }
        }
    }
}
