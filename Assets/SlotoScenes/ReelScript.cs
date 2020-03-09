using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelScript : MonoBehaviour
{
    public IconScript[] icons;

    private Sprite[] iconSprite;

    public int spin;

    private float speed;
    private int speedBlur;
    private float gear;
    private readonly int maxIconType = 8;
    public readonly int maxIconBlur = 5;
    private readonly float upSpeed = 50.0f;
    private readonly float downSpeed = 25.0f;
    private readonly float maxSpeed = -25.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(Sprite[] iconS)
    {
        spin = 0;
        speed = 0.0f;
        speedBlur = 0;
        gear = Mathf.Floor(maxSpeed / maxIconBlur-1);

        iconSprite = iconS;
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].Init();
            int tempValue = Random.Range(0, maxIconType);
            icons[i].SetIconValue(tempValue, iconSprite[tempValue * maxIconBlur + speedBlur]);
        }
    }

    public void StartSpin()
    {
        spin = 1;
        speed = 10.0f;
    }

    public void StopSpin()
    {
        spin = 3;
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        int tempMoveIcon = -1;
        if (spin == 1 || spin == 2)
        {
            for (i = 0; i < icons.Length; i++)
            {
                if (icons[i].Spin(speed * Time.deltaTime)) tempMoveIcon = i;
            }
            if (tempMoveIcon >= 0)
            {
                icons[tempMoveIcon].transform.localPosition += new Vector3(0.0f, icons.Length * 1.5f, 0.0f);
                if (!icons[tempMoveIcon].fixedValue)
                {
                    int tempValue = Random.Range(0, maxIconType);
                    icons[tempMoveIcon].SetIconValue(tempValue, iconSprite[tempValue * maxIconBlur + speedBlur]);
                }
            }
            if(speed != maxSpeed)
            {
                speed = Mathf.Max(speed - (speed >= 0 ? upSpeed : downSpeed) * Time.deltaTime, maxSpeed);
                int tempSpeedBlur = Mathf.FloorToInt(Mathf.Max(0, -speed) / -gear);
                if (tempSpeedBlur != speedBlur)
                {
                    speedBlur = tempSpeedBlur;
                    for (i = 0; i < icons.Length; i++)
                    {
                        icons[i].SetSpriteRenderer(iconSprite[icons[i].GetIconValue() * maxIconBlur + speedBlur]);
                    }
                }
            }
            else
            {
                spin = 2;
            }
        }
        else if (spin == 3)
        {
            for (i = 0; i < icons.Length; i++)
            {
                if (icons[i].Spin(speed * Time.deltaTime)) tempMoveIcon = i;
            }
            if (tempMoveIcon >= 0)
            {
                icons[tempMoveIcon].transform.localPosition += new Vector3(0.0f, icons.Length * 1.5f, 0.0f);
                if (tempMoveIcon == 0) spin = 4;
            }
        }
        else if (spin == 4)
        {
            for (i = 0; i < icons.Length; i++)
            {
                icons[i].Spin(speed * Time.deltaTime);
            }
            if (icons[0].transform.localPosition.y <= Random.Range(-0.30f, -0.70f)) spin = 5;
        }
        else if (spin == 5)
        {
            for (i = 0; i < icons.Length; i++)
            {
                if (icons[0].transform.localPosition.y <= 0.0f)
                {
                    icons[i].Spin(upSpeed * 0.1f * Time.deltaTime);
                }
                else
                {
                    spin = 6;
                }
            }
        }
        else if (spin == 6)
        {
            for (i = 0; i < icons.Length; i++)
            {
                icons[i].transform.localPosition = new Vector3(0.0f, i * -1.5f, 0.0f);
                icons[i].SetSpriteRenderer(iconSprite[icons[i].GetIconValue() * maxIconBlur + 0]);
            }
            spin = 0;
        }
    }
}
