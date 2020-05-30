using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager s_Instance = null;
    public static SettingsManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (SettingsManager)) as SettingsManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an SettingsManager object. \n You have to have exactly one SettingsManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Button btnClose;
    public Button btnLogout;
    public Button btnPageFB;
    public Slider sliderSFX;
    public Slider sliderBGM;

    private SceneType prevSceneType;
    private bool isInit;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
        btnLogout.onClick.AddListener (Logout);
        btnPageFB.onClick.AddListener (OnPageFB);

    }

    public void SetCanvas (bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    private void Show()
    {
        if (!isInit)
        {
            isInit = true;
            canvas.sortingOrder = (int) SceneType.SETTINGS;
        }

        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.SETTINGS;
    }

    private void Hide()
    {
        canvas.enabled = false;
    }

    private void Logout ()
    {
        FacebookManager.instance.Logout ();
        ShopManager.instance.Logout ();
        HeroManager.instance.Logout ();
        LeaderboardManager.instance.Logout ();
        FriendManager.instance.Logout ();
        Hide ();
        PlayerPrefs.DeleteAll ();
        _SceneManager.instance.SetActiveScene (SceneType.HOME, false);
        _SceneManager.instance.SetActiveScene (SceneType.LOGIN, true);

    }

    private void OnPageFB()
    {
        //go to fb page
    }

    private void OnVolumeChangeBGM ( float _val )
    {
        PlayerPrefs.SetFloat (PrefKey.VolBGM.ToString (), _val);
        PlayerPrefs.Save ();
        SoundManager.instance.SetVolume (_val);
    }

    private void OnVolumeChangeSFX ( float _val )
    {
        PlayerPrefs.SetFloat (PrefKey.VolSFX.ToString (), _val);
        PlayerPrefs.Save ();
    }
}
