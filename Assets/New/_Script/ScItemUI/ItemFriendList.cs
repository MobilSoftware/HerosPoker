using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemFriendList : MonoBehaviour
{
    public Text txtDisplayName;
    public Text txtStatus;
    public TextMeshProUGUI tmpCoinValue;
    public Button btnViewProfile;

    [HideInInspector]
    public int playerID;

    private JFriend jFriend;

    private void Start ()
    {
        btnViewProfile.onClick.AddListener (OnViewProfile);
    }

    private void OnViewProfile ()
    {
        //ApiManager.instance.GetFriend (jFriend.player_id);
        OtherProfileManager.instance.SetData (jFriend);
    }

    public void SetData ( JFriend json )
    {
        jFriend = json;
        if (json.display_name.Length > 10)
        {
            json.display_name = json.display_name.Substring (0, 7) + "...";
        }
        txtDisplayName.text = json.display_name;
        tmpCoinValue.text = long.Parse (json.coin).toShortCurrency ();
    }
}
