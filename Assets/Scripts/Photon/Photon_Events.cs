using System;
using UnityEngine.Networking.Match;

namespace PlayRivals
{
    public class Photon_Events
    {
        public static Action<bool> onPhotonNetworkConnected;
        public static Action<bool> onJoinedRoom;
        public static Action<bool> onCreatedRoom;
        public static Action<bool> onLeftRoom;		

        public static Action<string> operationResponseEvent; // in the NetworkingPeer.cs Operation failed:

        public static Action<PhotonPlayer> onPhotonPlayerConnected;
        public static Action<PhotonPlayer> onPhotonPlayerDisconnected;
        public static Action<bool> onFailedJoinRoom;
        public static Action<bool> onFailedCreateRoom;
        public static Action<bool> onFailedJoinRandomRoom;
        public static Action<bool> onGameCancelled;
        public static Action<bool> onGamePreparing;
        public static Action<bool> onGameStarted;

        public static Action<PhotonPlayer[]> onPhotonPlayerPropertiesChanged;

        public static void FirePhotonNetworkConnectedEvent(bool val)
        {
//            Debug.Log("FirePhotonNetworkConnectedEvent Event");
            if (onPhotonNetworkConnected != null)
                onPhotonNetworkConnected(val);
        }

        public static void FireOnJoinedRoomEvent(bool val)
        {
            if (onJoinedRoom != null)
                onJoinedRoom(val);
        }

        public static void FireOnLeftRoomEvent(bool val)
        {
            if (onLeftRoom != null)
                onLeftRoom(val);
        }

        public static void FireOnPhotonPlayerConnectedEvent(PhotonPlayer val)
        {
            if (onPhotonPlayerConnected != null)
                onPhotonPlayerConnected(val);
        }

        public static void FireOnPhotonPlayerDisconnectedEvent(PhotonPlayer val)
        {
            if (onPhotonPlayerDisconnected != null)
                onPhotonPlayerDisconnected(val);
        }

        public static void FireOnCreatedRoomEvent(bool val)
        {
            if (onCreatedRoom != null)
                onCreatedRoom(val);
        }

        public static void FireOperationResponseEvent(string val)
        {
            if (operationResponseEvent != null)
                operationResponseEvent(val);
        }

        public static void FireOnFailedCreateRoomEvent(bool val)
        {
            if (onFailedCreateRoom != null)
                onFailedCreateRoom(val);
        }

        public static void FireOnFailedJoinRoomEvent(bool val)
        {
            if (onFailedJoinRoom != null)
                onFailedJoinRoom(val);
        }

        public static void FireOnFailedJoinRandomRoomEvent(bool val)
        {
            if (onFailedJoinRandomRoom != null)
                onFailedJoinRandomRoom(val);
        }

        public static void FireOnGameStartedEvent(bool val)
        {
            if (onGameStarted != null)
                onGameStarted(val);
        }

        public static void FireOnGameCancelledEvent(bool val)
        {
            if (onGameCancelled != null)
                onGameCancelled(val);
        }

        public static void FireOnGamePreparingEvent(bool val)
        {
            if (onGamePreparing != null)
                onGamePreparing(val);
        }

        public static void FirePhotonPlayerPropertiesChangedEvent(PhotonPlayer[] playerList)
        {
            if (onPhotonPlayerPropertiesChanged != null)
                onPhotonPlayerPropertiesChanged(playerList);
        }
    }
}
