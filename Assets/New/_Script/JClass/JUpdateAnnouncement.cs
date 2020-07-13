[System.Serializable]
public class JUpdateAnnouncement
{
    public JAnnouncement announcement;
}

[System.Serializable]
public class JAnnouncement
{
    public int announcement_id;
    public string announcement_text;
    public int priority;
}