using UnityEngine;

public class HomeManager : MonoBehaviour
{
    private static HomeManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static HomeManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (HomeManager)) as HomeManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an SeMenuManager object. \n You have to have exactly one SeMenuManager in the scene.");
            }
            return s_Instance;
        }
    }

    public GameObject objMenu;
    private bool isInit;

    public void Init ()
    {
        Debug.LogError ("init semenu");

        if (!isInit)
            PhotonNetwork.ConnectUsingSettings ("v1.0");

        isInit = true;
    }
}
