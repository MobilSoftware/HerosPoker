using System;
using UnityEngine;

public class PlayerUtility{

    public static long GetPlayerCreditsLeft()
    {
        //long playerCredits = DataManager.instance.ownedGold;
        long playerCredits = PlayerData.owned_gold;
            
        return playerCredits;
    }

    public static void BuyInFromBankAccount(long val)
    {        
        if (GlobalVariables.bIsCoins)
        {
            //DataManager.instance.ownedGold = GetPlayerCreditsLeft () - val;
            PlayerData.owned_gold -= val;
        }
    }
}
