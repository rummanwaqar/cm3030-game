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

    // Get timer
    [SerializeField] private GameManager gameManager;
    private float localTimer;

    private void Start()
    {
        emptyColor = lowHealthFilter.color;         // Low-health filter
        redFilter = new Color(120, 0, 0, 0.25f);    // Low-health filter
    }

    private void Update()
    {
        // Display time in seconds since the beginning
        DisplayTime();
        // Display a red screen on low-health
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

    private void DisplayTime()
    {
        localTimer = gameManager.GetTimer();         
        int timeMinutes = Mathf.FloorToInt(localTimer / 60F);
        int timeSeconds = Mathf.FloorToInt(localTimer - timeMinutes * 60);
        string timeString = string.Format("{0:0}:{1:00}", timeMinutes, timeSeconds);
        timeTMP.SetText(timeString + " AM");
    }
}
