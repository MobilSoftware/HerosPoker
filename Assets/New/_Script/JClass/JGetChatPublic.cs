[System.Serializable]
public class JGetChatPublic
{
    public JPublicChat[] chat;
}

[System.Serializable]
public class JPublicChat
{
    public int chat_public_id;
    public int from_id;
    public string content;
    public string created_at;
    public string from_name;
    public string from_picture;
    public bool from_verified;
    public int from_vip_level;
}