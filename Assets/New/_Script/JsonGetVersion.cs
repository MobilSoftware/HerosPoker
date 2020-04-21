[System.Serializable]
public class JsonGetVersion
{
    public JsonVersion version;
    public JsonScene[] scenes;
    public JsonItem[] items;
}

[System.Serializable]
public class JsonVersion
{
    public string new_ver;
    public string old_ver;
}

[System.Serializable]
public class JsonScene
{
    public int scene_id;
    public string scene_name;
    public int asset_version;
    public string asset_url;
}

[System.Serializable]
public class JsonItem
{
    public int item_id;
    public int type;
    public int asset_version;
    public string thumb_url;
    public string asset_url;
}
