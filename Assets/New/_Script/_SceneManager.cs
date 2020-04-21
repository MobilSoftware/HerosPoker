using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    SPLASH,
    HOME,
    BEGIN,
    PROFILE,
    VERIFY,
    VIP,
    POKER,
    SLOTO,
    MESSAGE
}

public class _SceneManager : MonoBehaviour
{
    private static _SceneManager s_Instance = null;

    public static _SceneManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (_SceneManager)) as _SceneManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an _SceneManager object. \n You have to have exactly one _SceneManager in the scene.");
            }
            return s_Instance;
        }
    }

    [HideInInspector]
    public SceneType activeSceneType;

    public Camera mainCamera;

    private List<Scene> loadedScenes = new List<Scene> ();
    private HomeManager homeManager;
    private ProfileManager profileManager;
    private VerifyManager verifyManager;
    private VipManager vipManager;
    private PokerManager pokerManager;
    private SlotoManagerScript slotoManager;
    private BeginManager beginManager;
    private MessageManager msgManager;

    private void Start ()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad (this);
        activeSceneType = SceneType.SPLASH;
        //StartCoroutine (_LoadAllScenes ());
        Init ();
    }

    private void Init ()
    {
        ApiManager.instance.GetVersion ();
    }

    public void LoadAllScenes()
    {
        StartCoroutine (_LoadAllScenes ());
    }

    IEnumerator _LoadAllScenes ()
    {
        AsyncOperation async;
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            async = SceneManager.LoadSceneAsync (i, LoadSceneMode.Additive);
            while (!async.isDone)
                yield return _WFSUtility.wef;

            loadedScenes.Add (SceneManager.GetSceneByBuildIndex (i));
        }
        yield return _WFSUtility.wef;
        homeManager = HomeManager.instance;
        profileManager = ProfileManager.instance;
        verifyManager = VerifyManager.instance;
        vipManager = VipManager.instance;
        yield return _WFSUtility.wef;
        pokerManager = PokerManager.instance;
        beginManager = BeginManager.instance;
        msgManager = MessageManager.instance;
        slotoManager = FindObjectOfType<SlotoManagerScript> ();
        SetActiveSloto (false);
        SetActiveScene (SceneType.BEGIN, true);
        SceneManager.UnloadSceneAsync ("SeSplash");
    }

    public void SetActiveScene (SceneType st, bool val )
    {
        switch (st)
        {
            case SceneType.BEGIN: beginManager.SetCanvas (val); break;
            case SceneType.HOME: homeManager.SetCanvas (val); break;
            case SceneType.PROFILE: profileManager.SetCanvas (val); break;
            case SceneType.VERIFY: verifyManager.SetCanvas (val); break;
            case SceneType.VIP: vipManager.SetCanvas (val); break;
            case SceneType.POKER: pokerManager.SetCanvas (val); break;
            case SceneType.SLOTO: SetActiveSloto (val); break;
        }
    }

    private void SetActiveSloto (bool val )
    {
        if (val)
        {
            slotoManager.gameObject.SetActive (true);
            slotoManager.SetMoney ();
            mainCamera.gameObject.SetActive (false);
            activeSceneType = SceneType.SLOTO;
        }
        else { slotoManager.gameObject.SetActive (false); mainCamera.gameObject.SetActive (true); }
    }

    private void OnEscape ()
    {
        switch (activeSceneType)
        {
            case SceneType.BEGIN:
            case SceneType.HOME:
                MessageManager.instance.Show (gameObject, "Apakah kamu yakin ingin keluar?", ButtonMode.OK_CANCEL, -2);
                break;
            case SceneType.POKER:
                Debug.Log ("Open Pause Menu");
                //open pause menu
                break;
            case SceneType.SLOTO:
            case SceneType.PROFILE:
            case SceneType.VERIFY:
            case SceneType.VIP:
                SetActiveScene (activeSceneType, false);
                SetActiveScene (SceneType.HOME, true);
                break;
        }
    }

    private void OnPositiveClicked ( int returnCode )
    {
        switch (returnCode)
        {
            case -2: Application.Quit (); break;
        }
    }

    private void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Escape))
        {
            OnEscape ();
        }
    }
}
