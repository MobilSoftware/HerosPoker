using UnityEngine;
using UnityEngine.UI;

public class _BuyInHUD : MonoBehaviour
{
    [SerializeField] Slider sliderUI;

    _SliderValue sliderTools;
    long lasSliderVal;

    [SerializeField] Text txtVal, myAcc;
    [SerializeField] Text txtMin, txtMax;

    [SerializeField] Button btnMinBuy, btnMaxBuy, btnBuy, btnClose;
    public Toggle toggleAuto;

    [SerializeField] long minBuy, maxBuy;
    public long myMoney;

    private void Start()
    {
        sliderTools = new _SliderValue();

        sliderUI.onValueChanged.AddListener(ReturnScrollValue);

        btnMinBuy.onClick.AddListener(ConfirmMinBuy);
        btnMaxBuy.onClick.AddListener(ConfirmMaxBuy);
        btnBuy.onClick.AddListener(ConfirmBuy);
        btnClose.onClick.AddListener(Exit);

        Debug.LogError ("start buy in hud");
        gameObject.SetActive(false);
    }

    private void SetMinMax()
    {
        minBuy = GlobalVariables.MinBetAmount * 10;
        //myMoney = DataManager.instance.ownedGold;
        myMoney = PlayerData.owned_gold;
        maxBuy = myMoney > minBuy * 20 ? minBuy * 20 : myMoney;

        if (myMoney < maxBuy)
            maxBuy = myMoney;
    }

    public void Show()
    {
        SetMinMax();
        
        gameObject.SetActive(true);
        myAcc.text = "Bank Account : <color=yellow>" + myMoney.toShortCurrency() + "</color>";

        txtMin.text = minBuy.toShortCurrency();
        txtMax.text = maxBuy.toShortCurrency();

        ReturnScrollValue(0);

        _PokerGameHUD.instance.boxThrow.Hide();
    }

    public void AutoBuyIn()
    {
        SetMinMax();
        PlayerUtility.BuyInFromBankAccount(maxBuy);
        PhotonUtility.SetPlayerProperties(PhotonNetwork.player, PhotonEnums.Player.Money, maxBuy);
        _PokerGameManager.instance.unsortedPlayers[0].AutoBuyIn(maxBuy);
    }

    public void Hide()
    {
        gameObject.SetActive (false);
    }

    public void Exit()
    {
        GlobalVariables.bQuitOnNextRound = false;

        //StartCoroutine(PokerManager.instance.uiPause._LoadMenu ());
        PokerManager.instance.uiPause.LoadMenu ();
    }

    void ReturnScrollValue(float _val)
    {
        //Debug.LogError("Slider Buy IN" + _val + " Max : " +maxBuy +" Min " +minBuy);
        lasSliderVal = sliderTools.CalculateScore(maxBuy, minBuy, _val);
        txtVal.text = lasSliderVal.toFlexibleCurrency();
    }

    void ConfirmBuy()
    {
        PhotonTexasPokerManager.instance.ReJoinBuyIn(lasSliderVal);
    }

    void ConfirmMinBuy()
    {
        PhotonTexasPokerManager.instance.ReJoinBuyIn(minBuy);
    }

    void ConfirmMaxBuy()
    {
        PhotonTexasPokerManager.instance.ReJoinBuyIn(maxBuy);
    }
}
