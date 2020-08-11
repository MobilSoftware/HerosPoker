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

    public SpriteRenderer BGSR;
    public Sprite[] BGSprite;
    public SpriteRenderer FGSR;
    public Sprite[] FGSprite;
    public SpriteRenderer[] FrameSR;
    public Sprite[] FrameSprite;
    public SpriteRenderer SpinButtonSR;
    public Sprite[] SpinButtonSprite;
    public Sprite[] StopButtonSprite;
    public GameObject AccesoriesGO;
    public Sprite[] tileSprite1;
    public Sprite[] iconSprite1;
    public Sprite[] tileSprite2;
    public Sprite[] iconSprite2;

    public SpriteRenderer helpBG;
    public SpriteRenderer helpFG;
    public Sprite[] helpFGSprite;

    private Sprite[] tileSprite;
    private Sprite[] iconSprite;

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
    private int slotType;

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
        //Init(2);
    }

    public void Init(int _slotType = 1)
    {
        slotType = _slotType;
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
        ArrangeScreen();
        if (api)
        {
            api.SetPlayerId(PlayerPrefs.GetInt(PrefEnum.PLAYER_ID.ToString(), 0));
            api.SetToken(PlayerPrefs.GetString(PrefEnum.TOKEN.ToString(), ""));
        }
    }

    private void ArrangeScreen()
    {
        if (slotType == 1)
        {
            BGSR.sprite = BGSprite[slotType - 1];
            FGSR.sprite = FGSprite[slotType - 1];
            FrameSR[0].sprite = FrameSprite[slotType - 1];
            FrameSR[1].sprite = FrameSprite[slotType - 1];
            SpinButtonSR.sprite = SpinButtonSprite[slotType - 1];
            jackpotReel.gameObject.SetActive(true);
            AccesoriesGO.SetActive(true);
            tileSprite = tileSprite1;
            iconSprite = iconSprite1;
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].Init(1.88f, tileSprite, iconSprite);
                reels[i].transform.localPosition = new Vector3((i-2)*1.89f-3.12f, 0f, 0f);
            }
        }
        else if (slotType == 2)
        {
            BGSR.sprite = BGSprite[slotType - 1];
            FGSR.sprite = FGSprite[slotType - 1];
            FrameSR[0].sprite = FrameSprite[slotType - 1];
            FrameSR[1].sprite = FrameSprite[slotType - 1];
            SpinButtonSR.sprite = SpinButtonSprite[slotType - 1];
            jackpotReel.gameObject.SetActive(false);
            AccesoriesGO.SetActive(false);
            tileSprite = tileSprite2;
            iconSprite = iconSprite2;
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].Init(2.12f, tileSprite, iconSprite);
                reels[i].transform.localPosition = new Vector3((i - 2) * 2.13f - 3.12f, 1.3f, 0f);
            }
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
            if (reels[0].spin + reels[1].spin + reels[2].spin + reels[3].spin + reels[4].spin == 0 && !jackpotReel.GetSpin())
            {
                pushOk = true;
                freeSpinTM.gameObject.SetActive(false);
                if (pushAuto) OnMouseUp(ButtonScript.ButtonType.Spin);
            }
        }
    }

    public void OnMouseDown(ButtonScript.ButtonType type, SpriteRenderer sr = null)
    {
        if (type == ButtonScript.ButtonType.Spin)
        {
            pushAutoTimer = 0f;
            pushAuto = false;
            //sr.flipX = false;
            sr.sprite = SpinButtonSprite[slotType-1];
        }
    }

    public void OnMouseDrag(ButtonScript.ButtonType type, SpriteRenderer sr = null)
    {
        if (type == ButtonScript.ButtonType.Spin)
        {
            pushAutoTimer += Time.deltaTime;
            if(pushAutoTimer >= 1.5f && !pushAuto)
            {
                pushAuto = true;
                //sr.flipX = true;
                sr.sprite = StopButtonSprite[slotType - 1];
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
            _SceneManager.instance.SetActiveScene(SceneType.SHOP, true);
        }
        else if (type == ButtonScript.ButtonType.Help)
        {
            helpBG.gameObject.SetActive(true);
            helpFG.sprite = helpFGSprite[slotType-1];
        }
        else if (type == ButtonScript.ButtonType.Minimize)
        {
            helpBG.gameObject.SetActive(false);
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
            if (slot_spin.slot_matrix[i] >= 0) //Jackpot Type
            {
                slotIcons[i].SetIconValue(slot_spin.slot_matrix[i], iconSprite[slot_spin.slot_matrix[i] * reels[0].maxIconBlur + 0], tileSprite[0]);
            }
            else //No Jackpot Type
            {
                int tempIconValue = Random.Range(0, 6);
                slotIcons[i].SetIconValue(tempIconValue, iconSprite[tempIconValue], tileSprite[0]);
            }
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
        api.StartSlot(slotType, bet);
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
                freeSpinTM.text = (9 - jsonIndex == 1 ? "Last Free Spin" : "Free Spin x" + (9 - jsonIndex));
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
