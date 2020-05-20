using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PokerRoomManager : MonoBehaviour
{
    private static PokerRoomManager s_Instance = null;
    public static PokerRoomManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (PokerRoomManager)) as PokerRoomManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an PokerRoomManager object. \n You have to have exactly one PokerRoomManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public TextMeshProUGUI tmpCoin;
    public TextMeshProUGUI tmpCoupon;
    public long[] minBets;      //length = 4
    public long[] maxBuyIn;     //length = 4
    public long[] minOwnedCoin; //length = 4
    public long[] maxOwnedCoin; //length = 4
    public Button[] btnRooms;
    public Button btnQuickPlay;
    public Button btnClose;

    private SceneType prevSceneType;
    private bool isInit;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
        btnQuickPlay.onClick.AddListener (OnQuickPlay);
        btnRooms[0].onClick.AddListener (() => OnClick (0));
        btnRooms[1].onClick.AddListener (() => OnClick (1));
        btnRooms[2].onClick.AddListener (() => OnClick (2));
        btnRooms[3].onClick.AddListener (() => OnClick (3));
    }

    public void SetCanvas ( bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    private void OnClick (int btnIndex )
    {
        Logger.E ("btn room clicked: " + btnIndex);
        if (btnIndex != 3)
        {
            if (PlayerData.owned_coin > minOwnedCoin[btnIndex] && PlayerData.owned_coin <= maxOwnedCoin[btnIndex])
            {
                GlobalVariables.MinBetAmount = minBets[btnIndex];
                GlobalVariables.MaxBuyIn = maxBuyIn[btnIndex];
                Hide ();
                _SceneManager.instance.SetActiveScene (SceneType.HOME, false);
                _SceneManager.instance.SetActiveScene (SceneType.POKER, true);
            }
            else if (PlayerData.owned_coin < minOwnedCoin[btnIndex])
                MessageManager.instance.Show (this.gameObject, "Koin tidak cukup, harap isi ulang atau ke ruang lain untuk bermain", ButtonMode.OK_CANCEL, 1, "Pindah", "Beli");
            else if (PlayerData.owned_coin > maxOwnedCoin[btnIndex])
                MessageManager.instance.Show (this.gameObject, "Jumlah koin anda mencapai " + PlayerData.owned_coin.toShortCurrency () + ", harap pergi ke ruang selanjutnya untuk bermain", ButtonMode.OK, 2, "Tentukan");
        }
        else
        {
            if (PlayerData.owned_coin > minOwnedCoin[btnIndex])
            {
                GlobalVariables.MinBetAmount = minBets[btnIndex];
                GlobalVariables.MaxBuyIn = maxBuyIn[btnIndex];
                Hide ();
                _SceneManager.instance.SetActiveScene (SceneType.HOME, false);
                _SceneManager.instance.SetActiveScene (SceneType.POKER, true);
            }
            else if (PlayerData.owned_coin < minOwnedCoin[btnIndex])
                MessageManager.instance.Show (this.gameObject, "Koin tidak cukup, harap isi ulang atau ke ruang lain untuk bermain", ButtonMode.OK_CANCEL, 1, "Pindah", "Beli");
            else if (PlayerData.owned_coin > maxOwnedCoin[btnIndex])
                MessageManager.instance.Show (this.gameObject, "Jumlah koin anda mencapai " + PlayerData.owned_coin.toShortCurrency () + ", harap pergi ke ruang selanjutnya untuk bermain", ButtonMode.OK, 2, "Tentukan");
        }
    }

    public void OnQuickPlay ()
    {
        int index = GetSuitableIndex ();
        if (index == -1)
            MessageManager.instance.Show (this.gameObject, "Koin anda tidak cukup");
        else
            OnClick (index);
    }

    private int GetSuitableIndex ()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i != 3)
            {
                if (PlayerData.owned_coin > minOwnedCoin[i] && PlayerData.owned_coin <= maxOwnedCoin[i])
                {
                    return i;
                }
            }
            else
            {
                if (PlayerData.owned_coin > minOwnedCoin[i])
                    return i;
            }
        }

        return -1;
    }

    public void Show ()
    {
        if (!isInit)
        {
            isInit = true;
            canvas.sortingOrder = (int) SceneType.POKER_ROOM;
        }
        canvas.enabled = true;
        tmpCoin.text = Convert.ToInt64 (PlayerData.owned_coin).toShortCurrency ();
        tmpCoupon.text = Convert.ToInt64 (PlayerData.owned_coupon).toCouponShortCurrency ();
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.POKER_ROOM;

    }

    public void Hide ()
    {
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }

    public void OnPositiveClicked ( int returnCode )
    {
        switch (returnCode)
        {
            case 1:
            case 2:
                MessageManager.instance.Hide ();
                OnQuickPlay ();
                break;
        }
    }

    public void OnNegativeClicked ( int returnCode )
    {
        switch (returnCode)
        {
            case 1:
                Hide ();
                MessageManager.instance.Hide ();
                _SceneManager.instance.SetActiveScene (SceneType.SHOP, true);
                break;
        }
    }

    public void UpdateCoinAndCoupon ()
    {
        tmpCoin.text = PlayerData.owned_coin.toShortCurrency ();
        tmpCoupon.text = PlayerData.owned_coupon.toCouponShortCurrency ();
    }
}
