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
    public TextMesh freeSpinTM;
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
    private int serverMoney;
    private int updateSlotSpin;
    private int[] showLine;
    private float timeWait;
    private int jsonIndex;
    private int jsonCombinationIndex;
    private bool pushOk;
    private bool pushAuto;
    private float pushAutoTimer;

    private ApiBridge api;
    private JSlot json;
    private JSlotCombination[] jsonCombination;

    private int[] betArray = new int[] {100, 200, 500, 700, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 15000, 20000};

    void Awake()
    {
        if (!api) api = GetComponent<ApiBridge>();
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        serverMoney = -1;
        bet = 100;
        betWinning = 0;
        currBetWinning = 0;
        betLabelTM.text = bet.ToString("N0")+".000";
        currBet = bet;
        pushOk = true;
        pushAuto = false;
        pushAutoTimer = 0f;
        SetMoney();
        ClearJson();
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

    private void ClearJson()
    {
        json = null;
        jsonIndex = -1;
        jsonCombination = null;
        jsonCombinationIndex = -1;
        freeSpinTM.gameObject.SetActive(false);
    }

    public void SetMoney()
    {
        int ownedGold = System.Convert.ToInt32 (PlayerData.owned_coin);
        //int ownedGold = System.Convert.ToInt32(10000); //Uncomment above
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
            currBetLabelTM.text = currBetWinning.ToString("N0") + ".000";
            if (currBetWinning == betWinning)
            {
                if (currBetWinning != 0) StartBlink(currBetLabelTM.gameObject);
                if (jsonIndex == json.slot_spin.Length - 1) money = serverMoney;
            }
        }
        if (!pushOk)
        {
            if (reels[0].spin == 0 && reels[1].spin == 0 && reels[2].spin == 0 && reels[3].spin == 0 && reels[4].spin == 0 && !jackpotReel.GetSpin()) pushOk = true;
            if (pushAuto) OnMouseUp(ButtonScript.ButtonType.Spin);
        }
    }

    public void OnMouseDown(ButtonScript.ButtonType type, SpriteRenderer sr = null)
    {
        if (type == ButtonScript.ButtonType.Spin)
        {
            pushAutoTimer = 0;
            pushAuto = false;
            sr.flipX = false;
        }
    }

    public void OnMouseDrag(ButtonScript.ButtonType type, SpriteRenderer sr = null)
    {
        if (type == ButtonScript.ButtonType.Spin)
        {
            pushAutoTimer += Time.deltaTime;
            if(pushAutoTimer >= 3.0f && !pushAuto)
            {
                pushAuto = true;
                sr.flipX = true;
            }
        }
    }

    public void OnMouseUp(ButtonScript.ButtonType type, SpriteRenderer sr = null)
    {
        if (type == ButtonScript.ButtonType.Spin)
        {
            if (money >= bet)
            {
                if (pushOk)
                {
                    pushOk = false;
                    StartSpin();
                }
            }
            else
            {
                ToMessageManager(gameObject, "Uang tak cukup", ButtonMode.OK, -1);
                pushAuto = false;
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

    private void StartSpin()
    {
        if (reels[0].spin == 0 && !jackpotReel.GetSpin())
        {
            HitAPI();
            money -= bet;
            currBet = bet;
            StartCoroutine(ContinueSpin());
        }
    }

    IEnumerator ContinueSpin()
    {
        updateSlotSpin = 0;
        HideLine();
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
        jsonCombination = slot_spin.slot_combination;
        if(jsonCombination.Length > 0)
        {
            jsonCombinationIndex = 0;
        }
        for (i = 0; i < reels.Length; i++)
        {
            reels[i].StopSpin();
            yield return new WaitForSeconds(0.1f * Random.Range(1, 6));
        }
    }

    IEnumerator StartCombinationSlideShow()
    {
        yield return new WaitForSeconds(1.0f);
        if(jsonCombinationIndex > -1)
        {
            jsonCombinationIndex = (jsonCombinationIndex + 1) % jsonCombination.Length;
            ShowLine();
        }
    }

    private void HitAPI()
    {
        ClearJson();
        timeWait = Time.time + 2.0f;
        betWinning = 0;
        currBetWinning = 0;
        currBetLabelTM.text = "0";
        api.StartSlot(1, bet);
    }

    public void RStartSlot(ApiBridge.ResponseParam response) {
        Debug.Log("Return " + response.uri + " in " + response.post_time + " (Seed #" + response.seed.ToString() + ")\n" + response.post_data + "\n\nRaw = " + response.post_id);
        JStartSlot jsonStart = JsonUtility.FromJson<JStartSlot>(response.post_data);
        serverMoney = jsonStart.player.coin;
        json = jsonStart.slot;
        jsonIndex = 0;
        StartCoroutine(StopSpin(json.slot_spin[jsonIndex]));
    }

    private void HideLine()
    {
        for (int i = 0; i < fxsGO.Length; i++)
        {
            fxsGO[i].SetActive(false);
        }
        winningBetAnimTM.gameObject.SetActive(false);
    }

    private void ShowLine()
    {
        showLine = jsonCombination[jsonCombinationIndex].combination_matrix;
        for (int i = 0; i < showLine.Length; i++)
        {
            fxsGO[i].transform.position = new Vector3(slotIcons[showLine[i]].transform.position.x, slotIcons[showLine[i]].transform.position.y, fxsGO[i].transform.position.z);
            fxsGO[i].SetActive(true);
        }
        if (jsonCombination.Length > 1) StartCoroutine(StartCombinationSlideShow());
    }

    public void UpdateSlotSpin()
    {
        if(++updateSlotSpin == 5)
        {
            if (jsonCombinationIndex > -1) ShowLine();
            if(json.slot_spin[jsonIndex].slot_matrix[0] == 0)
            {
                jackpotReel.Spin(json.slot_spin[jsonIndex].slot_bonus_id);
            }
            else
            {
                UpdateMoney();
            }
        }
    }

    public void UpdateMoney()
    {
        if(json.slot_spin[jsonIndex].slot_payout > 0)
        {
            betWinning += json.slot_spin[jsonIndex].slot_payout;
            winningBetAnimTM.text = json.slot_spin[jsonIndex].slot_payout.ToString("N0") + ".000";
            winningBetAnimTM.gameObject.SetActive(true);
        }
        if (json.slot_spin.Length > 1 && jsonIndex < json.slot_spin.Length-1)
        {
            timeWait = Time.time + 2.0f;
            jsonIndex += 1;
            jsonCombination = null;
            jsonCombinationIndex = -1;
            StartCoroutine(ContinueSpin());
            StartCoroutine(StopSpin(json.slot_spin[jsonIndex]));
            if (jsonIndex >= 1)
            {
                freeSpinTM.text = "Free Spin x"+(9-jsonIndex);
                freeSpinTM.gameObject.SetActive(true);
            }
        }
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
        ToMessageManager(gameObject, error.error_msg[0], ButtonMode.OK, -1);
    }

    private string ToShortCurrency(int value)
    {
        return value.toShortCurrency();
    }

    private void ToMessageManager(GameObject gameObject, string error_msg, ButtonMode bt = ButtonMode.OK, int error_code = -1)
    {
        MessageManager.instance.Show(gameObject, error_msg, bt, error_code);
        //Debug.Log(error_msg); //Uncomment above
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
    private class JPlayer
    {
        public int coin;
    }

    [System.Serializable]
    private class JStartSlot
    {
        public JSlot slot;
        public JPlayer player;
    }

}
