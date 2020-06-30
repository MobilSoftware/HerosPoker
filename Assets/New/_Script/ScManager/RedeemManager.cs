using UnityEngine;
using UnityEngine.UI;

public class RedeemManager : MonoBehaviour
{
    private static RedeemManager s_Instance = null;
    public static RedeemManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (RedeemManager)) as RedeemManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an RedeemManager object. \n You have to have exactly one RedeemManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public GameObject objInput;
    public GameObject objRetry;
    public InputField ipfCode;
    public Text txtError;
    public Button btnClose;
    public Button btnSubmit;
    public Button btnRetry;

    private bool isInit;
    private SceneType prevSceneType;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
        btnSubmit.onClick.AddListener (OnSendCode);
        btnRetry.onClick.AddListener (OnSendCode);
    }

    public void SetCanvas (bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    public void OnSendCode ()
    {
        if (ipfCode.text == string.Empty)
        {
            MessageManager.instance.Show (this.gameObject, "Kode tidak boleh kosong");
            return;
        }
        ApiManager.instance.ClaimPromoCode (ipfCode.text);
    }

    private void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int)SceneType.REDEEM;
            isInit = true;
        }

        objInput.SetActive (true);
        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.REDEEM;
    }

    private void Hide ()
    {
        canvas.enabled = false;
        ipfCode.text = string.Empty;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }
}
