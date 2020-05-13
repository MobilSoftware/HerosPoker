[System.Serializable]
public class JGetVersion
{
    public JVersion version;
    public JScene[] scenes;
    public JItem[] items;
}

[System.Serializable]
public class JVersion
{
    public string new_ver;
    public string old_ver;
}

[System.Serializable]
public class JScene
{
    public int scene_id;
    public string scene_name;
    public int asset_version;
    public string asset_url;
}

[System.Serializable]
public class JItem
{
    public int item_id;
    public int item_type_id;
    public int asset_version;
    public string thumb_url;
    public string asset_url;
}
