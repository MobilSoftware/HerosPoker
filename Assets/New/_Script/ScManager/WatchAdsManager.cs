using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WatchAdsManager : MonoBehaviour
{
    private static WatchAdsManager s_Instance = null;
    public static WatchAdsManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (WatchAdsManager)) as WatchAdsManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an WatchAdsManager object. \n You have to have exactly one WatchAdsManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Transform trFrame;
    public Text txtDesc;        //format the watch ads reward to this
    public Text txtProgress;    //eg. ( 1/3 )
    public Button btnClose;
    public Button btnWatchAds;

    private SceneType prevSceneType;
    private bool isInit;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
        btnWatchAds.onClick.AddListener (OnWatch);
    }

    private void OnWatch ()
    {

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
            canvas.sortingOrder = (int) SceneType.WATCH_ADS;
            isInit = true;
        }
        trFrame.localScale = Vector3.zero;
        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.WATCH_ADS;
        trFrame.LeanScale (Vector3.one, _SceneManager.TWEEN_DURATION);
    }

    private void Hide()
    {
        trFrame.LeanScale (Vector3.zero, _SceneManager.TWEEN_DURATION).setOnComplete
        (() =>
        {
            canvas.enabled = false;
            _SceneManager.instance.activeSceneType = prevSceneType;
        });
    }

    public void Logout ()
    {
        isInit = false;
    }
}
