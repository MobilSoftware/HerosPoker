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
        SePokerManager.instance.uiThrowItem.Hide();

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
                    StartCoroutine (LoadMenu ());
                }
                else if (PhotonTexasPokerManager.instance != null && PhotonTexasPokerManager.instance.GetNumActivePlayers () > 1)
                {

                    Debug.LogError ("Quit Game55");
                    SePokerManager.instance.uiMessageBox.Show (this.gameObject, "ID_QuitRoom", MessageBoxType.OK_CANCEL, 1, true);
                }
                else
                {
                    GlobalVariables.bQuitOnNextRound = false;
                    PhotonTexasPokerManager.instance.ImLeaving ();
                    StartCoroutine (LoadMenu ());
                }
            }
            else
            {
                StartCoroutine(LoadMenu());
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
                if (!PhotonUtility.GetPlayerProperties<bool>(PhotonNetwork.player, PhotonEnums.Player.Active))
                {
                    GlobalVariables.bSwitchTableNextRound = false;
                    PhotonTexasPokerManager.instance.ImLeaving();
                    StartCoroutine(LoadSwitchTable());
                }
                else if (PhotonTexasPokerManager.instance != null && PhotonTexasPokerManager.instance.GetNumActivePlayers() > 1)
                    SePokerManager.instance.uiMessageBox.Show(gameObject, "ID_ConfirmSwitchTable", MessageBoxType.OK_CANCEL, 2, true);
                else
                {
                    GlobalVariables.bSwitchTableNextRound = false;
                    PhotonTexasPokerManager.instance.ImLeaving();
                    StartCoroutine(LoadSwitchTable());
                }
            }
        }

        Hide();
    }

    public IEnumerator LoadMenu()
    {
        Logger.E ("start load menu");

        //_SceneManager.instance.SetActiveSceneByIndex (1);
        _SceneManager.instance.SetActiveMenu ();
        yield return null;

        //GlobalVariables.bInGame = false;
        //yield return null;
        ////        Debug.Log("Going back to main menu!");
        ////LoginSceneManager.Instance.uiBusyIndicator.Show(true);

        //Box_RoundRestart.instance.Hide();
        ////HomeSceneManager.Instance.myHomeMenuReference.wholeUI.SetActive (true);
        ////CameraManager.instance.HideGameCamera();

        //SoundManager.instance.sfxSource.Stop();
        //SoundManager.instance.sfxSource2.Stop();

        //Box_WaitingPlayers.instance.Hide();
        //Box_WaitingNextRound.instance.Hide();

        //Hide();

        //if (GlobalVariables.gameType == GameType.TexasPoker)
        //{
        //    PhotonTexasPokerManager.instance.StopAllCoroutines();
        //    _PokerGameManager.instance.StopAllCoroutines();

        //    _PokerGameHUD.instance.boxThrow.Hide();
        //    _PokerGameHUD.instance.buyInHUD.Hide();
        //    _PokerGameHUD.instance.Hide();

        //    _PokerGameManager.instance.CleanEverything();

        //    PhotonTexasPokerManager.instance.ClearPlayerProperties(PhotonNetwork.player);
        //    //HomeSceneManager.Instance.ResetPropertiesServer(GameType.TexasPoker);
        //}

        //PhotonRoomInfoManager.instance.RemoveCardGameScripts();

        //if (PhotonNetwork.room != null)
            //PhotonNetwork.LeaveRoom();


        //RoomInfoManager.instance.DeactiveGameEnvi();

        //HomeSceneManager.stockBot.Clear();

        //HomeSceneManager.Instance.myHomeMenuReference.uiHome.SetDataOffline();
        //HomeSceneManager.Instance.GetHome();
        //LoginSceneManager.Instance.uiBusyIndicator.Hide ();
    }

    public IEnumerator LoadSwitchTable()
    {
        GlobalVariables.bInGame = false;
        yield return null;
        //LoginSceneManager.Instance.uiBusyIndicator.Show(true);

        SePokerManager.instance.uiRoundRestart.Hide ();
        SePokerManager.instance.uiWaitingPlayers.Hide ();
        SePokerManager.instance.uiWaitingNextRound.Hide ();

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

        StartCoroutine(RoomInfoManager.instance.SwitchingRoom());
    }

    public void Hide()
    {
        gameObject.SetActive (false);
        SoundManager.instance.PlaySFX(SFXType.SFX_PopupClose, Vector3.zero, isFirst);
        isFirst = false;

    }

    private void onMessageBoxOKClicked(int returnedCode)
    {
        Debug.LogError ("Quit Game 100");
        Debug.LogError (returnedCode);
        if (returnedCode == 1)
        {
            GlobalVariables.bQuitOnNextRound = false;
            PhotonTexasPokerManager.instance.ImLeavingInTheMiddleOfTheGame();
            StartCoroutine(LoadMenu());

            Hide ();
        }
        else if (returnedCode == 2)
        {
            GlobalVariables.bSwitchTableNextRound = false;
            PhotonTexasPokerManager.instance.ImLeavingInTheMiddleOfTheGame();
            StartCoroutine(LoadSwitchTable());

            Hide ();
        }
    }
}
