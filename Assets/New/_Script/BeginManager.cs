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
    public GameObject objDisplayName;
    public GameObject objPickHero;
    public TMP_InputField ipfDisplayName;
    public Button btnOK;
    public Button btnLuBu;
    public Button btnCleopatra;

    private bool isInit;

    private void Start ()
    {
        btnOK.onClick.AddListener (OnOK);
        btnLuBu.onClick.AddListener (OnLuBu);
        btnCleopatra.onClick.AddListener (OnCleopatra);
    }

    public void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.BEGIN;
            isInit = true;
        }
        canvas.enabled = true;
        objPickHero.SetActive (false);
        objDisplayName.SetActive (true);
    }

    public void Hide ()
    {
        ipfDisplayName.text = string.Empty;
        HomeManager.instance.Init ();
        canvas.enabled = false;
    }

    private void OnOK ()
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

        PlayerData.SetData (displayName);

        ipfDisplayName.text = string.Empty;
        objDisplayName.SetActive (false);
        objPickHero.SetActive (true);
    }

    private void OnLuBu ()
    {
        PlayerData.hero_id = 100;
        Hide ();
    }

    private void OnCleopatra ()
    {
        PlayerData.hero_id = 200;
        Hide ();
    }
}
