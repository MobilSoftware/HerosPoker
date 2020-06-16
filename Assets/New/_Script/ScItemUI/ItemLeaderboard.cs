using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemLeaderboard : MonoBehaviour
{
    public Text txtDisplayName;
    public Text txtStatus;
    public Text txtLevel;
    public TextMeshProUGUI tmpCoinValue;
    public Button btnViewProfile;

    JLeaderboard jLeaderboard;

    private void Start ()
    {
        btnViewProfile.onClick.AddListener (OnViewProfile);
    }

    private void OnViewProfile ()
    {
        ApiManager.instance.GetFriend (jLeaderboard.player_id);
    }

    public void SetData (JLeaderboard json)
    {
        jLeaderboard = json;
        if (json.profile.display_name == null)
            json.profile.display_name = string.Empty;
        if (json.profile.display_name.Length > 10)
        {
            json.profile.display_name = json.profile.display_name.Substring (0, 7) + "...";
        }
        txtDisplayName.text = json.profile.display_name;
        tmpCoinValue.text = long.Parse (json.scoring).toShortCurrency ();
        txtStatus.text = json.profile.status_message;
    }
}
