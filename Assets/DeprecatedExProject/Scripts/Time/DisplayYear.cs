using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayYear : MonoBehaviour
{
    int year;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>(); 
    }

    public void Update()
    {
        DateTime currentDateTime = DateTime.Now;
        year = currentDateTime.Year;
        text.text = year.ToString();
    }
}
