using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSymbol : MonoBehaviour
{
    //bg putih termasuk disini
    public Text txtNumber;

    public void SetText (string strResult )
    {
        txtNumber.text = strResult;
    }

    public void RandomText ()
    {
        int rand = Random.Range (0, 10);
        txtNumber.text = rand.ToString ();
    }
}
