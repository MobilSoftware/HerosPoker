using System.Collections;
using UnityEngine;

public class StandHero : MonoBehaviour
{
    public int id;
    private _SpineObject spineHero;

    private Vector3 scaleLubu = new Vector3 (1.5f, 1.5f, 1f);

    public void Reset ()
    {
        if (spineHero != null)
        {
            spineHero.StopStandRandomMove ();
            Destroy (spineHero.gameObject);
        }
    }

    public void LoadSpine ( int heroUsed )
    {
        StartCoroutine (_LoadSpine (heroUsed));
    }

    IEnumerator _LoadSpine ( int heroUsed )
    {
        if (spineHero != null)
        {
            Destroy (spineHero.gameObject);
            yield return null;
        }

        id = heroUsed;
        _SpineObject spHero = null;
        if (id == 100)
            spHero = HomeManager.instance.spStandLubu;
        else if (id == 200)
            spHero = HomeManager.instance.spStandCleo;

        spineHero = Instantiate (spHero, this.transform);
        yield return null;

        spineHero.transform.localEulerAngles = Vector3.zero;
        spineHero.transform.localPosition = Vector3.one;
        if (id == 100)
            spineHero.transform.localScale = scaleLubu;
        else if (id == 200)
            spineHero.transform.localScale = Vector3.one;
        spineHero.StartStandRandomMove ();
    }
}
