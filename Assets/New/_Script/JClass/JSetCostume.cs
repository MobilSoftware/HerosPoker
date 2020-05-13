[System.Serializable]
public class JSetCostume
{
    public JCostumeSet costume;
    public JProfile player;
}

[System.Serializable]
public struct JCostumeSet
{
    public int item_id;
    public string[] item_name;
    public string[] item_desc;
    public int item_type_id;
    public int hero_type_id;
}