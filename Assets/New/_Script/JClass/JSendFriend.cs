[System.Serializable]
public class JSendFriend
{
    public JListSendFriendType list;
}

[System.Serializable]
public struct JListSendFriendType
{
    public int player_id;
    public int[] friend_list;
    public int[] friend_request;
    public int[] friend_request_me;
    public int[] friend_block;
    public int[] friend_block_me;
    public int[] friend_chat_me;
    public int[] friend_gift_me;
}