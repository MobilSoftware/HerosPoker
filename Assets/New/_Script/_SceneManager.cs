using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _SceneManager : MonoBehaviour
{
    private static _SceneManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static _SceneManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first MenuManager object in the scene.
                s_Instance = FindObjectOfType (typeof (_SceneManager)) as _SceneManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an _SceneManager object. \n You have to have exactly one _SceneManager in the scene.");
            }
            return s_Instance;
        }
    }

    private List<Scene> loadedScenes = new List<Scene> ();
    private SeMenuManager menuManager;
    private SePokerManager pokerManager;

    private void Start ()
    {
        DontDestroyOnLoad (this);
        StartCoroutine (_LoadAllScenes ());
    }

    IEnumerator _LoadAllScenes ()
    {
        AsyncOperation async;
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            async = SceneManager.LoadSceneAsync (i, LoadSceneMode.Additive);
            while (!async.isDone)
            {
                yield return new WaitForEndOfFrame ();
            }
            loadedScenes.Add (SceneManager.GetSceneByBuildIndex (i));
        }
        yield return new WaitForEndOfFrame ();
        menuManager = SeMenuManager.instance;
        pokerManager = SePokerManager.instance;
        SetActiveMenu ();
        SceneManager.UnloadSceneAsync ("SeSplash");
    }

    public void SetActiveMenu ()
    {
        menuManager.objMenu.SetActive (true);
        menuManager.Init ();
        pokerManager.objPoker.SetActive (false);
    }

    public void SetActivePoker ()
    {
        menuManager.objMenu.SetActive (false);
        pokerManager.objPoker.SetActive (true);

        PhotonRoomInfoManager.instance.InitialiseCardGameScripts ();
        RoomInfoManager.instance.JoinRandomRoom ();
    }
}
