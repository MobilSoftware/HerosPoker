using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System.Text;

public class SlotoManagerScript : MonoBehaviour
{
    public Camera sceneCamera;
    public TextMesh betLabelTM;
    public TextMesh currBetLabelTM;
    public TextMesh currMoneyLabelTM;
    public ReelScript[] reels;
    public JackpotReelScript jackpotReel;
    public IconScript[] slotIcons;
    public GameObject[] fxsGO;
    public Sprite[] tileSprite;
    public Sprite[] iconSprite;

    private int bet;
    private int currBet;
    private int money;
    private int currMoney;
    private int[] showLine = new int[5];
    private int updateSlotSpin;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        //money = 10000;
        //currMoney = 10000;

        bet = 100;
        betLabelTM.text = bet.toShortCurrency ();
        currBet = bet;
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].Init(tileSprite, iconSprite);
        }
    }

    public void SetMoney ()
    {
        int ownedGold = System.Convert.ToInt32 (PlayerData.owned_gold);
        money = ownedGold;
        currMoney = ownedGold;
        currMoneyLabelTM.text = currMoney.toShortCurrency ();
    }

    // Update is called once per frame
    void Update()
    {
        if(currMoney != money)
        {
            int temp = Mathf.CeilToInt(Mathf.Abs(money - currMoney) / 10.0f);
            currMoney += (currMoney < money ? temp : -temp);
            //currMoneyLabelTM.text = currMoney.ToString("N0");
            currMoneyLabelTM.text = currMoney.toShortCurrency ();
        }
    }

    public void OnMouseUp(ButtonScript.ButtonType type)
    {
        if(type == ButtonScript.ButtonType.Spin)
        {
            if(money >= bet)
            {
                money -= bet;
                StartCoroutine(StartSpin());
            }
        } else if (type == ButtonScript.ButtonType.Stop)
        {
            StartCoroutine(StopSpin());
        }
        else if (type == ButtonScript.ButtonType.Max)
        {
            bet = 10000;
            //betLabelTM.text = bet.ToString("N0");
            betLabelTM.text = bet.toShortCurrency ();
        }
        else if (type == ButtonScript.ButtonType.Up)
        {
            bet = Mathf.Clamp(bet + 100, 100, 10000);
            //betLabelTM.text = bet.ToString("N0");
            betLabelTM.text = bet.toShortCurrency ();
        }
        else if (type == ButtonScript.ButtonType.Down)
        {
            bet = Mathf.Clamp(bet - 100, 100, 10000);
            //betLabelTM.text = bet.ToString("N0");
            betLabelTM.text = bet.toShortCurrency ();
        }
        else if (type == ButtonScript.ButtonType.Back)
        {
            Debug.Log("Replace this function at SlotoManagerScript");
            //Application.Quit();
            PlayerData.owned_gold = System.Convert.ToInt32 (money);
            _SceneManager.instance.SetActiveScene (SceneType.HOME, true);
            _SceneManager.instance.SetActiveScene (SceneType.SLOTO, false);
        }
    }

    IEnumerator StartSpin()
    {
        if (reels[0].spin == 0 && !jackpotReel.GetSpin())
        {
            updateSlotSpin = 0;
            HideLine();
            currBet = bet;
            //currBetLabelTM.text = currBet.ToString("N0");
            currBetLabelTM.text = currBet.toShortCurrency ();
            for (int i = 0; i < slotIcons.Length; i++)
            {
                slotIcons[i].fixedValue = false;
            }
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].StartSpin();
                yield return new WaitForSeconds(0.1f);
            }
            StartCoroutine(HitAPI());
        }
    }

    IEnumerator StopSpin()
    {
        if (reels[0].spin == 2)
        {
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].StopSpin();
                yield return new WaitForSeconds(0.1f * Random.Range(1, 11));
            }
        }
    }

    IEnumerator HitAPI()
    {
        int i;
        int[] tempValue = new int[16];
        int rand = Random.Range(0, 3);
        if (rand == 0)
        {
            //tempValue = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            showLine = new int[] { 1 + (Random.Range(0, 3) * 5), 2 + (Random.Range(0, 3) * 5), 3 + (Random.Range(0, 3) * 5) };
        }
        else if (rand == 1)
        {
            //tempValue = new int[] { 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3 };
            showLine = new int[] { 1 + (Random.Range(0, 3) * 5), 2 + (Random.Range(0, 3) * 5), 3 + (Random.Range(0, 3) * 5), 4 + (Random.Range(0, 3) * 5) };
        }
        else if (rand == 2)
        {
            //tempValue = new int[] { 2, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4 };
            showLine = new int[] { 1 + (Random.Range(0, 3) * 5), 2 + (Random.Range(0, 3) * 5), 3 + (Random.Range(0, 3) * 5), 4 + (Random.Range(0, 3) * 5), 5 + (Random.Range(0, 3) * 5) };
        }
        for (i = 0; i < tempValue.Length; i++)
        {
            tempValue[i] = Random.Range(0, reels[0].maxIconType);
        }
        yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));
        for (i = 0; i < slotIcons.Length; i++)
        {
            slotIcons[i].SetIconValue(tempValue[i], iconSprite[tempValue[i] * reels[0].maxIconBlur + 0], tileSprite[0]);
            slotIcons[i].fixedValue = true;
        }
        StartCoroutine(StopSpin());
    }

    private void HideLine()
    {
        for (int i = 0; i < fxsGO.Length; i++)
        {
            fxsGO[i].SetActive(false);
        }
    }

    private void ShowLine()
    {
        for (int i=0; i<showLine.Length; i++)
        {
            fxsGO[i].transform.position = new Vector3(slotIcons[showLine[i]].transform.position.x, slotIcons[showLine[i]].transform.position.y, fxsGO[i].transform.position.z);
            fxsGO[i].SetActive(true);
        }
    }

    public void UpdateSlotSpin()
    {
        if(++updateSlotSpin == 5)
        {
            ShowLine();
            jackpotReel.Spin(Random.Range(0, 16));
        }
    }

    public void UpdateMoney()
    {
        money += (100 * showLine.Length);
    }
}
