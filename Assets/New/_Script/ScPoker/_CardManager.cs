using UnityEngine;
using System.Collections.Generic;

public class _CardManager : MonoBehaviour
{
    public static _CardScoreCounting cardCounter;

    void Awake()
    {
        cardCounter = new _CardScoreCounting();
    }

    /// <summary>
    /// Setup The Ranks
    /// </summary>
    public void SetupRanks()
    {
        List<_PlayerPokerActor> playersOnTable = new List<_PlayerPokerActor>();
        playersOnTable.AddRange(_PokerGameManager.turnManager.matchPlayers);
        int loop = playersOnTable.Count;
        int rank = 1;

        foreach (_PlayerPokerActor player in playersOnTable)
        {
            player._playerHandRank = EvaluatPlayerHand(_PokerGameManager.instance.tableCard, player._myHandCard);

            player._myHighlightedCard = cardCounter.CardHighlight;
            player._kicker = cardCounter.kickerValue;
            player.secondKicker = cardCounter.secondKickerValue;

            player.valueHand = cardCounter.tieBrakerValue;
            player.secondValueHand = cardCounter.secondTieBrakerValue;
        }

        for (int x = loop; x > 0; x--)
        {
            if (playersOnTable.Count == 0)
                break;

            List<_PlayerPokerActor> tmpwinners = new List<_PlayerPokerActor>();
            List<_PlayerPokerActor> Winners = new List<_PlayerPokerActor>();
            HandRankPoker winningHand = MaxRankeInTable(playersOnTable.ToArray());//Hand Val Max

            int _kickerCard = -1;
            int _secKickerCard = -1;

            int _tieBreaker = -1;
            int _secTieBreaker = -1;

            foreach (_PlayerPokerActor p in playersOnTable)
            {
                if (p._playerHandRank == winningHand)
                    tmpwinners.Add(p);
            }

            if (tmpwinners.Count > 1)
            {
                if (DoWeTieBracker(winningHand))
                {
                    List<_PlayerPokerActor> winnersTie = new List<_PlayerPokerActor>();
                    _tieBreaker = MaxTieBreaker(tmpwinners.ToArray());//Max total value in numbers

                    foreach (_PlayerPokerActor p in tmpwinners)
                    {
                        if (p.valueHand == _tieBreaker)
                            winnersTie.Add(p);
                    }

                    tmpwinners.Clear();
                    tmpwinners.AddRange(winnersTie);
                }

                if (DoWeSeccondTieBracker(winningHand))
                {
                    List<_PlayerPokerActor> winnersTie = new List<_PlayerPokerActor>();
                    _secTieBreaker = MaxSecondTieBreaker(tmpwinners.ToArray());//Max total value in numbers

                    foreach (_PlayerPokerActor p in tmpwinners)
                    {
                        if (p.secondValueHand == _secTieBreaker)
                            winnersTie.Add(p);
                    }

                    tmpwinners.Clear();
                    tmpwinners.AddRange(winnersTie);
                }

                if (DoWeNeedKicker(winningHand))
                {
                    List<_PlayerPokerActor> winnersKicker = new List<_PlayerPokerActor>();
                    _kickerCard = MaxKickerValue(tmpwinners.ToArray());//Max total value in numbers

                    foreach (_PlayerPokerActor p in tmpwinners)
                    {
                        if (p._kicker == _kickerCard)
                            winnersKicker.Add(p);
                    }

                    tmpwinners.Clear();
                    tmpwinners.AddRange(winnersKicker);
                }

                if (DoWeNeedSecondKicker(winningHand))
                {
                    List<_PlayerPokerActor> winnersKicker = new List<_PlayerPokerActor>();
                    _secKickerCard = MaxSecKickerValue(tmpwinners.ToArray());//Max total value in numbers

                    foreach (_PlayerPokerActor p in tmpwinners)
                    {
                        if (p.secondKicker == _secKickerCard)
                            winnersKicker.Add(p);
                    }

                    tmpwinners.Clear();
                    tmpwinners.AddRange(winnersKicker);
                }

                Winners.AddRange(tmpwinners);

                int numWinners = Winners.Count;
                foreach (_PlayerPokerActor p in Winners)
                {
                    //p.PullTheChips(_mainPot / Winners.Count);
                    p.RANK = rank;
                    //Debug.LogError("player " + p.name + " : " + Winners.Count);

                    playersOnTable.Remove(p);
                }
            }
            else
            {
                //Debug.LogError("player " + tmpwinners[0].name + " : 1");

                tmpwinners[0].RANK = rank;
                playersOnTable.Remove(tmpwinners[0]);
                //tmpwinners[0].PullTheChips((int)_mainPot);
            }

            rank++;
        }

    }

    /// <summary>
    /// Hand value : 2 pairs ,one pair etc.
    /// </summary>
    /// <returns></returns>
    public HandRankPoker MaxRankeInTable(_PlayerPokerActor[] _players)
    {
        HandRankPoker MaxHand = HandRankPoker.none;

        foreach (_PlayerPokerActor p in _players)
            if (p._playerHandRank > MaxHand)//&& p.playerState != _PlayerPokerActor.PlayerState.fold)
                MaxHand = p._playerHandRank;

        return MaxHand;

    }

    /// <summary>
    /// Check do we need kicker, only for pair, 2 pairs, triple, four of a kind
    /// </summary>
    /// <returns></returns>
    public bool DoWeNeedKicker(HandRankPoker _rank)
    {
        if (_rank == HandRankPoker.onePair || _rank == HandRankPoker.twoPairs || _rank == HandRankPoker.threeOfaKind || _rank == HandRankPoker.fourOfAKind || _rank == HandRankPoker.flush)
            return true;
        else
            return false;

    }

    /// <summary>
    /// Check do we need kicker, only for pair, triple
    /// </summary>
    /// <returns></returns>
    public bool DoWeNeedSecondKicker(HandRankPoker _rank)
    {
        if (_rank == HandRankPoker.onePair || _rank == HandRankPoker.threeOfaKind)
            return true;
        else
            return false;

    }

    /// <summary>
    /// Check do we need tieBreaker, for all except royal flush
    /// </summary>
    /// <returns></returns>
    public bool DoWeTieBracker(HandRankPoker _rank)
    {
        if (_rank == HandRankPoker.royalFlush)
            return false;
        else
            return true;

    }

    /// <summary>
    /// Check do we need Second tieBreaker, for two pair & full house
    /// </summary>
    /// <returns></returns>
    public bool DoWeSeccondTieBracker(HandRankPoker _rank)
    {
        if (_rank == HandRankPoker.fullHouse || _rank == HandRankPoker.twoPairs)
            return true;
        else
            return false;

    }

    private int MaxKickerValue(_PlayerPokerActor[] winnersTie)
    {
        int max = -1;

        foreach (_PlayerPokerActor p in winnersTie)
        {
            if (p._kicker > max) //&& p.playerState != _PlayerPokerActor.PlayerState.fold)
                max = p._kicker;
        }

        return max;
    }

    private int MaxSecKickerValue(_PlayerPokerActor[] winnersTie)
    {
        int max = -1;

        foreach (_PlayerPokerActor p in winnersTie)
        {
            if (p.secondKicker > max) //&& p.playerState != _PlayerPokerActor.PlayerState.fold)
                max = p.secondKicker;
        }

        return max;
    }

    private int MaxTieBreaker(_PlayerPokerActor[] winnersTie)
    {
        int max = 0;

        foreach (_PlayerPokerActor p in winnersTie)
        {
            if (p.valueHand > max) //&& p.playerState != _PlayerPokerActor.PlayerState.fold)
                max = p.valueHand;
        }

        return max;
    }

    private int MaxSecondTieBreaker(_PlayerPokerActor[] winnersTie)
    {
        int max = 0;

        foreach (_PlayerPokerActor p in winnersTie)
        {
            if (p.secondValueHand > max) //&& p.playerState != _PlayerPokerActor.PlayerState.fold)
                max = p.secondValueHand;
        }

        return max;
    }

    public HandRankPoker EvaluatPlayerHand(_CardActor[] tableCard, _CardActor[] handCard)
    {
        List<_CardActor> finalHand = new List<_CardActor>();
        finalHand.AddRange(tableCard);
        finalHand.AddRange(handCard);

        cardCounter.CardHighlight = handCard;

        cardCounter.kickerValue = -1;
        cardCounter.tieBrakerValue = -1;
        cardCounter.secondTieBrakerValue = -1;

        cardCounter.SetHandRanker(finalHand);        

        if (cardCounter.IsRoyalFlush(finalHand.ToArray()))
            return HandRankPoker.royalFlush;

        if (cardCounter.IsStraightFlush(finalHand.ToArray()))
            return HandRankPoker.straightFlush;

        if (cardCounter.Rankcount[0] == 4)
        {
            cardCounter.tieBrakerValue = (int)cardCounter.Rankcard[0];
            cardCounter.SetKicker(handCard, 1);
            return HandRankPoker.fourOfAKind;
        }

        if (cardCounter.Rankcount[0] == 3 && cardCounter.Rankcount[1] == 2)
        {
            if ((int)cardCounter.Rankcard[0] > (int)cardCounter.Rankcard[1])
            {
                cardCounter.tieBrakerValue = (int)cardCounter.Rankcard[0];
                cardCounter.secondTieBrakerValue = (int)cardCounter.Rankcard[1];
            }
            else
            {
                cardCounter.tieBrakerValue = (int)cardCounter.Rankcard[1];
                cardCounter.secondTieBrakerValue = (int)cardCounter.Rankcard[0];
            }

            return HandRankPoker.fullHouse;
        }

        if (cardCounter.IsFlush(finalHand.ToArray()))
            return HandRankPoker.flush;

        if (cardCounter.IsStraight(finalHand.ToArray()))
            return HandRankPoker.straight;

        if (cardCounter.Rankcount[0] == 3)
        {
            cardCounter.tieBrakerValue = (int)cardCounter.Rankcard[0];
            cardCounter.SetKicker(handCard, 1, 2);
            return HandRankPoker.threeOfaKind;
        }

        if (cardCounter.Rankcount[0] == 2 && cardCounter.Rankcount[1] == 2)
        {
            if ((int)cardCounter.Rankcard[0] > (int)cardCounter.Rankcard[1])
            {
                cardCounter.tieBrakerValue = (int)cardCounter.Rankcard[0];
                cardCounter.secondTieBrakerValue = (int)cardCounter.Rankcard[1];
            }
            else
            {
                cardCounter.tieBrakerValue = (int)cardCounter.Rankcard[1];
                cardCounter.secondTieBrakerValue = (int)cardCounter.Rankcard[0];
            }

            cardCounter.SetKicker(handCard, 2);
            return HandRankPoker.twoPairs;
        }

        if (cardCounter.Rankcount[0] == 2)
        {
            cardCounter.tieBrakerValue = (int)cardCounter.Rankcard[0];
            cardCounter.SetKicker(handCard, 1, 2);

            return HandRankPoker.onePair;
        }
        else
        {
            if ((int)handCard[0]._numberCard > (int)handCard[1]._numberCard)
            {
                cardCounter.tieBrakerValue = (int)handCard[0]._numberCard;
                cardCounter.kickerValue = (int)handCard[1]._numberCard;
            }
            else
            {
                cardCounter.tieBrakerValue = (int)handCard[1]._numberCard;
                cardCounter.kickerValue = (int)handCard[0]._numberCard;
            }

            return HandRankPoker.highCard;
        }


    }

    #region Util card poker
    public static int[] GetCardPoker(int idx)
    {
        int[] cInfo = new int[2];
        //idx++;
        int a = idx + 1;
        cInfo[0] = a % 4 == 0 ? 0 : 4 - (a % 4); // 0 = spade, hearth, club, diamond

        //int a = (int)(idx / 4);
        //cInfo[1] = idx - (a * 4);

        switch (idx)
        {
            case 0:case 1:case 2:case 3:
                cInfo[1] = 12;
                break;
            case 4:case 5:case 6:case 7:
                cInfo[1] = 0;
                break;
            case 8:case 9:case 10:case 11:
                cInfo[1] = 1;
                break;
            case 12:case 13:case 14:case 15:
                cInfo[1] = 2;
                break;
            case 16:case 17:case 18:case 19:
                cInfo[1] = 3;
                break;
            case 20:case 21:case 22:case 23:
                cInfo[1] = 4;
                break;
            case 24:case 25:case 26:case 27:
                cInfo[1] = 5;
                break;
            case 28:case 29:case 30:case 31:
                cInfo[1] = 6;
                break;
            case 32:case 33:case 34:case 35:
                cInfo[1] = 7;
                break;
            case 36:case 37:case 38:case 39:
                cInfo[1] = 8;
                break;
            case 40:case 41:case 42:case 43:
                cInfo[1] = 9;
                break;
            case 44:case 45:case 46:case 47:
                cInfo[1] = 10;
                break;
            case 48:case 49:case 50:case 51:
                cInfo[1] = 11;
                break;
        }

        return cInfo;
    }
    #endregion
}
