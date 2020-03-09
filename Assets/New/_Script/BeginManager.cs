using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeginManager : MonoBehaviour
{
    private static BeginManager s_Instance = null;
    public static BeginManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (BeginManager)) as BeginManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an BeginManager object. \n You have to have exactly one BeginManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;

    public TMP_InputField ipfDisplayName;
    public Button btnSet;

    private bool isInit;

    private void Start ()
    {
        btnSet.onClick.AddListener (OnSet);
    }

    public void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.BEGIN;
            isInit = true;
        }
        canvas.enabled = true;
    }

    public void Hide ()
    {
        ipfDisplayName.text = string.Empty;
        canvas.enabled = false;
    }

    private void OnSet ()
    {
        string displayName = string.Empty;
        if (ipfDisplayName.text == string.Empty)
        {
            displayName = "NoName";
        }
        else
        {
            displayName = ipfDisplayName.text;
        }

        PlayerData.hero_id = 200;       //lubu = 100, cleo = 200
        PlayerData.SetData (displayName);
        _SceneManager.instance.SetActiveScene (SceneType.HOME, true);
        Hide ();
    }
}
