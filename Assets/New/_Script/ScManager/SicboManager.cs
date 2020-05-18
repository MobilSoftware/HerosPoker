using UnityEngine;
using UnityEngine.UI;

public class SicboManager : MonoBehaviour
{
    private static SicboManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static SicboManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (SicboManager)) as SicboManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an SicboManager object. \n You have to have exactly one SicboManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Button[] btnBetValues;   //should be 3?
    public Button[] btnBetTypes;   //should be length = 50

    private long betValue;
    private ApiBridge.SicboBetType betType;

    private void Start ()
    {
        for (int a = 0; a < btnBetValues.Length; a++)
        {
            btnBetValues[a].onClick.AddListener (() => OnBetValue (a));
        }
        for (int i = 0; i < btnBetTypes.Length; i++)
        {
            btnBetTypes[i].onClick.AddListener (() => OnBetType (i));
        }
    }

    private void OnBetValue (int btnIndex )
    {
        switch (btnIndex)
        {
            case 0: betValue = 1; break;
            case 1: betValue = 2; break;
            case 2: betValue = 5; break;
        }
    }

    private void OnBetType (int btnIndex )
    {
        betType = (ApiBridge.SicboBetType) (btnIndex + 1);
    }


}
