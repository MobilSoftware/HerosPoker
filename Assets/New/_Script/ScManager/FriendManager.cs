using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendManager : MonoBehaviour
{
    private static FriendManager s_Instance = null;
    public static FriendManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (FriendManager)) as FriendManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an FriendManager object. \n You have to have exactly one FriendManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Button btnClose;
    public Button btnSearch;
    public InputField ipfSearch;
    public ItemFriendList prefabItemFriendList;
    public ItemFriendReqMe prefabItemFriendReqMe;
    public ItemSearchPlayer prefabItemSearchPlayer;
    public Text txtTogFriendList;
    public Text txtTogFriendReqMe;
    public Text txtTogPlayerSearch;
    public Toggle togFriendList;
    public Toggle togFriendReqMe;
    public Toggle togPlayerSearch;
    public GameObject objFriendListContainer;
    public GameObject objFriendReqMeContainer;
    public GameObject objPlayerSearchContainer;
    public Transform parentFriendList;
    public Transform parentFriendReqMe;
    public Transform parentPlayerSearch;

    private bool isInit;
    [HideInInspector]
    public bool isSettingJsonFriendList = true;
    [HideInInspector]
    public bool isSettingJsonFriendReqMe = true;
    private SceneType prevSceneType;
    private JFriend[] listFriends;
    private JFriend[] reqMeFriends;
    private JFriend[] searchPlayers;
    private List<ItemFriendList> itemListFriends;
    private List<ItemFriendReqMe> itemReqMeFriends;
    private List<ItemSearchPlayer> itemSearchPlayers;
    private string strSearch;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
        btnSearch.onClick.AddListener (OnSearch);
        ipfSearch.onEndEdit.AddListener (OnEndEdit);
        togFriendList.onValueChanged.AddListener (OnToggle);
        togFriendReqMe.onValueChanged.AddListener (OnToggle);
        togPlayerSearch.onValueChanged.AddListener (OnToggle);
    }

    private void OnSearch ()
    {
        ApiManager.instance.SearchFriend (strSearch);
    }

    private void OnEndEdit (string strIpf )
    {
        strSearch = strIpf;
    }

    private void OnToggle (bool val )
    {
        if (val)
        {
            if (togFriendList.isOn)
            {
                ResetSearchPlayerTab ();
                OpenFriendListTab ();
            }
            else if (togFriendReqMe.isOn)
            {
                ResetSearchPlayerTab ();
                OpenFriendReqMeTab ();
            } else if (togPlayerSearch.isOn)
            {
                OpenPlayerSearchTab ();
            }
        }
    }

    public void OpenFriendListTab ()
    {
        togFriendList.image.sprite = ShopManager.instance.sprToggleOn;
        togFriendReqMe.image.sprite = ShopManager.instance.sprToggleOff;
        togPlayerSearch.image.sprite = ShopManager.instance.sprToggleOff;
        txtTogFriendList.color = ShopManager.instance.colTextToggleOn;
        txtTogFriendReqMe.color = ShopManager.instance.colTextToggleOff;
        txtTogPlayerSearch.color = ShopManager.instance.colTextToggleOff;
        objFriendListContainer.SetActive (true);
        objFriendReqMeContainer.SetActive (false);
        objPlayerSearchContainer.SetActive (false);
    }

    public void OpenFriendReqMeTab ()
    {
        togFriendList.image.sprite = ShopManager.instance.sprToggleOff;
        togFriendReqMe.image.sprite = ShopManager.instance.sprToggleOn;
        togPlayerSearch.image.sprite = ShopManager.instance.sprToggleOff;
        txtTogFriendList.color = ShopManager.instance.colTextToggleOff;
        txtTogFriendReqMe.color = ShopManager.instance.colTextToggleOn;
        txtTogPlayerSearch.color = ShopManager.instance.colTextToggleOff;
        objFriendListContainer.SetActive (false);
        objFriendReqMeContainer.SetActive (true);
        objPlayerSearchContainer.SetActive (false);

    }

    public void OpenPlayerSearchTab ()
    {
        togFriendList.image.sprite = ShopManager.instance.sprToggleOff;
        togFriendReqMe.image.sprite = ShopManager.instance.sprToggleOff;
        togPlayerSearch.image.sprite = ShopManager.instance.sprToggleOn;
        txtTogFriendList.color = ShopManager.instance.colTextToggleOff;
        txtTogFriendReqMe.color = ShopManager.instance.colTextToggleOff;
        txtTogPlayerSearch.color = ShopManager.instance.colTextToggleOn;
        objFriendListContainer.SetActive (false);
        objFriendReqMeContainer.SetActive (false);
        objPlayerSearchContainer.SetActive (true);
    }

    public void SetCanvas (bool val )
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
            isInit = true;
            canvas.sortingOrder = (int) SceneType.FRIEND;
            StartCoroutine (_WaitSetJson ());
        }

        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.FRIEND;
    }

    private void Hide ()
    {
        ResetSearchPlayerTab ();
        UpdateAllFriendTypes ();
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }

    private void ResetSearchPlayerTab ()
    {
        DeleteSearchResult ();
        ipfSearch.text = string.Empty;
        strSearch = string.Empty;
    }

    public void SetJson ( JGetFriend json, ApiBridge.FriendType friendType )
    {
        switch (friendType)
        {
            case ApiBridge.FriendType.FriendList:
                isSettingJsonFriendList = true;
                listFriends = json.friends;
                isSettingJsonFriendList = false;
                break;
            case ApiBridge.FriendType.FriendRequestMe:
                isSettingJsonFriendReqMe = true;
                reqMeFriends = json.friends;
                isSettingJsonFriendReqMe = false;
                break;
        }
    }

    IEnumerator _WaitSetJson ()
    {
        while (isSettingJsonFriendList || isSettingJsonFriendReqMe)
        {
            yield return _WFSUtility.wef;
        }

        SetItemFriendList ();
        yield return _WFSUtility.wef;
        SetItemFriendReqMe ();
    }

    private void SetItemFriendList ()
    {
        if (listFriends == null)
            return;

        if (parentFriendList.childCount > 0)
        {
            for (int a = 0; a < parentFriendList.childCount; a++)
            {
                Destroy (parentFriendList.GetChild (a).gameObject);
            }
        }

        itemListFriends = new List<ItemFriendList> ();
        for (int i = 0; i < listFriends.Length; i++)
        {
            ItemFriendList ifl = Instantiate (prefabItemFriendList, parentFriendList);
            ifl.SetData (listFriends[i]);
            itemListFriends.Add (ifl);
        }
    }

    private void SetItemFriendReqMe ()
    {
        if (reqMeFriends == null)
            return;

        itemReqMeFriends = new List<ItemFriendReqMe> ();

        if (parentFriendReqMe.childCount > 0)
        {
            for (int a = 0; a < parentFriendReqMe.childCount; a++)
            {
                Destroy (parentFriendReqMe.GetChild (a).gameObject);
            }
        }

        for (int i = 0; i < reqMeFriends.Length; i++)
        {
            ItemFriendReqMe ifrm = Instantiate (prefabItemFriendReqMe, parentFriendReqMe);
            ifrm.SetData (reqMeFriends[i]);
            itemReqMeFriends.Add (ifrm);
        }
    }

    public void ShowSearchResult (JFriend[] players )
    {
        DeleteSearchResult ();
        for (int i = 0; i < players.Length; i++)
        {
            ItemSearchPlayer isp = Instantiate (prefabItemSearchPlayer, parentPlayerSearch);
            isp.SetData (players[i]);
        }
    }

    private void DeleteSearchResult ()
    {
        for (int a = 0; a < parentPlayerSearch.childCount; a++)
        {
            Destroy (parentPlayerSearch.GetChild (a).gameObject);
        }
    }

    private void UpdateAllFriendTypes ()
    {
        Logout ();
        ApiManager.instance.GetFriend ();
    }

    public void Logout ()
    {
        isInit = false;
        isSettingJsonFriendList = true;
        isSettingJsonFriendReqMe = true;
    }

    public void AddAcceptedFriend(JFriend json)
    {
        ItemFriendList ifl = Instantiate (prefabItemFriendList, parentFriendList);
        ifl.SetData (json);
        itemListFriends.Add (ifl);
    }

    public void RemoveFriend (int playerID )
    {
        ItemFriendList toRemove = null;
        for (int i = 0; i < itemListFriends.Count; i++)
        {
            if (itemListFriends[i].playerID == playerID)
            {
                toRemove = itemListFriends[i];
                break;
            }
        }
        if (toRemove != null)
        {
            itemListFriends.Remove (toRemove);
            Destroy (toRemove.gameObject);
        }
    }
}
