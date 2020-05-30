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

    public void LoadFromBundle ( int _costumeID, bool _isMine )
    {
        Logger.E ("loading bundle");
        StartCoroutine (_LoadFromBundle (_costumeID, _isMine));
    }

    IEnumerator _LoadFromBundle ( int _costumeID, bool _isMine )
    {
        Logger.E ("costumeID: " + _costumeID);
        AssetBundle ab = AssetBundle.LoadFromFile (BundleManager.instance.GetItemLoadPath (DownloadType.ASSET, 6, _costumeID));
        GameObject objHero = (GameObject) ab.LoadAsset (_costumeID + "_1", typeof (GameObject));
        _SpineObject hero = objHero.GetComponent<_SpineObject> ();

        if (hero)
        {
            Logger.E ("hero not null");
            spineHero = Instantiate (hero, this.transform);
            if (_isMine)
                SetMyPartBack ();
            yield return _WFSUtility.wef;
            spineHero.transform.localEulerAngles = Vector3.zero;
            //if (_costumeID == 3)
            //{
            //    spineHero.transform.localScale = scaleLuBu;
            //    spineHero.transform.localPosition = posLuBu;
            //}
            //else if (_costumeID == 7)
            //{
            spineHero.transform.localScale = Vector3.one;
            spineHero.transform.localPosition = posLuBu;
            //}
            spineHero.StartRandomMove ();
        }
        ab.Unload (false);
    }

    //public void LoadSpine (int heroUsed, bool isMine)
    //{
    //    StartCoroutine (_LoadSpine (heroUsed, isMine));
    //}

    //IEnumerator _LoadSpine (int heroUsed, bool isMine)
    //{
    //    if (spineHero != null)
    //    {
    //        Destroy (spineHero.gameObject);
    //        yield return null;
    //    }

    //    id = heroUsed;
    //    _SpineObject spHero = null;
    //    if (id == 100)
    //        spHero = PokerManager.instance.spLuBu;
    //    else if (id == 200)
    //        spHero = PokerManager.instance.spCleo;

    //    spineHero = Instantiate (spHero, this.transform);
    //    if (isMine)
    //        SetMyPartBack ();
    //    yield return null;

    //    spineHero.transform.localEulerAngles = Vector3.zero;
    //    if (id == 100)
    //    {
    //        spineHero.transform.localPosition = posLuBu;
    //        spineHero.transform.localScale = scaleLuBu;
    //    }
    //    else if (id == 200)
    //    {
    //        spineHero.transform.localPosition = Vector3.zero;
    //        spineHero.transform.localScale = Vector3.one;
    //    }
    //    spineHero.StartRandomMove ();
    //}

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
