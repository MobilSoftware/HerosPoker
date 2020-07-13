using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReel : MonoBehaviour
{
    public int symbolHeight;
    public float reelSpeed = 3000;
    public List<ItemSymbol> symbols = new List<ItemSymbol> ();  //length = 5
    public List<float> trOriginal = new List<float> ();
    public GameObject objReel;

    private string strResult;       // == sprResult
    private bool inSpin;
    private int spinStatus;     //0 = not spinning, 1 = spin done goto arrange, 2 = arrange done goto reel animation (lean tween)
    private int count;
    private bool hasReplied;
    private int maxCount;
    [HideInInspector]
    public bool isValid = true;

    public void SpinReel ( int counter )
    {
        inSpin = true;
        maxCount = counter;
    }

    //called when server successfully replied
    public void SetReplied ()
    {
        hasReplied = true;
    }

    //called in error handler
    public void ResetReel ()
    {
        spinStatus = 0;
        inSpin = false;
    }

    public void SetInitialState ()
    {
        symbols[1].SetText ("$");
    }

    public void SetStringResult (int integer )
    {
        strResult = integer.ToString ();
    }

    private void Update ()
    {
        if (inSpin)
        {
            switch (spinStatus)
            {
                case 0:
                    foreach (ItemSymbol symbol in symbols)
                    {
                        float currentReelSpeed = reelSpeed * Time.deltaTime;
                        symbol.transform.Translate (Vector3.down * currentReelSpeed);

                        if (symbol.transform.localPosition.y <= trOriginal[trOriginal.Count - 1])
                        {
                            Vector3 currentPosition = symbol.transform.localPosition;
                            currentPosition.y = (currentPosition.y) + (symbols.Count - 1) * symbolHeight;
                            symbol.transform.localPosition = currentPosition;

                            if (symbol == symbols[0] && hasReplied)
                            {
                                count++;
                                if (count >= (5 + maxCount))
                                {
                                    spinStatus = 1;
                                    count = 0;
                                    hasReplied = false;
                                }
                            }
                            else if (symbol == symbols[1] && count == 1)
                            {
                                //setting to true result
                                symbol.SetText (strResult);
                            }
                            else if (symbol == symbols[0] || symbol == symbols[2])
                            {
                                symbol.RandomText ();
                            }
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < symbols.Count; i++)
                    {
                        symbols[i].transform.localPosition = new Vector3 (0f, trOriginal[i], 0f);
                    }
                    spinStatus = 2;
                    break;
                case 2:
                    float defLocalY = objReel.transform.localPosition.y;
                    LeanTween.moveLocalY (objReel, defLocalY - 75, 0.2f).setOnComplete (
                    () => LeanTween.moveLocalY (objReel, defLocalY, 0.05f).setEaseOutBounce ().setDelay (0.05f));
                    spinStatus = 0;
                    inSpin = false;
                    MoneySlotManager.instance.CheckShowResult (this);
                    break;
            }
        }
    }
}
