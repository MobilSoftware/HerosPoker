using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CardPoints{    One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, None}
public enum CardType{ Diamond, Club, Heart, Spade, None}


//Other has all the  cards
public enum CardInfoType
{
    Spade_One, Heart_One, Club_One, Diamond_One, Spade_Two, Heart_Two, Club_Two, Diamond_Two, Spade_Three, Heart_Three, Club_Three, Diamond_Three, Spade_Four, Heart_Four, Club_Four, Diamond_Four,
    Spade_Five, Heart_Five, Club_Five, Diamond_Five, Spade_Six, Heart_Six, Club_Six, Diamond_Six, Spade_Seven, Heart_Seven, Club_Seven, Diamond_Seven, Spade_Eight, Heart_Eight, Club_Eight, Diamond_Eight,
    Spade_Nine, Heart_Nine, Club_Nine, Diamond_Nine, Spade_Ten, Heart_Ten, Club_Ten, Diamond_Ten, Spade_Jack, Heart_Jack, Club_Jack, Diamond_Jack, Spade_Queen, Heart_Queen, Club_Queen, Diamond_Queen,
    Spade_King, Heart_King, Club_King, Diamond_King, Max
}

public enum CardRank
{
    Club_Two, Club_Three, Club_Four, Club_Five, Club_Six, Club_Seven, Club_Eight, Club_Nine, Club_Ten, Club_Jack, Club_Queen, Club_King, Club_One,
    Diamond_Two, Diamond_Three, Diamond_Four, Diamond_Five, Diamond_Six, Diamond_Seven, Diamond_Eight, Diamond_Nine, Diamond_Ten, Diamond_Jack, Diamond_Queen, Diamond_King, Diamond_One,
    Heart_Two, Heart_Three, Heart_Four, Heart_Five, Heart_Six, Heart_Seven, Heart_Eight, Heart_Nine, Heart_Ten, Heart_Jack, Heart_Queen, Heart_King, Heart_One,
    Spade_Two, Spade_Three, Spade_Four, Spade_Five, Spade_Six, Spade_Seven, Spade_Eight, Spade_Nine, Spade_Ten, Spade_Jack, Spade_Queen, Spade_King, Spade_One, Max
}

public class CardBrief
{
    public CardType cardType;
    public CardPoints cardPoints;
    public int points = -1;

    public CardBrief(CardType cardType, int points) { this.cardType = cardType; this.points = points; }
}


public class Card : MonoBehaviour 
{
    public GameObject objBack;
    public Image imgIcon;
    public CardInfoType cardInfoType;
    public CardType cardType;
    public CardPoints cardPoints;
    public int points = -1;
    public bool bShowing = false;

    public Card() { }
    public Card(CardType cardType, int points) { this.cardType = cardType; this.points = points; }

    // Use this for initialization
    public void AssignCard (CardInfoType cardInfoType, CardPoints cardPoints) {
        this.cardInfoType = cardInfoType;
        this.cardPoints = cardPoints;

        string[] cardInfo = cardInfoType.ToString().Split('_');
        cardType = (CardType)System.Enum.Parse(typeof(CardType), cardInfo[0], true);


        switch (cardPoints)
        {
            case CardPoints.One: points = 1; break;
            case CardPoints.Two: points = 2; break;
            case CardPoints.Three: points = 3; break;
            case CardPoints.Four: points = 4; break;
            case CardPoints.Five: points = 5; break;
            case CardPoints.Six: points = 6; break;
            case CardPoints.Seven: points = 7; break;
            case CardPoints.Eight: points = 8; break;
            case CardPoints.Nine: points = 9; break;
            default: points = 10; break;
        }

        Sprite sprite = null;
	    switch(cardType)
        {
            case CardType.Club: sprite = CardsManager.instance.cardsClub[(int)cardPoints]; break;
            case CardType.Diamond: sprite = CardsManager.instance.cardsDiamond[(int)cardPoints]; break;
            case CardType.Heart: sprite = CardsManager.instance.cardsHeart[(int)cardPoints]; break;
            case CardType.Spade: sprite = CardsManager.instance.cardsSpade[(int)cardPoints]; break;
        }

        if(imgIcon != null)
            imgIcon.sprite = sprite;
	}

    public IEnumerator MakeCardActive(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(true);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    public void ShowPoints()
    {
        LeanTween.scale(objBack, new Vector3(0, 1, 1), 1.0f).setEase(LeanTweenType.easeOutQuart);
        bShowing = true;
    }

    public void HidePoints()
    {
        LeanTween.scale(objBack, new Vector3(1, 1, 1), 0.3f).setEase(LeanTweenType.easeOutQuart);
        Invoke("HideNormal", 0.5f);
        bShowing = false;
    }

    //TODO : Show move back animation to original position
    public void HideNormal()
    {
        Hide();
    }


    public void Hide(bool bInstant = false)
    {
        if (bInstant)
            transform.localScale = Vector3.zero;
        {
            //this.gameObject.SetActive(false);
            points = -1;
            LeanTween.scale(gameObject, new Vector3(0, 0, 0), 0.3f).setEase(LeanTweenType.easeOutQuart);

            bShowing = false;
            Invoke("SetUnactiveCard", 0.5f);
        }
    }

    private void SetUnactiveCard()
    {
        bShowing = false;
        objBack.transform.localScale = new Vector3(1, 1, 1);
        this.gameObject.SetActive(false);
    }

}
