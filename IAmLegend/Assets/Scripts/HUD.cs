using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("UI Texts")]
    [SerializeField] private TMP_Text scoreTMP;
    [SerializeField] private TMP_Text timeTMP;
    [SerializeField] private TMP_Text ammoTMP;
   
    [Header("Low Health Filter")]
    [SerializeField] private HealthSystem playerHealth;
    [SerializeField] private Image lowHealthFilter;
    private Color emptyColor;
    private Color redFilter;
    float lerpToRed;
    float lerpToEmpty;
    bool colorChanged;

    private void Start()
    {
        emptyColor = lowHealthFilter.color;
        redFilter = new Color(120, 0, 0, 0.25f);
    }

    private void Update()
    {
        // Display time in seconds since the beginning 
        timeTMP.SetText(Mathf.Round(Time.time).ToString());

        LowHealthFilter();
    }

    private void LowHealthFilter()
    {
        // Lerp to a red filter screen when player's health is low
        if( playerHealth.GetHealth() <= 30 && lerpToRed <= 1 )
        {
            // 1.5 seconds of interpolation
            lerpToRed += Time.deltaTime / 1.5f;
            lowHealthFilter.color = Color.Lerp(emptyColor, redFilter, lerpToRed);
            lerpToEmpty = 0;                // reset the second interpolation
            colorChanged = true;
        }
        // Lerp back to empty color when player's health is restored
        else if( playerHealth.GetHealth() > 30 && lerpToEmpty <= 1 && colorChanged == true )
        {
            lerpToEmpty += Time.deltaTime / 1.5f;
            lowHealthFilter.color = Color.Lerp(redFilter, emptyColor, lerpToEmpty);
            lerpToRed = 0;                  // reset the first interpolation
        }
    }
}
