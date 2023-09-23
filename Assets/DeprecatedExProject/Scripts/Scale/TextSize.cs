using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextSize : ChangeSize
{
    private Vector2 InitialPosition;
    public override void Awake()
    {
        base.Awake();
        InitialPosition = rt.anchoredPosition;
    }


    public override void WindowMoved()
    {
        float newScale = ((Screen.height * 1.0f) / (Screen.currentResolution.height * 1.0f));
        rt.localScale = new Vector3(newScale, newScale, newScale);

        rt.anchoredPosition = InitialPosition * newScale;
    }
}
