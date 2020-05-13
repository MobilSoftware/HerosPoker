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

    public void LoadFromBundle (int _costumeID )
    {
        StartCoroutine (_LoadFromBundle (_costumeID));
    }

    IEnumerator _LoadFromBundle (int _costumeID )
    {
        AssetBundle ab = AssetBundle.LoadFromFile (BundleManager.instance.GetItemLoadPath (DownloadType.ASSET, 6, _costumeID));
        Logger.E (BundleManager.instance.GetItemLoadPath (DownloadType.ASSET, 6, _costumeID));
        Logger.E (_costumeID + "_0");
        GameObject objHero = (GameObject) ab.LoadAsset (_costumeID + "_0", typeof (GameObject));
        _SpineObject hero = objHero.GetComponent<_SpineObject> ();
        if (hero)
        {
            spineHero = Instantiate (hero, this.transform);
            yield return _WFSUtility.wef;
            spineHero.transform.localEulerAngles = Vector3.zero;
            spineHero.transform.localPosition = Vector3.one;
            if (_costumeID == 3)
                spineHero.transform.localScale = scaleLubu;
            else if (_costumeID == 7)
                spineHero.transform.localScale = Vector3.one;
            spineHero.StartStandRandomMove ();
        }
        ab.Unload (false);
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
