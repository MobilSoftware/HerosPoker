using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    private static LoginManager s_Instance = null;

    public static LoginManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (LoginManager)) as LoginManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an LoginManager object. \n You have to have exactly one LoginManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Button btnGuest;
    public Button btnFB;
    public Button btnGoogle;

    private bool isInit;

    private void Start ()
    {
        btnGuest.onClick.AddListener (OnGuest);
        btnFB.onClick.AddListener (OnFB);
        btnGoogle.onClick.AddListener (OnGoogle);
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
            canvas.sortingOrder = (int) SceneType.LOGIN;
            isInit = true;
        }

        canvas.enabled = true;
        _SceneManager.instance.activeSceneType = SceneType.LOGIN;
    }

    public void Hide ()
    {
        canvas.enabled = false;
    }

    private void OnGuest ()
    {
        StartCoroutine (_UserLogin (ApiBridge.LoginType.Guest));
    }

    private void OnFB ()
    {
        StartCoroutine (_UserLogin (ApiBridge.LoginType.Facebook));
    }

    private void OnGoogle ()
    {
        StartCoroutine (_UserLogin (ApiBridge.LoginType.Google));
    }

    IEnumerator _UserLogin (ApiBridge.LoginType loginType )
    {
        ApiManager.instance.GetOtp ();
        while (!ApiManager.instance.bOtpSet)
        {
            yield return _WFSUtility.wef;
        }

        if (loginType == ApiBridge.LoginType.Guest)
            ApiManager.instance.GuestLogin ();
        else if (loginType == ApiBridge.LoginType.Facebook)
            FacebookManager.instance.Login ();
        else if (loginType == ApiBridge.LoginType.Google)
            Logger.E ("Login Google");
    }
}
