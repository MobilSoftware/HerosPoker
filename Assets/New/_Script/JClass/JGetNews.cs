[System.Serializable]
public class JGetNews
{
    public JNews[] news;
}

[System.Serializable]
public struct JNews
{
    public int news_id;
    public int news_tab_type_id;
    public string[] news_title;
    public string[] news_desc;
    public string banner_potrait;
    public string banner_landscape;
    public string news_url;
    public string news_date;
    public string button1_text;
    public string button1_open;
    public string button2_text;
    public string button2_open;
    public string button3_text;
    public string button3_open;
    public int hide_popup;
}