using UnityEngine;

public class ApiManager : MonoBehaviour
{
    private static ApiManager s_Instance = null;

    public static ApiManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (ApiManager)) as ApiManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an ApiManager object. \n You have to have exactly one ApiManager in the scene.");
            }
            return s_Instance;
        }
    }

    [SerializeField]
    private ApiBridge api;

    public void GetVersion ()
    {
        api.GetVersion ();
    }

    private void RGetVersion (string inputJson )
    {
        Logger.E ("Return Get Version: " + inputJson);
        JsonGetVersion jsonGetVersion = JsonUtility.FromJson<JsonGetVersion> (inputJson);
        Logger.E (jsonGetVersion.version.new_ver);
    }
}
