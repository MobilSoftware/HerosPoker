using UnityEngine;

public class CardBack : MonoBehaviour
{
    public Transform trStart;
    public GameObject objCard1;
    public GameObject objCard2;

    private Vector3 vrEnd1;
    private Vector3 vrEnd2;
    private float duration = 0.25f;

    private void Start ()
    {
        vrEnd1 = objCard1.transform.position;
        vrEnd2 = objCard2.transform.position;

        objCard1.transform.position = Vector3.one * 1000;
        objCard2.transform.position = Vector3.one * 1000;
    }

    private void OnDisable()
    {
        objCard1.SetActive(false);
        objCard2.SetActive(false);
    }

    public void Hide()
    {
        objCard1.transform.position = Vector3.one * 1000;
        objCard2.transform.position = Vector3.one * 1000;
    }

    public void DealFirstCard () //Draw Card1
    {
        LeanTween.move(objCard1, vrEnd1, duration).setFrom(trStart.position);
        objCard1.SetActive(true);
    }

    public void DealSecondCard () //Draw Card1
    {
        LeanTween.move(objCard2, vrEnd2, duration).setFrom(trStart.position);
        objCard2.SetActive(true);
    }
}
