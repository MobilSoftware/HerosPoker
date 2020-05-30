using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public enum ButtonType {Spin, Stop, Max, Up, Down, Back, Plus};

    public ButtonType type;

    private SlotoManagerScript managerScript;
    private SpriteRenderer sr;

    void Awake()
    {
        managerScript = GameObject.Find("SlotoManager").GetComponent<SlotoManagerScript>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        if (managerScript) managerScript.OnMouseDown(type, sr);
    }

    void OnMouseDrag()
    {
        if (managerScript) managerScript.OnMouseDrag(type, sr);
    }

    void OnMouseUp()
    {
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        if (managerScript) managerScript.OnMouseUp(type, sr);
    }
}
