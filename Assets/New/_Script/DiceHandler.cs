using System.Collections;
using UnityEngine;

public class DiceHandler : MonoBehaviour
{
    public Transform parentTop;
    public Transform parentCenter;
    public GameObject objAnimDice1;
    public GameObject objAnimDice2;
    public GameObject objAnimDice3;

    public GameObject[] facesDice1;
    public GameObject[] facesDice2;
    public GameObject[] facesDice3;

    private Coroutine crPlayAnimation;

    //flow: whole object true -> facesDice[all] true -> false -> animDice true-> false -> facesDice[index] change -> true -> whole object false -> whole object true
    //-> facesDice[all] false -> animDice true -> false -> facesDice[index] change -> true -> whole object false

    public void MoveToCenter ( int index1, int index2, int index3 )
    {
        this.transform.SetParent (parentCenter);
        LeanTween.scale (this.gameObject, Vector3.one, 0.15f);
        LeanTween.moveLocal (this.gameObject, Vector3.zero, 0.2f).setOnComplete (() => PlayAnimation (index1 - 1, index2 - 1, index3 - 1));
    }

    private void PlayAnimation(int index1, int index2, int index3)
    {
        StopAnimation ();
        crPlayAnimation = StartCoroutine (_PLayAnimation (index1, index2, index3));
    }

    IEnumerator _PLayAnimation(int index1, int index2, int index3)
    {
        yield return _WFSUtility.wfs03;
        HideAllFaces ();
        //yield return _WFSUtility.wef;
        objAnimDice1.SetActive (true);
        objAnimDice2.SetActive (true);
        objAnimDice3.SetActive (true);
        yield return _WFSUtility.wfs05;
        objAnimDice1.SetActive (false);
        objAnimDice2.SetActive (false);
        objAnimDice3.SetActive (false);
        facesDice1[index1].SetActive (true);
        facesDice2[index2].SetActive (true);
        facesDice3[index3].SetActive (true);
        yield return _WFSUtility.wfs1;
        MoveToTop ();
    }

    private void MoveToTop ()
    {
        this.transform.SetParent (parentTop);
        LeanTween.scale (this.gameObject, new Vector3 (0.25f, 0.25f), 0.15f);
        LeanTween.moveLocal (this.gameObject, Vector3.one, 0.2f);
    }

    private void HideAllFaces ()
    {
        for (int i = 0; i < facesDice1.Length; i++)
        {
            facesDice1[i].SetActive (false);
            facesDice2[i].SetActive (false);
            facesDice3[i].SetActive (false);
        }
    }

    public void StopAnimation ()
    {
        if (crPlayAnimation != null)
            StopCoroutine (crPlayAnimation);
    }
}
