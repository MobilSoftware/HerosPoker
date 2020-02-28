using UnityEngine;
using UnityEngine.UI;

public class _CardActor : MonoBehaviour
{
    public Suite _suite;
    public CardNumber _numberCard;
    public bool handCard;

    public Image _imageCard;
    public GameObject objFxCard;
    private Sprite sprCard;
    private float duration = 0.1f;

    private void OnEnable ()
    {
        FlipCardUp ();
    }

    public void RefreshCard(int idx)
    {
        int[] cardInfo = _CardManager.GetCardPoker(idx);

        _suite = (Suite)cardInfo[0];
        _numberCard = (CardNumber)cardInfo[1];

        switch ((int)_suite)
        {
            case 0: //diamond
                sprCard = CardsManager.instance.cardsDiamond[(int)_numberCard + 1 > 12 ? 0 : (int)_numberCard + 1];
                break;
            case 1: //club
                sprCard = CardsManager.instance.cardsClub[(int)_numberCard + 1 > 12 ? 0 : (int)_numberCard + 1];
                break;
            case 2: //heart
                sprCard = CardsManager.instance.cardsHeart[(int)_numberCard + 1 > 12 ? 0 : (int)_numberCard + 1];
                break;
            case 3: //spade
                sprCard = CardsManager.instance.cardsSpade[(int)_numberCard + 1 > 12 ? 0 : (int)_numberCard + 1];
                break;
        }

        _imageCard.sprite = sprCard;
    }

    public void FlipCardUp ()
    {
        _imageCard.sprite = CardsManager.instance.cardBack;
        LeanTween.scaleX (gameObject, 0f, duration).setOnComplete (OpenUp);
    }

    private void OpenUp ()
    {
        _imageCard.sprite = sprCard;
        LeanTween.scaleX (gameObject, 1f, duration);
    }

    public void FlipCardDown () //Tutup
    {
        LeanTween.scaleX (gameObject, 0f, duration).setOnComplete (OpenDown);
    }

    private void OpenDown ()
    {
        _imageCard.sprite = CardsManager.instance.cardBack;
        LeanTween.scaleX (gameObject, 1f, duration).setOnComplete (() => Invoke ("SetSpriteAndHide", 0.2f));
    }

    private void SetSpriteAndHide()
    {
        _imageCard.sprite = sprCard;
        gameObject.SetActive (false);
    }
}
