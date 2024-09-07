using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsScript : MonoBehaviour
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
                case "Level":
                    tmp.SetText("Level: " + player.Level.ToString());
                    break;
                case "Max HP":
                    tmp.SetText("Max HP: " + player.MaxHP.ToString());
                    break;
                case "HP":
                    tmp.SetText("HP: " + player.CurrentHP.ToString());
                    break;
                case "Souls":
                    tmp.SetText("Souls: " + player.Souls.ToString());
                    break;
            }
        }
    }


}
