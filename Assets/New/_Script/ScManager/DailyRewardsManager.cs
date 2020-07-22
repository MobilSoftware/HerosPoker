using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardsManager : MonoBehaviour
{
    private static DailyRewardsManager s_Instance = null;
    public static DailyRewardsManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (DailyRewardsManager)) as DailyRewardsManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an DailyRewardsManager object. \n You have to have exactly one DailyRewardsManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Transform trFrame;
    public Button btnClaim;
    public Button btnClose;

    private bool isInit;
    private SceneType prevSceneType;

    private void Start ()
    {
        btnClaim.onClick.AddListener (OnClaim);
        btnClose.onClick.AddListener (Hide);
    }

    private void OnClaim ()
    {
        ApiManager.instance.GetDailyLogin ();
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
            canvas.sortingOrder = (int) SceneType.DAILY_REWARDS;
            isInit = true;
        }

        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.DAILY_REWARDS;
        trFrame.localScale = Vector3.zero;
        canvas.enabled = true;
        trFrame.LeanScale (Vector3.one, _SceneManager.TWEEN_DURATION);
    }

    private void Hide ()
    {
        trFrame.LeanScale (Vector3.zero, _SceneManager.TWEEN_DURATION).setOnComplete
        (() =>
        {
            _SceneManager.instance.activeSceneType = prevSceneType;
            canvas.enabled = false;
        });
    }
}
