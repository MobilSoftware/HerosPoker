using UnityEngine;
using UnityEngine.UI;

public class _PotObject : MonoBehaviour
{
    public Text txtPot;
    public _PlayerPokerActor[] owner;

    long moneyPot;
    public long GetMoney
    {
        get
        {
            return moneyPot;
        }
    }

    public void SetPotOwner(_PlayerPokerActor[] _owner)
    {
        owner = _owner;
    }

    public void SetPotValue(long _val)
    {
        moneyPot = _val;
        txtPot.text = moneyPot.toFlexibleCurrency();
    }

    public void AddMoneyToPot(long _val)
    {
        moneyPot += _val;
        txtPot.text = moneyPot.toFlexibleCurrency();
    }

    public void GiveThePotToWinner()
    {
        if (owner.Length == 0)
            return;

        int biggestRank = owner[0].RANK;
        System.Collections.Generic.List<_PlayerPokerActor> pWinner = new System.Collections.Generic.List<_PlayerPokerActor>();

        for (int x = 0; x < owner.Length; x++)
            if (owner[x].RANK <= biggestRank)
                biggestRank = owner[x].RANK;

        for (int a = 0; a < owner.Length; a++)
            if (owner[a].RANK == biggestRank)
                pWinner.Add(owner[a]);

        long cut = System.Convert.ToInt64(moneyPot / pWinner.Count);

        for (int w = 0; w < pWinner.Count; w++)
            pWinner[w].PullTheChips(cut);
    }

    public string GetOwner()
    {
        string slotsOwner = "";
        for (int x = 0; x < _PokerGameManager.turnManager.currentPlayers.Count; x++)
        {
            if (x != _PokerGameManager.turnManager.currentPlayers.Count - 1)
                slotsOwner += _PokerGameManager.turnManager.currentPlayers[x].slotIndex + ",";
            else
                slotsOwner += _PokerGameManager.turnManager.currentPlayers[x].slotIndex;
        }

        return slotsOwner;
    }
}
