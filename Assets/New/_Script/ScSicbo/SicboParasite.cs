using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SicboParasite : Photon.MonoBehaviour
{
    [HideInInspector]
    public SicboPlayer uiPlayer;
    [HideInInspector]
    public string nickname;


    public void Initiate ()
    {
        photonView.RPC ("RPC_Initiate", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    private void RPC_Initiate ()
    {
        int ownerSlotIndex = PhotonUtility.GetPlayerProperties<int> (this.photonView.owner, PhotonEnums.Player.SlotIndex);
        uiPlayer = SicboManager.instance.stockPlayers[ownerSlotIndex];
        uiPlayer.gameObject.SetActive (true);
        uiPlayer.parasite = this;
        uiPlayer.photonPlayer = this.photonView.owner;
        nickname = this.photonView.owner.NickName;

        transform.SetParent (uiPlayer.transform);
        transform.localScale = new Vector3 (1, 1, 1);
        transform.localPosition = new Vector3 (0, 0, 0);

        uiPlayer.Initiate (ownerSlotIndex, photonView.isMine);
        if (photonView.isMine)
        {
            Logger.W ("is mine set player properties array");
            PhotonUtility.SetPlayerPropertiesArray (uiPlayer.photonPlayer, new string[] { PhotonEnums.Player.ReadyInitialized, PhotonEnums.Player.NextRoundIn }, new object[] { true, true });
        }

    }

    public void SetSicboBets (string strBets )
    {
        PhotonUtility.SetPlayerProperties (uiPlayer.photonPlayer, PhotonEnums.Player.Money, PlayerData.owned_coin);
        Logger.E ("parasite strBets length: " + strBets.Length);
        PhotonUtility.SetPlayerProperties (uiPlayer.photonPlayer, PhotonEnums.Player.SICBO_BETS, strBets);
    }

    public void InitiateSync (PhotonPlayer newPlayer)
    {
        photonView.RPC ("RPC_InitiateSync", newPlayer);
    }

    [PunRPC]
    void RPC_InitiateSync ()
    {
        Initiate ();
    }
}
