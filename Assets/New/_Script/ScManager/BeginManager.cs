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
                s_Instance = FindObjectOfType (typeof (BeginManager)) as BeginManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an BeginManager object. \n You have to have exactly one BeginManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Button btnLuBu;
    public Button btnCleopatra;

    private bool isInit;

    private void Start ()
    {
        btnLuBu.onClick.AddListener (OnLuBu);
        btnCleopatra.onClick.AddListener (OnCleopatra);
    }

    public void SetCanvas ( bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    public void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.BEGIN;
            isInit = true;
        }

        canvas.enabled = true;
        _SceneManager.instance.activeSceneType = SceneType.BEGIN;
    }

    public void Hide ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.HOME, true);
        canvas.enabled = false;
    }

    private void OnLuBu ()
    {
        //PlayerData.SetData ("Proto");
        //PlayerData.costume_id = 100;
        ApiManager.instance.SetHero (1);
    }

    private void OnCleopatra ()
    {
        //PlayerData.SetData ("Proto");
        //PlayerData.costume_id = 200;
        ApiManager.instance.SetHero (2);
    }
}
