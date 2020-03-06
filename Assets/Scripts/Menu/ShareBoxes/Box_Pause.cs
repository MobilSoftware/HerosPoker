using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Box_Pause : MonoBehaviour 
{
    public Text txtRoomTitle;
    public Button btnQuitGame, btnForceQuit, btnSwitchTable;
    private bool isFirst = true;

    private void Start()
    {
        btnQuitGame.onClick.AddListener (onQuitGame);
        btnForceQuit.onClick.AddListener(onQuitGame);
        btnSwitchTable.onClick.AddListener (onSwitchTable);
    }

    public void Show()
    {
        gameObject.SetActive (true);
        SoundManager.instance.PlaySFX(SFXType.SFX_PopupOpen, Vector3.zero);
        PokerManager.instance.uiThrowItem.Hide();

        //if (PhotonNetwork.room != null)
        //    txtRoomTitle.SetText(PhotonNetwork.room.Name, false);
        btnForceQuit.gameObject.SetActive(GlobalVariables.gameType == GameType.TexasPoker);
    }

    private void onQuitGame()
    {
        Debug.LogError ("Quit Game");
        if (GlobalVariables.gameType == GameType.TexasPoker)
        {
            if (PhotonNetwork.room != null || PhotonTexasPokerManager.instance.GetNumActivePlayers() <= 1)
            {
                Debug.LogError ("Quit Game 1");
                if (!PhotonUtility.GetPlayerProperties<bool> (PhotonNetwork.player, PhotonEnums.Player.Active))
                {
                    GlobalVariables.bQuitOnNextRound = false;
                    PhotonTexasPokerManager.instance.ImLeaving ();

                    Debug.LogError ("Quit Game 2");
                    //StartCoroutine (_LoadMenu ());
                    LoadMenu ();
                }
                else if (PhotonTexasPokerManager.instance != null && PhotonTexasPokerManager.instance.GetNumActivePlayers () > 1)
                {
                    Debug.LogError ("Quit Game55");
                    MessageManager.instance.Show (gameObject, "Keluar Ruangan", ButtonMode.OK_CANCEL, 1);
                }
                else
                {
                    GlobalVariables.bQuitOnNextRound = false;
                    PhotonTexasPokerManager.instance.ImLeaving ();
                    //StartCoroutine (_LoadMenu ());
                    LoadMenu ();
                }
            }
            else
            {
                //StartCoroutine(_LoadMenu());
                LoadMenu ();
            }
        }

        //Hide();
    }

    void onSwitchTable()
    {
        if (GlobalVariables.gameType == GameType.TexasPoker)
        {
            if (PhotonNetwork.room != null || PhotonTexasPokerManager.instance.GetNumActivePlayers() <= 1)
            {
                if (!PhotonUtility.GetPlayerProperties<bool> (PhotonNetwork.player, PhotonEnums.Player.Active))
                {
                    GlobalVariables.bSwitchTableNextRound = false;
                    PhotonTexasPokerManager.instance.ImLeaving ();
                    //StartCoroutine(_LoadSwitchTable());
                    LoadSwitchTable ();
                }
                else if (PhotonTexasPokerManager.instance != null && PhotonTexasPokerManager.instance.GetNumActivePlayers () > 1)
                    MessageManager.instance.Show (gameObject, "Apakah kamu yakin ingin pindah ke meja lain?", ButtonMode.OK_CANCEL, 2);
                else
                {
                    GlobalVariables.bSwitchTableNextRound = false;
                    PhotonTexasPokerManager.instance.ImLeaving();
                    //StartCoroutine(_LoadSwitchTable());
                    LoadSwitchTable ();
                }
            }
        }

        //Hide();
    }

    public void LoadMenu ()
    {
        Logger.E ("start load menu");

        _SceneManager.instance.SetActiveScene (SceneType.HOME, true);
        _SceneManager.instance.SetActiveScene (SceneType.POKER, false);

        Hide ();

        GlobalVariables.bInGame = false;
        if (GlobalVariables.gameType == GameType.TexasPoker)
        {
            PhotonTexasPokerManager.instance.StopAllCoroutines ();
            _PokerGameManager.instance.StopAllCoroutines ();

            _PokerGameHUD.instance.boxThrow.Hide ();
            _PokerGameHUD.instance.buyInHUD.Hide ();
            _PokerGameHUD.instance.Hide ();

            //yield return null;

            _PokerGameManager.instance.CleanEverything ();

            PhotonTexasPokerManager.instance.ClearPlayerProperties (PhotonNetwork.player);

            //yield return null;
            PokerData.Reset ();
        }

        PokerManager.instance.uiRoundRestart.Hide ();
        PokerManager.instance.uiWaitingPlayers.Hide ();
        PokerManager.instance.uiWaitingNextRound.Hide ();

        if (PhotonNetwork.room != null)
        {
            PhotonNetwork.LeaveRoom ();
            Debug.LogError ("leaving photon room");
        }

        PhotonRoomInfoManager.instance.RemoveCardGameScripts ();
    }

    public IEnumerator _LoadMenu()
    {
        Logger.E ("start load menu");

        _SceneManager.instance.SetActiveScene (SceneType.HOME, true);
        _SceneManager.instance.SetActiveScene (SceneType.POKER, false);

        Hide ();

        GlobalVariables.bInGame = false;
        if (GlobalVariables.gameType == GameType.TexasPoker)
        {
            PhotonTexasPokerManager.instance.StopAllCoroutines();
            _PokerGameManager.instance.StopAllCoroutines();

            _PokerGameHUD.instance.boxThrow.Hide();
            _PokerGameHUD.instance.buyInHUD.Hide();
            _PokerGameHUD.instance.Hide();

            //yield return null;

            _PokerGameManager.instance.CleanEverything();

            PhotonTexasPokerManager.instance.ClearPlayerProperties(PhotonNetwork.player);

            //yield return null;
            PokerData.Reset ();
        }

        PokerManager.instance.uiRoundRestart.Hide ();
        PokerManager.instance.uiWaitingPlayers.Hide ();
        PokerManager.instance.uiWaitingNextRound.Hide ();

        if (PhotonNetwork.room != null)
        {
            PhotonNetwork.LeaveRoom ();
            Debug.LogError ("leaving photon room");
        }

        PhotonRoomInfoManager.instance.RemoveCardGameScripts();

        yield return null;
        ////        Debug.Log("Going back to main menu!");
        ////LoginSceneManager.Instance.uiBusyIndicator.Show(true);

        ////HomeSceneManager.Instance.myHomeMenuReference.wholeUI.SetActive (true);
        ////CameraManager.instance.HideGameCamera();

        //SoundManager.instance.sfxSource.Stop();
        //SoundManager.instance.sfxSource2.Stop();


        //RoomInfoManager.instance.DeactiveGameEnvi();

        //HomeSceneManager.stockBot.Clear();

        //HomeSceneManager.Instance.myHomeMenuReference.uiHome.SetDataOffline();
        //HomeSceneManager.Instance.GetHome();
        //LoginSceneManager.Instance.uiBusyIndicator.Hide ();
    }

    public void LoadSwitchTable ()
    {
        GlobalVariables.bInGame = false;
        //LoginSceneManager.Instance.uiBusyIndicator.Show(true);

        PokerManager.instance.uiRoundRestart.Hide ();
        PokerManager.instance.uiWaitingPlayers.Hide ();
        PokerManager.instance.uiWaitingNextRound.Hide ();

        //SoundManager.instance.sfxSource.Stop ();
        //SoundManager.instance.sfxSource2.Stop ();


        if (GlobalVariables.gameType == GameType.TexasPoker)
        {
            PhotonTexasPokerManager.instance.StopAllCoroutines ();
            _PokerGameManager.instance.StopAllCoroutines ();

            _PokerGameHUD.instance.boxThrow.Hide ();
            _PokerGameHUD.instance.buyInHUD.Hide ();
            _PokerGameHUD.instance.Hide ();

            _PokerGameManager.instance.CleanEverything ();

            PhotonTexasPokerManager.instance.ClearPlayerProperties (PhotonNetwork.player);

            PokerData.Reset ();
        }

        PhotonRoomInfoManager.instance.RemoveCardGameScripts ();
        PhotonRoomInfoManager.instance.InitialiseCardGameScripts ();

        //HomeSceneManager.stockBot.Clear();

        //StartCoroutine (RoomInfoManager.instance._SwitchingRoom ());
        RoomInfoManager.instance.SwitchingRoom ();

        Hide ();
    }

    public IEnumerator _LoadSwitchTable()
    {
        GlobalVariables.bInGame = false;
        yield return null;
        //LoginSceneManager.Instance.uiBusyIndicator.Show(true);

        PokerManager.instance.uiRoundRestart.Hide ();
        PokerManager.instance.uiWaitingPlayers.Hide ();
        PokerManager.instance.uiWaitingNextRound.Hide ();

        SoundManager.instance.sfxSource.Stop();
        SoundManager.instance.sfxSource2.Stop();       

        Hide();

        if (GlobalVariables.gameType == GameType.TexasPoker)
        {
            PhotonTexasPokerManager.instance.StopAllCoroutines();
            _PokerGameManager.instance.StopAllCoroutines();

            _PokerGameHUD.instance.boxThrow.Hide();
            _PokerGameHUD.instance.buyInHUD.Hide();
            _PokerGameHUD.instance.Hide();

            _PokerGameManager.instance.CleanEverything();

            PhotonTexasPokerManager.instance.ClearPlayerProperties(PhotonNetwork.player);

            //HomeSceneManager.Instance.ResetPropertiesServer(GameType.TexasPoker);
        }

        PhotonRoomInfoManager.instance.RemoveCardGameScripts();
        PhotonRoomInfoManager.instance.InitialiseCardGameScripts();

        //HomeSceneManager.stockBot.Clear();

        //StartCoroutine(RoomInfoManager.instance._SwitchingRoom());
        RoomInfoManager.instance.SwitchingRoom ();
    }

    public void Hide()
    {
        gameObject.SetActive (false);
        SoundManager.instance.PlaySFX(SFXType.SFX_PopupClose, Vector3.zero, isFirst);
        isFirst = false;

    }

    private void OnPositiveClicked (int returnCode )
    {
        switch (returnCode)
        {
            case 1:
                GlobalVariables.bQuitOnNextRound = false;
                PhotonTexasPokerManager.instance.ImLeavingInTheMiddleOfTheGame ();
                LoadMenu ();
                break;
            case 2:
                GlobalVariables.bSwitchTableNextRound = false;
                PhotonTexasPokerManager.instance.ImLeavingInTheMiddleOfTheGame ();
                LoadSwitchTable ();
                break;
        }
    }
}
