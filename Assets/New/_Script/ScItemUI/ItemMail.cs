using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMail : MonoBehaviour
{
    public Text txtTitle;
    public Text txtReceivedDate;
    public Image imgFrame;
    public Button btnMail;
    public GameObject objNotif;
    [HideInInspector]
    public JInbox json;
    private string strTitle;

    private void Start ()
    {
        btnMail.onClick.AddListener (OnMail);
    }

    private void OnMail ()
    {
        InboxManager.instance.UnSelectMail ();
        InboxManager.instance.selectedMail = this;
        imgFrame.sprite = InboxManager.instance.sprSelected;
        txtTitle.color = InboxManager.instance.colSelected;
        InboxManager.instance.btnReceive.gameObject.SetActive (true);
        InboxManager.instance.btnDelete.gameObject.SetActive (true);
        InboxManager.instance.OpenMail (json, strTitle);
        if (json.mail_read != 1)
        {
            ApiManager.instance.ReadInbox (json.item_send_history_id);
            json.mail_read = 1;
            InboxManager.instance.SetNotifStatus (this);
        }
    }

    public void SetData (JInbox _json )
    {
        json = _json;
        strTitle = "Hadiah dari " + json.sender_name;
        string strSubTitle = strTitle.Substring (0, 7) + "...";

        txtTitle.text = strSubTitle;
        txtReceivedDate.text = json.when_created[1];
        if (json.mail_claimed == 0 || json.mail_read == 0)
            objNotif.SetActive (true);
        else
            objNotif.SetActive (false);
    }
}
