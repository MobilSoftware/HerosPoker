//using System.Collections.Generic;
//using UnityEngine;

//public enum WinType
//{
//    Normal,
//    Minor,
//    Grand
//}

//public class Reel : MonoBehaviour
//{
//    public bool isCenter;
//    public bool isTop;
//    public int symbolHeight;
//    public GameObject fxSlotRow;
//    public GameObject fxSlotRowEnd;
//    public float reelSpeed = 1000;

//    public List<ItemSlot> symbols = new List<ItemSlot> ();
//    public List<float> trOriginal = new List<float> ();
//    public GameObject[] fxIconsWinNormal;   //length = 3, 0 = top, 1 = mid, 2 = bottom, 3 = center top
//    public GameObject[] fxIconsWinMinor;    //length = 3, 0 = top, 1 = mid, 2 = bottom, 3 = center top
//    public GameObject[] fxIconsWinGrand;    //length = 3, 0 = top, 1 = mid, 2 = bottom, 3 = center top
//    public GameObject objReel;

//    private Sprite[] sprResults = new Sprite[4];
//    public Sprite sprBonus;

//    private bool inSpin;
//    private int spinStatus;     //0 = not spinning, 1 = spin done goto arrange, 2 = arrange done goto reel animation (lean tween)
//    private int count;
//    private bool hasReplied;
//    public bool isValid = true;
//    public bool hasDragon;
//    public int delayCounter;
//    private int maxCount;

//    public void SpinReel (int counter)
//    {
//        inSpin = !inSpin;
//        maxCount = counter;
//    }

//    public void StopReel ()
//    {
//        inSpin = false;
//    }

//    //called when server successfully replied
//    public void SetReplied ()
//    {
//        hasReplied = true;
//    }

//    //called in error handler
//    public void ResetReel ()
//    {
//        spinStatus = 0;
//        inSpin = false;
//    }

//    public void SetInitialSprites (Sprite[] _sprites )
//    {
//        for (int i = 0; i < symbols.Count; i++)
//        {
//            symbols[i].ChangeIcon (_sprites[i]);
//        }
//    }

//    public void SetSpriteResults (Sprite top, Sprite mid, Sprite bot, Sprite topCenter)
//    {
//        sprResults[0] = top;
//        sprResults[1] = mid;
//        sprResults[2] = bot;
//        sprResults[3] = topCenter;
//    }

//    public void SetSpriteBonus (Sprite _sprBonus )
//    {
//        sprBonus = _sprBonus;
//    }

//    public void ChangeSpriteBonus ()
//    {
//        symbols[3].ChangeIcon (sprBonus);
//    }

//    public void HideFxIconSpecific (int index )
//    {
//        if (index == 3 && !isCenter)
//            return;

//        fxSlotRowEnd.SetActive (false);
//        fxIconsWinGrand[index].SetActive (false);
//        fxIconsWinMinor[index].SetActive (false);
//        fxIconsWinNormal[index].SetActive (false);
//    }

//    public void ShowFxIcon (int index, WinType _wt )
//    {
//        //index = 0 top, 1 mid, 2 bottom, 3 = center top
//        if (index == 3 && !isCenter)
//            return;

//        fxSlotRowEnd.SetActive (true);
//        switch (_wt)
//        {
//            case WinType.Normal:
//                fxIconsWinNormal[index].SetActive (true);
//                break;
//            case WinType.Minor:
//                fxIconsWinMinor[index].SetActive (true);
//                break;
//            case WinType.Grand:
//                fxIconsWinGrand[index].SetActive (true);
//                break;
//        }

//        if (index != 3)
//        {
//            symbols[index + 2].PlayShiny ();
//            LeanTween.scale (symbols[index + 2].gameObject, new Vector3 (1.2f, 1.2f, 1.2f), 0.15f).setEaseOutBounce ();
//        }
//        else
//        {
//            symbols[1].PlayShiny ();
//            LeanTween.scale (symbols[1].gameObject, new Vector3 (1.2f, 1.2f, 1.2f), 0.15f).setEaseOutBounce ();
//        }
//    }

//    public void RevertScale ()
//    {
//        LeanTween.scale (symbols[2].gameObject, Vector3.one, 0.05f).setEaseOutBounce ();
//        LeanTween.scale (symbols[3].gameObject, Vector3.one, 0.05f).setEaseOutBounce ();
//        LeanTween.scale (symbols[4].gameObject, Vector3.one, 0.05f).setEaseOutBounce ();
//        symbols[2].Reset ();
//        symbols[3].Reset ();
//        symbols[4].Reset ();
//        if (isCenter)
//        {
//            LeanTween.scale (symbols[1].gameObject, Vector3.one, 0.05f).setEaseOutBounce ();
//            symbols[1].Reset ();
//        }
//    }

//    void Update ()
//    {
//        if (inSpin)
//        {
//            if (!isTop)
//                fxSlotRow.SetActive (true);
//            else
//            {
//                HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.fxSlotTop.SetActive (true);
//                SoundManager.instance.PlaySFX2Loop (SFXType.SFX_SlotTop);
//            }

//            switch (spinStatus) {
//                case 0:
//                    foreach (ItemSlot symbol in symbols)
//                    {
//                        float currentReelSpeed = reelSpeed * Time.deltaTime;
//                        symbol.transform.Translate (Vector3.down * currentReelSpeed);

//                        if (symbol.transform.localPosition.y <= trOriginal[trOriginal.Count - 1])
//                        {
//                            Vector3 currentPosition = symbol.transform.localPosition;
//                            currentPosition.y = (currentPosition.y) + (symbols.Count - 1) * symbolHeight;
//                            symbol.transform.localPosition = currentPosition;

//                            if (symbol == symbols[0] && !isValid)
//                            {
//                                count = 0;
//                                spinStatus = 1;
//                            }

//                            if (symbol == symbols[0] && hasReplied)
//                            {
//                                count++;
//                                if (count >= (5 + maxCount + delayCounter))
//                                {
//                                    spinStatus = 1;
//                                    count = 0;
//                                    hasReplied = false;
//                                }
//                            } else if (symbol == symbols[1] && count == 1 && isCenter)
//                            {
//                                //changing symbol
//                                symbol.ChangeIcon ( sprResults[3]);
//                            } else if (symbol == symbols[3] && count == 1 && isTop)
//                            {
//                                //changing symbol
//                                symbol.ChangeIcon (sprBonus);
//                            } else if (symbol == symbols[2] && count == 1 && !isTop)
//                            {
//                                //changing symbol
//                                symbol.ChangeIcon ( sprResults[0]);
//                            } else if (symbol == symbols[3] && count == 1 && !isTop)
//                            {
//                                //changing symbol
//                                symbol.ChangeIcon ( sprResults[1]);
//                            } else if (symbol == symbols[4] && count == 1 && !isTop)
//                            {
//                                //changing symbol
//                                symbol.ChangeIcon ( sprResults[2]);
//                            }
//                        }
//                    }
//                    break;
//                case 1:
//                    for (int i = 0; i < symbols.Count; i++)
//                    {
//                        symbols[i].transform.localPosition = new Vector3 (0f, trOriginal[i], 0f);
//                    }
//                    if (isValid)
//                        spinStatus = 2;
//                    else
//                        isValid = true;
//                    break;
//                case 2:
//                    float defLocalY = objReel.transform.localPosition.y;
//                    LeanTween.moveLocalY (objReel, defLocalY - 75, 0.2f).setOnComplete (
//                    () => LeanTween.moveLocalY (objReel, defLocalY, 0.05f).setEaseOutBounce ().setDelay (0.05f).setOnComplete (
//                        () => PlayBonus()));
//                    spinStatus = 0;
//                    inSpin = false;
//                    break;
//            }
//        }
//    }

//    private void PlayBonus ()
//    {
//        if (!isTop)
//        {
//            fxSlotRow.SetActive (false);
//        }
//        else
//        {
//            HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.fxSlotTop.SetActive (false);
//            SoundManager.instance.StopSFX2 ();
//        }

//        SoundManager.instance.PlaySFX (SFXType.SFX_SlotSegment, Vector3.zero);
//        if (this == HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.reels[4])
//        {
//            HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.StartBonus ();
//        } else if (this == HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.topReel)
//        {
//            HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.BonusEnd ();
//        }

//        if (hasDragon)
//        {
//            SoundManager.instance.PlaySFX (SFXType.SFX_SlotDragonA, Vector3.zero);
//        }

//        if (this == HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.reels[1] && HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.reels[0].hasDragon && hasDragon)
//        {
//            SoundManager.instance.PlaySFX2Loop (SFXType.SFX_SlotDragonB);
//        }
//        else if (this == HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.reels[2] && delayCounter != 5)
//        {
//            SoundManager.instance.StopSFX2 ();
//        } else if (this == HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.reels[3] && delayCounter != 10)
//        {
//            SoundManager.instance.StopSFX2 ();
//        } else if (this == HomeSceneManager.Instance.myHomeMenuReference.uiSlotMachine.reels[4] && delayCounter != 15)
//        {
//            SoundManager.instance.StopSFX2 ();
//        }

//        delayCounter = 0;
//    }
//}
