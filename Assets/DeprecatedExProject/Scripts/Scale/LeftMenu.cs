using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftMenu : ChangeSize
{

    public override void WindowMoved()
    {
        float newWidth = width * ((Screen.height*1.0f) / (Screen.currentResolution.height*1.0f));
        rt.sizeDelta = new Vector2(newWidth, rt.sizeDelta.y);

        rt.anchoredPosition = new Vector2(newWidth/2, rt.anchoredPosition.y);
    }
}
