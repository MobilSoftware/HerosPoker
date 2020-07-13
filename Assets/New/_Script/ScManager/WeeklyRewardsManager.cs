using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeeklyRewardsManager : MonoBehaviour
{
    private static WeeklyRewardsManager s_Instance = null;
    public static WeeklyRewardsManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (WeeklyRewardsManager)) as WeeklyRewardsManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an WeeklyRewardsManager object. \n You have to have exactly one WeeklyRewardsManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Button btnClose;
    public ItemWeeklyDay[] itemWeeklyDays;
    public ItemWeeklyBonus[] itemWeeklyBonus;
    public Image fillProgressBar;

    [HideInInspector]
    public bool isSettingJson;

    private bool isInit;
    private SceneType prevSceneType;
    [HideInInspector]
    public JGetWeeklyLogin json;

    private void Start ()
    {
        btnClose.onClick.AddListener (Hide);
    }

    public void SetCanvas (bool val )
    {
        if (val)
            Show ();
        else
            Hide ();
    }

    public void OnClaim ()
    {
        Logger.E ("Claiming weekly login");
        ApiManager.instance.GetWeeklyLogin (true);
    }

    private void Show ()
    {
        if (!isInit)
        {
            canvas.sortingOrder = (int) SceneType.WEEKLY_REWARDS;
            isInit = true;
        }

        StartCoroutine (_WaitSetJson ());
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.WEEKLY_REWARDS;
        canvas.enabled = true;
    }

    private void Hide ()
    {
        canvas.enabled = false;
        _SceneManager.instance.activeSceneType = prevSceneType;
    }

    public void SetJson (JGetWeeklyLogin _json )
    {
        json = _json;
        isSettingJson = false;
    }

    IEnumerator _WaitSetJson ()
    {
        while (isSettingJson)
        {
            yield return _WFSUtility.wef;
        }

        SetWeeklyDaysStatus (json);

        yield return _WFSUtility.wef;

        for (int i = 0; i < json.rewards.Length; i++)
        {
            itemWeeklyDays[i].SetData (json.rewards[i].reward.item_type, json.rewards[i].reward.item_id, json.rewards[i].reward.item_amount);
        }
        yield return _WFSUtility.wef;
        for (int x = 0; x < json.bonus.Length; x++)
        {
            itemWeeklyBonus[x].SetData (json.bonus[x].item_type, json.bonus[x].item_id, json.bonus[x].item_amount);
        }
        yield return _WFSUtility.wef;
        UpdateProgressBar (json.login_count);

    }

    private void UpdateProgressBar (int loginCount )
    {
        Logger.E ("json login count: " + json.login_count);
        Logger.E ("login count: " + loginCount);
        int weeklyLoginCount = (loginCount % 7);
        if (weeklyLoginCount == 0)
        {
            if (PlayerData.jHome.can_claim_weekly)
            {
                fillProgressBar.fillAmount = 0f;
                for (int i = 0; i < itemWeeklyBonus.Length; i++)
                {
                    itemWeeklyBonus[i].objClaimed.SetActive (false);
                }
            }
            else
            {
                fillProgressBar.fillAmount = 1f;
                for (int i = 0; i < itemWeeklyBonus.Length; i++)
                {
                    itemWeeklyBonus[i].objClaimed.SetActive (true);
                }
            }

            return;
        }

        if (weeklyLoginCount < 3)
        {
            itemWeeklyBonus[0].objClaimed.SetActive (false);
            itemWeeklyBonus[1].objClaimed.SetActive (false);
            itemWeeklyBonus[2].objClaimed.SetActive (false);
        } else if (weeklyLoginCount < 5)
        {
            itemWeeklyBonus[0].objClaimed.SetActive (true);
            itemWeeklyBonus[1].objClaimed.SetActive (false);
            itemWeeklyBonus[2].objClaimed.SetActive (false);
        } else if (weeklyLoginCount < 7)
        {
            itemWeeklyBonus[0].objClaimed.SetActive (true);
            itemWeeklyBonus[1].objClaimed.SetActive (true);
            itemWeeklyBonus[2].objClaimed.SetActive (false);
        }
        float percentage = weeklyLoginCount / 7f;
        Logger.E ("Percentage: " + percentage);
        fillProgressBar.fillAmount = percentage;
    }

    public void SetWeeklyDaysStatus (JGetWeeklyLogin json )
    {
        for (int a = 0; a < itemWeeklyDays.Length; a++)
        {
            if (json.today - 1 == a)
            {
                itemWeeklyDays[a].objDimmer.SetActive (false);
                if (PlayerData.jHome.can_claim_weekly)
                {
                    //itemWeeklyDays[a].objDimmer.SetActive (false);
                    itemWeeklyDays[a].objClaimed.SetActive (false);
                }
                else
                {
                    //itemWeeklyDays[a].objDimmer.SetActive (true);
                    itemWeeklyDays[a].objClaimed.SetActive (true);
                }
            }
            else
            {
                itemWeeklyDays[a].objDimmer.SetActive (true);
                itemWeeklyDays[a].objClaimed.SetActive (false);
            }
        }
    }

    public void IncrementProgressBar ()
    {
        UpdateProgressBar (json.login_count + 1);
    }
}
