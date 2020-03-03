using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeginUI : MonoBehaviour
{
    public TMP_InputField ipfDisplayName;

    public Button btnSet;


    private void Start ()
    {
        btnSet.onClick.AddListener (OnSet);
    }

    public void OnSet ()
    {
        string displayName = string.Empty;
        if (ipfDisplayName.text == string.Empty)
        {
            displayName = "NoName";
        }
        else
        {
            displayName = ipfDisplayName.text;
        }

        PlayerData.hero_id = 100;
        PlayerData.SetData (displayName);
        gameObject.SetActive (false);
    }
}
