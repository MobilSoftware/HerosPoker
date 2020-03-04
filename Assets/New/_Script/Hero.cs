using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    //equivalent of GenericAvater
    public string strCodeName;
    public int id;
    private _SpineObject spineHero;

    public void Show ()
    {
        gameObject.SetActive (true);
    }

    public void Hide ()
    {
        gameObject.SetActive (false);
    }

    public void LoadSpine ()
    {
        StartCoroutine (_LoadSpine ());
    }

    IEnumerator _LoadSpine ()
    {
        if (spineHero != null)
        {
            Destroy (spineHero.gameObject);
            yield return null;
        }

        spineHero = Instantiate (PokerManager.instance.spCleo, this.transform);
        yield return null;

        spineHero.transform.localPosition = Vector3.zero;
        spineHero.transform.localEulerAngles = Vector3.zero;
        spineHero.transform.localScale = Vector3.one;
    }
}
