using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectSize : ChangeSize
{
    Vector2 initialPosition;

    public void SetInitialPosition(Vector2 position)
    {
        initialPosition = position;
        rt.anchoredPosition = position * GetScreenRatio();
        WindowMoved();
    }
    public override void WindowMoved()
    {
        rt.localScale = new Vector2(GetScreenRatio(), GetScreenRatio());

        rt.anchoredPosition = initialPosition * GetScreenRatio();
    }

    private float GetScreenRatio()
    {
        return ((Screen.height * 1.0f) / (Screen.currentResolution.height * 1.0f));
    }
}
