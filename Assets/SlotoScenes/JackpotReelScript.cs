using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackpotReelScript : MonoBehaviour
{
    public SlotoManagerScript slotoManager;
    public SpriteRenderer jackpotFG;
    public Sprite[] jackpotSprite;

    private bool spin;
    private int jackpotValue;
    private int displayValue;
    private int animLoop;
    private float displayNow;
    //private readonly float downSpeed = 25.0f;
    //private readonly float iconWidth = 0.9f;
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
            jackpotFG.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            jackpotFG.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (spin)
        {
            displayNow += Time.deltaTime;
            if(displayNow >= displayTime)
            {
                displayNow = 0f;
                displayValue = (displayValue + 1) % jackpotSprite.Length;
                jackpotFG.sprite = jackpotSprite[displayValue];
                if (displayValue == jackpotValue)
                {
                    animLoop -= 1;
                    if (animLoop < 0) StopSpin();
                }
            }
        }
    }
}
