using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayHour : MonoBehaviour
{
    int minute;
    int hour;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>(); 
    }

    public void Update()
    {
        DateTime currentDateTime = DateTime.Now;
        minute = currentDateTime.Minute;
        hour = currentDateTime.Hour;
        text.text = hour.ToString("D2") + ":" +minute.ToString("D2");
    }
}
