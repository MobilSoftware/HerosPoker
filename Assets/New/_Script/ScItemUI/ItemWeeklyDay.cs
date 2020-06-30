using UnityEngine;
using UnityEngine.UI;

public class ItemWeeklyDay : MonoBehaviour
{
    public Button btnClaim;
    public Text txtRewardAmount;
    public RawImage imgReward;
    public GameObject objDimmer;
    public GameObject objClaimed;

    private void Start ()
    {
        btnClaim.onClick.AddListener (OnClaim);
    }

    private void OnClaim ()
    {
        WeeklyRewardsManager.instance.OnClaim ();
    }

    public void SetData (int itemTypeID, int itemID, string strAmount )
    {
        if (itemTypeID == 1)
        {
            imgReward.texture = HomeManager.instance.sprCoin.texture;
            txtRewardAmount.text = long.Parse (strAmount).toShortCurrency ();
        }
        else if (itemTypeID == 2)
        {
            imgReward.texture = HomeManager.instance.sprCoupon.texture;
            txtRewardAmount.text = long.Parse (strAmount).toCouponShortCurrency ();
        }
        else
        {
            string imagePath = BundleManager.instance.GetItemLoadPath (DownloadType.THUMB, itemTypeID, itemID);
            BundleManager.instance.LoadImage (imgReward, imagePath);
            txtRewardAmount.text = strAmount;
        }
    }
}
