using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDailyQuest : MonoBehaviour
{
    public RawImage imgReward;
    public Text txtDescription;
    public Text txtProgress;        //eg. "( 1/3 )"
    public TextMeshProUGUI tmpCoin;
    public Button btnGet;
    public Button btnStart;

    private string json;

    private void Start ()
    {
        btnGet.onClick.AddListener (OnGet);
        btnStart.onClick.AddListener (OnStart);
    }

    private void OnGet ()
    {

    }

    private void OnStart ()
    {

    }

    public void SetData (string _json )
    {
        json = _json;
        //set
    }
}
