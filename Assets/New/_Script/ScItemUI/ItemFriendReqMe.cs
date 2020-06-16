using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemFriendReqMe : MonoBehaviour
{
    public Text txtDisplayName;
    public Text txtStatus;
    public Text txtLevel;
    public Button btnYes;
    public Button btnNo;
    public Button btnViewProfile;

    JFriend jFriend;

    private void Start ()
    {
        btnViewProfile.onClick.AddListener (OnViewProfile);
        btnYes.onClick.AddListener (OnYes);
        btnNo.onClick.AddListener (OnNo);
    }

    private void OnViewProfile ()
    {
        OtherProfileManager.instance.SetData (jFriend);
    }

    private void OnYes ()
    {
        ApiManager.instance.SendFriend (jFriend.player_id, ApiBridge.SendFriendType.ResponseFriendRequestMeYes);
        FriendManager.instance.AddAcceptedFriend (jFriend);
        Destroy (this.gameObject);
    }

    private void OnNo ()
    {
        ApiManager.instance.SendFriend (jFriend.player_id, ApiBridge.SendFriendType.ResponseFriendRequestMeYes);
        Destroy (this.gameObject);
    }

    public void SetData ( JFriend json )
    {
        jFriend = json;
        if (json.display_name.Length > 10)
        {
            json.display_name = json.display_name.Substring (0, 7) + "...";
        }
        txtDisplayName.text = json.display_name;
        txtStatus.text = json.status_message;
        txtLevel.text = "Lv. " + json.level;
    }
}
