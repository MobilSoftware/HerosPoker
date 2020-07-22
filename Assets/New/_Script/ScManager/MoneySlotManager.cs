using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneySlotManager : MonoBehaviour
{
    private static MoneySlotManager s_Instance = null;
    public static MoneySlotManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (MoneySlotManager)) as MoneySlotManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an MoneySlotManager object. \n You have to have exactly one MoneySlotManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Transform trFrame;
    public Button btnSpin;
    public Button btnClose;
    public ItemReel[] reels;
    public Text txtTimer;
    [HideInInspector]
    public JGetMoneySlot json;

    private bool isInit;
    private SceneType prevSceneType;
    private Coroutine crSpin;
    private Coroutine crStartTimer;
    private int timerSeconds;

    private void Start ()
    {
        btnSpin.onClick.AddListener (OnSpin);
        btnClose.onClick.AddListener (Hide);
    }

    private void OnSpin ()
    {
        btnSpin.interactable = false;
        if (crSpin != null)
            StopCoroutine (crSpin);
        crSpin = StartCoroutine (_Spin ());
        ApiManager.instance.GetMoneySlot ();
    }

    IEnumerator _Spin ()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].SpinReel (10);
            yield return _WFSUtility.wfs05;
        }
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
            canvas.sortingOrder = (int) SceneType.MONEY_SLOT;
            isInit = true;
        }
        trFrame.localScale = Vector3.zero;
        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.MONEY_SLOT;
        trFrame.LeanScale (Vector3.one, _SceneManager.TWEEN_DURATION);
    }

    private void Hide ()
    {
        trFrame.LeanScale (Vector3.zero, _SceneManager.TWEEN_DURATION).setOnComplete
        (() =>
        {
            canvas.enabled = false;
            _SceneManager.instance.activeSceneType = prevSceneType;
        });
    }

    public void SetJson (JGetMoneySlot _json )
    {
        json = _json;

        if (json.money_slot > 0)        // = 86
        {
            string strResult = json.money_slot.ToString ().PadLeft (5, '0');
            char[] digits = strResult.ToCharArray ();
            if (digits.Length != 5)
            {
                for (int i = 0; i < reels.Length; i++)
                {
                    reels[i].SetStringResult (0);
                    reels[i].SetReplied ();
                }
                Logger.E ("char array length not 5");
                return;
            }

            Logger.E ("char array length = 5 | " + json.money_slot);
            for (int i = 0; i < digits.Length; i++)
            {
                reels[i].SetStringResult (int.Parse (digits[i].ToString()));
                reels[i].SetReplied ();
            }
        }
        else
        {
            MessageManager.instance.Show (this.gameObject, "Putaran uang belum bisa digunakan lagi");
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].SetStringResult (0);
                reels[i].SetReplied ();
            }
        }

        StartTimer ();
    }

    public void CheckShowResult (ItemReel reel)
    {
        if (reel == reels [reels.Length - 1])
        {
            Invoke ("ShowResult", 0.5f);
            //show reward here
            //use receiveitemmanager with reward from json
        }
    }

    private void ShowResult ()
    {
        if (json.money_slot > 0)
        {
            ItemReceiveData item = new ItemReceiveData (1, 0, json.money_slot);
            ReceiveItemManager.instance.Show (new ItemReceiveData[] { item });
            PlayerData.owned_coin = long.Parse (json.player.coin);
            _SceneManager.instance.UpdateAllCoinAndCoupon ();
        }
    }

    public void StartTimer ()
    {
        if (crStartTimer != null)
            StopCoroutine (crStartTimer);
        crStartTimer = StartCoroutine (_StartTimer());
    }

    IEnumerator _StartTimer ()
    {
        timerSeconds = json.next_in;
        btnSpin.interactable = false;
        while (timerSeconds > 0)
        {
            timerSeconds--;
            TimeSpan t = TimeSpan.FromSeconds (timerSeconds);

            txtTimer.text = string.Format ("{0:D2} : {1:D2} : {2:D2}",
                t.Hours,
                t.Minutes,
                t.Seconds);
            yield return new WaitForSecondsRealtime (1f);
        }

        btnSpin.interactable = true;
        txtTimer.text = "PUTAR";
    }

    public void Logout ()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].SetInitialState ();
        }
        if (crStartTimer != null)
        {
            StopCoroutine (crStartTimer);
            crStartTimer = null;
        }
        if (crSpin != null)
        {
            StopCoroutine (crSpin);
            crSpin = null;
        }
        btnSpin.interactable = true;
        timerSeconds = 0;
        txtTimer.text = "PUTAR";
    }
}
