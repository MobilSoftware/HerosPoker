using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayRivals;
using System;
using UnityEngine.U2D;

public class CardsManager : MonoBehaviour 
{
    #region Singleton
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static CardsManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static CardsManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first CardsManager object in the scene.
                s_Instance = FindObjectOfType(typeof(CardsManager)) as CardsManager;
                if (s_Instance == null)
                    Debug.Log("Could not locate an CardsManager object. \n You have to have exactly one CardsManager in the scene.");
            }
            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        s_Instance = null;
    }
    #endregion

    public SpriteAtlas cardsAtlas;
    public Sprite[] cardsClub, cardsDiamond, cardsHeart, cardsSpade;

    public bool bOfflineMode, bResettingCards = false;
    protected Hashtable playerCardIndexes = new Hashtable();

    public Sprite cardBack;

    private Vector3 origPos;

    void Start()
    {
        cardsClub = new Sprite[13];
        cardsDiamond = new Sprite[13];
        cardsHeart = new Sprite[13];
        cardsSpade = new Sprite[13];

        for (int i = 0; i < 13; i++)
        {
            cardsClub[i] = cardsAtlas.GetSprite("Card_" + (i + 1) + "C");
            cardsDiamond[i] = cardsAtlas.GetSprite("Card_" + (i + 1) + "D");
            cardsHeart[i] = cardsAtlas.GetSprite("Card_" + (i + 1) + "S");
            cardsSpade[i] = cardsAtlas.GetSprite("Card_" + (i + 1) + "P");
        }
    }

    public CardType GetCardTypeAndPoints(string strCardType, out int cardPoints)
    {
        string[] cardTypes = strCardType.Split('_');

        CardType type = CardType.Club;
        try
        {
            type = (CardType)Enum.Parse(typeof(CardType), cardTypes[0], true);
        }
        catch (Exception ex) { }

        cardPoints = 1;
        if (cardTypes.Length > 1)
        {
            string strPoint = cardTypes[1];
            cardPoints = GetCardPoints(strPoint);
        }

        return type;
    }

    public string GetStringCardType (CardType ct, int points )
    {
        return ct.ToString () + "_" +  GetCardIndexString (points);
    }

    public string GetCardIndexString (int cardIndex)
    {
        string indexString = "";

        switch (cardIndex)
        {
            case 2: indexString = "Two"; break;
            case 3: indexString = "Three"; break;
            case 4: indexString = "Four"; break;
            case 5: indexString = "Five"; break;
            case 6: indexString = "Six"; break;
            case 7: indexString = "Seven"; break;
            case 8: indexString = "Eight"; break;
            case 9: indexString = "Nine"; break;
            case 10: indexString = "Ten"; break;
            case 11: indexString = "Jack"; break;
            case 12: indexString = "Queen"; break;
            case 13: indexString = "King"; break;
            case 14: indexString = "Ace"; break;
        }

        return indexString;
    }

    public int GetCardPoints(string strPoint)
    {
        int cardPoints = 1;
        switch (strPoint)
        {
            case "Ace": cardPoints = 14; break;
            case "One": cardPoints = 14; break;
            case "Two": cardPoints = 2; break;
            case "Three": cardPoints = 3; break;
            case "Four": cardPoints = 4; break;
            case "Five": cardPoints = 5; break;
            case "Six": cardPoints = 6; break;
            case "Seven": cardPoints = 7; break;
            case "Eight": cardPoints = 8; break;
            case "Nine": cardPoints = 9; break;
            case "Ten": cardPoints = 10; break;
            case "Jack": cardPoints = 11; break;
            case "Queen": cardPoints = 12; break;
            case "King": cardPoints = 13; break;
        }

        return cardPoints;
    }

    public Sprite GetCardSprite(string cardType)
    {
        Sprite[] collections = null;
        int cardPoint = 1;
        CardType type = GetCardTypeAndPoints(cardType, out cardPoint);
        switch (type)
        {
            case CardType.Club: collections = cardsClub; break;
            case CardType.Diamond: collections = cardsDiamond; break;
            case CardType.Heart: collections = cardsHeart; break;
            case CardType.Spade: collections = cardsSpade; break;
        }

        //One or Ace has a index of one
        if (cardPoint >= 14)
            cardPoint = 1;

        int cardIndex = cardPoint - 1;
        return collections[cardIndex];
    }

    public Sprite GetCardSpriteByInt (int _cardType )
    {
        Sprite[] collections = null;
        int cardPoint = 1;
        CardInfoType cardType = (CardInfoType) _cardType;
        CardType type = GetCardTypeAndPoints (cardType.ToString(), out cardPoint);
        switch (type)
        {
            case CardType.Club: collections = cardsClub; break;
            case CardType.Diamond: collections = cardsDiamond; break;
            case CardType.Heart: collections = cardsHeart; break;
            case CardType.Spade: collections = cardsSpade; break;
        }

        //One or Ace has a index of one
        if (cardPoint >= 14)
            cardPoint = 1;

        int cardIndex = cardPoint - 1;
        return collections[cardIndex];
    }

    private static int CompareTransform(Transform A, Transform B)
    {
        return A.name.CompareTo(B.name);
    }
}
