using System.Collections;
using Spine.Unity;
using UnityEngine;

public enum SpineAnim
{
    FOLD,
    CALL,
    CHECK
}

public class _SpineObject : MonoBehaviour
{
    public SkeletonAnimation mySkelAnim;
    public SkeletonGraphic mySkelGraphic;
    public SkeletonPartsRenderer partBack;

    private Coroutine crRandomMove;
    private Coroutine crStandRandomMove;
    private string strBlink = "Blink";  //90%
    private string strNod = "Nod";      //5%
    private string strEye = "Eye";      //5%
    private int luckyEye = 6;
    private int luckyNod = 11;

    public void StartRandomMove ()
    {
        StopRandomMove ();
        crRandomMove = StartCoroutine (_StartRandomMove ());
    }

    private IEnumerator _StartRandomMove ()
    {
        while (gameObject.activeSelf)
        {
            int randWaitTime = Random.Range (5, 16);
            yield return new WaitForSeconds (randWaitTime);
            int randLuckyNumber = Random.Range (1, 101);
            if (randLuckyNumber < luckyEye)
                _SpineUtility.PlayAnimation (mySkelAnim, 1, strEye, false);
            else if (randLuckyNumber < luckyNod)
                _SpineUtility.PlayAnimation (mySkelAnim, 1, strNod, false, 1.5f);
            else
                _SpineUtility.PlayAnimation (mySkelAnim, 1, strBlink, false);
        }
    }

    public void StopRandomMove()
    {
        if (crRandomMove != null)
            StopCoroutine (crRandomMove);
    }

    public void SetAction (SpineAnim action )
    {
        string strActionName = string.Empty;
        switch (action)
        {
            case SpineAnim.FOLD:
                strActionName = "Fold";
                break;
            case SpineAnim.CALL:
                strActionName = "Call";
                break;
            case SpineAnim.CHECK:
                strActionName = "Check";
                break;
        }

        if (!strActionName.Equals (string.Empty))
            _SpineUtility.PlayAnimation (mySkelAnim, 0, strActionName, false);
    }

    public void StartStandRandomMove ()
    {
        StopStandRandomMove ();
        crStandRandomMove = StartCoroutine (_StartStandRandomMove ());
    }

    IEnumerator _StartStandRandomMove ()
    {
        while (gameObject.activeSelf)
        {
            int randWaitTime = Random.Range (5, 16);
            yield return new WaitForSeconds (randWaitTime);
            _SpineUtility.PlayAnimation (mySkelGraphic, 1, strBlink, false);
        }
    }

    public void StopStandRandomMove()
    {
        if (crStandRandomMove != null)
            StopCoroutine (crStandRandomMove);
    }

}
