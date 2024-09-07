using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoulsPanelScript : MonoBehaviour
{
    private TextMeshProUGUI soulsText;
    private PlayerScript player;

    private void Start()
    {
        soulsText = GetComponentInChildren<TextMeshProUGUI>();
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        soulsText.text = player.Souls.ToString();
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
