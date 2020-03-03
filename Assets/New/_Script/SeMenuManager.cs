using UnityEngine;

public class SeMenuManager : MonoBehaviour
{
    private static SeMenuManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static SeMenuManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (SeMenuManager)) as SeMenuManager;
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
