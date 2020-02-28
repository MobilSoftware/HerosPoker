using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Box_WaitingNextRound : MonoBehaviour 
{

    public void Show()
    {
        gameObject.SetActive (true);
    }

    public void Hide ()
    {
        gameObject.SetActive (false);
    }
}
