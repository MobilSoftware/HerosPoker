[System.Serializable]
public class JSetHero
{
    public JHeroSet hero;
    public JProfile player;
}

[System.Serializable]
public struct JHeroSet
{
    public int hero_type_id;
    public string hero_name;
    public int hero_gender;
}