using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    //equivalent of GenericAvater
    public int id;
    private _SpineObject spineHero;

    private Vector3 posLuBu = new Vector3 (20f, -33f, 0f);
    private Vector3 scaleLuBu = new Vector3 (2f, 2f, 1f);

    public void Reset ()
    {
        if (spineHero != null)
        {
            spineHero.StopRandomMove ();
            Destroy (spineHero.gameObject);
        }
    }

    public void LoadSpine (int heroUsed, bool isMine)
    {
        StartCoroutine (_LoadSpine (heroUsed, isMine));
    }

    IEnumerator _LoadSpine (int heroUsed, bool isMine)
    {
        if (spineHero != null)
        {
            Destroy (spineHero.gameObject);
            yield return null;
        }

        id = heroUsed;
        _SpineObject spHero = null;
        if (id == 100)
            spHero = PokerManager.instance.spLuBu;
        else if (id == 200)
            spHero = PokerManager.instance.spCleo;

        spineHero = Instantiate (spHero, this.transform);
        if (isMine)
            SetMyPartBack ();
        yield return null;

        spineHero.transform.localEulerAngles = Vector3.zero;
        if (id == 100)
        {
            spineHero.transform.localPosition = posLuBu;
            spineHero.transform.localScale = scaleLuBu;
        }
        else if (id == 200)
        {
            spineHero.transform.localPosition = Vector3.zero;
            spineHero.transform.localScale = Vector3.one;
        }
        spineHero.StartRandomMove ();
    }

    public void SetMyPartBack ()
    {
        spineHero.partBack.MeshRenderer.sortingOrder = 0;
    }

    public void Revert ()
    {
        _SpineUtility.SetSkinColor (spineHero.mySkelAnim, Color.white);
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
