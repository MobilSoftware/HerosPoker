using UnityEngine;

public class WaitingPlayersUI : MonoBehaviour 
{
    public void Show ()
    {
        gameObject.SetActive (true);
    }

    public void Hide ()
    {
        gameObject.SetActive (false);
    }
}
