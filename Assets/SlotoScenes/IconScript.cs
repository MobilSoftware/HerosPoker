using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconScript : MonoBehaviour
{
    public bool fixedValue;

    private SpriteRenderer iconSR;
    private int iconValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        iconSR = GetComponent<SpriteRenderer>();
    }

    public void SetIconValue(int value, Sprite iconSprite)
    {
        iconValue = value;
        if (iconSprite) SetSpriteRenderer(iconSprite);
    }

    public int GetIconValue()
    {
        return iconValue;
    }

    public void SetSpriteRenderer(Sprite iconSprite)
    {
        if (iconSR) iconSR.sprite = iconSprite;
    }

    public bool Spin(float speed)
    {
        transform.position += new Vector3(0.0f, speed, 0.0f);
        return (transform.localPosition.y <= -9.0f ? true : false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
