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
    public TextMesh winningBetAnimTM;
    public ReelScript[] reels;
    public JackpotReelScript jackpotReel;
    public IconScript[] slotIcons;
    public GameObject[] fxsGO;
    public Sprite[] tileSprite;
    public Sprite[] iconSprite;

    private int bet;
    private int currBet;
    private int betWinning;
    private int currBetWinning;
    private int money;
    private int currMoney;
    private int[] showLine = new int[5];
    private int updateSlotSpin;

    private int[] betArray = new int[] {100, 200, 500, 700, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 15000, 20000};

    void Start()
    {
        Init();
        SetMoney();
    }

    private void Init()
    {
        bet = 100;
        betWinning = 0;
        currBetWinning = 0;
        betLabelTM.text = bet.ToString("N0") + ".000";
        currBet = bet;
        winningBetAnimTM.gameObject.SetActive(false);
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].Init(tileSprite, iconSprite);
        }
    }

    public void SetMoney()
    {
        int ownedGold = System.Convert.ToInt32 (PlayerData.owned_coin);
        //int ownedGold = System.Convert.ToInt32(10000);
        money = ownedGold;
        currMoney = ownedGold;
        currMoneyLabelTM.text = ToShortCurrency(currMoney);
    }

    void Update()
    {
        //if(currMoney != money)
        //{
        //    int temp = Mathf.CeilToInt(Mathf.Abs(money - currMoney) / 10.0f);
        //    currMoney += (currMoney < money ? temp : -temp);
        //    currMoneyLabelTM.text = ToShortCurrency(currMoney);
        //}
        //if (currBetWinning != betWinning)
        //{
        //    int temp = Mathf.CeilToInt(Mathf.Abs(betWinning - currBetWinning) / 10.0f);
        //    currBetWinning += (currBetWinning < betWinning ? temp : -temp);
        //    currBetLabelTM.text = currBetWinning.ToString("N0") + ".000";
        //    winningBetAnimTM.text = currBetLabelTM.text;
        //    if (currBetWinning == betWinning) StartBlink(currBetLabelTM.gameObject);
        //}
    }

    public void OnMouseUp(ButtonScript.ButtonType type)
    {
        if(type == ButtonScript.ButtonType.Spin)
        {
            if (money >= bet)
            {
                StartCoroutine(StartSpin());
            }
            else
            {
                MessageManager.instance.Show(gameObject, "Uang tak cukup", ButtonMode.OK, -1);
            }
        } else if (type == ButtonScript.ButtonType.Stop)
        {
            StartCoroutine(StopSpin());
        }
        else if (type == ButtonScript.ButtonType.Max)
        {
            bet = betArray[betArray.Length-1];
            betLabelTM.text = bet.ToString("N0") + ".000";
        }
        else if (type == ButtonScript.ButtonType.Up)
        {
            int betIndex = 0;
            for (int i=0; i< betArray.Length; i++)
            {
                if(bet == betArray[i])
                {
                    betIndex = i;
                    break;
                }
            }
            if(betIndex+1 < betArray.Length) bet = betArray[betIndex + 1];
            betLabelTM.text = bet.ToString("N0") + ".000";
        }
        else if (type == ButtonScript.ButtonType.Down)
        {
            int betIndex = 0;
            for (int i = 0; i < betArray.Length; i++)
            {
                if (bet == betArray[i])
                {
                    betIndex = i;
                    break;
                }
            }
            if (betIndex - 1 >= 0) bet = betArray[betIndex - 1];
            betLabelTM.text = bet.ToString("N0") + ".000";
        }
        else if (type == ButtonScript.ButtonType.Back)
        {
            PlayerData.owned_coin = System.Convert.ToInt32 (money);
            _SceneManager.instance.SetActiveScene (SceneType.HOME, true);
            _SceneManager.instance.SetActiveScene (SceneType.SLOTO, false);
        }
        else if (type == ButtonScript.ButtonType.Plus)
        {
            Debug.Log("Insert Plus Code Here");
        }
    }

    IEnumerator StartSpin()
    {
        if (reels[0].spin == 0 && !jackpotReel.GetSpin())
        {
            money -= bet;
            updateSlotSpin = 0;
            HideLine();
            winningBetAnimTM.gameObject.SetActive(false);
            currBet = bet;
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
                yield return new WaitForSeconds(0.05f * Random.Range(1, 11));
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
            showLine = new int[] { 1 + (Random.Range(0, 3) * 5), 2 + (Random.Range(0, 3) * 5), 3 + (Random.Range(0, 3) * 5) };
        }
        else if (rand == 1)
        {
            showLine = new int[] { 1 + (Random.Range(0, 3) * 5), 2 + (Random.Range(0, 3) * 5), 3 + (Random.Range(0, 3) * 5), 4 + (Random.Range(0, 3) * 5) };
        }
        else if (rand == 2)
        {
            showLine = new int[] { 1 + (Random.Range(0, 3) * 5), 2 + (Random.Range(0, 3) * 5), 3 + (Random.Range(0, 3) * 5), 4 + (Random.Range(0, 3) * 5), 5 + (Random.Range(0, 3) * 5) };
        }
        for (i = 0; i < tempValue.Length; i++)
        {
            tempValue[i] = Random.Range(0, reels[0].maxIconType);
        }
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
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
        betWinning = (100 * showLine.Length);
        money += betWinning;
        winningBetAnimTM.gameObject.SetActive(true);
    }

    private string ToShortCurrency(int value)
    {
        return value.toShortCurrency();
        //return value.ToString("N0") + ".000";
    }

    private void StartBlink(GameObject go)
    {
        StartCoroutine(StartBlinking(go));
    }

    IEnumerator StartBlinking(GameObject go)
    {
        for (int i = 0; i < 4; i++)
        {
            go.SetActive(false);
            winningBetAnimTM.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            go.SetActive(true);
            winningBetAnimTM.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1.0f);
        winningBetAnimTM.gameObject.SetActive(false);
    }

    private void OnPositiveClicked(int returnedCode)
    {
        Debug.Log("returnedCode = "+returnedCode);
    }
}
