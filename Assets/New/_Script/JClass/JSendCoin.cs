[System.Serializable]
public class JSendCoin
{
    public JTransfer transfer;
    public JHome player;
}

[System.Serializable]
public class JTransfer
{
    public int recipient_id;
    public string recipient_name;
    public string coin_amount;
    public string coin_limit;
}