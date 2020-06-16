using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSearchPlayer : MonoBehaviour
{
    public Text txtDisplayName;
    public Text txtStatus;
    public Text txtLevel;
    public Button btnAdd;
    public Button btnViewProfile;

    JFriend jFriend;

    private void Start ()
    {
        btnViewProfile.onClick.AddListener (OnViewProfile);
        btnAdd.onClick.AddListener (OnAdd);
    }

    private void OnViewProfile ()
    {
        OtherProfileManager.instance.SetData (jFriend);
    }

    private void OnAdd ()
    {
        ApiManager.instance.SendFriend (jFriend.player_id, ApiBridge.SendFriendType.SendFriendRequest);
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
