[System.Serializable]
public class JGetEvent
{
    public JEvent[] events;
}

[System.Serializable]
public struct JEvent
{
    public int event_type_id;
    public string[] event_type_title;
    public string[] event_type_rule;
    public string event_start;
    public int to_start;
    public string event_end;
    public int to_end;
}