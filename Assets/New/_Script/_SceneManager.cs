using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    SPLASH,
    HOME,
    BEGIN,
    POKER,
    MESSAGE
}

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

    [HideInInspector]
    public SceneType activeSceneType;

    private List<Scene> loadedScenes = new List<Scene> ();
    private HomeManager homeManager;
    private PokerManager pokerManager;
    private BeginManager beginManager;
    private MessageManager msgManager;

    private void Start ()
    {
        DontDestroyOnLoad (this);
        activeSceneType = SceneType.SPLASH;
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
        homeManager = HomeManager.instance;
        pokerManager = PokerManager.instance;
        beginManager = BeginManager.instance;
        msgManager = MessageManager.instance;
        SetActiveBegin (true);
        SceneManager.UnloadSceneAsync ("SeSplash");
    }

    public void SetActiveScene (SceneType st, bool val )
    {
        switch (st)
        {
            case SceneType.BEGIN:
                SetActiveBegin (val);
                break;
            case SceneType.HOME:
                SetActiveHome (val);
                break;
            case SceneType.POKER:
                SetActivePoker (val);
                break;
        }
    }

    private void SetActiveHome (bool val)
    {
        if (val)
        {
            homeManager.Show ();
            activeSceneType = SceneType.HOME;
        }
        else
            homeManager.Hide ();
    }

    private void SetActivePoker (bool val)
    {
        if (val)
        {
            pokerManager.Show ();
            activeSceneType = SceneType.POKER;
        }
        else
            pokerManager.Hide ();
    }

    private void SetActiveBegin (bool val )
    {
        if (val)
        {
            beginManager.Show ();
            activeSceneType = SceneType.BEGIN;
        }
        else
            beginManager.Hide ();
    }
}
