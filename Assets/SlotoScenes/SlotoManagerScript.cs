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
    private float timeWait;

    private ApiBridge api;

    private int[] betArray = new int[] {100, 200, 500, 700, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 15000, 20000};

    void Awake()
    {
        if (!api) api = GetComponent<ApiBridge>();
    }

    void Start()
    {
        /*for (var i=9510; i>9490; i--)
        {
            Debug.Log(i+" = "+ ToShortCurrency(i));
        }*/
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
        if (api)
        {
            api.SetPlayerId(PlayerPrefs.GetInt(PrefEnum.PLAYER_ID.ToString(), 0));
            api.SetToken(PlayerPrefs.GetString(PrefEnum.TOKEN.ToString(), ""));
        }
    }

    public void SetMoney()
    {
        //int ownedGold = System.Convert.ToInt32 (PlayerData.owned_coin);
        int ownedGold = System.Convert.ToInt32(10000); //Uncomment above
        money = ownedGold;
        currMoney = ownedGold;
        currMoneyLabelTM.text = ToShortCurrency(currMoney);
    }

    void Update()
    {
        if (currMoney != money)
        {
            int temp = Mathf.CeilToInt(Mathf.Abs(money - currMoney) / 10.0f);
            currMoney += (currMoney < money ? temp : -temp);
            currMoneyLabelTM.text = ToShortCurrency(currMoney);
        }
        if (currBetWinning != betWinning)
        {
            int temp = Mathf.CeilToInt(Mathf.Abs(betWinning - currBetWinning) / 10.0f);
            currBetWinning += (currBetWinning < betWinning ? temp : -temp);
            currBetLabelTM.text = currBetWinning.ToString("N0");
            winningBetAnimTM.text = currBetLabelTM.text;
            if (currBetWinning == betWinning) StartBlink(currBetLabelTM.gameObject);
        }
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
                //MessageManager.instance.Show(gameObject, "Uang tak cukup", ButtonMode.OK, -1);
                Debug.Log("Uang tak cukup"); //Uncomment above
            }
        } else if (type == ButtonScript.ButtonType.Stop)
        {
            //StartCoroutine(StopSpin());
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
            HitAPI();
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
        }
    }

    IEnumerator StopSpin(JSlotSpin slot_spin)
    {
        do
        {
            yield return new WaitForSeconds(0.1f);
        } while (reels[4].spin != 2 || Time.time < timeWait);
        int i;
        for (i=0; i<slotIcons.Length; i++)
        {
            slotIcons[i].SetIconValue(slot_spin.slot_matrix[i], iconSprite[slot_spin.slot_matrix[i] * reels[0].maxIconBlur + 0], tileSprite[0]);
            slotIcons[i].fixedValue = true;
        }
        JSlotCombination[] slot_combination = slot_spin.slot_combination;
        for (i = 0; i < slot_combination.Length; i++)
        {
            showLine = slot_combination[i].combination_matrix;
        }
        for (i = 0; i < reels.Length; i++)
        {
            reels[i].StopSpin();
            yield return new WaitForSeconds(0.05f * Random.Range(1, 11));
        }
    }

    private void HitAPI()
    {
        api.StartSlot(1);
        timeWait = Time.time + 2.0f;
    }

    [System.Serializable]
    private class JSlotCombination
    {
        public int[] combination_matrix;
        public string combination_name;
        public int combination_payout;
    }

    [System.Serializable]
    private class JSlotSpin
    {
        public int[] slot_matrix;
        public JSlotCombination[] slot_combination;
        public int slot_bonus_id;
        public int slot_bonus_coin;
        public int slot_payout;
    }

    [System.Serializable]
    private class JSlot
    {
        public int slot_type;
        public int slot_cost;
        public JSlotSpin[] slot_spin;
        public int total_payout;
        public int jackpot;
    }

    [System.Serializable]
    private class JStartSlot
    {
        public JSlot slot;
    }

    public void RStartSlot(ApiBridge.ResponseParam response) {
        Debug.Log("Return " + response.uri + " in " + response.post_time + " (Seed #" + response.seed.ToString() + ")\n" + response.post_data + "\n\nRaw = " + response.post_id);
        JStartSlot jsonStart = JsonUtility.FromJson<JStartSlot>(response.post_data);
        JSlot json = jsonStart.slot;
        for (var i=0; i< json.slot_spin.Length; i++)
        {
            //JSlotSpin slot_spin = json.slot_spin[i];
            StartCoroutine(StopSpin(json.slot_spin[i]));
        }
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

    private string ToShortCurrency(long longValue)
    {
        if (longValue == 0)
            return "0";

        bool bIsBillion = false;
        bool bIsMillion = false;
        //bool bIsThousands = false;
        string strSuffix = "";

        if (System.Math.Abs(longValue) > 999999)
        {
            bIsBillion = true;
            strSuffix = " B";
        }
        else if (System.Math.Abs(longValue) > 999)
        {
            bIsMillion = true;
            strSuffix = " M";
        }
        else if (System.Math.Abs(longValue) > 0)
        {
            //bIsThousands = true;
            strSuffix = " K";
        }

        float newVal = 0.0f;
        if (bIsBillion)
            newVal = longValue / 1000000.0f;
        else if (bIsMillion)
            newVal = longValue / 1000.0f;
        else
            newVal = longValue;

        long oldVal = longValue;
        //return newVal.ToString("#.###") + strSuffix;
        string wholeNumber = newVal.ToString();
        if(wholeNumber.IndexOf('.') >= 0)
        {
            wholeNumber = newVal.ToString().Split('.')[0];
        }
        else if (wholeNumber.IndexOf(',') >= 0)
        {
            wholeNumber = newVal.ToString().Split(',')[0];
        }
        else
        {
            wholeNumber = newVal.ToString().Split(',')[0];
        }

        string roundedNumber = newVal.ToString();

        if (wholeNumber.Length > 2)
        {
            roundedNumber = newVal.ToString("N0");
        }
        else if (wholeNumber.Length > 1)
        {
            //roundedNumber = newVal.ToString("N1");
            roundedNumber = newVal.ToString("#.#");
        }
        else if (wholeNumber.Length > 0)
        {
            //roundedNumber = newVal.ToString("N2");
            roundedNumber = newVal.ToString("#.##");
        }

        return roundedNumber + strSuffix;
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

    public void RErrorHandler(ApiBridge.ResponseParam error)
    {
        Debug.Log("RErrorHandler from " + error.uri + " (Seed #" + error.seed.ToString() + ")\n(Code #" + error.error_code + ") {" + error.error_msg[0] + " || " + error.error_msg[1] + "}");
        //MessageManager.instance.Show(gameObject, error.error_msg[0], ButtonMode.OK, -1);
        Debug.Log(error.error_msg[0]); //Uncomment above
    }
}
