using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayDate : MonoBehaviour
{
    int day;
    int month;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>(); 
    }

    public void Update()
    {
        DateTime currentDateTime = DateTime.Now;
        day = currentDateTime.Day;
        month = currentDateTime.Month;
        text.text = day.ToString("D2") + " " +month.ToString("D2");
    }
}
