using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InboxManager : MonoBehaviour
{
    private static InboxManager s_Instance = null;

    public static InboxManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (InboxManager)) as InboxManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an InboxManager object. \n You have to have exactly one InboxManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public ScrollRect scrRect;
    public Sprite sprSelected;
    public Sprite sprNotSelected;
    public Color colSelected;
    public Text txtTitle;
    public Text txtDesc;
    public Text txtSender;
    public Text txtDeleteDate;
    public Text txtReceive;
    public GameObject objDimmerBtnReceive;
    public Button btnClose;
    public Button btnReceiveAll;
    public Button btnReceive;
    public Button btnDelete;
    public ItemMail prefabItemMail;
    public Transform parentMail;
    public ItemMailReward[] rewards;    //length = 5
    private SceneType prevSceneType;
    private bool isInit;
    private List<ItemMail> mails;
    private JGetInbox json;
    [HideInInspector]
    public bool isSettingJSON;
    [HideInInspector]
    public ItemMail selectedMail;
    [HideInInspector]
    public int countClaim;
    [HideInInspector]
    public int currentLength;

    private void Start ()
    {
        isSettingJSON = true;
        btnClose.onClick.AddListener (Hide);
        btnReceive.onClick.AddListener (OnReceive);
        btnReceiveAll.onClick.AddListener (OnReceiveAll);
        btnDelete.onClick.AddListener (OnDelete);
    }

    private void OnReceive ()
    {
        countClaim = 0;
        ApiManager.instance.ClaimInbox (selectedMail.json.item_send_history_id);
        selectedMail.json.mail_claimed = 1;
        SetNotifStatus (selectedMail);
        SetReceiveStatus (selectedMail.json);
    }

    private void OnReceiveAll ()
    {
        countClaim = -1;
        List<ItemMail> claimingMails = new List<ItemMail> ();
        for (int i = 0; i < mails.Count; i++)
        {
            if (CheckClaimable (mails[i].json))
            {
                countClaim++;
                claimingMails.Add (mails[i]);
            }
        }

        if (countClaim > 0)
        {
            for (int x = 0; x < claimingMails.Count; x++)
            {
                ApiManager.instance.ClaimInbox (claimingMails[x].json.item_send_history_id);
                claimingMails[x].json.mail_claimed = 1;
                SetNotifStatus (claimingMails[x]);
            }
        }
        else if (countClaim == 0 && claimingMails.Count == 1)
        {
            ApiManager.instance.ClaimInbox (claimingMails[0].json.item_send_history_id);
            claimingMails[0].json.mail_claimed = 1;
            SetNotifStatus (claimingMails[0]);
        }

        SetReceiveStatus (selectedMail.json);

    }

    public void SetNotifStatus ( ItemMail itemMail )
    {
        if (itemMail.json.mail_read == 0 || itemMail.json.mail_claimed == 0)
            itemMail.objNotif.SetActive (true);
        else
            itemMail.objNotif.SetActive (false);
    }

    private bool CheckClaimable (JInbox _json )
    {
        if (_json.mail_claimed == 0 && _json.item_type_id != 0)
            return true;

        return false;
    }

    private void OnDelete ()
    {
        if (selectedMail.json.mail_claimed == 1)
        {
            ApiManager.instance.HideInbox (selectedMail.json.item_send_history_id);
            mails.Remove (selectedMail);
            ResetMail ();
            Destroy (selectedMail.gameObject);
            selectedMail = null;
        }
        else
        {
            MessageManager.instance.Show (this.gameObject, "Tidak bisa menghapus karena hadiah belum diambil");
        }
    }

    public void SetCanvas ( bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    private void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.PROFILE;
            isInit = true;
        }
        ResetMail ();
        StartCoroutine (_WaitSetJson ());

    }

    IEnumerator _WaitSetJson ()
    {
        //on loading
        while (isSettingJSON)
        {
            yield return _WFSUtility.wef;
        }
        //off loading

        SetData ();
        canvas.enabled = true;
        scrRect.verticalNormalizedPosition = 1f;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.INBOX;
    }

    private void SetData ()
    {
        for (int a = 0; a < parentMail.childCount; a++)
        {
            Destroy (parentMail.GetChild (a).gameObject);
        }

        mails = new List<ItemMail> ();
        json.inboxs = json.inboxs.OrderBy (x => x.mail_claimed == 0 ? 0 : 1).ThenBy (x => x.mail_read == 0 ? 0 : 1).ToArray ();
        for (int i = 0; i < json.inboxs.Length; i++)
        {
            ItemMail im = Instantiate (prefabItemMail, parentMail);
            im.SetData (json.inboxs[i]);
            mails.Add (im);
        }
    }

    public void SetJson (JGetInbox _json )
    {
        json = _json;

        for (int i = 0; i < json.inboxs.Length; i++)
        {
            if (json.inboxs[i].mail_read == 0 || json.inboxs[i].mail_claimed == 0)
            {
                HomeManager.instance.objNotifInbox.gameObject.SetActive (true);
                break;
            }
        }
        isSettingJSON = false;
    }

    private void Hide ()
    {
        HomeManager.instance.objNotifInbox.gameObject.SetActive (false);
        for (int i = 0; i < mails.Count; i++)
        {
            if (mails[i].json.mail_read == 0 || json.inboxs[i].mail_claimed == 0)
            {
                HomeManager.instance.objNotifInbox.gameObject.SetActive (true);
                break;
            }
        }
        ResetMail ();
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
        currentLength = mails.Count;
        ApiManager.instance.GetInbox ();
    }

    public void UnSelectMail ()
    {
        if (selectedMail != null)
        {
            selectedMail.imgFrame.sprite = sprNotSelected;
            selectedMail.txtTitle.color = Color.white;
        }

        ResetMail ();
        selectedMail = null;
    }

    private void ResetMail ()
    {
        txtTitle.text = string.Empty;
        txtDesc.text = string.Empty;
        txtSender.text = string.Empty;
        txtDeleteDate.text = string.Empty;
        for (int i = 0; i < rewards.Length; i++)
        {
            rewards[i].Reset ();
        }
        btnReceive.gameObject.SetActive (false);
        btnDelete.gameObject.SetActive (false);
    }

    public void OpenMail (JInbox _json, string strTitle )
    {
        txtTitle.text = strTitle;
        txtDesc.text = _json.mail_msg;
        txtSender.text = "Dari: " + _json.sender_name;
        txtDeleteDate.text = _json.when_deleted[1];
        if (_json.item_type_id != 0)
            rewards[0].SetData (_json.item_type_id, _json.item_id, _json.item_amount);
        else
            _json.mail_claimed = 1;

        SetReceiveStatus (_json);
    }

    private void SetReceiveStatus (JInbox _json)
    {
        if (_json.mail_claimed == 1)
        {
            rewards[0].objDimmer.SetActive (true);
            btnReceive.interactable = false;
            objDimmerBtnReceive.SetActive (true);
            txtReceive.text = "Diterima";
        }
        else
        {
            rewards[0].objDimmer.SetActive (false);
            btnReceive.interactable = true;
            objDimmerBtnReceive.SetActive (false);
            txtReceive.text = "Terima";
        }
    }

    public void Logout ()
    {
        isInit = false;
        isSettingJSON = true;
        currentLength = 0;
    }
}
