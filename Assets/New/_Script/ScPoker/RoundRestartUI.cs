using UnityEngine;
using System.Collections;
using TMPro;

public class RoundRestartUI : MonoBehaviour 
{

    public TextMeshProUGUI txtTimer;

    public void Show()
    {
        gameObject.SetActive (true);
        //Only call it for one time since show is sending two time to the round restart

        if (PhotonNetwork.isMasterClient)
        {
            if (GlobalVariables.gameType == GameType.TexasPoker)
                SetupPoker();
        }

        //Don't allow any other players to be in this round
        PhotonPlayer[] _bots = PhotonTexasPokerManager.instance.bots.ToArray();
        foreach (PhotonPlayer bot in _bots)
        {
            if (PhotonUtility.GetPlayerProperties<bool>(bot, PhotonEnums.Player.ReadyInitialized))
                PhotonUtility.SetPlayerPropertiesArray(bot, new string[] { PhotonEnums.Player.Active, PhotonEnums.Player.NextRoundIn }, new object[] { true, true });
            else
                PhotonUtility.SetPlayerPropertiesArray(bot, new string[] { PhotonEnums.Player.Active, PhotonEnums.Player.NextRoundIn }, new object[] { false, false });
        }

        //Reset bet money to calculate giftpoints
        GlobalVariables.playerBetMoney = 0;

        if (PhotonUtility.GetPlayerProperties<bool>(PhotonNetwork.player, PhotonEnums.Player.ReadyInitialized))
            PhotonUtility.SetPlayerPropertiesArray(PhotonNetwork.player, new string[] { PhotonEnums.Player.Active, PhotonEnums.Player.NextRoundIn }, new object[] { true, true });
        else
            PhotonUtility.SetPlayerPropertiesArray(PhotonNetwork.player, new string[] { PhotonEnums.Player.Active, PhotonEnums.Player.NextRoundIn }, new object[] { false, false});

        if (GlobalVariables.gameType == GameType.TexasPoker)
            StartCoroutine(StartRoundTimerPoker());
    }

    private IEnumerator StartRoundTimerPoker()
    {
        PhotonTexasPokerManager.msgDelayPoker = "Start Round";

        txtTimer.SetText("", false);
        PokerManager.instance.uiWaitingNextRound.Hide ();
        PokerManager.instance.uiWaitingPlayers.Hide ();

        yield return _WFSUtility.wfs1;

        if (PhotonTexasPokerManager.instance.GetNumActivePlayers() < 2)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonTexasPokerManager.instance.ForceStopTheMatch();
                yield break;
            }
        }

        txtTimer.SetText("", false);
        SoundManager.instance.PlaySFX(SFXType.SFX_Timer, Vector3.zero);
        txtTimer.SetText("3", false);
        yield return _WFSUtility.wfs1;

        txtTimer.SetText("2", false);
        yield return _WFSUtility.wfs1;

        txtTimer.SetText("1", false);
        yield return _WFSUtility.wfs1;
        SoundManager.instance.PlaySFX(SFXType.SFX_RoundStart, Vector3.zero);
        txtTimer.SetText("GO", false);

        yield return null;

        if (PhotonNetwork.isMasterClient)
            CallStartRoundPoker();

        Hide();
    }

    void CallStartRoundPoker()
    {
        if (PhotonTexasPokerManager.instance.GetNumActivePlayers() > 1)
            PhotonTexasPokerManager.instance.StartRound();
        else
            PhotonTexasPokerManager.instance.ForceStopTheMatch();
    }

    void SetupPoker()
    {
        //int[] playerIDs = PhotonTexasPokerManager.instance.GetPlayerIDs(PhotonEnums.Player.ReadyInitialized);
        //Debug.LogError("HOI : " +playerIDs.Length);

        ApiManager.instance.pokerPlayers = PhotonTexasPokerManager.instance.GetPokerPlayers (PhotonEnums.Player.ReadyInitialized);
        ApiManager.instance.StartPoker (PhotonNetwork.room.Name, GlobalVariables.MinBetAmount);
    }

    public void Hide ()
    {
        gameObject.SetActive (false);
    }


}
