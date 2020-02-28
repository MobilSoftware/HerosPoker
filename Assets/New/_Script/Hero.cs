using UnityEngine;

public class Hero : MonoBehaviour
{
    //equivalent of GenericAvater
    public string strCodeName;
    public int id;
    public void Show ()
    {
        gameObject.SetActive (true);
    }

    public void Hide ()
    {
        gameObject.SetActive (false);
    }

    public void LoadSkin ()
    {

    }
}
