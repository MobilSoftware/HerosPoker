using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconScript : MonoBehaviour
{
    public SpriteRenderer tileSR;
    public SpriteRenderer faceSR;
    public bool fixedValue;

    private int iconValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        //iconSR = GetComponent<SpriteRenderer>();
    }

    public void SetIconValue(int value, Sprite iconSprite, Sprite tileSprite)
    {
        iconValue = value;
        if (iconSprite) SetSpriteRenderer(iconSprite, tileSprite);
    }

    public int GetIconValue()
    {
        return iconValue;
    }

    public void SetSpriteRenderer(Sprite iconSprite, Sprite tileSprite)
    {
        if (faceSR) faceSR.sprite = iconSprite;
        if (tileSR) tileSR.sprite = tileSprite;
    }

    public bool Spin(float speed)
    {
        transform.position += new Vector3(0.0f, speed, 0.0f);
        return (transform.localPosition.y <= -10.0f ? true : false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
