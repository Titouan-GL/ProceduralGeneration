using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logo : ChangeSize
{

    public override void WindowMoved()
    {
        float newWidth = width * ((Screen.height * 1.0f) / (Screen.currentResolution.height * 1.0f));
        float newHeight = height * ((Screen.height * 1.0f) / (Screen.currentResolution.height * 1.0f));
        rt.sizeDelta = new Vector2(newWidth,newHeight);

        rt.anchoredPosition = new Vector2(0,0);
    }
}
