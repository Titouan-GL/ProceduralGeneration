using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationHidden : ChangeSize
{

    public override void WindowMoved()
    {
        float newHeight = height * ((Screen.height*1.0f) / (Screen.currentResolution.height*1.0f));

        rt.sizeDelta = new Vector2(0, newHeight);

        rt.anchoredPosition = new Vector2(0, -newHeight/2);
    }
}
