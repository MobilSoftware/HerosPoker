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
            PhotonNetwork.ConnectUsingSettings ("v1.0");
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
        //ApiManager.instance.GuestLogin ();
        StartCoroutine (_OnGuest ());
    }

    IEnumerator _OnGuest()
    {
        ApiManager.instance.GetOtp ();
        while (!ApiManager.instance.bOtpSet)
        {
            //loading ui
            yield return _WFSUtility.wef;
        }
        ApiManager.instance.GuestLogin ();
    }

    private void OnFB ()
    {

    }

    private void OnGoogle ()
    {

    }
}
