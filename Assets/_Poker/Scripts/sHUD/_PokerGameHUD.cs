using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class _PokerGameHUD : MonoBehaviour
{
    public static _PokerGameHUD instance;
    public _BuyInHUD buyInHUD;
    public ThrowItemUI boxThrow;

    public Text bidTxt;
    public Sprite coinSprite, gemSprite;

    //public Button[] inviteButtons = new Button[6];
    public Button btnChat;
    public Sprite sprClosed;
    public Sprite sprOpened;

    public Image imgConnection, iconCoin;
    public Sprite connGood, connNormal, connLow, connVeryLow;
    public Text txtSpeed;

    private ConnectionStrength connectionStrength;
    private int lowConnectionCount = 0;

    private MenuOrBoxType prevMenuOrBox;

    public Text locationTxt, _roomText;

    private void Awake()
    {
        instance = this;
        btnChat.onClick.AddListener(onChat);
    }

    private void Start()
    {
        //transform.parent.gameObject.SetActive(false);
        Hide();
    }

    public void CheckConnection()
    {
        if (!PhotonNetwork.connected)
        {
            imgConnection.sprite = connVeryLow;
            lowConnectionCount++;

            if (GlobalVariables.bInGame)
            {
                SceneManager.LoadScene("Menu");
                MenuManager.instance.uiMessageBox.Show(null, "ID_PhotonDisconnectedMessage");

                //int widthR = HomeSceneManager.Instance.fsResolution.width;
                //int heightR = HomeSceneManager.Instance.fsResolution.height;
                //Screen.SetResolution (widthR, heightR, true);
            }

            CancelInvoke("CheckConnection");
            return;
        }

        int roundTripTime = PhotonNetwork.networkingPeer.RoundTripTime;
        txtSpeed.text = roundTripTime + " ms";

        if (roundTripTime > 800)
            connectionStrength = ConnectionStrength.NotPlayable;
        else if (roundTripTime > 300)
            connectionStrength = ConnectionStrength.VeryLow;
        else if (roundTripTime > 200)
            connectionStrength = ConnectionStrength.Low;
        else if (roundTripTime > 150)
            connectionStrength = ConnectionStrength.Normal;
        else if (roundTripTime > 100)
            connectionStrength = ConnectionStrength.Good;
        else
            connectionStrength = ConnectionStrength.Excellent;

        if (connectionStrength == ConnectionStrength.NotPlayable)
        {
            PlayerPrefs.SetString("LastConnection", "low");

            imgConnection.sprite = connVeryLow;
            lowConnectionCount++;
        }
        else if (connectionStrength == ConnectionStrength.VeryLow)
        {
            PlayerPrefs.SetString("LastConnection", "low");

            imgConnection.sprite = connLow;
            lowConnectionCount++;
        }
        else if (connectionStrength == ConnectionStrength.Low || connectionStrength == ConnectionStrength.Normal)
        {
            PlayerPrefs.SetString("LastConnection", "normal");

            imgConnection.sprite = connNormal;
            lowConnectionCount = 0;
        }
        else
        {
            PlayerPrefs.SetString("LastConnection", "normal");

            imgConnection.sprite = connGood;
            lowConnectionCount = 0;
        }

        PlayerPrefs.Save();

        if (lowConnectionCount >= 15 && GlobalVariables.bInGame)
        {
            GlobalVariables.bQuitOnNextRound = false;
            StartCoroutine(MenuManager.instance.uiPause.LoadMenu ());
            MenuPhotonNetworkManager.instance.Disconnect();
            lowConnectionCount = 0;
        }
    }

    //public void ActivateAllInvite()
    //{
    //    foreach (Button i in inviteButtons)
    //        i.gameObject.SetActive(true);
    //}

    public void Show()
    {
        gameObject.SetActive(true);
        SetupMenu();

        _PokerGameManager.instance.fxTableSpotlite.DeactiveSpot();
        _PokerGameManager.instance.chipD.SetActive(false);

        //HomeSceneManager.Instance.myHomeMenuReference.uiMyCouponPoker.SetMinimumValue ();
        SoundManager.instance.ChangeBackgroundMusic(1);
    }

    public void Hide()
    {
        CancelInvoke("CheckConnection");
        gameObject.SetActive(false);
        SoundManager.instance.ChangeBackgroundMusic(0);
    }

    public void SetupMenu()
    {

        //imgLocation.sprite = RoomInfoManager.instance.spriteCity[(int)GlobalVariables.environment];
        locationTxt.text = "PROTOTYPE";

        _roomText.text = "#" + PhotonNetwork.room.Name;
        bidTxt.text = GlobalVariables.bIsCoins ? GlobalVariables.MinBetAmount.toShortCurrency() : GlobalVariables.MinBetAmount.toGemShortCurrency();

        iconCoin.sprite = GlobalVariables.bIsCoins ? coinSprite : gemSprite;

        InvokeRepeating("CheckConnection", 0.0f, 2.0f);
    }

    public void onPause() { MenuManager.instance.uiPause.Show(); }

    public void onInvite()
    {

    }

    public void SitDown()
    {
        PhotonTexasPokerManager.instance.RequestToSit();
    }

    public void StandUp()
    {


    }

    private void onChat()
    {

    }

    public void OnMission ()
    {
        //if (HomeSceneManager.Instance.myHomeMenuReference.uiMyCouponPoker.gameObject.activeSelf)
        //{
        //    HomeSceneManager.Instance.myHomeMenuReference.uiMyCouponPoker.Hide ();
        //}
        //else
        //{
        //    HomeSceneManager.Instance.myHomeMenuReference.uiMyCouponPoker.Show ();
        //    imgBgMission.sprite = sprOpened;
        //}
    }

    public void ShowPanelThrow(Transform _to, _PlayerPokerActor _target)
    {
        boxThrow.SetupMenuPoker(_to, _target._myParasitePlayer.IDX_READONLY);
    }
}
