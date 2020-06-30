[System.Serializable]
public class JGetDailyLogin
{
    public JDailyReward reward;
    public int today;
    public JHome player;
}

[System.Serializable]
public class JDailyReward
{
    public int item_type;
    public int item_id;
    public string item_amount;
    public string[] item_name;
}