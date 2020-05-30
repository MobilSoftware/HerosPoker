using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    private static LeaderboardManager s_Instance = null;
    public static LeaderboardManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (LeaderboardManager)) as LeaderboardManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an LeaderboardManager object. \n You have to have exactly one LeaderboardManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Button btnClose;
    //public Transform parentItems;
    //public ItemLeaderboard prefabItemLeaderboard;
    public ItemLeaderboard[] itemLeaderboards;      //length = 20 for now, should be 50

    private bool isSettingJson;
    private JLeaderboard[] jLeaderboards;
    private bool isInit;
    private SceneType prevSceneType;

    private void Start ()
    {
        isSettingJson = true;
        btnClose.onClick.AddListener (Hide);
    }

    public void SetCanvas (bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    private void Show ()
    {
        if (!isInit)
        {
            isInit = true;
            canvas.sortingOrder = (int) SceneType.LEADERBOARD;
            StartCoroutine (_WaitSetJson ());
        }

        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.LEADERBOARD;
    }

    public void SetJson ( JGetLeaderboard json )
    {
        isSettingJson = true;
        jLeaderboards = json.leaderboard;
        isSettingJson = false;
    }

    IEnumerator _WaitSetJson ()
    {
        while (isSettingJson)
        {
            yield return _WFSUtility.wef;
        }

        SetItemLeaderboards ();
    }

    private void Hide ()
    {
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }

    private void SetItemLeaderboards ()
    {
        if (jLeaderboards == null)
            return;

        for (int i = 0; i < jLeaderboards.Length; i++)
        {
            itemLeaderboards[i].SetData (jLeaderboards[i]);
        }
        for (int x = jLeaderboards.Length; x < itemLeaderboards.Length; x++)
        {
            itemLeaderboards[x].gameObject.SetActive (false);
        }
    }

    public void Logout ()
    {
        isInit = false;
        isSettingJson = true;
    }
}
