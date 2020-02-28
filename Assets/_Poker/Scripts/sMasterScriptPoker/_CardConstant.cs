public enum Suite
{
    Diamonds = 0,
    Clubs = 1,
    Hearts = 2,
    Spades = 3
}

public enum HandRankPoker
{
    none = 0,
    highCard = 1,
    onePair,
    twoPairs,
    threeOfaKind,
    straight,
    flush,
    fullHouse,
    fourOfAKind,
    straightFlush,
    royalFlush

}

public enum CardNumber
{
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace,
};

public enum GameTurnState
{
    ShufflingDeck,
    WaitingForBets,
    DealingCards,
    PlayingPlayerHand,
    Flop,
    Turn,
    River,
    PlayingDealerHand,
    Complete
};
