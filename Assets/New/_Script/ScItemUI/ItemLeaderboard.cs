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
        if (json.display_name.Length > 10)
        {
            json.display_name = json.display_name.Substring (0, 7) + "...";
        }
        txtDisplayName.text = json.display_name;
        tmpCoinValue.text = long.Parse (json.scoring).toShortCurrency ();
    }
}
