using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoulsPanelScript : MonoBehaviour
{
    private TextMeshProUGUI soulsText;

    private void Start()
    {
        soulsText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnUpdateSoulsPanel(object souls)
    {
        soulsText.text = souls.ToString();
    }

    private void OnEnable()
    {
        EventManager.StartListening("UpdateSoulsPanel", OnUpdateSoulsPanel);
    }

    private void OnDisable()
    {
        EventManager.StopListening("UpdateSoulsPanel", OnUpdateSoulsPanel);
    }
}