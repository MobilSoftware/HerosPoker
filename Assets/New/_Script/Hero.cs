using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    //equivalent of GenericAvater
    public string strCodeName;
    public int id;
    private _SpineObject spineHero;

    public void Reset ()
    {
        if (spineHero != null)
        {
            spineHero.StopRandomMove ();
            Destroy (spineHero.gameObject);
        }
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
        spineHero.StartRandomMove ();
    }

    public void FoldAction ()
    {
        spineHero.SetAction (SpineAnim.FOLD);
        _SpineUtility.SetSkinColor (spineHero.mySkelAnim, Color.grey);
    }

    public void CallAction ()
    {
        spineHero.SetAction (SpineAnim.CALL);
    }

    public void CheckAction ()
    {
        spineHero.SetAction (SpineAnim.CHECK);
    }
}
