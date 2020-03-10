using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackpotReelScript : MonoBehaviour
{
    public SpriteRenderer[] jackpots;

    private bool spin;
    private int jackpotValue;
    private int animLoop;
    private readonly float downSpeed = 25.0f;
    private readonly float iconWidth = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        spin = false;
        jackpotValue = 0;
        animLoop = 0;
    }

    public bool GetSpin()
    {
        return spin;
    }

    public void Spin(int value)
    {
        spin = true;
        jackpotValue = value;
        animLoop = Random.Range(3, 6);
    }

    // Update is called once per frame
    void Update()
    {
        if (spin)
        {
            for (int i=0; i<jackpots.Length; i++)
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
            }
        }
    }
}
