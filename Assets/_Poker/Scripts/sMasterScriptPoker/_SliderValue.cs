using System;
public class _SliderValue
{
    // Update is called once per frame
    public long CalculateScore(long myMoney, long lastBet, float val) //Value Slider = 0.43
    {
        if (myMoney < lastBet * 2)
            lastBet = Convert.ToInt64((myMoney/2));

        if (val == 0)
            return lastBet == 0 ? _PokerGameManager.startBet : lastBet * 2; // 200
        else if (val == 1)
            return myMoney; //9800 ALL IN

        if (lastBet == 0)
            lastBet = _PokerGameManager.startBet;

        long valueRaiseMax = myMoney - (lastBet * 2); //9800-200 = 9600
        int countSlider =(int) (valueRaiseMax / 10); //9600/10 = 960
        float valueSlider = 1f / (float)countSlider; // 1/960 = 0.01 PEMBULATAN
        long RAISE = Convert.ToInt64(val / valueSlider); //0.43/0.01 = 43

        RAISE = (lastBet * 2) + (RAISE *10); //200 + 43*100  //10 adalah multiply value tiap naikin slider biar angkanya ga satuan

        if (RAISE > myMoney)
            RAISE = myMoney;

        return RAISE;
    }
}
