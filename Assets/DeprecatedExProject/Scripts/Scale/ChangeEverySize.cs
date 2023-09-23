using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeEverySize : MonoBehaviour
{
    public List<ChangeSize> UI;

    public void Awake()
    {
        UI = GetComponentsInChildren<ChangeSize>().ToList();
    }

    private void Start()
    {
        foreach (ChangeSize changeSize in UI)
        {
            changeSize.WindowMoved();
        }
    }

    void OnRectTransformDimensionsChange()
    {
        foreach (ChangeSize changeSize in UI)
        {
            changeSize.WindowMoved();
        }
    }
}
