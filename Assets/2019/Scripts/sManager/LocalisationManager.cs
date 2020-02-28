using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayRivals;
using UnityEngine;

public enum LanguageType
{
    English,
    Indonesia,
    None
}

public class LanguageData
{
    public string English; 
    public string Indonesia;

    public void SetData ( Hashtable data )
    {
        English = PR_Utility.ParseString (data, "EN");
        Indonesia = PR_Utility.ParseString (data, "INDO");
    }

    public void SetData ( JObject data )
    {
        English = PR_Utility.ParseString (data, "EN");
        Indonesia = PR_Utility.ParseString (data, "INDO");
    }
}

public class LocalisationManager : MonoBehaviour
{
    #region Singleton
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static LocalisationManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static LocalisationManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first Localisation object in the scene.
                s_Instance = FindObjectOfType (typeof (LocalisationManager)) as LocalisationManager;
                //if (s_Instance == null)
                //    Debug.Log("Could not locate an Localisation object. \n You have to have exactly one Localisation in the scene.");
            }
            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit ()
    {
        s_Instance = null;
    }
    #endregion

    public TextAsset assetLocalisation;

    protected Hashtable stringIDMap;
    protected LanguageType currLanguage;
    [HideInInspector]
    public LanguageType Language { get { return currLanguage; } set { currLanguage = value; } }

    private void Awake ()
    {
        //Initialise hashtables
        stringIDMap = new Hashtable ();
        SetupLanguages ();
    }

    private void ChangeLanguageAgain ()
    {
        ChangeLanguage (Language);
    }

    void SetupLastLanguage()
    {
        int indexLanguage = PlayerPrefs.GetInt (PrefKey.Language.ToString (), -1);
        if (indexLanguage == -1)
        {
            if (Application.systemLanguage == SystemLanguage.Indonesian)
            {
                Language = LanguageType.Indonesia;
                PlayerPrefs.SetInt (PrefKey.Language.ToString (), 1);
                PlayerPrefs.Save ();
            }
            else
            {
                Language = LanguageType.English;
                PlayerPrefs.SetInt (PrefKey.Language.ToString (), 0);
                PlayerPrefs.Save ();
            }
        } else
            Language = (LanguageType)indexLanguage;

        //PlayerPrefs.SetInt (PrefKey.Language.ToString (), 1);
        //PlayerPrefs.Save ();
        //PlayerProfile.Language = LanguageType.Indonesia.ToString ();
    }

    //Get the text back for the current language with the given ID
    public string GetText ( string textID ) { return GetLocalisedText (textID, Language); }
    public string GetText ( string textID, LanguageType language ) { return GetLocalisedText (textID, language); }

    //Change language on the fly
    public void ChangeLanguage ( LanguageType language )
    {
        Language = language;
        //PlayerProfile.Language = language.ToString ();
        PlayerPrefs.SetInt (PrefKey.Language.ToString (), (int)language);
        PlayerPrefs.Save ();
        //Messenger<bool>.Invoke ("OnLanguageChanged", true);
    }
    public void SetupLanguages ()
    {
        stringIDMap.Clear ();
        SetupLastLanguage();
        //SetupLanguage(LanguageConfig.text);
        SetupLanguage (assetLocalisation.text);
        //SetupLanguage(MultiplayerGamesConfig.text);
    }

    //Helper function to parse the language with the IDs
    private void SetupLanguage ( string dataString )
    {
        if (dataString == null)
        {
            Debug.LogWarning ("TextAsset Doesn't exist");
            return;
        }

        //Config
        JObject objJson = JsonConvert.DeserializeObject<JObject> (dataString);
        JArray dataList = new JArray ();
        dataList = PR_Utility.ParseArray (objJson, "Sheet1");

        //Get the datas from  the JSON array
        foreach (JObject data in dataList)
        {
            //var data = hashData as Hashtable;
            string strKey = "";
            strKey = data["ID"].ToString ();

            LanguageData languageText = new LanguageData ();
            languageText.SetData (data);

            if (stringIDMap.Contains (strKey))
                Debug.LogWarning (strKey + " already in the list");
            else
                stringIDMap.Add (strKey, languageText);
        }
    }

    //Helper function to get the localised text
    private string GetLocalisedText ( string key, LanguageType newLanguage )
    {
        LanguageData text = null;
        string strText = key;

        if (stringIDMap == null)
        {
            //Debug.LogWarning("StringID Map is null");
            return strText;
        }

        //Check whether the given ID contains or not
        if (stringIDMap.ContainsKey (key))
        {
            text = (LanguageData) stringIDMap[key];
        }
        else
        {
            //Debug.LogWarning(string.Format("ID with {0} currently not existed in the list", key));
            return strText;
        }

        if (text == null)
        {
            Logger.W (string.Format ("ID with {0} currently not existed in the list", key));
            return strText;
        }


        //Check for the languages
        if (newLanguage == LanguageType.None)
        {
            switch (Language)
            {
                case LanguageType.English: strText = text.English.ToString (); break;
                case LanguageType.Indonesia: strText = text.Indonesia.ToString (); break;
            }
        }
        else
        {
            switch (newLanguage)
            {
                case LanguageType.English: strText = text.English.ToString (); break;
                case LanguageType.Indonesia: strText = text.Indonesia.ToString (); break;
            }
        }

        return strText;
    }
}
