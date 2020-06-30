[System.Serializable]
public class JGetInbox
{
    public JInbox[] inboxs;
}

[System.Serializable]
public class JInbox
{
    public int item_send_history_id;
    public int item_type_id;
    public int item_id;
    public string item_amount;
    public string[] item_name;
    public int recipient_id;
    public int sender_id;
    public string sender_name;
    public string sending_time;
    public string mail_msg;
    public int mail_read;
    public int mail_claimed;
    public string hide_by;      //playerid1,playerid2,playerid3,...
    public string[] when_created;
    public string[] when_deleted;
}