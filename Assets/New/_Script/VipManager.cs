using UnityEngine;
using UnityEngine.UI;

public class VipManager : MonoBehaviour
{
    private static VipManager s_Instance = null;
    public static VipManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (VipManager)) as VipManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an VipManager object. \n You have to have exactly one VipManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Button btnClose;



    private SceneType prevSceneType;
    private bool isInit;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
    }

    public void SetCanvas ( bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    public void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.VIP;
            isInit = true;
        }

        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.VIP;
    }

    public void Hide ()
    {
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }
}
