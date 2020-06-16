using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherProfileManager : MonoBehaviour
{
    private static OtherProfileManager s_Instance = null;
    public static OtherProfileManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (OtherProfileManager)) as OtherProfileManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an OtherProfileManager object. \n You have to have exactly one OtherProfileManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Text txtDisplayName;
    public Text txtStatus;
    public Text txtLevel;
    public Text txtCoinValue;
    public Text txtCouponValue;
    public Text txtTag;
    public Image fillExpBar;
    public Button btnClose;
    public Button btnAdd;
    public Button btnRemove;
    public Button btnBlock;
    public StandHero standHero;

    private SceneType prevSceneType;
    private bool isInit;
    private JFriend jFriend;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
        btnAdd.onClick.AddListener (OnAdd);
        btnRemove.onClick.AddListener (OnRemove);
        btnBlock.onClick.AddListener (OnBlock);
    }

    private void OnAdd ()
    {
        ApiManager.instance.SendFriend (jFriend.player_id, ApiBridge.SendFriendType.SendFriendRequest);
        btnAdd.gameObject.SetActive (false);
    }

    private void OnRemove ()
    {
        ApiManager.instance.SendFriend (jFriend.player_id, ApiBridge.SendFriendType.SendFriendRemove);
        btnRemove.gameObject.SetActive (false);
        FriendManager.instance.RemoveFriend (jFriend.player_id);
    }

    private void OnBlock ()
    {
        ApiManager.instance.SendFriend (jFriend.player_id, ApiBridge.SendFriendType.SendFriendBlock);
        btnBlock.gameObject.SetActive (false);
        FriendManager.instance.RemoveFriend (jFriend.player_id);
    }

    public void SetData (JFriend json )
    {
        if (json.on_friend_block || json.on_friend_block_me)
        {
            MessageManager.instance.Show (this.gameObject, "Tidak bisa menampilkan profil karena anda berada di daftar blok pemain ini");
            return;
        }
        jFriend = json;
        txtDisplayName.text = json.display_name;
        txtStatus.text = json.status_message;
        txtLevel.text = json.level.ToString ();
        txtCoinValue.text = long.Parse (json.coin).toShortCurrency ();
        txtCouponValue.text = long.Parse (json.coupon).toCouponShortCurrency ();
        txtTag.text = "Tag: " + json.player_tag;
        fillExpBar.fillAmount = float.Parse (json.exp_percentage);
        standHero.LoadFromBundle (json.costume_equiped);
        if (json.on_friend_list)
        {
            btnAdd.gameObject.SetActive (false);
            btnRemove.gameObject.SetActive (true);
            btnBlock.gameObject.SetActive (true);
        }
        else
        {
            btnAdd.gameObject.SetActive (true);
            btnRemove.gameObject.SetActive (false);
            btnBlock.gameObject.SetActive (false);
        }
        Show ();
    }

    public void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.OTHER_PROFILE;
            isInit = true;
        }

        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.OTHER_PROFILE;
    }

    public void Hide ()
    {
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }
}
