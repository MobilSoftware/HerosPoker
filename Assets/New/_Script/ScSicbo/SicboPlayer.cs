using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SicboPlayer : MonoBehaviour
{
    public Text textCoinValue;
    public Button btnReset;
    public Button[] btnBetValues;   //length = 4
    public Button[] btnBetTypes;    //length = 50
    public Image[] imgChips;        //length = 50
    public Text[] txtChipValues;    //length = 50
    public Sprite[] sprChips;       //length = 4
    public GameObject[] fxSelectedTypes;        //length = 50
    public GameObject[] fxWinTypes;     //length = 50
    public GameObject[] fxLoseTypes;    //length = 50

    [HideInInspector]
    public string strBets;
    [HideInInspector]
    public PhotonPlayer photonPlayer;

    private Dictionary<ApiBridge.SicboBetType, long> records;
    private Sprite sprChip;
    private long betValue;
    private ApiBridge.SicboBetType betType;

    private void Start ()
    {
        if (btnBetValues != null)
        {
            btnReset.onClick.AddListener (ResetBet);
            for (int a = 0; a < btnBetValues.Length; a++)
            {
                btnBetValues[a].onClick.AddListener (() => OnBetValue (a));
            }
            for (int i = 0; i < btnBetTypes.Length; i++)
            {
                btnBetTypes[i].onClick.AddListener (() => OnBetType (i));
            }
        }
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
        sprChip = sprChips[btnIndex];
    }

    private void OnBetType ( int btnIndex )
    {
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

        imgChips[btnIndex].sprite = sprChip;
        imgChips[btnIndex].gameObject.SetActive (true);

        RecordBet ();
    }

    private void RecordBet ()
    {
        if (records.ContainsKey (betType))
        {
            records[betType] += betValue;
        }
        else
        {
            records.Add (betType, betValue);
        }

        betValue = 0;
        betType = ApiBridge.SicboBetType.None;
    }

    private void SubmitRecords ()
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
        //call SicboManager method to pass strBets to master
    }

    private void ResetBet ()
    {
        records.Clear ();
        for (int i = 0; i < 50; i++)
        {
            fxSelectedTypes[i].gameObject.SetActive (false);
            txtChipValues[i].text = string.Empty;
            imgChips[i].gameObject.SetActive (false);
        }
    }

    private void HideWinLose ()
    {
        for (int i = 0; i < 50; i++)
        {
            fxWinTypes[i].gameObject.SetActive (false);
            fxLoseTypes[i].gameObject.SetActive (false);
        }
    }
}
