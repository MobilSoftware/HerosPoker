using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelScript : MonoBehaviour
{
    public SlotoManagerScript slotoManager;
    public IconScript[] icons;

    private Sprite[] tileSprite;
    private Sprite[] iconSprite;

    public int spin;

    private float speed;
    private int speedBlur;
    private float gear;
    public readonly int maxIconType = 14;
    public readonly int maxIconBlur = 2;
    private readonly float upSpeed = 50.0f;
    private readonly float downSpeed = 25.0f;
    private readonly float maxSpeed = -25.0f;
    private float iconWidth = 1.88f;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(float _iconWidth, Sprite[] tileS, Sprite[] iconS)
    {
        iconWidth = _iconWidth;
        spin = 0;
        speed = 0.0f;
        speedBlur = 0;
        gear = Mathf.Floor(maxSpeed / maxIconBlur-1);

        tileSprite = tileS;
        iconSprite = iconS;
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].Init(iconWidth * 5.5f);
            int tempValue = Random.Range(0, maxIconType);
            icons[i].SetIconValue(tempValue, iconSprite[tempValue * maxIconBlur + 0], tileSprite[0]);
            icons[i].transform.localPosition = new Vector3(0f, -iconWidth*i, 0f);
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
        if (spin == 1 || spin == 2) //1. Accelerate, 2. Steady
        {
            for (i = 0; i < icons.Length; i++)
            {
                if (icons[i].Spin(speed * Time.deltaTime)) tempMoveIcon = i;
            }
            if (tempMoveIcon >= 0)
            {
                icons[tempMoveIcon].transform.localPosition += new Vector3(0.0f, icons.Length * iconWidth, 0.0f);
                if (!icons[tempMoveIcon].fixedValue)
                {
                    int tempValue = Random.Range(0, maxIconType);
                    icons[tempMoveIcon].SetIconValue(tempValue, iconSprite[tempValue * maxIconBlur + speedBlur], tileSprite[(speedBlur==0?0:1)]);
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
                        icons[i].SetSpriteRenderer(iconSprite[icons[i].GetIconValue() * maxIconBlur + speedBlur], tileSprite[(speedBlur == 0 ? 0 : 1)]);
                    }
                }
            }
            else if (spin == 1)
            {
                spin = 2;
            }
        }
        else if (spin == 3) //3. Set to Stop
        {
            for (i = 0; i < icons.Length; i++)
            {
                if (icons[i].Spin(speed * Time.deltaTime)) tempMoveIcon = i;
            }
            if (tempMoveIcon >= 0)
            {
                icons[tempMoveIcon].transform.localPosition += new Vector3(0.0f, icons.Length * iconWidth, 0.0f);
                if (tempMoveIcon == 0) spin = 4;
            }
        }
        else if (spin == 4) //4. Icon position correct
        {
            for (i = 0; i < icons.Length; i++)
            {
                icons[i].Spin(speed * Time.deltaTime);
            }
            if (icons[0].transform.localPosition.y < Random.Range(-0.3f, -0.6f)) spin = 5;
        }
        else if (spin == 5) //5. Stop position is over and need correction
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
        else if (spin == 6) //6. Stop position is correct
        {
            for (i = 0; i < icons.Length; i++)
            {
                icons[i].transform.localPosition = new Vector3(0.0f, i * -iconWidth, 0.0f);
                icons[i].SetSpriteRenderer(iconSprite[icons[i].GetIconValue() * maxIconBlur + 0], tileSprite[0]);
            }
            spin = 0;
            slotoManager.UpdateSlotSpin();
        }
    }
}
