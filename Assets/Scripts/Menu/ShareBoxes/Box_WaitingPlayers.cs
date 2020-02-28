using UnityEngine;

public class Box_WaitingPlayers : MonoBehaviour 
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
