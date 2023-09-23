using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSize : MonoBehaviour
{
    protected float width = 100;
    protected float height = 100;
    protected RectTransform rt;

    public virtual void Awake()
    {
        rt = GetComponent<RectTransform>();
        width = rt.sizeDelta.x;
        height = rt.sizeDelta.y;
    }

    public virtual void WindowMoved()
    {

    }
}
