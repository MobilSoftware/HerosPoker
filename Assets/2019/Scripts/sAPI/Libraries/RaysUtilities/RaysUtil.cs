using System;
using System.Linq;

namespace Rays.Utilities
{
    public static class Util
    {
        public const string ALPHANUMERIC = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [Serializable]
        public class DeckType
        {
            public int row = 0; //0. Top, 1. Middle, 2. Bottom
            public int[] card = { };
            public int score = 0;
            public ApiBridge.ScoringType scoreType = 0;
            public int highCard = -1; //-1. Unset
            public int deckKind = -1; //-1. Unset, 0. Spade, 1. Heart, 2. Club, 3. Diamond
        }

        public static string GetBestFormation ( string _cardset )
        {
            try
            {
                int[] deck = StringToDeck (_cardset);
                int[] top = SortCard (deck);
                if (deck.Length >= 13)
                {
                    DeckType bottomDeck = new DeckType ();
                    int[] bottom = ReturnCard (FindBestHand (AdditionalCard (top), out bottomDeck));
                    top = ReturnDeck (deck, bottom);

                    DeckType middleDeck = new DeckType ();
                    int[] middle = ReturnCard (FindBestHand (AdditionalCard (top), out middleDeck));
                    top = ReturnDeck (top, middle);

                    //Rank Flush based on kind
                    /*if (bottomDeck.deckKind >= 0 && middleDeck.deckKind >= 0)
                    {
                        if (bottomDeck.deckKind > middleDeck.deckKind)
                        {
                            int[] tempDeck = bottom;
                            bottom = middle;
                            middle = tempDeck;
                        }
                    }*/

                    //Rank Straight based on Ace
                    /*if (bottomDeck.scoreType == ApiBridge.ScoringType.Straight_Bottom && middleDeck.scoreType == ApiBridge.ScoringType.Straight_Middle)
                    {
                        if(middleDeck.card[4] < 4 && bottomDeck.card[0] > 3)
                        {
                            int[] tempDeck = bottom;
                            bottom = middle;
                            middle = tempDeck;
                        }
                    }*/

                    //Rank Flush based on second best card
                    if (bottomDeck.deckKind >= 0 && middleDeck.deckKind >= 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int middleVal = UnityEngine.Mathf.FloorToInt (middleDeck.card[i] / 4);
                            int bottomVal = UnityEngine.Mathf.FloorToInt (bottomDeck.card[i] / 4);
                            if (i == 0)
                            {
                                if (middleVal == 0) middleVal += 13;
                                if (bottomVal == 0) bottomVal += 13;
                            }
                            //Print(middleVal+" = "+ bottomVal);
                            if (middleVal > bottomVal)
                            {
                                int[] tempDeck = bottom;
                                bottom = middle;
                                middle = tempDeck;
                                break;
                            }
                            else if (middleVal < bottomVal)
                            {
                                break;
                            }
                        }
                    }

                    //Rank Straight based on Ace
                    //Print(middleDeck.scoreType.ToString());
                    //Print(bottomDeck.scoreType.ToString());
                    if (bottomDeck.scoreType == ApiBridge.ScoringType.Straight_Bottom && middleDeck.scoreType == ApiBridge.ScoringType.Straight_Middle)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int middleVal = UnityEngine.Mathf.FloorToInt (middleDeck.card[i] / 4);
                            int bottomVal = UnityEngine.Mathf.FloorToInt (bottomDeck.card[i] / 4);
                            if (i == 0)
                            {
                                if (middleVal == 0) middleVal += 13;
                                if (bottomVal == 0) bottomVal += 13;
                            }
                            //Print(middleVal + " = " + bottomVal);
                            if (middleVal > bottomVal)
                            {
                                int[] tempDeck = bottom;
                                bottom = middle;
                                middle = tempDeck;
                                break;
                            }
                            else if (middleVal < bottomVal)
                            {
                                break;
                            }
                        }
                    }

                    if (bottom.Length == 13)
                    {
                        deck = ReverseDeck (bottom);
                    }
                    else if (top.Length + middle.Length + bottom.Length == 13)
                    {
                        deck = CompileNewDeck (top, middle, bottom);
                    }
                }
                else
                {
                    DeckType topDeck = new DeckType ();
                    deck = ReturnCard (FindBestHand (AdditionalCard (top), out topDeck));
                }

                return String.Join (",", deck.Select (i => i.ToString ()).ToArray ());
            }
            catch (Exception e)
            {
                Print (e.Message, "GetBestFormation error = ");
            }
            return "";
        }

        /*No Longer Used
        private static int DeckKind(int[] _deck)
        {
            int kind = -1;
            for (int i = 0; i < _deck.Length; i++)
            {
                if (kind == -1)
                {
                    kind = _deck[i] % 4;
                }
                else
                {
                    if (kind != _deck[i] % 4) return -1;
                }
            }
            return kind;
        }*/

        private static int[] StringToDeck ( string _cardset )
        {
            string[] cardset = _cardset.Split (',');
            int[] deck = new int[cardset.Length];
            for (int i = 0; i < deck.Length; i++) deck[i] = int.Parse (cardset[i]);
            return deck;
        }

        private static int[] SortCard ( int[] _deck )
        {
            for (int i = 0; i < _deck.Length - 1; i++) //Sort from the strongest first, Ace considered above King
            {
                for (int j = i + 1; j < _deck.Length; j++)
                {
                    int source = _deck[i];
                    if (source <= 3) source += 52;
                    int sourceVal = UnityEngine.Mathf.FloorToInt (source / 4);
                    int sourceKind = source % 4;
                    int destination = _deck[j];
                    if (destination <= 3) destination += 52;
                    int destinationVal = UnityEngine.Mathf.FloorToInt (destination / 4);
                    int destinationKind = destination % 4;
                    if ((sourceVal < destinationVal) || ((sourceVal == destinationVal) && (sourceKind > destinationKind)))
                    {
                        int swapCard = _deck[i];
                        _deck[i] = _deck[j];
                        _deck[j] = swapCard;
                    }
                }
            }
            return _deck;
        }

        private static int[] AdditionalCard ( int[] _deck )
        {
            int aceNum = 0;
            for (int i = 0; i < _deck.Length; i++) if (_deck[i] <= 3) aceNum += 1; //Check if Ace exist

            int[] deck = new int[_deck.Length + aceNum];
            aceNum = 0;
            for (int i = 0; i < _deck.Length; i++) //Make a new Deck with additional Ace card for low value
            {
                if (_deck[i] <= 3)
                {
                    deck[i] = _deck[i] + 52;
                    deck[_deck.Length + aceNum] = _deck[i];
                    aceNum += 1;
                }
                else
                {
                    deck[i] = _deck[i];
                }
            }
            return deck;
        }

        private static int[] ReturnCard ( int[] _deck )
        {
            for (int i = 0; i < _deck.Length; i++) //Convert Ace card back to low value
            {
                if (_deck[i] >= 52) _deck[i] = _deck[i] % 52;
            }
            return _deck;
        }

        private static int[] ReturnDeck ( int[] _deck, int[] minus )
        {
            int[] deck = new int[_deck.Length - minus.Length];
            int index = 0;
            for (int i = 0; i < _deck.Length; i++) //Remove already used card
            {
                if (Array.IndexOf (minus, _deck[i]) == -1) deck[index++] = _deck[i] % 52;
            }
            return deck;
        }

        private static int[] ReverseDeck ( int[] _deck )
        {
            int[] deck = new int[_deck.Length];
            for (int i = 0; i < _deck.Length; i++)
            {
                deck[i] = _deck[_deck.Length - 1 - i];
            }
            return deck;
        }

        private static int[] CompileNewDeck ( int[] top, int[] middle, int[] bottom )
        {
            int[] deck = new int[top.Length + middle.Length + bottom.Length];
            int index = 0;
            int topIndex = 0;
            for (int i = 0; i < deck.Length; i++) //Compile best card formation
            {
                if (i <= 2)
                {
                    deck[index++] = top[topIndex++] % 52;
                }
                else if (i <= 7)
                {
                    deck[index++] = (i - 3 < middle.Length ? middle[i - 3] % 52 : top[topIndex++] % 52);
                }
                else
                {
                    deck[index++] = (i - 8 < bottom.Length ? bottom[i - 8] % 52 : top[topIndex++] % 52);
                }
            }
            return deck;
        }

        public static int[] FindBestHand ( int[] _deck, out DeckType deckType, bool forPoker = false )
        {
            deckType = new DeckType ();
            deckType.card = _deck;
            deckType.scoreType = ApiBridge.ScoringType.None;

            int[] bestDragonKing = SetNewInt (13);
            int[] bestDragon = SetNewInt (13);
            int[] bestRoyalFlush = SetNewInt (5);
            int[] bestStraightFlush = SetNewInt (5);
            int[] bestFourOfAKind = SetNewInt (4);
            int[] bestFullHouse = SetNewInt (5);
            int[] bestFlush = SetNewInt (5);
            int[] bestStraight = SetNewInt (5);
            int[] bestThreeOfAKind = SetNewInt (3);
            int[] bestTwoPair = SetNewInt (4);
            int[] bestOnePair = SetNewInt (2);
            int[] bestHighCard = SetNewInt (1);
            for (int i = 0; i < _deck.Length; i++) //Find best card pattern
            {
                int[] dragonKing = PushToArray (SetNewInt (13), _deck[i]);
                int[] dragon = PushToArray (SetNewInt (13), _deck[i]);
                int[] royalFlush = PushToArray (SetNewInt (5), _deck[i]);
                int[] straightFlush = PushToArray (SetNewInt (5), _deck[i]);
                int[] fourOfAKind = PushToArray (SetNewInt (4), _deck[i]);
                int[] fullHouse = SetNewInt (5);
                int[] flush = PushToArray (SetNewInt (5), _deck[i]);
                int[] straight = PushToArray (SetNewInt (5), _deck[i]);
                int[] threeOfAKind = PushToArray (SetNewInt (3), _deck[i]);
                int[] twoPair = SetNewInt (4);
                int[] onePair = PushToArray (SetNewInt (2), _deck[i]);
                int[] highCard = PushToArray (SetNewInt (1), _deck[i]);

                if (!IsFound (bestHighCard)) bestHighCard = highCard;

                if (i < _deck.Length - 1)
                {
                    for (int j = i + 1; j < _deck.Length; j++)
                    {
                        int source = _deck[i];
                        int destination = _deck[j];
                        if (source % 52 != destination % 52)
                        {
                            int sourceVal = UnityEngine.Mathf.FloorToInt (source / 4);
                            int sourceKind = source % 4;
                            int destinationVal = UnityEngine.Mathf.FloorToInt (destination / 4);
                            int destinationKind = destination % 4;

                            if (!IsFound (bestDragonKing)) if (destinationVal == sourceVal - GetArraySize (dragonKing) && sourceKind == destinationKind) dragonKing = PushToArray (dragonKing, _deck[j]);
                            if (!IsFound (bestDragon)) if (destinationVal == sourceVal - GetArraySize (dragon)) dragon = PushToArray (dragon, _deck[j]);
                            if (!IsFound (bestRoyalFlush)) if (sourceVal >= 52 && destinationVal == sourceVal - GetArraySize (royalFlush) && sourceKind == destinationKind) royalFlush = PushToArray (royalFlush, _deck[j]);
                            if (!IsFound (bestStraightFlush)) if (destinationVal == sourceVal - GetArraySize (straightFlush) && sourceKind == destinationKind) straightFlush = PushToArray (straightFlush, _deck[j]);
                            if (!IsFound (bestFourOfAKind)) if (destinationVal == sourceVal) fourOfAKind = PushToArray (fourOfAKind, _deck[j]);
                            if (!IsFound (bestFlush)) if (sourceKind == destinationKind) flush = PushToArray (flush, _deck[j]);
                            if (!IsFound (bestStraight)) if (destinationVal == sourceVal - GetArraySize (straight)) straight = PushToArray (straight, _deck[j]);
                            if (!IsFound (bestThreeOfAKind)) if (destinationVal == sourceVal) threeOfAKind = PushToArray (threeOfAKind, _deck[j]);
                            if (!IsFound (bestOnePair)) if (destinationVal == sourceVal) onePair = PushToArray (onePair, _deck[j]);
                        }
                    }
                    if (!IsFound (bestFullHouse) && IsFound (threeOfAKind))
                    { //Complete full house pair
                        int[] pair = (forPoker ? FindLowestPair (_deck, threeOfAKind[0]) : FindLowestPair (_deck, threeOfAKind[0]));
                        if (IsFound (pair))
                        {
                            fullHouse = PushToArray (fullHouse, threeOfAKind);
                            fullHouse = PushToArray (fullHouse, pair);
                        }
                    }
                    if (!IsFound (bestTwoPair) && IsFound (onePair))
                    { //Complete second pair
                        int[] pair = (forPoker ? FindLowestPair (_deck, onePair[0]) : FindLowestPair (_deck, onePair[0]));
                        if (IsFound (pair))
                        {
                            twoPair = PushToArray (twoPair, onePair); ;
                            twoPair = PushToArray (twoPair, pair); ;
                        }
                    }
                }
                if (IsFound (dragonKing)) bestDragonKing = dragonKing;
                if (IsFound (dragon)) bestDragon = dragon;
                if (IsFound (royalFlush)) bestRoyalFlush = royalFlush;
                if (IsFound (straightFlush)) bestStraightFlush = straightFlush;
                if (IsFound (fourOfAKind)) bestFourOfAKind = fourOfAKind;
                if (IsFound (fullHouse)) bestFullHouse = fullHouse;
                if (IsFound (flush)) bestFlush = flush;
                if (IsFound (straight)) bestStraight = straight;
                if (IsFound (threeOfAKind)) bestThreeOfAKind = threeOfAKind;
                if (IsFound (twoPair)) bestTwoPair = twoPair;
                if (IsFound (onePair)) bestOnePair = onePair;
            }
            if (IsFound (bestDragonKing))
            {
                deckType.card = bestDragonKing;
                deckType.scoreType = ApiBridge.ScoringType.Dragon_King;
            }
            else if (IsFound (bestDragon))
            {
                deckType.card = bestFlush;
                deckType.scoreType = ApiBridge.ScoringType.Dragon;
            }
            else if (IsFound (bestRoyalFlush))
            {
                deckType.card = bestRoyalFlush;
                deckType.scoreType = ApiBridge.ScoringType.Royal_Flush_Bottom;
            }
            else if (IsFound (bestStraightFlush))
            {
                deckType.card = bestStraightFlush;
                deckType.scoreType = ApiBridge.ScoringType.Straight_Flush_Bottom;
            }
            else if (IsFound (bestFourOfAKind))
            {
                deckType.card = bestFourOfAKind;
                deckType.scoreType = ApiBridge.ScoringType.Four_Of_Kind_Bottom;
            }
            else if (IsFound (bestFullHouse))
            {
                deckType.card = bestFullHouse;
                deckType.scoreType = ApiBridge.ScoringType.Full_House_Bottom;
            }
            else if (IsFound (bestFlush))
            {
                deckType.card = bestFlush;
                deckType.scoreType = ApiBridge.ScoringType.Flush_Bottom;
                deckType.deckKind = bestFlush[0] % 4;
            }
            else if (IsFound (bestStraight))
            {
                deckType.card = bestStraight;
                deckType.scoreType = ApiBridge.ScoringType.Straight_Bottom;
            }
            else if (IsFound (bestThreeOfAKind))
            {
                deckType.card = bestThreeOfAKind;
                deckType.scoreType = ApiBridge.ScoringType.Three_Of_Kind_Bottom;
            }
            else if (IsFound (bestTwoPair))
            {
                deckType.card = bestTwoPair;
                deckType.scoreType = ApiBridge.ScoringType.Two_Pair;
            }
            else if (IsFound (bestOnePair))
            {
                deckType.card = bestOnePair;
                deckType.scoreType = ApiBridge.ScoringType.One_Pair;
            }
            else if (IsFound (bestHighCard))
            {
                deckType.card = bestHighCard;
                deckType.scoreType = ApiBridge.ScoringType.High_Card;
            }
            return deckType.card;
        }

        public static int[] FindLowestPair ( int[] _deck, int exclude = -1 )
        {
            int[] pair = SetNewInt (2);
            int[] pair2 = SetNewInt (2);
            int[] pair3 = SetNewInt (2);
            int[] pair4 = SetNewInt (2);
            int[] pair5 = SetNewInt (2);
            int[] pair6 = SetNewInt (2);
            for (int i = _deck.Length - 1; i > 0; i--) //Find lowest pair
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (_deck[i] > 3 && UnityEngine.Mathf.FloorToInt (_deck[i] / 4) == UnityEngine.Mathf.FloorToInt (_deck[j] / 4) && UnityEngine.Mathf.FloorToInt (_deck[i] / 4) % 13 != UnityEngine.Mathf.FloorToInt (exclude / 4) % 13)
                    {
                        if (!IsFound (pair))
                        {
                            pair = PushToArray (pair, _deck[i]);
                            pair = PushToArray (pair, _deck[j]);
                        }
                        else if (!IsFound (pair2))
                        {
                            pair2 = PushToArray (pair2, _deck[i]);
                            pair2 = PushToArray (pair2, _deck[j]);
                        }
                        else if (!IsFound (pair3))
                        {
                            if (pair[1] != _deck[i])
                            {
                                pair3 = PushToArray (pair3, _deck[i]);
                                pair3 = PushToArray (pair3, _deck[j]);
                            }
                        }
                        else if (!IsFound (pair4))
                        {
                            pair4 = PushToArray (pair4, _deck[i]);
                            pair4 = PushToArray (pair4, _deck[j]);
                        }
                        else if (!IsFound (pair5))
                        {
                            if (pair3[1] != _deck[i])
                            {
                                pair5 = PushToArray (pair5, _deck[i]);
                                pair5 = PushToArray (pair5, _deck[j]);
                            }
                        }
                        else if (!IsFound (pair6))
                        {
                            pair6 = PushToArray (pair6, _deck[i]);
                            pair6 = PushToArray (pair6, _deck[j]);
                        }
                    }
                }
            }
            if (IsFound (pair))
            {
                if (IsFound (pair2) && pair[0] == pair2[0])
                {
                    if (IsFound (pair3))
                    {
                        if (IsFound (pair4) && pair3[0] == pair4[0])
                        {
                            if (IsFound (pair5))
                            {
                                if (IsFound (pair6))
                                {
                                    if (pair5[0] == pair6[0])
                                    {
                                        return pair;
                                    }
                                    else
                                    {
                                        return pair5;
                                    }
                                }
                                else
                                {
                                    return pair;
                                }
                            }
                            else
                            {
                                return pair3;
                            }
                        }
                        else
                        {
                            return pair3;
                        }
                    }
                }
            }
            return pair;
        }

        private static int GetArraySize ( int[] _deck )
        {
            for (int i = 0; i < _deck.Length; i++) if (_deck[i] == -1) return i;
            return _deck.Length;
        }

        private static int[] SetNewInt ( int intSize )
        {
            int[] newInt = new int[intSize];
            for (int i = 0; i < newInt.Length; i++) newInt[i] = -1;
            return newInt;
        }

        private static bool IsFound ( int[] pattern )
        {
            return (pattern[pattern.Length - 1] == -1 ? false : true);
        }

        private static int[] PushToArray ( int[] haystack, int needle, bool excludeNeedle = true )
        {
            bool needleFound = false;
            for (int i = 0; i < haystack.Length; i++)
            {
                if (haystack[i] % 52 == needle % 52)
                {
                    needleFound = true;
                    break;
                }
            }
            if (!needleFound || !excludeNeedle)
            {
                for (int i = 0; i < haystack.Length; i++)
                {
                    if (haystack[i] == -1)
                    {
                        haystack[i] = needle;
                        break;
                    }
                }
            }
            return haystack;
        }

        private static int[] PushToArray ( int[] haystack, int[] needle )
        {
            for (int i = 0; i < needle.Length; i++)
            {
                haystack = PushToArray (haystack, needle[i]);
            }
            return haystack;
        }

        public static string RandomChar ( int charlen, string charset = ALPHANUMERIC )
        {
            string output = "";
            while (charlen-- > 0)
            {
                output += charset[UnityEngine.Random.Range (0, charset.Length)];
            }
            return output;
        }

        public static void Print ( string log, string prefix = "Log = " )
        {
            UnityEngine.Debug.Log (prefix + log);
        }

        public static void Print ( int log, string prefix = "Log = " )
        {
            Print (log.ToString (), prefix);
        }

        public static void Print ( string[] log, string prefix = "Log = " )
        {
            Print (String.Join (",", log), prefix);
        }

        public static void Print ( int[] log, string prefix = "Log = " )
        {
            Print (String.Join (",", new System.Collections.Generic.List<int> (log).ConvertAll (i => i.ToString ()).ToArray ()), prefix);
        }

        public static string PlayerID2SearchTag ( int id )
        {
            return (id + 4096).ToString ("X");
        }

        public static int SearchTag2PlayerID ( string id )
        {
            return Convert.ToInt32 (id, 16) - 4096;
        }
    }
}