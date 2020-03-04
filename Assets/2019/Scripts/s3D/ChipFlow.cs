using System.Collections;
using UnityEngine;

public enum FlowType
{
    Ten,
    Hundred,
    Thousand,
    TenAndHundred,
    TenAndThousand,
    HundredAndThousand,
    All
}

public class ChipFlow : MonoBehaviour
{
    public Transform[] transCashes;     // last index for center position on table

    public GameObject[] chips;
    public bool isReady = true;

    private int lastIndex;
    //private MeshFilter[] meshChips;

    Vector3 fdOut = new Vector3(0.5f, 0.5f, 0.5f);
    float ofsetX = 0, ofsetY = 0;

    private void Start ()
    {
        lastIndex = transCashes.Length - 1;
        //meshChips = new MeshFilter[chips.Length];
        //for (int i = 0; i < meshChips.Length; i++)
        //{
        //    meshChips[i] = chips[i].GetComponent<MeshFilter> ();
        //}

        gameObject.SetActive(false);
    }

    //public void SetFlowType (FlowType _ft )
    //{
    //    switch (_ft)
    //    {
    //        case FlowType.Ten:
    //            for (int i = 0; i < meshChips.Length; i++)
    //            {
    //                meshChips[i].mesh = _PokerGameManager.instance.chipTen;
    //            }
    //            break;
    //        case FlowType.Hundred:
    //            for (int i = 0; i < meshChips.Length; i++)
    //            {
    //                meshChips[i].mesh = _PokerGameManager.instance.chipHundred;
    //            }
    //            break;
    //        case FlowType.Thousand:
    //            for (int i = 0; i < meshChips.Length; i++)
    //            {
    //                meshChips[i].mesh = _PokerGameManager.instance.chipThousand;
    //            }
    //            break;
    //        case FlowType.TenAndHundred:
    //            for (int i = 0; i < meshChips.Length; i++)
    //            {
    //                if (i % 2 == 0)
    //                    meshChips[i].mesh = _PokerGameManager.instance.chipTen;
    //                else
    //                    meshChips[i].mesh = _PokerGameManager.instance.chipHundred;
    //            }
    //            break;
    //        case FlowType.TenAndThousand:
    //            for (int i = 0; i < meshChips.Length; i++)
    //            {
    //                if (i % 2 == 0)
    //                    meshChips[i].mesh = _PokerGameManager.instance.chipTen;
    //                else
    //                    meshChips[i].mesh = _PokerGameManager.instance.chipThousand;
    //            }
    //            break;
    //        case FlowType.HundredAndThousand:
    //            for (int i = 0; i < meshChips.Length; i++)
    //            {
    //                if (i % 2 == 0)
    //                    meshChips[i].mesh = _PokerGameManager.instance.chipHundred;
    //                else
    //                    meshChips[i].mesh = _PokerGameManager.instance.chipThousand;
    //            }
    //            break;
    //        case FlowType.All:
    //            for (int i = 0; i < meshChips.Length; i++)
    //            {
    //                if (i == meshChips.Length - 1 || i == meshChips.Length - 2)
    //                    meshChips[i].mesh = _PokerGameManager.instance.chipThousand;
    //                else if (i % 2 == 0)
    //                    meshChips[i].mesh = _PokerGameManager.instance.chipTen;
    //                else
    //                    meshChips[i].mesh = _PokerGameManager.instance.chipHundred;
    //            }
    //            break;
    //    }
    //}

    IEnumerator _Flow(Vector3 _sp, Vector3 _ep, int indexTo)
    {
        if (indexTo != transCashes.Length - 1)
        {
            yield return _WFSUtility.wfs05;
        }   

        for (int i = 0; i < chips.Length; i++)
        {
            int x = i;
            chips[x].SetActive(true);

            if (indexTo != lastIndex)
            {
                //ofsetX = Random.Range(-0.3f, 0.3f);
                //ofsetY = Random.Range(-0.3f, 0.3f);

            }
            else
            {
                ofsetX = 0;
                ofsetY = 0;
            }

            LeanTween.move(chips[x], new Vector3(_ep.x + ofsetX, _ep.y + ofsetY, _ep.z), indexTo != lastIndex ? 1.3f: 0.8f).setEaseOutQuad().setOnComplete(() => Deactive(x));

            yield return _WFSUtility.wfs006;
        }
        isReady = true;
    }

    public void ShowFx(int _fromIndex, int _toIndex) // Player ke meja, meja ke player, Meja itu Index 7, My Player itu Index 6
    {
        gameObject.SetActive(true);
        isReady = false;

        if (_toIndex == lastIndex)
        {
            ofsetX = Random.Range(-0.3f, 0.3f);
            ofsetY = Random.Range(-0.3f, 0.3f);
        }
        else
        {
            ofsetX = 0;
            ofsetY = 0;
        }


        for (int i = 0; i < chips.Length; i++)
        {
            chips[i].transform.position =  new Vector3 (transCashes[_fromIndex].position.x + ofsetX, transCashes[_fromIndex].position.y + ofsetY , transCashes[_fromIndex].position.z);
            chips[i].transform.localScale = Vector3.one*2;
        }

        StartCoroutine(_Flow(transCashes[_fromIndex].position, transCashes[_toIndex].position, _toIndex));
    }

    void Deactive(int idx)
    {        
        chips[idx].SetActive(false);

        if (idx == chips.Length - 1)
            gameObject.SetActive(false);
    }
}
