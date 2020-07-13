using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMinichat : MonoBehaviour
{
    public Text txtChat;
    [HideInInspector]
    public JPublicChat json;

    public void SetData (JPublicChat _json )
    {
        json = _json;
        string displayName = _json.from_name;
        if (displayName.Length > 10)
        {
            displayName = displayName.Substring (0, 7) + "...";
        }
        txtChat.text = displayName + " : " + _json.content;
    }
}
