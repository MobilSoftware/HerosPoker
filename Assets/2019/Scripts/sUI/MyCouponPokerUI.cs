using System;
using UnityEngine;
using UnityEngine.UI;

public class MyCouponPokerUI : MonoBehaviour
{
    public Text txtJackpotValue;
    public Text txtMyCouponValue1;
    public Text txtMyCouponValue2;

    [SerializeField]
    private string[] strMyCouponValue1;
    [SerializeField]
    private string[] strMyCouponValue2;

    public void Show ()
    {
        gameObject.SetActive (true);
    }

    public void SetData()
    {
        txtMyCouponValue1.text = strMyCouponValue1[(int) GlobalVariables.environment - 1];
        txtMyCouponValue2.text = strMyCouponValue2[(int) GlobalVariables.environment - 1];
    }

    public void Hide ()
    {
        gameObject.SetActive (false);
        //_PokerGameHUD.instance.imgBgMission.sprite = _PokerGameHUD.instance.sprClosed;
    }

    private void MisssionCompleted (int index )
    {
        //Logger.E ("mission " + index + " triggered");
        //long lCoupon = Convert.ToInt64 (HomeSceneManager.Instance.myPlayerData.player.coupon);
        //long theCoupon = 0;
        //switch (index)
        //{
        //    case 1:
        //        theCoupon = Convert.ToInt64 (strMyCouponValue1[(int)GlobalVariables.environment - 1]);
        //        break;
        //    case 2:
        //        theCoupon = Convert.ToInt64 (strMyCouponValue2[(int) GlobalVariables.environment - 1]);
        //        break;
        //}
        //lCoupon += theCoupon;
        //HomeSceneManager.Instance.myPlayerData.player.coupon = lCoupon.ToString ();
        //HomeSceneManager.Instance.UpdatePlayerDataUI ();
    }

    public void UpdateJackpotValue ()
    {
        //txtJackpotValue.text = HomeSceneManager.Instance.myStartPoker.game.jackpot_now;
    }

    public void SetMinimumValue ()
    {
        switch ((int)GlobalVariables.environment)
        {
            case 1:
                txtJackpotValue.text = "500";
                break;
            case 2:
                txtJackpotValue.text = "1000";
                break;
            case 3:
                txtJackpotValue.text = "1500";
                break;
            case 4:
                txtJackpotValue.text = "2500";
                break;
            case 5:
                txtJackpotValue.text = "5000";
                break;
            case 6:
                txtJackpotValue.text = "7500";
                break;
            case 7:
                txtJackpotValue.text = "10000";
                break;
            case 8:
                txtJackpotValue.text = "15000";
                break;
        }
    }
}
