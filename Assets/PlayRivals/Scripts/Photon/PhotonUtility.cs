#if PHOTON
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PhotonUtility : MonoBehaviour 
{
    public static string ParseString(ExitGames.Client.Photon.Hashtable data, string name)
    {
        if (data != null && data[name] != null)
        {
            if (data[name].GetType() != typeof(string))
            {
//                Debug.Log("Invalid type string and parse with : " + data[name].GetType());
                return "";
            }

            //print((data[name].GetType()));
            string strFinal = ((String)data[name]).Replace("&#x0040;", "@");
            return strFinal;
        }

        return "";
    }

    public static int ParseInt(ExitGames.Client.Photon.Hashtable data, string name)
    {
        int val = 0;
        if (data != null && data[name] != null)
        {
            if (int.TryParse(data[name].ToString(), out val))
                return int.Parse(data[name].ToString());
        }

        return 0;
    }

    public static long ParseLong(ExitGames.Client.Photon.Hashtable data, string name)
    {
        long val = 0;
        if (data != null && data[name] != null)
        {
            if (long.TryParse(data[name].ToString(), out val))
                return long.Parse(data[name].ToString());
        }

        return 0;
    }

    public static float ParseFloat(ExitGames.Client.Photon.Hashtable data, string name)
    {
        float val = 0.0f;
        if (data != null && data[name] != null)
        {
            if (float.TryParse(data[name].ToString(), out val))
                return val;
        }

        return val;
    }

    public static double ParseDouble(ExitGames.Client.Photon.Hashtable data, string name)
    {
        double val = 0.0;
        if (data != null && data[name] != null)
        {
            if (double.TryParse(data[name].ToString(), out val))
                return double.Parse(data[name].ToString());
        }

        return 0.0;
    }

    public static bool ParseBool(ExitGames.Client.Photon.Hashtable data, string name)
    {
        bool val = false;
        if (data != null && data[name] != null)
        {
            if (bool.TryParse(data[name].ToString(), out val))
                return bool.Parse(data[name].ToString());
        }

        return false;
    }

    public static int[] ParseIntArray(ExitGames.Client.Photon.Hashtable data, string name)
    {
        if (data != null && data[name] != null)
        {
            return (int[])data[name];
        }

        return null;
    }

    //Get or Set the index of player's properties
    public static T GetPlayerProperties<T>(PhotonPlayer player, string propertiesName)
    {
        object strVal = null;
        if (typeof(T) == typeof(string))
            strVal = "";
        else if (typeof(T) == typeof(int))
            strVal = 0;
        else if (typeof(T) == typeof(long))
            strVal = 0;
        else if (typeof(T) == typeof(double))
            strVal = 0.0;
        else if (typeof(T) == typeof(float))
            strVal = 0.0f;
        else if (typeof(T) == typeof(bool))
            strVal = false;
        else if (typeof(T) == typeof(int[]))
            strVal = new int[20];
        else if (typeof(T) == typeof(string[]))
            strVal = new string[20];
        else if (typeof(T) == typeof(Hashtable))
            strVal = new Hashtable();
        else
            strVal = null;

        if (player == null)
        {
            Debug.Log("PhotonPlayer is null");
            return (T)strVal;
        }

        ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;

        if (properties.ContainsKey(propertiesName))
        {
            strVal = properties[propertiesName];
            //Debug.Log("Money : " + strVal);
            try
            {
                return (T)strVal;
            }
            catch (Exception ex)
            {
                Debug.Log("Exception : " + ex.Message);
                Debug.Log("Type should be : " + strVal.GetType());
                throw new Exception();
            }
        }
        else
            return (T)strVal;
    }

    public static PhotonPlayer GetPhotonPlayerFromID(string userid)
    {

        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.UserId == userid)
                return player;
        }

        return null;
    }

    public static PhotonPlayer GetPhotonPlayerFromSlotIndex(int slot_index)
    {
        List<PhotonPlayer> pWithBot = new List<PhotonPlayer>();
        pWithBot.AddRange(PhotonNetwork.playerList);         

        foreach (PhotonPlayer player in pWithBot)
        {
            int curSlotIndex = GetPlayerProperties<int>(player, PhotonEnums.Player.SlotIndex);

            if (curSlotIndex == slot_index)
                return player;
        }
  

        return null;
    }

    public static void SetPlayerProperties(PhotonPlayer player, string propertiesName, object val)
    {
        if (player != null)
        {
            ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;
            properties[propertiesName] = val;
            player.SetCustomProperties(properties);
        }
    }

    public static void ClearPlayerProperties(PhotonPlayer player, List<string> excludedKeys = null)
    {
        //Need to set null to each entry to reset player's custom properties
        ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;
        List<string> keys = new List<string>();

        //slot_index
        foreach (DictionaryEntry entry in properties)
        {
            string strKey = entry.Key.ToString();
            keys.Add(strKey);
        }

        foreach (string strKey in keys)
        {
            if (excludedKeys == null || !excludedKeys.Contains(strKey))
                properties[strKey] = null;
        }

        player.SetCustomProperties(properties);
    }

    public static void AddToPlayerProperties(PhotonPlayer player, string propertiesName, string val, string seperation = ",")
    {
        if (player != null)
        {
            ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;
            properties[propertiesName] += val + seperation;
            player.SetCustomProperties(properties);
        }
    }

    public static void SetPlayerPropertiesArray(PhotonPlayer player, string[] propertiesName, object[] val)
    {
        if (player != null)
        {
            ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;

            for(int i=0; i<propertiesName.Length; i++)
                properties[propertiesName[i]] = val[i];

            player.SetCustomProperties(properties);
        }
    }

    //Get or Set the index of room's properties
    //public static object GetRoomProperties<T>(string propertiesName)
    public static T GetRoomProperties<T>(string propertiesName)
    {
        object strVal = null;

        if (typeof(T) == typeof(string))
            strVal = "";
        else if (typeof(T) == typeof(int))
            strVal = 0;
        else if (typeof(T) == typeof(double))
            strVal = 0.0;
        else if (typeof(T) == typeof(float))
            strVal = 0.0f;
        else if (typeof(T) == typeof(bool))
            strVal = false;
        else if (typeof(T) == typeof(bool[]))
            strVal = new bool[6];
        else if (typeof(T) == typeof(int[]))
            strVal = new int[6];
        else if (typeof(T) == typeof(string[]))
            strVal = new string[0];
        else if (typeof(T) == typeof(Hashtable))
            strVal = new Hashtable();
        else
            strVal = null;

        if (PhotonNetwork.room == null)
        {
            Debug.Log("You're not connected to photon room!");
            return (T)strVal;
        }

        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.room.CustomProperties;

        if (properties.ContainsKey(propertiesName))
            strVal = properties[propertiesName];

        try
        {
            return (T)strVal;
        }
        catch (Exception ex)
        {
            Debug.Log("Exception : " + ex.Message);
            Debug.Log("Type should be : " + strVal.GetType());
            throw new Exception();
        }
    }

    public static T GetRoomProperties<T>(RoomInfo room, string propertiesName)
    {
        if (room == null)
        {
            Debug.Log("You're not connected to photon network!");
        }

        ExitGames.Client.Photon.Hashtable properties = room.CustomProperties;
        object strVal = null;

        if (typeof(T) == typeof(string))
            strVal = "";
        else if (typeof(T) == typeof(int))
            strVal = 0;
        else if (typeof(T) == typeof(double))
            strVal = 0.0;
        else if (typeof(T) == typeof(float))
            strVal = 0.0f;
        else if (typeof(T) == typeof(bool))
            strVal = false;
        else if (typeof(T) == typeof(int[]))
            strVal = new int[20];
        else if (typeof(T) == typeof(string[]))
            strVal = new string[20];
        else if (typeof(T) == typeof(Hashtable))
            strVal = new Hashtable();
        else
            strVal = null;

        if (properties.ContainsKey(propertiesName))
            strVal = properties[propertiesName];

        return (T)strVal;
    }

    public static object GetRoomProperties(RoomInfo roomInfo, string propertiesName)
    {
        ExitGames.Client.Photon.Hashtable properties = roomInfo.CustomProperties;
        if (properties.ContainsKey(propertiesName))
            return properties[propertiesName];

        return null;
    }

    public static void SetRoomProperties(string propertiesName, object val)
    {
        if (PhotonNetwork.room == null)
        {
            Debug.Log("You're not connected to photon room!");
            return;
        }
        
        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.room.CustomProperties;
        properties[propertiesName] = val;
        PhotonNetwork.room.SetCustomProperties(properties);
    }

    public static int IncreaseRoomProperties(string propertiesName, int val)
    {
        if (PhotonNetwork.room == null)
            Debug.Log("You're not connected to photon room!");

        int newVal = GetRoomProperties<int>(propertiesName);
        newVal += val;
        SetRoomProperties(propertiesName, newVal);
        return newVal;
    }

    public static void SetRoomPropertiesArray(string[] propertiesName, object[] val)
    {
        if (PhotonNetwork.room == null)
        {
            Debug.Log("You're not connected to photon room!");
            return;
        }

        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.room.CustomProperties;

        for (int i = 0; i < propertiesName.Length; i++)
            properties[propertiesName[i]] = val[i];

        PhotonNetwork.room.SetCustomProperties(properties);
    }

    public static bool ComparePhotonPlayers(PhotonPlayer player01, PhotonPlayer player02)
    {
        return (player01.UserId == player02.UserId);
    }

}
#endif