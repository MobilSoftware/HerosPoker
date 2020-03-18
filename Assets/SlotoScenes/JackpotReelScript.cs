using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackpotReelScript : MonoBehaviour
{
    public SlotoManagerScript slotoManager;
    public SpriteRenderer[] jackpots;
    public Sprite[] jackpotSprite;

    private bool spin;
    private int jackpotValue;
    private int displayValue;
    private int animLoop;
    private float displayNow;
    private readonly float downSpeed = 25.0f;
    private readonly float iconWidth = 0.9f;
    private readonly float displayTime = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        spin = false;
        jackpotValue = 0;
        displayValue = 0;
        animLoop = 0;
        displayNow = 0f;
    }

    public bool GetSpin()
    {
        return spin;
    }

    public void Spin(int value)
    {
        spin = true;
        jackpotValue = value;
        animLoop = Random.Range(1, 3);
    }

    private void StopSpin()
    {
        spin = false;
        StartCoroutine(StopAnim());
    }

    IEnumerator StopAnim()
    {
        slotoManager.UpdateMoney();
        for (int i = 0; i < 3; i++)
        {
            jackpots[0].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            jackpots[0].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (spin)
        {
            /*for (int i=0; i<jackpots.Length; i++)
            {
                jackpots[i].transform.localPosition -= new Vector3(0.0f, Time.deltaTime * downSpeed, 0.0f);
                if(jackpots[i].transform.localPosition.y <= -8.0f)
                {
                    jackpots[i].transform.localPosition = jackpots[(i+1)% jackpots.Length].transform.localPosition + new Vector3(0.0f, iconWidth, 0.0f);
                    if(jackpotValue == i) animLoop -= 1;
                }
            }
            if(animLoop <= 0)
            {
                if(Mathf.Abs(jackpots[jackpotValue].transform.localPosition.y) <= iconWidth / 2.0f)
                {
                    spin = false;
                    int temp = jackpotValue; 
                    for (int i = jackpotValue; i < jackpotValue + 9; i++)
                    {
                        temp = i;
                        jackpots[temp % jackpots.Length].transform.localPosition = new Vector3(0.0f, (jackpotValue - temp) * iconWidth, 0.0f);
                    }
                    float temp2 = 6.3f;
                    for (int i = 1; i <= jackpots.Length - 9; i++)
                    {
                        jackpots[(temp + i) % jackpots.Length].transform.localPosition = new Vector3(0.0f, temp2 - ((i-1) * iconWidth), 0.0f);
                    }
                }
            }*/
            displayNow += Time.deltaTime;
            if(displayNow >= displayTime)
            {
                displayNow = 0f;
                displayValue = (displayValue + 1) % jackpotSprite.Length;
                jackpots[0].sprite = jackpotSprite[displayValue];
                if (displayValue == jackpotValue)
                {
                    animLoop -= 1;
                    if (animLoop < 0) StopSpin();
                }
            }
        }
    }
}
