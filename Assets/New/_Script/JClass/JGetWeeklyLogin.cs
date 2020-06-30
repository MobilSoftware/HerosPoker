[System.Serializable]
public class JGetWeeklyLogin
{
    public JWeeklyReward[] rewards;
    public JWeeklyBonus[] bonus;
    public int today;
    public int login_count;
    public JDailyReward reward;
    public JDailyReward bonus_reward;
    public JHome player;
}

[System.Serializable]
public class JWeeklyReward
{
    public int day_of_week;
    public JDailyReward reward;
}

[System.Serializable]
public class JWeeklyBonus
{
    public int daily_login_count;
    public int item_type;
    public int item_id;
    public string[] item_name;
    public string item_amount;
}