using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestManager : MonoBehaviour
{
    private static DailyQuestManager s_Instance = null;
    public static DailyQuestManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (DailyQuestManager)) as DailyQuestManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an DailyQuestManager object. \n You have to have exactly one DailyQuestManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Transform trFrame;
    public Transform parentItems;
    public ScrollRect scrRect;
    public ItemDailyQuest prefabItemDailyQuest;
    public Button btnClose;
    public Button btnGetAll;

    private bool isInit;
    private bool isSettingJson;
    private SceneType prevSceneType;

    public void SetCanvas (bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    private void Show ()
    {
        if (!isInit)
        {
            isInit = true;
            canvas.sortingOrder = (int) SceneType.DAILY_QUEST;
        }
        trFrame.localScale = Vector3.zero;
        canvas.enabled = true;
        scrRect.verticalNormalizedPosition = 1f;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.DAILY_QUEST;
        trFrame.LeanScale (Vector3.one, _SceneManager.TWEEN_DURATION);
    }

    private void Hide ()
    {
        trFrame.LeanScale (Vector3.zero, _SceneManager.TWEEN_DURATION).setOnComplete
        (() =>
        {
            canvas.enabled = false;
            _SceneManager.instance.activeSceneType = prevSceneType;
        });
    }

    public void Logout ()
    {
        isInit = false;
        isSettingJson = true;
    }
}
