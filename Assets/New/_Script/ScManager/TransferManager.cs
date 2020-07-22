using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransferManager : MonoBehaviour
{
    private static TransferManager s_Instance = null;
    public static TransferManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (TransferManager)) as TransferManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an TransferManager object. \n You have to have exactly one TransferManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Transform trFrame;
    public Button btnClose;
    public GameObject objInput;
    public GameObject objConfirm;

    public InputField ipfPlayerTag;
    public InputField ipfCoinAmount;
    public Text txtCoinLimitAmount;
    public Button btnNext;

    public Text txtCoinAmount;
    public Text txtReceiverName;
    public Text txtConfirmation;
    public Button btnBack;
    public Button btnSend;

    [HideInInspector]
    public bool isWaitingJSON;
    [HideInInspector]
    public bool isInit;
    private SceneType prevSceneType;
    private long limitCoinAmount;
    private string strPlayerTag;
    private long transferCoinAmount;
    private Coroutine crToConfirm;


    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
        btnNext.onClick.AddListener (ToConfirm);
        btnBack.onClick.AddListener (ToInput);
        btnSend.onClick.AddListener (OnSend);
    }

    private void ToConfirm ()
    {
        if (CheckValidity ())
        {
            if (crToConfirm != null)
            {
                StopCoroutine (crToConfirm);
                crToConfirm = null;
            }

            crToConfirm = StartCoroutine (_ToConfirm ());
        }
    }

    IEnumerator _ToConfirm ()
    {
        while (isWaitingJSON)
        {
            yield return _WFSUtility.wef;
        }

        objConfirm.SetActive (true);
        objInput.SetActive (false);
    }

    public void ToInput ()
    {
        objConfirm.SetActive (false);
        objInput.SetActive (true);
    }

    private bool CheckValidity ()
    {
        if (ipfPlayerTag.text == string.Empty)
        {
            MessageManager.instance.Show (this.gameObject, "Mohon isi ID penerima transfer");
            return false;
        }

        if (ipfCoinAmount.text == string.Empty)
        {
            MessageManager.instance.Show (this.gameObject, "Mohon isi jumlah Koin yang ingin di transfer");
            return false;
        }

        Logger.E ("ipf coin amount: " + ipfCoinAmount.text);
        long coinAmount = long.Parse (ipfCoinAmount.text);
        if (coinAmount < 1000)
        {
            MessageManager.instance.Show (this.gameObject, "Nominal minimum per transfer adalah 1,000 Koin");
            return false;
        }

        if (coinAmount < PlayerData.owned_coin / 1000)
        {
            MessageManager.instance.Show (this.gameObject, "Koin yang dimiliki tidak mencukupi");
            return false;
        }

        if (coinAmount < limitCoinAmount)
        {
            MessageManager.instance.Show (this.gameObject, "Nominal Koin melewati batas hari ini");
            return false;
        }

        transferCoinAmount = coinAmount / 1000;
        strPlayerTag = ipfPlayerTag.text;
        isWaitingJSON = true;
        try
        {
            int receiverID = Rays.Utilities.Util.SearchTag2PlayerID (strPlayerTag);
            ApiManager.instance.GetFriend (receiverID);
        } catch (Exception e)
        {
            Logger.E ("Exception: " + e.Message);
            MessageManager.instance.Show (this.gameObject, "Player tidak ditemukan");
            ResetAll ();
            ToInput ();
            return false;
        }
        return true;
    }

    public void SetReceiverInfo (string strDisplayName, string strDisplayPicture)
    {
        txtReceiverName.text = strDisplayName;
        txtCoinAmount.text = long.Parse (ipfCoinAmount.text).ToString ("N0");
        //set display picture here
        isWaitingJSON = false;
        string strConfirmation = "Yakin ingin memberikan {0} ke {1}?";
        txtConfirmation.text = string.Format (strConfirmation, txtCoinAmount.text, txtReceiverName.text);
    }

    private void OnSend ()
    {
        ApiManager.instance.SendCoin (strPlayerTag, transferCoinAmount);
    }

    private void ResetConfirm ()
    {
        txtCoinAmount.text = string.Empty;
        txtReceiverName.text = string.Empty;
    }

    public void ResetAll ()
    {
        ipfCoinAmount.text = string.Empty;
        ipfPlayerTag.text = string.Empty;
        strPlayerTag = string.Empty;
        transferCoinAmount = 0;
        ResetConfirm ();
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
            canvas.sortingOrder = (int) SceneType.TRANSFER;
            isInit = true;
        }
        trFrame.localScale = Vector3.zero;
        canvas.enabled = true;
        trFrame.LeanScale (Vector3.one, _SceneManager.TWEEN_DURATION);
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.TRANSFER;
    }

    private void Hide ()
    {
        if (crToConfirm != null)
        {
            StopCoroutine (crToConfirm);
            crToConfirm = null;
        }
        trFrame.LeanScale (Vector3.zero, _SceneManager.TWEEN_DURATION).setOnComplete
        (() =>
        { 
            canvas.enabled = false;
            isInit = false;
            ResetAll ();
            ToInput ();
            _SceneManager.instance.activeSceneType = prevSceneType;
        });
    }
}
