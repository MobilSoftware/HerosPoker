using Photon;
using UnityEngine;
using UnityEngine.UI;

public class SicboManager : PunBehaviour
{
    private static SicboManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static SicboManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (SicboManager)) as SicboManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an SicboManager object. \n You have to have exactly one SicboManager in the scene.");
            }
            return s_Instance;
        }
    }

    [HideInInspector]
    public SicboPlayer[] unsortedPlayers;   //LOCAL | [0] is myPlayer
    [HideInInspector]
    public SicboPlayer[] stockPlayers;  //LOCAL

    public void PrepareGame ()
    {
        InitMyPlayerProperties ();
        photonView.RPC (PhotonEnums.RPC.RequestSicboSlot, PhotonTargets.MasterClient);
    }

    private void InitMyPlayerProperties ()
    {
        PhotonPlayer player = PhotonNetwork.player;
        ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;
        properties[PhotonEnums.Player.Active] = false;
        properties[PhotonEnums.Player.NextRoundIn] = true;
        properties[PhotonEnums.Player.ReadyInitialized] = false;

        properties[PhotonEnums.Player.SlotIndex] = -1;

        //long _money = PlayerData.owned_coin;
        //long _buyIn = _money > GlobalVariables.MinBetAmount * 200 ? GlobalVariables.MinBetAmount * 200 : _money;
        //PlayerUtility.BuyInFromBankAccount (_buyIn);

        properties[PhotonEnums.Player.Money] = PlayerData.owned_coin;

        properties[PhotonEnums.Player.PlayerID] = PlayerData.id;

        //properties[PhotonEnums.Player.ValueHand] = 0;
        //properties[PhotonEnums.Player.SecondValueHand] = 0;

        //properties[PhotonEnums.Player.RankPoker] = 10;
        //properties[PhotonEnums.Player.Kicker] = 0;
        //properties[PhotonEnums.Player.SecondKicker] = 0;

        //properties[PhotonEnums.Player.TotalBet] = (long) 0;
        //properties[PhotonEnums.Player.ChipsBet] = (long) 0;

        player.SetCustomProperties (properties);
    }

    [PunRPC]
    void RPC_SicboRequestSlot ( PhotonMessageInfo info )
    {
        SyncSlots ();
        int _available_slot = GetAvailableRoomSlotIndex ();

        PhotonUtility.SetPlayerProperties (info.sender, PhotonEnums.Player.SlotIndex, _available_slot);
        photonView.RPC (PhotonEnums.RPC.ReturnSicboSlot, info.sender, _available_slot);
    }

    private void SyncSlots () //NEW FUNCTION Syncing Slot Available
    {
        if (PhotonNetwork.isMasterClient)
        {
            bool[] slots = PhotonUtility.GetRoomProperties<bool[]> (PhotonEnums.Room.Slots);
            for (int i = 0; i < slots.Length; i++)
                slots[i] = false;

            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                int slotIndex = PhotonUtility.GetPlayerProperties<int> (player, PhotonEnums.Player.SlotIndex);

                if (slotIndex != -1)
                    slots[slotIndex] = true;
            }

            PhotonUtility.SetRoomProperties (PhotonEnums.Room.Slots, slots);
        }
    }

    private int GetAvailableRoomSlotIndex ()
    {
        if (PhotonNetwork.room == null)
            return -1;

        //Make the next available slot occupied
        int slot_index = -1;
        bool[] slots = PhotonUtility.GetRoomProperties<bool[]> (PhotonEnums.Room.Slots);

        //debug.LogError("Slot Count : " + slots.Length);

        bool bFoundNewIndex = false;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == false)
            {
                slot_index = i;
                slots[i] = true;
                bFoundNewIndex = true;
                break;
            }
        }

        if (bFoundNewIndex)
            PhotonUtility.SetRoomProperties (PhotonEnums.Room.Slots, slots);

        return slot_index;
    }

    [PunRPC]
    void RPC_ReturnSlot ( int _mySlot )
    {
        int mySlot = _mySlot;

        if (mySlot >= 0)
        {
            SortingStockPlayers (mySlot);
            PhotonUtility.SetPlayerProperties (PhotonNetwork.player, PhotonEnums.Player.SlotIndex, mySlot);
        }

        if (PhotonNetwork.isMasterClient)
        {
            PhotonUtility.SetRoomProperties (PhotonEnums.Room.MasterClientID, PhotonNetwork.player.UserId);
        }
    }

    public void SortingStockPlayers ( int myIndex )
    {
        for (int x = 0; x < unsortedPlayers.Length; x++)
        {
            stockPlayers[myIndex] = unsortedPlayers[x];

            myIndex++;
            if (myIndex >= stockPlayers.Length)
                myIndex = 0;
        }
    }

    //bet timer up => submit record => round timer up => master calls api.StartSicbo() => chips go to dealer => myplayer.ResetBet() [records cleared here] => roll dice 3s or until RStartSicbo => myplayer.ShowWinLose() => myplayer.HideWinLose => next round

}
