using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class LevelUpPanel : MonoBehaviour
{
    TextMeshProUGUI[] textMeshProUGUIs;
    PlayerScript player;



    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        textMeshProUGUIs = GetComponentsInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        updateStatsText(textMeshProUGUIs);
        EventManager.StartListening("UpdateLevelUpPanel", OnUpdateLevelUpPanel);

    }

    void OnUpdateLevelUpPanel(object obj)
    {
        updateStatsText(textMeshProUGUIs);
    }

    void updateStatsText(TextMeshProUGUI[] textMeshProUGUIs)
    {
        foreach (var tmp in textMeshProUGUIs)
        {
            switch (tmp.name)
            {
                case "Strength":
                    tmp.SetText("Strength: " + player.Strength.ToString());
                    break;
                case "Dexterity":
                    tmp.SetText("Dexterity: " + player.Dexterity.ToString());
                    break;
                case "Intelligence":
                    tmp.SetText("Intelligence: " + player.Intelligence.ToString());
                    break;


            }
        }
    }
}
