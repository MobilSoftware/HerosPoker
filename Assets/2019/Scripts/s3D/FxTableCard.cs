using UnityEngine;

public class FxTableCard : MonoBehaviour
{
    public Transform trStart;
    public Transform trEnd;

    private float duration = 0.25f;

    public void DealTableCard (GameObject objTableCard ) //Animasi Kartu dari dealer ke table card
    {
        gameObject.SetActive (true);
        LeanTween.move (gameObject, trEnd.position, duration).setFrom (trStart.position).setOnComplete (() => AfterComplete(objTableCard));
    }

    private void AfterComplete (GameObject objTableCard)
    {
        gameObject.SetActive (false);
        objTableCard.SetActive (true);
    }
}
