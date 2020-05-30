using UnityEngine;
using UnityEngine.UI;

public class VerifyManager : MonoBehaviour
{
    private static VerifyManager s_Instance = null;
    public static VerifyManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (VerifyManager)) as VerifyManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an VerifyManager object. \n You have to have exactly one VerifyManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Button btnClose;
    public Button btnOK;
    public InputField ipfPhoneNumber;
    public Dropdown ddCountryCode;

    private SceneType prevSceneType;
    private bool isInit;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
        btnOK.onClick.AddListener (OnVerify);
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
            canvas.sortingOrder = (int) SceneType.VERIFY;
            isInit = true;
        }

        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.VERIFY;
    }

    public void Hide ()
    {
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }

    private void OnVerify ()
    {

    }
}
