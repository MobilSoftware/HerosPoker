using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCanvasSize : MonoBehaviour {

    public float desiredRatio = 1.7778f;

	void Start () {
        CanvasScaler cs = this.GetComponent<CanvasScaler>();
        int width = Screen.width;
        int height = Screen.height;
        float currentRatio = (float) (height / width);
        if (currentRatio > desiredRatio)
        {
            cs.matchWidthOrHeight = 0;
        } else if (currentRatio < desiredRatio)
        {
            cs.matchWidthOrHeight = 1;
        } else
        {
            cs.matchWidthOrHeight = 0.5f;
        }
	}

}
