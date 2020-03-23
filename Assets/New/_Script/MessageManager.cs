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
                    Debug.Log ("Could not locate an MessageManager object. \n You have to have exactly one MessageManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;

    public TextMeshProUGUI tmpDescription;
    public TextMeshProUGUI tmpBtnPositive;
    public TextMeshProUGUI tmpBtnNegative;
    public Button btnPositive;
    public Button btnNegative;

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

    public void Show (GameObject _listener, string strDescription, ButtonMode bt = ButtonMode.OK, int _returnCode = -1, string strBtnPositive = "OK", string strBtnNegative = "Batal" )
    {
        objListener = _listener;
        returnCode = _returnCode;

        tmpDescription.text = strDescription;

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
        canvas.enabled = true;
        _SceneManager.instance.activeSceneType = SceneType.MESSAGE;
    }

    public void Hide ()
    {
        canvas.enabled = false;
    }
}
