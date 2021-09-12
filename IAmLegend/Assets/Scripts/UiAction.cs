using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiAction : MonoBehaviour
{
    private TextMeshProUGUI _actionText;
    private TextMeshProUGUI _actionDisplay;
    
    void Awake()
    {
        _actionText = gameObject.transform.Find("ActionText").GetComponent<TextMeshProUGUI>();
        _actionDisplay = gameObject.transform.Find("ActionDisplay").GetComponent<TextMeshProUGUI>();
    }

    public void ShowNoActionMessage(String message)
    {
        gameObject.SetActive(true);
        _actionText.SetText(message);
        _actionDisplay.SetText("");
    }

    public void ShowActionMessage(String message)
    {
        gameObject.SetActive(true);
        _actionText.SetText(message);
        _actionDisplay.SetText("[E]");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
