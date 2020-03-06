using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    private static HomeManager s_Instance = null;
    public static HomeManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (HomeManager)) as HomeManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an HomeManager object. \n You have to have exactly one HomeManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Button btnPoker;

    private bool isInit;

    private void Start ()
    {
        btnPoker.onClick.AddListener (OnPoker);
    }

    private void OnPoker ()
    {
        _SceneManager.instance.SetActiveScene (SceneType.POKER, true);
        _SceneManager.instance.SetActiveScene (SceneType.HOME, false);
    }

    public void Show ()
    {
        if (!isInit)
        {
            PhotonNetwork.ConnectUsingSettings ("v1.0");
            canvas.sortingOrder = (int) SceneType.HOME;
            isInit = true;
        }

        canvas.enabled = true;
    }

    public void Hide ()
    {
        canvas.enabled = false;
    }
}
