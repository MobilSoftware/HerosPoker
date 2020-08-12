using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SicboPlayer : MonoBehaviour
{
    //public Text textDisplayName;
    public Text textCoinValue;
    public Text textCoinDiff;
    public Button btnReset;
    public Button[] btnBetValues;   //length = 4
    public Image[] imgBtnBets;      //length = 4
    public Button[] btnBetTypes;    //length = 50
    public Image[] imgChips;        //length = 50
    public Text[] txtChipValues;    //length = 50
    public Sprite[] sprChips;       //length = 4
    public GameObject[] fxSelectedTypes;        //length = 50
    public GameObject[] fxWinTypes;     //length = 50
    public GameObject[] fxLoseTypes;    //length = 50
    public Transform[] parentChips;     //length = 50
    public Image prefabChips;

    [HideInInspector]
    public SicboParasite parasite;
    [HideInInspector]
    public string strBets;
    [HideInInspector]
    public int slotIndex;
    [HideInInspector]
    public bool isMine;
    [HideInInspector]
    public string displayName;
    [HideInInspector]
    public long coinOwned;
    [HideInInspector]
    public PhotonPlayer photonPlayer;

    private Dictionary<ApiBridge.SicboBetType, long> records;
    private Sprite sprChip;
    private long betValue;
    private ApiBridge.SicboBetType betType;
    private long totalBet;      //just for syncing when leavin before master receive submit record
    private Coroutine crSetTextCoinDiff;

    private void Start ()
    {
        records = new Dictionary<ApiBridge.SicboBetType, long> ();
        if (btnReset != null)
        {
            btnReset.onClick.AddListener (OnReset);
            btnBetValues[0].onClick.AddListener (() => OnBetValue (0));
            btnBetValues[1].onClick.AddListener (() => OnBetValue (1));
            btnBetValues[2].onClick.AddListener (() => OnBetValue (2));
            btnBetValues[3].onClick.AddListener (() => OnBetValue (3));

            btnBetTypes[0].onClick.AddListener (() => OnBetType (0));
            btnBetTypes[1].onClick.AddListener (() => OnBetType (1));
            btnBetTypes[2].onClick.AddListener (() => OnBetType (2));
            btnBetTypes[3].onClick.AddListener (() => OnBetType (3));
            btnBetTypes[4].onClick.AddListener (() => OnBetType (4));
            btnBetTypes[5].onClick.AddListener (() => OnBetType (5));
            btnBetTypes[6].onClick.AddListener (() => OnBetType (6));
            btnBetTypes[7].onClick.AddListener (() => OnBetType (7));
            btnBetTypes[8].onClick.AddListener (() => OnBetType (8));
            btnBetTypes[9].onClick.AddListener (() => OnBetType (9));
            btnBetTypes[10].onClick.AddListener (() => OnBetType (10));
            btnBetTypes[11].onClick.AddListener (() => OnBetType (11));
            btnBetTypes[12].onClick.AddListener (() => OnBetType (12));
            btnBetTypes[13].onClick.AddListener (() => OnBetType (13));
            btnBetTypes[14].onClick.AddListener (() => OnBetType (14));
            btnBetTypes[15].onClick.AddListener (() => OnBetType (15));
            btnBetTypes[16].onClick.AddListener (() => OnBetType (16));
            btnBetTypes[17].onClick.AddListener (() => OnBetType (17));
            btnBetTypes[18].onClick.AddListener (() => OnBetType (18));
            btnBetTypes[19].onClick.AddListener (() => OnBetType (19));
            btnBetTypes[20].onClick.AddListener (() => OnBetType (20));
            btnBetTypes[21].onClick.AddListener (() => OnBetType (21));
            btnBetTypes[22].onClick.AddListener (() => OnBetType (22));
            btnBetTypes[23].onClick.AddListener (() => OnBetType (23));
            btnBetTypes[24].onClick.AddListener (() => OnBetType (24));
            btnBetTypes[25].onClick.AddListener (() => OnBetType (25));
            btnBetTypes[26].onClick.AddListener (() => OnBetType (26));
            btnBetTypes[27].onClick.AddListener (() => OnBetType (27));
            btnBetTypes[28].onClick.AddListener (() => OnBetType (28));
            btnBetTypes[29].onClick.AddListener (() => OnBetType (29));
            btnBetTypes[30].onClick.AddListener (() => OnBetType (30));
            btnBetTypes[31].onClick.AddListener (() => OnBetType (31));
            btnBetTypes[32].onClick.AddListener (() => OnBetType (32));
            btnBetTypes[33].onClick.AddListener (() => OnBetType (33));
            btnBetTypes[34].onClick.AddListener (() => OnBetType (34));
            btnBetTypes[35].onClick.AddListener (() => OnBetType (35));
            btnBetTypes[36].onClick.AddListener (() => OnBetType (36));
            btnBetTypes[37].onClick.AddListener (() => OnBetType (37));
            btnBetTypes[38].onClick.AddListener (() => OnBetType (38));
            btnBetTypes[39].onClick.AddListener (() => OnBetType (39));
            btnBetTypes[40].onClick.AddListener (() => OnBetType (40));
            btnBetTypes[41].onClick.AddListener (() => OnBetType (41));
            btnBetTypes[42].onClick.AddListener (() => OnBetType (42));
            btnBetTypes[43].onClick.AddListener (() => OnBetType (43));
            btnBetTypes[44].onClick.AddListener (() => OnBetType (44));
            btnBetTypes[45].onClick.AddListener (() => OnBetType (45));
            btnBetTypes[46].onClick.AddListener (() => OnBetType (46));
            btnBetTypes[47].onClick.AddListener (() => OnBetType (47));
            btnBetTypes[48].onClick.AddListener (() => OnBetType (48));
            btnBetTypes[49].onClick.AddListener (() => OnBetType (49));
        }
    }

    public void Initiate (int _slotIndex, bool _isMine)
    {
        slotIndex = _slotIndex;
        isMine = _isMine;
        strBets = string.Empty;

        ExitGames.Client.Photon.Hashtable properties = photonPlayer.CustomProperties;
        displayName = (string) properties[PhotonEnums.Player.Name];
        coinOwned = (long) properties[PhotonEnums.Player.Money];
        //textDisplayName.text = displayName;
        textCoinValue.text = coinOwned.toShortCurrency ();
        PhotonUtility.SetPlayerProperties (photonPlayer, PhotonEnums.Player.Money, coinOwned);
    }

    private void OnBetValue ( int btnIndex )
    {
        switch (btnIndex)
        {
            case 0: betValue = 100; break;
            case 1: betValue = 200; break;
            case 2: betValue = 500; break;
            case 3: betValue = 1000; break;
        }

        SetDimmerBetChip(btnIndex);

        if (betValue > PlayerData.owned_coin)
        {
            betValue = 0;
            MessageManager.instance.Show (this.gameObject, "Koin anda tidak mencukupi");
            return;
        }
        sprChip = sprChips[btnIndex];
    }

    private void SetDimmerBetChip (int indexSelectedBtn)
    {
        if (indexSelectedBtn == -1)
        {
            for (int i = 0; i < imgBtnBets.Length; i++)
            {
                imgBtnBets[i].color = Color.white;
            }
        } else
        {
            for (int i = 0; i < imgBtnBets.Length; i++)
            {
                if (i != indexSelectedBtn)
                {
                    imgBtnBets[i].color = Color.gray;
                } else
                {
                    imgBtnBets[i].color = Color.white;
                }
            }
        }
    }

    private void OnBetType ( int btnIndex )
    {
        if (betValue == 0)
            return;

        if (!SicboManager.instance.bCanBet)
        {
            MessageManager.instance.Show (this.gameObject, "Waktu pemasangan chip telah ditutup, tunggu ronde selanjutnya");
            return;
        }

        switch (btnIndex)
        {
            case 0: betType = ApiBridge.SicboBetType.Small; break;
            case 1: betType = ApiBridge.SicboBetType.Double1; break;
            case 2: betType = ApiBridge.SicboBetType.Double2; break;
            case 3: betType = ApiBridge.SicboBetType.Double3; break;
            case 4: betType = ApiBridge.SicboBetType.Triple1; break;
            case 5: betType = ApiBridge.SicboBetType.Triple2; break;
            case 6: betType = ApiBridge.SicboBetType.Triple3; break;
            case 7: betType = ApiBridge.SicboBetType.TripleAny; break;
            case 8: betType = ApiBridge.SicboBetType.Triple4; break;
            case 9: betType = ApiBridge.SicboBetType.Triple5; break;
            case 10: betType = ApiBridge.SicboBetType.Triple6; break;
            case 11: betType = ApiBridge.SicboBetType.Double4; break;
            case 12: betType = ApiBridge.SicboBetType.Double5; break;
            case 13: betType = ApiBridge.SicboBetType.Double6; break;
            case 14: betType = ApiBridge.SicboBetType.Big; break;
            case 15: betType = ApiBridge.SicboBetType.Dadu4; break;
            case 16: betType = ApiBridge.SicboBetType.Dadu5; break;
            case 17: betType = ApiBridge.SicboBetType.Dadu6; break;
            case 18: betType = ApiBridge.SicboBetType.Dadu7; break;
            case 19: betType = ApiBridge.SicboBetType.Dadu8; break;
            case 20: betType = ApiBridge.SicboBetType.Dadu9; break;
            case 21: betType = ApiBridge.SicboBetType.Dadu10; break;
            case 22: betType = ApiBridge.SicboBetType.Dadu11; break;
            case 23: betType = ApiBridge.SicboBetType.Dadu12; break;
            case 24: betType = ApiBridge.SicboBetType.Dadu13; break;
            case 25: betType = ApiBridge.SicboBetType.Dadu14; break;
            case 26: betType = ApiBridge.SicboBetType.Dadu15; break;
            case 27: betType = ApiBridge.SicboBetType.Dadu16; break;
            case 28: betType = ApiBridge.SicboBetType.Dadu17; break;
            case 29: betType = ApiBridge.SicboBetType.Couple12; break;
            case 30: betType = ApiBridge.SicboBetType.Couple13; break;
            case 31: betType = ApiBridge.SicboBetType.Couple14; break;
            case 32: betType = ApiBridge.SicboBetType.Couple15; break;
            case 33: betType = ApiBridge.SicboBetType.Couple16; break;
            case 34: betType = ApiBridge.SicboBetType.Couple23; break;
            case 35: betType = ApiBridge.SicboBetType.Couple24; break;
            case 36: betType = ApiBridge.SicboBetType.Couple25; break;
            case 37: betType = ApiBridge.SicboBetType.Couple26; break;
            case 38: betType = ApiBridge.SicboBetType.Couple34; break;
            case 39: betType = ApiBridge.SicboBetType.Couple35; break;
            case 40: betType = ApiBridge.SicboBetType.Couple36; break;
            case 41: betType = ApiBridge.SicboBetType.Couple45; break;
            case 42: betType = ApiBridge.SicboBetType.Couple46; break;
            case 43: betType = ApiBridge.SicboBetType.Couple56; break;
            case 44: betType = ApiBridge.SicboBetType.Single1; break;
            case 45: betType = ApiBridge.SicboBetType.Single2; break;
            case 46: betType = ApiBridge.SicboBetType.Single3; break;
            case 47: betType = ApiBridge.SicboBetType.Single4; break;
            case 48: betType = ApiBridge.SicboBetType.Single5; break;
            case 49: betType = ApiBridge.SicboBetType.Single6; break;
        }

        fxSelectedTypes[btnIndex].SetActive (true);

        long chipValue = 0;
        if (txtChipValues[btnIndex].text != string.Empty)
            chipValue = txtChipValues[btnIndex].text.toLongCurrency ();
        chipValue += betValue;
        txtChipValues[btnIndex].text = chipValue.toShortCurrency ();

        //imgChips[btnIndex].sprite = sprChip;
        imgChips[btnIndex].gameObject.SetActive (true);
        SetChips (btnIndex, betValue);

        PlayerData.owned_coin -= betValue;
        totalBet += betValue;
        textCoinValue.text = PlayerData.owned_coin.toShortCurrency ();
        string strSendBets = betValue + "," + btnIndex;
        SicboManager.instance.SendPutOtherBets (strSendBets);
        RecordBet ();
    }

    public void PutOthersBet (string strOtherBets)      //strOtherBets = betAmount,btnIndex,playerID
    {
        string[] split = strOtherBets.Split (',');
        long betAmount = long.Parse (split[0]);
        int btnIndex = int.Parse (split[1]);

        //if (betAmount >= 1000)
        //    imgChips[btnIndex].sprite = sprChips[3];
        //else if (betAmount >= 500)
        //    imgChips[btnIndex].sprite = sprChips[2];
        //else if (betAmount >= 200)
        //    imgChips[btnIndex].sprite = sprChips[1];
        //else
        //    imgChips[btnIndex].sprite = sprChips[0];

        //imgChips[btnIndex].gameObject.SetActive (true);

        SetChips (btnIndex, betAmount);
    }

    private void RecordBet ()
    {
        if (betType == ApiBridge.SicboBetType.None)
        {
            Logger.E ("bet type is NONE");
            return;
        }

        if (records.ContainsKey (betType))
        {
            records[betType] += betValue;
        }
        else
        {
            records.Add (betType, betValue);
        }

        //betValue = 0;
        betType = ApiBridge.SicboBetType.None;
    }

    public void SubmitRecords ()
    {
        strBets = string.Empty;
        foreach (var bt in records.Keys)
        {
            if (bt == 0)
                continue;

            if (strBets == string.Empty)
                strBets = PlayerData.id + ";" + (int) bt + ";" + records[bt];
            else
                strBets += "|" + PlayerData.id + ";" + (int) bt + ";" + records[bt];
        }
        //strBets eg: 1001;4;200|1001;6;300
        parasite.SetSicboBets (strBets);
    }

    private void OnReset ()
    {
        MessageManager.instance.Show(this.gameObject, "Yakin ingin reset?", ButtonMode.OK_CANCEL, 1, "Reset", "Batal");
    }

    private void ResetBet (bool isButton = true)
    {
        if (isButton)
        {
            PlayerData.owned_coin += totalBet;
            textCoinValue.text = PlayerData.owned_coin.toShortCurrency();
        }
        totalBet = 0;
        records.Clear ();
        for (int i = 0; i < 50; i++)
        {
            fxSelectedTypes[i].gameObject.SetActive (false);
            txtChipValues[i].text = string.Empty;
            //imgChips[i].gameObject.SetActive (false);
        }

        ResetChipsImage ();
    }

    private void HideWinLose ()
    {
        for (int i = 0; i < 50; i++)
        {
            fxWinTypes[i].gameObject.SetActive (false);
            fxLoseTypes[i].gameObject.SetActive (false);
        }
    }

    public void CleanUp ()
    {
        ResetBet (false);
        HideWinLose ();
        SetDimmerBetChip(-1);
        betValue = 0;
        betType = ApiBridge.SicboBetType.None;
    }

    //public void SetOtherPlayerBets ()
    //{
    //    for (int i = 0; i < SicboManager.instance.apiPlayers.Length; i++)
    //    {
    //        ApiBridge.SicboPlayer p = SicboManager.instance.apiPlayers[i];
    //        int btnIndex = ConvertToButtonIndex (p.sicbo_type);
    //        if (p.coin_bet >= 1000)
    //            imgChips[btnIndex].sprite = sprChips[3];
    //        else if (p.coin_bet >= 500)
    //            imgChips[btnIndex].sprite = sprChips[2];
    //        else if (p.coin_bet >= 200)
    //            imgChips[btnIndex].sprite = sprChips[1];
    //        else
    //            imgChips[btnIndex].sprite = sprChips[0];

    //        imgChips[btnIndex].gameObject.SetActive (true);
    //    }

    //}

    private int ConvertToButtonIndex ( int _betType )
    {
        int btnIndex = 0;
        switch (_betType)
        {
            case 1: btnIndex = 44; break;
            case 2: btnIndex = 45; break;
            case 3: btnIndex = 46; break;
            case 4: btnIndex = 47; break;
            case 5: btnIndex = 48; break;
            case 6: btnIndex = 49; break;
            case 7: btnIndex = 1; break;
            case 8: btnIndex = 2; break;
            case 9: btnIndex = 3; break;
            case 10: btnIndex = 11; break;
            case 11: btnIndex = 12; break;
            case 12: btnIndex = 13; break;
            case 13: btnIndex = 4; break;
            case 14: btnIndex = 5; break;
            case 15: btnIndex = 6; break;
            case 16: btnIndex = 8; break;
            case 17: btnIndex = 9; break;
            case 18: btnIndex = 10; break;
            case 19: btnIndex = 7; break;
            case 20: btnIndex = 15; break;
            case 21: btnIndex = 16; break;
            case 22: btnIndex = 17; break;
            case 23: btnIndex = 18; break;
            case 24: btnIndex = 19; break;
            case 25: btnIndex = 20; break;
            case 26: btnIndex = 21; break;
            case 27: btnIndex = 22; break;
            case 28: btnIndex = 23; break;
            case 29: btnIndex = 24; break;
            case 30: btnIndex = 25; break;
            case 31: btnIndex = 26; break;
            case 32: btnIndex = 27; break;
            case 33: btnIndex = 28; break;
            case 34: btnIndex = 29; break;
            case 35: btnIndex = 30; break;
            case 36: btnIndex = 31; break;
            case 37: btnIndex = 32; break;
            case 38: btnIndex = 33; break;
            case 39: btnIndex = 34; break;
            case 40: btnIndex = 35; break;
            case 41: btnIndex = 36; break;
            case 42: btnIndex = 37; break;
            case 43: btnIndex = 38; break;
            case 44: btnIndex = 39; break;
            case 45: btnIndex = 40; break;
            case 46: btnIndex = 41; break;
            case 47: btnIndex = 42; break;
            case 48: btnIndex = 43; break;
            case 49: btnIndex = 0; break;
            case 50: btnIndex = 14; break;
        }
        return btnIndex;
    }

    public void ShowWinLoseTiles (bool bWin, int _betType )
    {
        int btnIndex = ConvertToButtonIndex (_betType);
        Logger.E ("btn index: " + btnIndex);
        if (bWin)
        {
            fxWinTypes[btnIndex].gameObject.SetActive (true);
            fxLoseTypes[btnIndex].gameObject.SetActive (false);
        }
        else
        {
            fxWinTypes[btnIndex].gameObject.SetActive (false);
            fxLoseTypes[btnIndex].gameObject.SetActive (true);
        }
    }

    public void SetChips (int btnIndex, long betAmount )
    {
        Image img = Instantiate (prefabChips, parentChips[btnIndex]);
        if (betAmount >= 1000)
            img.sprite = sprChips[3];
        else if (betAmount >= 500)
            img.sprite = sprChips[2];
        else if (betAmount >= 200)
            img.sprite = sprChips[1];
        else
            img.sprite = sprChips[0];

        int randX = UnityEngine.Random.Range (-30, 30);
        int randY = UnityEngine.Random.Range (-30, 30);

        switch (btnIndex)
        {
            case 4:
            case 5:
            case 6:
            case 8:
            case 9:
            case 10:
                randX = UnityEngine.Random.Range (-10, 10);
                randY = UnityEngine.Random.Range (-10, 10);
                break;

        }
        img.transform.localPosition = new Vector3 (randX, randY);
    }

    public void ResetChipsImage ()
    {
        for (int i = 0; i < parentChips.Length; i++)
        {
            if (parentChips[i].childCount > 0)
            {
                for (int j = 0; j < parentChips[i].childCount; j++)
                {
                    Destroy (parentChips[i].GetChild (j).gameObject);
                }
            }
        }
    }

    public void SetTextCoinDiff (long lCoinDiff)
    {
        if (crSetTextCoinDiff != null)
            StopCoroutine(crSetTextCoinDiff);
        crSetTextCoinDiff = StartCoroutine(_SetTextCoinDiff(lCoinDiff));
    }

    IEnumerator _SetTextCoinDiff (long lCoinDiff)
    {
        if (textCoinDiff != null)
        {
            lCoinDiff *= 1000;
            if (lCoinDiff > 0)
            {
                textCoinDiff.gameObject.SetActive(true);
                textCoinDiff.text = "+" + lCoinDiff.ToString ("N0");
            }
            else if (lCoinDiff < 0)
            {
                textCoinDiff.gameObject.SetActive(true);
                textCoinDiff.text = "-" + (lCoinDiff * (-1)).ToString ("N0");
            }

            yield return _WFSUtility.wfs3;

            textCoinDiff.gameObject.SetActive(false);
        }
    }

    private void OnPositiveClicked (int returnedCode)
    {
        switch (returnedCode)
        {
            case 1: ResetBet(); break;
        }
    }

    private void OnNegativeClicked (int returnedCode)
    {
        switch (returnedCode)
        {
            case 1: MessageManager.instance.Hide(); break;
        }
    }
}
