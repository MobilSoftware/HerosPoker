using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    SPLASH = 1,
    HOME = 2,
    BEGIN = 3,
    LOGIN = 4,
    PROFILE = 5,
    VERIFY = 6,
    VIP = 7,
    POKER = 8,
    SLOTO = 9,
    MESSAGE = 10
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

    private HomeManager homeM;
    private ProfileManager profileM;
    private VerifyManager verifyM;
    private VipManager vipM;
    private PokerManager pokerM;
    private SlotoManagerScript slotoM;
    private BeginManager beginM;
    private LoginManager loginM;

    private void Start ()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad (this);
        activeSceneType = SceneType.SPLASH;
        LoadMessageScene ();
    }

    private void LoadMessageScene ()
    {
        StartCoroutine (_LoadMessageScene ());
    }

    IEnumerator _LoadMessageScene ()
    {
        AsyncOperation async;
        async = SceneManager.LoadSceneAsync (1, LoadSceneMode.Additive);
        while (!async.isDone)
            yield return _WFSUtility.wef;

        CallGetVersion ();
    }

    private void CallGetVersion ()
    {
        bool hasConnection = CheckInternetConnection ();
        if (hasConnection)
            ApiManager.instance.GetVersion ();
        else
        {
            Debug.LogError ("check internet");
            MessageManager.instance.Show (this.gameObject, "Mohon periksa koneksi internet anda", ButtonMode.OK_CANCEL, -3, "Coba Lagi", "Keluar");
        }
    }

    private bool CheckInternetConnection ()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return false;

        return true;
    }

    public void LoadAllScenes()
    {
        //StartCoroutine (_LoadAllScenes ());
        StartCoroutine (_LoadLocalScenes ());
    }

    IEnumerator _LoadLocalScenes ()
    {
        AsyncOperation async;
        for (int i = 2; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            async = SceneManager.LoadSceneAsync (i, LoadSceneMode.Additive);
            while (!async.isDone)
                yield return _WFSUtility.wef;
        }

        yield return _WFSUtility.wef;
        Logger.E ("1");
        homeM = HomeManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("2");
        profileM = ProfileManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("3");
        verifyM = VerifyManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("4");
        vipM = VipManager.instance;
        Logger.E ("5");
        yield return _WFSUtility.wef;
        Logger.E ("6");
        pokerM = PokerManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("7");
        beginM = BeginManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("8");
        loginM = LoginManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("9");
        slotoM = FindObjectOfType<SlotoManagerScript> ();
        yield return _WFSUtility.wef;
        Logger.E ("10");
        SetActiveSloto (false);
        yield return _WFSUtility.wef;
        Logger.E ("11");
        //SetActiveScene (SceneType.BEGIN, true);
        SetActiveScene (SceneType.LOGIN, true);
        yield return _WFSUtility.wef;
        Logger.E ("12");
        BundleManager.instance.bLoadingScenes = true;
        yield return _WFSUtility.wef;
        Logger.E ("13");
        SceneManager.UnloadSceneAsync ("SeSplash");
    }

    IEnumerator _LoadAllScenes ()
    {
        AsyncOperation async;
        string[] enums = Enum.GetNames (typeof (SceneType));
        for (int i = 2; i < enums.Length; i++)
        {
            string loadPath = BundleManager.instance.GetSceneLoadPath (i);
            Logger.E ("loading scene: " + loadPath);
            AssetBundle ab = AssetBundle.LoadFromFile (loadPath);
            string[] scenePath = ab.GetAllScenePaths ();
            Logger.E (scenePath[0]);
            async = SceneManager.LoadSceneAsync (scenePath[0], LoadSceneMode.Additive);
            while (!async.isDone)
            {
                yield return _WFSUtility.wef;
                Logger.E ("name: " + scenePath[0] + " | progress: " + async.progress);
            }
            ab.Unload (false);
        }
        yield return _WFSUtility.wef;
        Logger.E ("1");
        homeM = HomeManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("2");
        profileM = ProfileManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("3");
        verifyM = VerifyManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("4");
        vipM = VipManager.instance;
        Logger.E ("5");
        yield return _WFSUtility.wef;
        Logger.E ("6");
        pokerM = PokerManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("7");
        beginM = BeginManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("8");
        loginM = LoginManager.instance;
        yield return _WFSUtility.wef;
        Logger.E ("9");
        slotoM = FindObjectOfType<SlotoManagerScript> ();
        yield return _WFSUtility.wef;
        Logger.E ("10");
        SetActiveSloto (false);
        yield return _WFSUtility.wef;
        Logger.E ("11");
        //SetActiveScene (SceneType.BEGIN, true);
        SetActiveScene (SceneType.LOGIN, true);
        yield return _WFSUtility.wef;
        Logger.E ("12");
        BundleManager.instance.bLoadingScenes = true;
        yield return _WFSUtility.wef;
        Logger.E ("13");
        SceneManager.UnloadSceneAsync ("SeSplash");
    }

    public void SetActiveScene (SceneType st, bool val )
    {
        switch (st)
        {
            case SceneType.LOGIN: loginM.SetCanvas (val); break;
            case SceneType.BEGIN: beginM.SetCanvas (val); break;
            case SceneType.HOME: homeM.SetCanvas (val); break;
            case SceneType.PROFILE: profileM.SetCanvas (val); break;
            case SceneType.VERIFY: verifyM.SetCanvas (val); break;
            case SceneType.VIP: vipM.SetCanvas (val); break;
            case SceneType.POKER: pokerM.SetCanvas (val); break;
            case SceneType.SLOTO: SetActiveSloto (val); break;
        }
    }

    private void SetActiveSloto (bool val )
    {
        Logger.E ("1001");
        if (val)
        {
            slotoM.gameObject.SetActive (true);
            slotoM.SetMoney ();
            mainCamera.gameObject.SetActive (false);
            activeSceneType = SceneType.SLOTO;
        }
        else 
        {
            slotoM.gameObject.SetActive (false);
            Logger.E ("1002");
            mainCamera.gameObject.SetActive (true);
            Logger.E ("1004");
        }
    }

    private void OnEscape ()
    {
        switch (activeSceneType)
        {
            case SceneType.LOGIN:
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
            case -3:
                MessageManager.instance.Hide ();
                CallGetVersion ();
                break;
        }
    }

    private void OnNegativeClicked (int returnCode )
    {
        switch (returnCode)
        {
            case -3: Application.Quit (); break;
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
