using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public enum ButtonType {Spin, Stop};

    public ButtonType type;

    private SlotoManagerScript managerScript;

    void Awake()
    {
        managerScript = GameObject.Find("SlotoManager").GetComponent<SlotoManagerScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseUp()
    {
        if(managerScript) managerScript.OnMouseUp(type);
    }
}
