using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    SPLASH = 1,
    HOME = 2,
    BEGIN = 3,
    LOGIN = 4,
    COUPON = 5,
    PROFILE = 6,
    SHOP = 7,
    VERIFY = 8,
    VIP = 9,
    POKER_ROOM = 10,
    POKER = 11,
    SLOTO = 12,
    SICBO = 13,
    SETTINGS = 14,
    HERO = 15,
    LEADERBOARD = 16,
    FRIEND = 17,
    OTHER_PROFILE = 18,
    RECEIVE_ITEM = 19,
    MESSAGE = 20
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
    private SicboManager sicboM;
    private BeginManager beginM;
    private LoginManager loginM;
    private ShopManager shopM;
    private PokerRoomManager proomM;
    private SettingsManager settingsM;
    private HeroManager heroM;
    private LeaderboardManager leaderboardM;
    private FriendManager friendM;

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
        StartCoroutine (_LoadAllScenes ());
    }

    IEnumerator _LoadAllScenes ()
    {
        AsyncOperation async;
        string[] enums = Enum.GetNames (typeof (SceneType));
        for (int i = 2; i < enums.Length; i++)
        {
            string loadPath = BundleManager.instance.GetSceneLoadPath (i);
            //Logger.E ("loading scene: " + loadPath);
            AssetBundle ab = AssetBundle.LoadFromFile (loadPath);
            string[] scenePath = ab.GetAllScenePaths ();
            //Logger.E (scenePath[0]);
            async = SceneManager.LoadSceneAsync (scenePath[0], LoadSceneMode.Additive);
            while (!async.isDone)
            {
                yield return _WFSUtility.wef;
                //Logger.E ("name: " + scenePath[0] + " | progress: " + async.progress);
            }
            ab.Unload (false);
        }
        yield return _WFSUtility.wef;
        homeM = HomeManager.instance;
        yield return _WFSUtility.wef;
        profileM = ProfileManager.instance;
        yield return _WFSUtility.wef;
        verifyM = VerifyManager.instance;
        yield return _WFSUtility.wef;
        vipM = VipManager.instance;
        yield return _WFSUtility.wef;
        pokerM = PokerManager.instance;
        yield return _WFSUtility.wef;
        beginM = BeginManager.instance;
        yield return _WFSUtility.wef;
        loginM = LoginManager.instance;
        yield return _WFSUtility.wef;
        shopM = ShopManager.instance;
        yield return _WFSUtility.wef;
        proomM = PokerRoomManager.instance;
        yield return _WFSUtility.wef;
        slotoM = FindObjectOfType<SlotoManagerScript> ();
        yield return _WFSUtility.wef;
        sicboM = SicboManager.instance;
        yield return _WFSUtility.wef;
        settingsM = SettingsManager.instance;
        yield return _WFSUtility.wef;
        heroM = HeroManager.instance;
        yield return _WFSUtility.wef;
        leaderboardM = LeaderboardManager.instance;
        yield return _WFSUtility.wef;
        friendM = FriendManager.instance;

        PhotonNetwork.ConnectUsingSettings ("v1.0");
        yield return _WFSUtility.wef;
        SetActiveScene (SceneType.SLOTO, false);
        yield return _WFSUtility.wef;
        int playerID = PlayerPrefs.GetInt (PrefEnum.PLAYER_ID.ToString (), 0);
        string token = PlayerPrefs.GetString (PrefEnum.TOKEN.ToString (), string.Empty);
        if (playerID != 0 && token != string.Empty)
            ApiManager.instance.GetHome ();
        else
            SetActiveScene (SceneType.LOGIN, true);
        yield return _WFSUtility.wef;
        BundleManager.instance.bLoadingScenes = true;
        yield return _WFSUtility.wef;
        SceneManager.UnloadSceneAsync ("SeSplash");
    }

    public void SetActiveScene (SceneType st, bool val, int indexSlotoType = 1 )
    {
        switch (st)
        {
            case SceneType.LOGIN: loginM.SetCanvas (val); break;
            case SceneType.BEGIN: beginM.SetCanvas (val); break;
            case SceneType.HOME: homeM.SetCanvas (val); break;
            case SceneType.PROFILE: profileM.SetCanvas (val); break;
            case SceneType.SHOP: shopM.SetCanvas (val); break;
            case SceneType.POKER_ROOM: proomM.SetCanvas (val); break;
            case SceneType.VERIFY: verifyM.SetCanvas (val); break;
            case SceneType.VIP: vipM.SetCanvas (val); break;
            case SceneType.POKER: pokerM.SetCanvas (val); break;
            case SceneType.SLOTO: SetActiveSloto (val, indexSlotoType); break;
            case SceneType.SICBO: sicboM.SetCanvas (val); break;
            case SceneType.SETTINGS: settingsM.SetCanvas (val); break;
            case SceneType.HERO: heroM.SetCanvas (val); break;
            case SceneType.LEADERBOARD: leaderboardM.SetCanvas (val); break;
            case SceneType.FRIEND: friendM.SetCanvas (val); break;
        }
    }

    private void SetActiveSloto (bool val, int indexSlotoType )
    {
        if (val)
        {
            slotoM.gameObject.SetActive (true);
            slotoM.SetMoney ();
            slotoM.Init (indexSlotoType);
            mainCamera.gameObject.SetActive (false);
            activeSceneType = SceneType.SLOTO;
        }
        else 
        {
            slotoM.gameObject.SetActive (false);
            mainCamera.gameObject.SetActive (true);
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
            case SceneType.SICBO:
                Debug.Log ("Open Pause Menu");
                break;
            case SceneType.SLOTO:
            case SceneType.PROFILE:
            case SceneType.SHOP:
            case SceneType.VERIFY:
            case SceneType.VIP:
            case SceneType.SETTINGS:
            case SceneType.LEADERBOARD:
            case SceneType.FRIEND:
            case SceneType.HERO:
                SetActiveScene (activeSceneType, false);
                break;
            case SceneType.MESSAGE:
                MessageManager.instance.Hide ();
                break;
            case SceneType.OTHER_PROFILE:
                OtherProfileManager.instance.Hide ();
                break;
            case SceneType.RECEIVE_ITEM:
                ReceiveItemManager.instance.Hide ();
                break;
        }
    }

    public void UpdateAllCoinAndCoupon ()
    {
        homeM.UpdateCoinAndCoupon ();
        profileM.UpdateCoinAndCoupon ();
        shopM.UpdateCoinAndCoupon ();
        proomM.UpdateCoinAndCoupon ();
        heroM.UpdateCoinAndCoupon ();
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
