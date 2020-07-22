using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ButtonMode
{
    OK,
    CANCEL,
    OK_CANCEL
}

public class MessageManager : MonoBehaviour
{
    private static MessageManager s_Instance = null;
    public static MessageManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (MessageManager)) as MessageManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an MessageManager object. \n You have to have exactly one MessageManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Transform trFrame;

    public GameObject objFrameTitle;
    public Text txtTitle;
    public TextMeshProUGUI tmpDescription;
    public Text tmpBtnPositive;
    public Text tmpBtnNegative;
    public Button btnPositive;
    public Button btnNegative;

    private SceneType prevSceneType;
    private GameObject objListener;
    private int returnCode;
    private bool isInit;

    private void Start ()
    {
        btnPositive.onClick.AddListener (OnPositive);
        btnNegative.onClick.AddListener (OnNegative);
    }

    private void OnPositive ()
    {
        if (objListener)
            objListener.SendMessage ("OnPositiveClicked", returnCode, SendMessageOptions.DontRequireReceiver);

        Hide ();
    }

    private void OnNegative ()
    {
        if (objListener)
            objListener.SendMessage ("OnNegativeClicked", returnCode, SendMessageOptions.DontRequireReceiver);

        Hide ();
    }

    public void Show (GameObject _listener, string strDescription, ButtonMode bt = ButtonMode.OK, int _returnCode = -1, string strBtnPositive = "OK", string strBtnNegative = "Batal", string strTitle = "" )
    {
        objListener = _listener;
        returnCode = _returnCode;

        tmpDescription.text = strDescription;
        if (strTitle == "")
        {
            objFrameTitle.SetActive (false);
            txtTitle.text = string.Empty;
        }
        else
        {
            objFrameTitle.SetActive (true);
            txtTitle.text = strTitle;
        }
        switch (bt)
        {
            case ButtonMode.OK:
                btnNegative.gameObject.SetActive (false);
                btnPositive.gameObject.SetActive (true);
                break;
            case ButtonMode.CANCEL:
                btnNegative.gameObject.SetActive (true);
                btnPositive.gameObject.SetActive (false);
                break;
            case ButtonMode.OK_CANCEL:
                btnNegative.gameObject.SetActive (true);
                btnPositive.gameObject.SetActive (true);
                break;
        }

        tmpBtnPositive.text = strBtnPositive;
        tmpBtnNegative.text = strBtnNegative;

        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.MESSAGE;
            isInit = true;
        }
        trFrame.localScale = Vector3.zero;
        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.MESSAGE;
        trFrame.LeanScale (Vector3.one, _SceneManager.TWEEN_DURATION);
    }

    public void Hide ()
    {
        trFrame.LeanScale (Vector3.zero, _SceneManager.TWEEN_DURATION).setOnComplete
        (() =>
        {
            canvas.enabled = false;
            _SceneManager.instance.activeSceneType = prevSceneType;
        });
    }
}
