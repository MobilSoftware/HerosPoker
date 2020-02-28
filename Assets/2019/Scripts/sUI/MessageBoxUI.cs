using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageBoxUI : MonoBehaviour
{
    public TextMeshProUGUI txtDescription;
    public TextMeshProUGUI txtBtnPositive;
    public TextMeshProUGUI txtBtnNegative;

    public Button btnPositive;
    public Button btnNegative;

    private GameObject listener;
    private int returnedCode;
    private MenuOrBoxType prevMenuOrBox;

    private void Awake ()
    {
        btnPositive.onClick.AddListener (OnPositive);
        btnNegative.onClick.AddListener (OnNegative);
    }

    public void Show ( GameObject listener, string strDescription, MessageBoxType type = MessageBoxType.OK, int returnedCode = -1, bool bTranslate = false, string strBtnPositive = null, string strBtnNegative = null )
    {

        if (!bTranslate)
            SoundManager.instance.PlaySFX (SFXType.SFX_PopupOpen, Vector3.zero);

        gameObject.SetActive (true);

        this.listener = listener;
        this.returnedCode = returnedCode;

        txtDescription.SetText (strDescription);

        if (type == MessageBoxType.OK)
        {
            btnNegative.gameObject.SetActive (false);
            btnPositive.gameObject.SetActive (true);
        }
        else if (type == MessageBoxType.NO_BUTTONS)
        {
            btnNegative.gameObject.SetActive (false);
            btnPositive.gameObject.SetActive (false);
        }
        else
        {
            btnNegative.gameObject.SetActive (true);
            btnPositive.gameObject.SetActive (true);
        }

        if (strBtnPositive != null)
            txtBtnPositive.SetText (strBtnPositive);
        else
            txtBtnPositive.SetText ("ID_Ok");

        if (btnNegative.gameObject.activeSelf)
        {
            if (strBtnNegative != null)
                txtBtnNegative.SetText (strBtnNegative);
            else
                txtBtnNegative.SetText ("ID_Cancel");
        }
    }

    public void Hide ()
    {
        gameObject.SetActive (false);
    }

    private void OnPositive ()
    {
        Hide ();
        if (listener)
            listener.SendMessage ("onMessageBoxOKClicked", returnedCode, SendMessageOptions.DontRequireReceiver);
    }

    private void OnNegative ()
    {
        Hide ();

        if (listener)
            listener.SendMessage ("onMessageBoxCancelledClicked", returnedCode, SendMessageOptions.DontRequireReceiver);
    }
}
