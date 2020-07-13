using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StandHero : MonoBehaviour
{
    public int id;
    public Image imgHero;
    [HideInInspector]
    public _SpineObject spineHero;

    private Vector3 scaleLubu = new Vector3 (1.5f, 1.5f, 1f);

    private void Reset ()
    {
        if (spineHero != null)
        {
            spineHero.StopStandRandomMove ();
            Destroy (spineHero.gameObject);
        }
    }

    public void LoadFromBundle (int _costumeID )
    {
        switch (_costumeID)
        {
            case 10:
            case 11:
                StartCoroutine (_LoadFromBundle (_costumeID));
                imgHero.gameObject.SetActive (false);
                break;
            case 12:
                Reset ();
                imgHero.sprite = PokerManager.instance.sprMusashi;
                imgHero.gameObject.SetActive (true);
                break;
            case 13:
                Reset ();
                imgHero.sprite = PokerManager.instance.sprNapoleon;
                imgHero.gameObject.SetActive (true);
                break;
            case 14:
                Reset ();
                imgHero.sprite = PokerManager.instance.sprGenghis;
                imgHero.gameObject.SetActive (true);
                break;
            case 15:
                Reset ();
                imgHero.sprite = PokerManager.instance.sprAlexander;
                imgHero.gameObject.SetActive (true);
                break;
            case 16:
                Reset ();
                imgHero.sprite = PokerManager.instance.sprMasamune;
                imgHero.gameObject.SetActive (true);
                break;
            case 17:
                Reset ();
                imgHero.sprite = PokerManager.instance.sprGajahMada;
                imgHero.gameObject.SetActive (true);
                break;
            case 18:
                Reset ();
                imgHero.sprite = PokerManager.instance.sprHercules;
                imgHero.gameObject.SetActive (true);
                break;
        }
    }

    IEnumerator _LoadFromBundle (int _costumeID )
    {
        _SpineObject hero = BundleManager.instance.LoadFromBundle (_costumeID);
        if (hero)
        {
            Reset ();
            yield return _WFSUtility.wef;
            spineHero = Instantiate (hero, this.transform);
            yield return _WFSUtility.wef;
            spineHero.transform.localEulerAngles = Vector3.zero;
            spineHero.transform.localPosition = Vector3.one;
            //if (_costumeID == 10)
            //    spineHero.transform.localScale = scaleLubu;
            //else
                //spineHero.transform.localScale = Vector3.one;
            spineHero.StartStandRandomMove ();
        }
    }

    //public void LoadSpine ( int heroUsed )
    //{
    //    StartCoroutine (_LoadSpine (heroUsed));
    //}

    //IEnumerator _LoadSpine ( int heroUsed )
    //{
    //    if (spineHero != null)
    //    {
    //        Destroy (spineHero.gameObject);
    //        yield return null;
    //    }

    //    id = heroUsed;
    //    _SpineObject spHero = null;
    //    if (id == 100)
    //        spHero = HomeManager.instance.spStandLubu;
    //    else if (id == 200)
    //        spHero = HomeManager.instance.spStandCleo;

    //    spineHero = Instantiate (spHero, this.transform);
    //    yield return null;

    //    spineHero.transform.localEulerAngles = Vector3.zero;
    //    spineHero.transform.localPosition = Vector3.one;
    //    if (id == 100)
    //        spineHero.transform.localScale = scaleLubu;
    //    else if (id == 200)
    //        spineHero.transform.localScale = Vector3.one;
    //    spineHero.StartStandRandomMove ();
    //}
}
